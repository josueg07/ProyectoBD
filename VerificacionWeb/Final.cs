using System.Data;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

internal static partial class Program
{
    private static readonly string PrefijoFinal = "PRUEBAWEBFINAL_" + Guid.NewGuid().ToString("N")[..6];
    private static readonly List<int> facturasFinal = new();
    private static string salidaFinal = "";
    private static SqlParameter FP(string nombre, object valor) => new(nombre, valor);
    private static DataTable FT(string sql) => TablaProceso(sql, FP("@cliente", clienteProceso), FP("@a", varianteA), FP("@b", varianteB), FP("@actor", actor));
    private static int StockFinal(int variante) => Convert.ToInt32(TablaProceso("SELECT ISNULL(SUM(CantidadDisponible),0) Stock FROM dbo.tbl_LoteInventario WHERE ID_ProductoV=@v", FP("@v", variante)).Rows[0][0]);
    private static string EstadoFinal(int id) => (string)TablaProceso("SELECT Estado FROM dbo.tbl_Factura WHERE ID_Factura=@id", FP("@id", id)).Rows[0][0];
    private static decimal SaldoFinal(int id) => Convert.ToDecimal(TablaProceso("SELECT Total-ISNULL((SELECT SUM(Monto) FROM dbo.tbl_PagoFactura WHERE ID_Factura=@id),0) FROM dbo.tbl_Factura WHERE ID_Factura=@id", FP("@id", id)).Rows[0][0]);
    private static string FotoFinal() => string.Join("|", FT(@"SELECT
      (SELECT COUNT(*) FROM dbo.tbl_Factura WHERE ID_Cliente=@cliente) Facturas,
      (SELECT SUM(Total) FROM dbo.tbl_Factura WHERE ID_Cliente=@cliente) Total,
      (SELECT COUNT(*) FROM dbo.tbl_PagoFactura p JOIN dbo.tbl_Factura f ON f.ID_Factura=p.ID_Factura WHERE f.ID_Cliente=@cliente) Pagos,
      (SELECT SUM(CantidadDisponible) FROM dbo.tbl_LoteInventario WHERE ID_ProductoV IN(@a,@b)) Stock,
      (SELECT COUNT(*) FROM dbo.tbl_MovimientoInventario WHERE ID_ProductoV IN(@a,@b)) Movimientos,
      (SELECT COUNT(*) FROM dbo.tbl_Auditoria WHERE TablaAfectada='tbl_Factura' AND RegistroAfectado IN(SELECT CONVERT(nvarchar(60),ID_Factura) FROM dbo.tbl_Factura WHERE ID_Cliente=@cliente)) Auditoria").Rows[0].ItemArray);
    private static async Task InvalidoFinal(HttpClient http, string ruta, string formulario, Dictionary<string,string> campos, string caso)
    {
        string antes = FotoFinal();
        using var r = await Enviar(http, ruta, formulario, campos);
        string html = await r.Content.ReadAsStringAsync();
        Exigir(r.StatusCode == HttpStatusCode.OK && (html.Contains("validation-summary-errors") || html.Contains("field-validation-error")), "No rechazó: " + caso + " / " + r.StatusCode);
        Exigir(FotoFinal() == antes, "La solicitud inválida alteró datos: " + caso);
        Exigir(!html.Contains("SqlException") && !html.Contains("StackTrace"), "Expone errores técnicos.");
        Console.WriteLine("OK rechazo sin cambios: " + caso);
    }
    private static async Task SeguridadFinal(HttpClient anonimo, HttpClient gerente, HttpClient cajero, HttpClient digitador)
    {
        foreach (var caso in new[] {
            ("/Facturas", digitador), ("/Facturas/Crear",digitador), ("/Cobros",cajero), ("/MediosPago",cajero),
            ("/Reportes",gerente), ("/Reportes/Consulta",gerente), ("/Anulacion",gerente) })
        {
            await Pagina(caso.Item2, caso.Item1);
            using var r = await anonimo.GetAsync(caso.Item1);
            Exigir(r.StatusCode == HttpStatusCode.Redirect && r.Headers.Location!.ToString().Contains("/Account/Login"), "No exige login: " + caso.Item1);
            foreach (var otro in new[] { gerente,cajero,digitador }.Where(h => h != caso.Item2))
            {
                using var prohibido = await otro.GetAsync(caso.Item1);
                Exigir(prohibido.StatusCode == HttpStatusCode.Redirect && prohibido.Headers.Location!.ToString().Contains("AccesoDenegado"), "Permiso GET incorrecto: " + caso.Item1);
            }
        }
        foreach (var caso in new[] { ("/Facturas/Crear",digitador), ("/Cobros/Cobrar/1",cajero), ("/Reportes/Generar",gerente), ("/Anulacion/Anular",gerente), ("/MediosPago/Crear",cajero) })
        {
            using var sinToken = await caso.Item2.PostAsync(caso.Item1, new FormUrlEncodedContent(new Dictionary<string,string>()));
            Exigir(sinToken.StatusCode == HttpStatusCode.BadRequest, "POST sin antiforgery: " + caso.Item1);
            foreach (var otro in new[] { gerente,cajero,digitador }.Where(h => h != caso.Item2))
            {
                using var prohibido = await otro.PostAsync(caso.Item1, new FormUrlEncodedContent(new Dictionary<string,string>()));
                Exigir(prohibido.StatusCode == HttpStatusCode.Redirect && prohibido.Headers.Location!.ToString().Contains("AccesoDenegado"), "Permiso POST incorrecto.");
            }
        }
        using (var get = await gerente.GetAsync("/Anulacion/Anular?numeroFactura=1&confirmar=true")) Exigir(get.StatusCode == HttpStatusCode.MethodNotAllowed, "Anulación por GET.");
        using (var pdf = await gerente.GetAsync("/Reportes/Pdf?token=" + new string('0',32))) Exigir(pdf.StatusCode == HttpStatusCode.NotFound, "PDF sin previsualización.");
        using (var detalle = await digitador.GetAsync("/Facturas/Detalle/2147483647")) Exigir(detalle.StatusCode == HttpStatusCode.NotFound, "Detalle inexistente.");
        foreach (var otro in new[] { gerente,cajero }) foreach (var modulo in new[] { "Cotizaciones","OrdenesCompra","IngresosBodega" })
        { using var r = await otro.GetAsync("/"+modulo); Exigir(r.StatusCode == HttpStatusCode.Redirect && r.Headers.Location!.ToString().Contains("AccesoDenegado"), "Cambió permiso anterior."); }
        foreach (string modulo in Modulos) await Pagina(digitador, "/"+modulo);
        foreach (var caso in new[] { (digitador,new[]{"Facturas","Cotizaciones","OrdenesCompra","IngresosBodega"}), (cajero,new[]{"Cobros","MediosPago"}), (gerente,new[]{"Reportes","Anulacion","Usuarios","Roles"}) })
        { string menu = await Pagina(caso.Item1,"/Home/Index"); foreach(string modulo in caso.Item2) Exigir(menu.Contains("href=\"/"+modulo), "Falta menú " + modulo); }
        Console.WriteLine("OK seguridad: anónimos, roles por URL GET/POST, antiforgery, menús, 404, PDF sin copia y anulación sólo POST.");
    }
    private static async Task<string> PrevisualizarFinal(HttpClient gerente, int tipo, int? numero = null)
    {
        using var r = await Enviar(gerente, "/Reportes/Generar", "/Reportes", new() { ["Tipo"] = tipo.ToString(), ["Inicial"] = DateTime.Today.ToString("yyyy-MM-dd"), ["Final"] = DateTime.Today.ToString("yyyy-MM-dd"), ["NumeroFactura"] = numero?.ToString() ?? "" });
        Exigir(r.StatusCode == HttpStatusCode.Redirect, "No genera reporte " + tipo + ": " + await r.Content.ReadAsStringAsync());
        string ruta = r.Headers.Location!.ToString();
        string html = await Pagina(gerente,ruta);
        Exigir(html.Contains("Generar PDF") && html.Contains("Generado:"), "No hay previsualización HTML.");
        var enlace = Regex.Match(html, "href=\"([^\"]*/Reportes/Pdf[^\"]+)\"");
        Exigir(enlace.Success,"No hay enlace PDF.");
        string pdfRuta = WebUtility.HtmlDecode(enlace.Groups[1].Value);
        using var pdf = await gerente.GetAsync(pdfRuta);
        byte[] bytes = await pdf.Content.ReadAsByteArrayAsync();
        Exigir(pdf.StatusCode == HttpStatusCode.OK && pdf.Content.Headers.ContentType?.MediaType == "application/pdf" && System.Text.Encoding.ASCII.GetString(bytes,0,5)=="%PDF-", "No genera PDF válido para reporte " + tipo);
        await File.WriteAllBytesAsync(Path.Combine(salidaFinal,$"Reporte_{tipo}.pdf"),bytes);
        await File.WriteAllTextAsync(Path.Combine(salidaFinal,$"Reporte_{tipo}.html"),html);
        Console.WriteLine($"OK reporte {tipo}: HTML y PDF {bytes.Length} bytes.");
        return ruta;
    }
    private static Dictionary<string,string> VentaDirectaFinal(int variante, int cantidad) => new() { ["accion"]="guardar", ["ID_Cliente"]=clienteProceso.ToString(),["Detalle[0].ID_ProductoV"]=variante.ToString(),["Detalle[0].Cantidad"]=cantidad.ToString(),["Detalle[0].PrecioUnitario"]="10.00" };
    private static Dictionary<string,string> PagoFinal(int factura, params (int metodo,string monto,string referencia)[] lineas)
    {
        var campos = new Dictionary<string,string> { ["ID_Factura"]=factura.ToString(),["accion"]="guardar" };
        for(int i=0;i<lineas.Length;i++) { campos[$"Pagos[{i}].ID_MetodoP"]=lineas[i].metodo.ToString(); campos[$"Pagos[{i}].Monto"]=lineas[i].monto; campos[$"Pagos[{i}].Referencia"]=lineas[i].referencia; }
        return campos;
    }
    private static async Task RecorridoFinal(HttpClient digitador, HttpClient cajero, HttpClient gerente, UsuarioSesion sesion, UsuarioSesion sesionGerente)
    {
        var negocio = new ProcesoNegocio(sesion);
        var reportes = new ReporteNegocio(sesionGerente);
        decimal facturadoInicial = Convert.ToDecimal(reportes.ReporteFacturacion(DateTime.Today,DateTime.Today).Tables[0].Rows[0]["TotalFacturado"]);
        string antesBorrador = FotoFinal();
        var borrador = VentaDirectaFinal(varianteA, 1); borrador["accion"] = "agregar";
        using (var r = await Enviar(digitador, "/Facturas/Crear", "/Facturas/Crear", borrador))
            Exigir(r.IsSuccessStatusCode && (await r.Content.ReadAsStringAsync()).Contains("name=\"Detalle[1].ID_ProductoV\""), "No agrega línea a factura.");
        borrador["accion"] = "quitar:0";
        using (var r = await Enviar(digitador, "/Facturas/Crear", "/Facturas/Crear", borrador)) Exigir(r.IsSuccessStatusCode, "No quita línea a factura.");
        Exigir(FotoFinal() == antesBorrador, "Borrador factura cambió datos.");
        Exigir(StockFinal(varianteA)==0 && StockFinal(varianteB)==0,"Stock inicial distinto de cero.");
        int orden = await GuardarDocumento(digitador,"OrdenesCompra",OrdenCampos());
        var lineas = Pendientes(sesion,orden);
        var recepcion = RecepcionCampos(orden,lineas,2,0); recepcion["FechaIngreso"]=DateTime.Now.AddDays(-2).ToString("yyyy-MM-ddTHH:mm:ss"); recepcion["Detalle[0].NumeroLote"]=PrefijoFinal+"_ANTIGUO";
        int parcial = await GuardarDocumento(digitador,"IngresosBodega",recepcion);
        Exigir(StockFinal(varianteA)==2 && StockFinal(varianteB)==0,"No incrementa inventario parcial.");
        recepcion=RecepcionCampos(orden,Pendientes(sesion,orden),3,7); recepcion["FechaIngreso"]=DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss"); recepcion["Detalle[0].NumeroLote"]=PrefijoFinal+"_RECIENTE"; recepcion["Detalle[1].NumeroLote"]=PrefijoFinal+"_B";
        int completa = await GuardarDocumento(digitador,"IngresosBodega",recepcion);
        Exigir(StockFinal(varianteA)==5 && StockFinal(varianteB)==7 && Pendientes(sesion,orden).AsEnumerable().All(r=>r.Field<int>("Pendiente")==0),"No completa recepción.");
        var cotizar=VentaDirectaFinal(varianteA,4);
        int cotizacion = await GuardarDocumento(digitador,"Cotizaciones",cotizar);
        await Redireccion(digitador,$"/Cotizaciones/Confirmar/{cotizacion}",$"/Cotizaciones/Detalle/{cotizacion}",new());
        var lotes = FT("SELECT ID_Lote,NumeroLote,CantidadDisponible FROM dbo.tbl_LoteInventario WHERE ID_ProductoV=@a ORDER BY FechaIngreso,ID_Lote");
        int loteAntiguo=(int)lotes.Rows[0]["ID_Lote"], loteReciente=(int)lotes.Rows[1]["ID_Lote"];
        var desdeCotizacion = new Dictionary<string,string> { ["ID_Cotizacion"]=cotizacion.ToString(),["ID_Cliente"]="2147483647",["Detalle[0].Cantidad"]="999",["Detalle[0].PrecioUnitario"]="0.01",["accion"]="guardar",["Total"]="0.01",["Estado"]="Pagada" };
        int factura = await GuardarDocumento(digitador,"Facturas",desdeCotizacion); facturasFinal.Add(factura);
        var f = TablaProceso("SELECT NumeroFactura,Total,Estado,ID_Usuario FROM dbo.tbl_Factura WHERE ID_Factura=@id",FP("@id",factura)).Rows[0]; int numero=(int)f["NumeroFactura"];
        Exigir((decimal)f["Total"]==40 && (string)f["Estado"]=="Pendiente" && (int)f["ID_Usuario"]==actor,"No conserva cotización/actor servidor.");
        Exigir(StockFinal(varianteA)==1,"No descuenta inventario.");
        var salida=TablaProceso(@"SELECT fl.ID_Lote,fl.CantidadSalida,l.CantidadDisponible FROM dbo.tbl_DetalleFacturaLote fl JOIN dbo.tbl_DetalleFactura d ON d.ID_DetalleF=fl.ID_DetalleF JOIN dbo.tbl_LoteInventario l ON l.ID_Lote=fl.ID_Lote WHERE d.ID_Factura=@id ORDER BY l.FechaIngreso,l.ID_Lote",FP("@id",factura));
        Exigir(salida.Rows.Count==2 && (int)salida.Rows[0]["ID_Lote"]==loteAntiguo && (int)salida.Rows[0]["CantidadSalida"]==2 && (int)salida.Rows[0]["CantidadDisponible"]==0 && (int)salida.Rows[1]["ID_Lote"]==loteReciente && (int)salida.Rows[1]["CantidadSalida"]==2 && (int)salida.Rows[1]["CantidadDisponible"]==1,"PEPS no consume antiguo primero.");
        Exigir(FT("SELECT COUNT(*) FROM dbo.tbl_MovimientoInventario WHERE ID_ProductoV=@a AND TipoMovimiento='SALIDA_VENTA'").Rows[0].Field<int>(0)==2,"Sin movimientos PEPS.");
        Console.WriteLine($"PEPS verificado: lote antiguo {loteAntiguo} recibió 2/salió 2/quedó 0; reciente {loteReciente} recibió 3/salió 2/quedó 1. Factura ID {factura}, N.º {numero}, Q40.");
        await InvalidoFinal(digitador,"/Facturas/Crear","/Facturas/Crear",desdeCotizacion,"cotización ya facturada");
        await InvalidoFinal(digitador,"/Facturas/Crear","/Facturas/Crear",VentaDirectaFinal(varianteA,2),"inventario insuficiente con reversión completa");
        var invalida=VentaDirectaFinal(varianteA,1); invalida["Detalle[0].PrecioUnitario"]="1.234"; await InvalidoFinal(digitador,"/Facturas/Crear","/Facturas/Crear",invalida,"precio de tres decimales");
        invalida=VentaDirectaFinal(varianteA,1); invalida["Detalle[0].Cantidad"]="0"; await InvalidoFinal(digitador,"/Facturas/Crear","/Facturas/Crear",invalida,"cantidad cero");
        invalida=VentaDirectaFinal(varianteA,1); invalida["Detalle[1].ID_ProductoV"]=varianteA.ToString(); invalida["Detalle[1].Cantidad"]="1"; invalida["Detalle[1].PrecioUnitario"]="10"; await InvalidoFinal(digitador,"/Facturas/Crear","/Facturas/Crear",invalida,"variantes duplicadas");
        int directa=await GuardarDocumento(digitador,"Facturas",VentaDirectaFinal(varianteB,7)); facturasFinal.Add(directa);
        int numeroDirecta=(int)TablaProceso("SELECT NumeroFactura FROM dbo.tbl_Factura WHERE ID_Factura=@id",FP("@id",directa)).Rows[0][0]; Exigir(StockFinal(varianteB)==0,"Venta directa no descuenta.");
        var metodos=new MetodoPagoNegocio().Listar(); int efectivo=metodos.Single(m=>m.NombreMetodo=="Efectivo").ID_MetodoP, tarjeta=metodos.Single(m=>m.NombreMetodo=="Tarjeta").ID_MetodoP, cheque=metodos.Single(m=>m.NombreMetodo=="Cheque").ID_MetodoP;
        string cobro=$"/Cobros/Cobrar/{factura}";
        var borradorPago=PagoFinal(factura,(efectivo,"1","")); borradorPago["accion"]="agregar";
        antesBorrador=FotoFinal();
        using (var r = await Enviar(cajero,cobro,cobro,borradorPago))
            Exigir(r.IsSuccessStatusCode && (await r.Content.ReadAsStringAsync()).Contains("name=\"Pagos[1].ID_MetodoP\""),"No agrega medio de pago.");
        borradorPago["accion"]="quitar:0";
        using (var r = await Enviar(cajero,cobro,cobro,borradorPago)) Exigir(r.IsSuccessStatusCode,"No quita pago.");
        Exigir(FotoFinal()==antesBorrador,"Borrador cobro cambió datos.");
        foreach(var caso in new[]{("41","sobrepago"),("0","monto cero"),("-1","monto negativo"),("1.234","monto tres decimales")}) await InvalidoFinal(cajero,cobro,cobro,PagoFinal(factura,(efectivo,caso.Item1,"")),caso.Item2);
        await InvalidoFinal(cajero,cobro,cobro,PagoFinal(factura,(int.MaxValue,"1","")),"método inexistente");
        await InvalidoFinal(cajero,cobro,cobro,PagoFinal(factura,(tarjeta,"1",new string('R',101))),"referencia demasiado larga");
        await Redireccion(cajero,cobro,cobro,PagoFinal(factura,(efectivo,"10",PrefijoFinal)));
        Exigir(EstadoFinal(factura)=="Pendiente" && SaldoFinal(factura)==30,"Pago parcial incorrecto.");
        await Redireccion(cajero,cobro,cobro,PagoFinal(factura,(tarjeta,"20",PrefijoFinal+"_T"),(cheque,"10",PrefijoFinal+"_C")));
        Exigir(EstadoFinal(factura)=="Pagada" && SaldoFinal(factura)==0,"Pago múltiple no cierra factura.");
        await InvalidoFinal(cajero,cobro,cobro,PagoFinal(factura,(efectivo,"1","")),"segundo cobro a factura pagada");
        await Pagina(cajero,$"/Cobros/Detalle/{factura}"); await Pagina(digitador,$"/Facturas/Detalle/{factura}");
        Console.WriteLine("OK pago parcial Q10 efectivo + operación múltiple Q20 tarjeta/Q10 cheque: tres pagos históricos, estado Pagada y saldo Q0.");
        await InvalidoFinal(gerente,"/Reportes/Generar","/Reportes",new(){["Tipo"]="1",["Inicial"]="2026-12-01",["Final"]="2026-01-01"},"fechas invertidas");
        await InvalidoFinal(gerente,"/Reportes/Generar","/Reportes",new(){["Tipo"]="7",["NumeroFactura"]="2147483647"},"número de factura inexistente");
        var periodo=reportes.ReporteFacturacion(DateTime.Today,DateTime.Today);
        Exigir(Convert.ToDecimal(periodo.Tables[0].Rows[0]["TotalFacturado"])==facturadoInicial+110,"Reporte1 no incluye facturas.");
        Exigir(reportes.ReporteProductosMayoresIngresos(DateTime.Today,DateTime.Today).AsEnumerable().Any(r=>r.Field<string>("Producto")==PrefijoFinal+" producto" && r.Field<decimal>("IngresoTotal")==40),"Reporte2 incorrecto.");
        Exigir(reportes.ReporteProductosMasVendidos(DateTime.Today,DateTime.Today).AsEnumerable().Any(r=>r.Field<string>("Producto")==PrefijoFinal+" producto" && Convert.ToInt64(r["CantidadVendida"])==4),"Reporte3 incorrecto.");
        Exigir(reportes.ReporteInventario().AsEnumerable().Any(r=>r.Field<string>("Lote")==PrefijoFinal+"_RECIENTE" && r.Field<int>("CantidadDisponible")==1),"Reporte4 incorrecto.");
        Exigir(reportes.ReporteProductosMenosVendidos(DateTime.Today,DateTime.Today).AsEnumerable().Any(r=>r.Field<string>("Producto")==PrefijoFinal+" producto" && Convert.ToInt64(r["CantidadVendida"])==4),"Reporte5 incorrecto.");
        Exigir(reportes.ReporteProductosSinExistencia().AsEnumerable().Any(r=>r.Field<string>("Producto")==PrefijoFinal+" producto"),"Reporte6 no incluye variante agotada.");
        string copia="";
        for(int tipo=1;tipo<=7;tipo++) { string ruta=await PrevisualizarFinal(gerente,tipo,numero); if(tipo==1)copia=ruta; }
        string consulta=await Pagina(gerente,"/Reportes/Consulta?numeroFactura="+numero);
        Exigir(consulta.Contains(PrefijoFinal) && consulta.Contains("Pagada") && consulta.Contains("Auditor"),"Consulta número incompleta.");
        var detalle=reportes.BuscarFactura(numero); Exigir(detalle.Tables[1].Rows.Count==1 && detalle.Tables[2].Rows.Count==3 && detalle.Tables[3].Rows.Count>=3,"Falta detalle/pagos/auditoría.");
        // Un Gerente distinto no puede descargar ni ver la copia privada del otro.
        string claveTemporal = Guid.NewGuid().ToString("N")+"A1!"; string otroNombre=PrefijoFinal+"G";
        Usuarios.Guardar(new Usuario{ NombreUsuario=otroNombre,Nombres="Prueba",Apellidos="Temporal",ID_Rol=sesionGerente.ID_Rol },claveTemporal,sesionGerente,true);
        int otroId=Usuarios.Listar(sesionGerente).Single(u=>u.NombreUsuario==otroNombre).ID_Usuario;
        try
        {
            using var otro=ClienteHttp(); await Entrar(otro,otroNombre,claveTemporal);
            string htmlCopia=await Pagina(gerente,copia); string rutaPdf=WebUtility.HtmlDecode(Regex.Match(htmlCopia,"href=\"([^\"]*/Reportes/Pdf[^\"]+)\"").Groups[1].Value);
            using var ajeno=await otro.GetAsync(rutaPdf); Exigir(ajeno.StatusCode==HttpStatusCode.NotFound,"Expone copia a otro Gerente.");
        }
        finally { Usuarios.Eliminar(otroId,sesionGerente); }
        string confirmar=await Pagina(gerente,"/Anulacion?numeroFactura="+numero);
        Exigir(confirmar.Contains("value=\"false\" selected") && confirmar.Contains("devolver"),"Confirmación no predeterminada en No.");
        string antes=FotoFinal(); await Redireccion(gerente,"/Anulacion/Anular","/Anulacion?numeroFactura="+numero,new(){["numeroFactura"]=numero.ToString()}); Exigir(FotoFinal()==antes,"Anula sin Sí explícito.");
        await Redireccion(gerente,"/Anulacion/Anular","/Anulacion?numeroFactura="+numero,new(){["numeroFactura"]=numero.ToString(),["confirmar"]="true"});
        Exigir(EstadoFinal(factura)=="Anulada" && StockFinal(varianteA)==5,"No restaura stock al anular.");
        detalle=reportes.BuscarFactura(numero);
        Exigir((decimal)detalle.Tables[0].Rows[0]["Total"]==0 && (decimal)detalle.Tables[0].Rows[0]["TotalOriginal"]==40 && detalle.Tables[1].Rows.Count==1 && detalle.Tables[2].Rows.Count==3 && detalle.Tables[3].AsEnumerable().Any(r=>r.Field<string>("Accion")=="ANULAR FACTURA"),"No conserva historial de anulada.");
        Exigir(Convert.ToDecimal(reportes.ReporteFacturacion(DateTime.Today,DateTime.Today).Tables[0].Rows[0]["TotalFacturado"])==facturadoInicial+70,"Reporte incluye anulada.");
        Exigir(!reportes.ReporteProductosMasVendidos(DateTime.Today,DateTime.Today).AsEnumerable().Any(r=>r.Field<string>("Producto")==PrefijoFinal+" producto" && Convert.ToInt64(r["CantidadVendida"])==4),"Cantidad incluye anulada.");
        string copiaPosterior = WebUtility.HtmlDecode(await Pagina(gerente,copia));
        Exigir(copiaPosterior.Contains("Q" + (facturadoInicial + 110).ToString("N2",System.Globalization.CultureInfo.GetCultureInfo("es-GT"))),"La copia PDF/HTML cambió después de anular.");
        antes=FotoFinal(); await Redireccion(gerente,"/Anulacion/Anular","/Anulacion?numeroFactura="+numero,new(){["numeroFactura"]=numero.ToString(),["confirmar"]="true"}); Exigir(FotoFinal()==antes,"Segunda anulación modifica datos.");
        await InvalidoFinal(cajero,cobro,cobro,PagoFinal(factura,(efectivo,"1","")),"cobro de anulada");
        string anulada=await Pagina(gerente,"/Reportes/Consulta?numeroFactura="+numero); Exigir(anulada.Contains("ANULADA") && anulada.Contains(PrefijoFinal),"No consulta anulada.");
        await PrevisualizarFinal(gerente,7,numero);
        await Redireccion(gerente,"/Anulacion/Anular","/Anulacion?numeroFactura="+numeroDirecta,new(){["numeroFactura"]=numeroDirecta.ToString(),["confirmar"]="true"});
        Exigir(StockFinal(varianteB)==7 && Convert.ToDecimal(reportes.ReporteFacturacion(DateTime.Today,DateTime.Today).Tables[0].Rows[0]["TotalFacturado"])==facturadoInicial,"No vuelve al total inicial.");
        Console.WriteLine($"RECORRIDO {PrefijoFinal}: cliente {clienteProceso}, proveedor {proveedorProceso}, producto {productoProceso}, variantes {varianteA}/{varianteB}, ubicación {ubicacionProceso}, usuario {actor}; orden {orden}, ingresos {parcial}/{completa}, cotización {cotizacion}, factura {factura}/número {numero}, directa {directa}/número {numeroDirecta}.");
        Console.WriteLine("OK inventario A: 0 → 2 → 5 → 1 → 5; B: 0 → 7 → 0 → 7. Anulada total Q0, original Q40, tres pagos conservados, dos asignaciones de lote conservadas, auditoría y segunda anulación bloqueada. Reportes excluyen anuladas.");
    }
    private static void LimpiarFinal(UsuarioSesion gerente)
    {
        using(var conexion=new ProyectoBD.Datos.ConexionBD().CrearConexion())
        {
            conexion.Open();
            using var cmd=new SqlCommand(@"SET XACT_ABORT ON; BEGIN TRANSACTION;
             DECLARE @f TABLE(ID int PRIMARY KEY); INSERT @f SELECT ID_Factura FROM dbo.tbl_Factura WHERE ID_Cliente=@cliente AND ID_Usuario=@actor;
             IF EXISTS(SELECT 1 FROM dbo.tbl_Factura WHERE ID_Cliente=@cliente AND ID_Usuario<>@actor) THROW 50001,'Limpieza detenida: factura ajena.',1;
             DELETE dbo.tbl_Auditoria WHERE TablaAfectada='tbl_Factura' AND RegistroAfectado IN(SELECT CONVERT(nvarchar(60),ID) FROM @f);
             DELETE dbo.tbl_MovimientoInventario WHERE TipoReferencia='FACTURA' AND ID_Referencia IN(SELECT ID FROM @f) AND ID_ProductoV IN(@a,@b);
             DELETE dbo.tbl_PagoFactura WHERE ID_Factura IN(SELECT ID FROM @f);
             DELETE l FROM dbo.tbl_DetalleFacturaLote l JOIN dbo.tbl_DetalleFactura d ON d.ID_DetalleF=l.ID_DetalleF WHERE d.ID_Factura IN(SELECT ID FROM @f);
             DELETE dbo.tbl_DetalleFactura WHERE ID_Factura IN(SELECT ID FROM @f);
             DELETE dbo.tbl_Factura WHERE ID_Factura IN(SELECT ID FROM @f);
             COMMIT TRANSACTION;",conexion);
            cmd.Parameters.AddRange(new[]{FP("@cliente",clienteProceso),FP("@actor",actor),FP("@a",varianteA),FP("@b",varianteB)}); cmd.ExecuteNonQuery();
        }
        LimpiarProcesos(gerente);
        Console.WriteLine("OK limpieza exclusivamente de documentos temporales propios; sin modificar usuarios ni documentos operativos anteriores.");
    }
    private static async Task<int> ProbarWebFinal(string? url)
    {
        UsuarioSesion? gerente=null;
        try
        {
            servidor=url??"http://127.0.0.1:5190";
            salidaFinal=Path.GetFullPath(Path.Combine("Documentacion","EvidenciaWebFinal")); Directory.CreateDirectory(salidaFinal);
            string claveGerente=Environment.GetEnvironmentVariable("PRUEBA_WEB_GERENTE_PASSWORD")??throw new InvalidOperationException("Falta variable de prueba Gerente.");
            string claveCajero=Environment.GetEnvironmentVariable("PRUEBA_WEB_CAJERO_PASSWORD")??throw new InvalidOperationException("Falta variable de prueba Cajero.");
            gerente=Usuarios.IniciarSesion("gerente",claveGerente)??throw new InvalidOperationException("No autentica Gerente.");
            string clave=Guid.NewGuid().ToString("N")+"A1!";
            Usuarios.Guardar(new Usuario{ NombreUsuario=PrefijoFinal,Nombres="Prueba Web",Apellidos="Temporal",ID_Rol=new RolNegocio().ListarRoles().Single(r=>r.NombreRol=="Digitador").ID_Rol},clave,gerente,true);
            actor=Usuarios.Listar(gerente).Single(u=>u.NombreUsuario==PrefijoFinal).ID_Usuario;
            var digitador=Usuarios.IniciarSesion(PrefijoFinal,clave)!;
            PrepararProcesos(gerente);
            // Renombrar únicamente catálogos recién creados para identificarlos en el recorrido final.
            var cliente=Clientes.Listar().Single(c=>c.ID_Cliente==clienteProceso); cliente.NombreRazonSocial=PrefijoFinal+" cliente"; Clientes.Actualizar(cliente);
            var producto=Productos.Listar().Single(p=>p.ID_Producto==productoProceso); producto.NombreProducto=PrefijoFinal+" producto"; Productos.Guardar(producto,false);
            using var anonimo=ClienteHttp(); using var gh=ClienteHttp(); using var ch=ClienteHttp(); using var dh=ClienteHttp();
            await Entrar(gh,"gerente",claveGerente); await Entrar(ch,"cajero",claveCajero); await Entrar(dh,PrefijoFinal,clave);
            Console.WriteLine("WEB FINAL " + DateTime.Now.ToString("s") + " " + PrefijoFinal);
            await SeguridadFinal(anonimo,gh,ch,dh);
            var medios = new MetodoPagoNegocio();
            await Redireccion(ch,"/MediosPago/Crear","/MediosPago/Crear",new(){["NombreMetodo"]=PrefijoFinal});
            int medioTemporal=medios.Listar().Single(m=>m.NombreMetodo==PrefijoFinal).ID_MetodoP;
            try
            {
                await Redireccion(ch,$"/MediosPago/Editar/{medioTemporal}",$"/MediosPago/Editar/{medioTemporal}",new(){["ID_MetodoP"]=medioTemporal.ToString(),["NombreMetodo"]=PrefijoFinal+"_editado"});
                Exigir(medios.Listar().Any(m=>m.ID_MetodoP==medioTemporal && m.NombreMetodo.EndsWith("_editado")),"No edita medio de pago.");
                await Redireccion(ch,$"/MediosPago/Eliminar/{medioTemporal}","/MediosPago",new());
                Exigir(!medios.Listar().Any(m=>m.ID_MetodoP==medioTemporal),"No elimina medio temporal.");
            }
            finally { if(medios.Listar().Any(m=>m.ID_MetodoP==medioTemporal)) medios.Eliminar(medioTemporal); }
            Console.WriteLine("OK mantenimiento Medios de pago Cajero: crear, editar, eliminar exclusivamente el registro temporal.");
            await RecorridoFinal(dh,ch,gh,digitador,gerente);
            Console.WriteLine("RESULTADO: recorrido integral HTTP/SQL Server completo y siete PDFs descargados.");
            return 0;
        }
        catch(Exception ex) { Console.Error.WriteLine(ex); return 1; }
        finally { if(gerente!=null && actor>0) LimpiarFinal(gerente); }
    }
}
