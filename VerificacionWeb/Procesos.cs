using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

internal static partial class Program
{
    private static int clienteProceso, proveedorProceso, productoProceso, ubicacionProceso, varianteA, varianteB;
    private static DataTable TablaProceso(string sql, params SqlParameter[] parametros)
    {
        using var conexion = new ProyectoBD.Datos.ConexionBD().CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddRange(parametros);
        using var adaptador = new SqlDataAdapter(comando);
        var tabla = new DataTable();
        adaptador.Fill(tabla);
        return tabla;
    }
    private static DataTable Pendientes(UsuarioSesion digitador, int orden) => new ProcesoNegocio(digitador).PendienteOrden(orden);
    private static string FotoProceso()
    {
        var t = TablaProceso(@"SELECT
          (SELECT COUNT(*) FROM dbo.tbl_Cotizacion WHERE ID_Cliente=@cliente) Cotizaciones,
          (SELECT COUNT(*) FROM dbo.tbl_OrdenCompra WHERE ID_Proveedor=@proveedor) Ordenes,
          (SELECT COUNT(*) FROM dbo.tbl_IngresoBodega WHERE ID_Usuario=@actor) Ingresos,
          (SELECT COUNT(*) FROM dbo.tbl_LoteInventario WHERE ID_ProductoV IN(@a,@b)) Lotes,
          (SELECT ISNULL(SUM(CantidadDisponible),0) FROM dbo.tbl_LoteInventario WHERE ID_ProductoV IN(@a,@b)) Stock,
          (SELECT COUNT(*) FROM dbo.tbl_MovimientoInventario WHERE ID_Usuario=@actor) Movimientos,
          (SELECT COUNT(*) FROM dbo.tbl_Auditoria WHERE ID_Usuario=@actor) Auditorias",
          new SqlParameter("@cliente", clienteProceso), new SqlParameter("@proveedor", proveedorProceso),
          new SqlParameter("@actor", actor), new SqlParameter("@a", varianteA), new SqlParameter("@b", varianteB));
        return string.Join("|", t.Rows[0].ItemArray);
    }
    private static Dictionary<string, string> CotizacionCampos()
    {
        return new() { ["ID_Cliente"] = clienteProceso.ToString(), ["accion"] = "guardar",
            ["Detalle[0].ID_ProductoV"] = varianteA.ToString(), ["Detalle[0].Cantidad"] = "2", ["Detalle[0].PrecioUnitario"] = "10.50",
            ["Detalle[1].ID_ProductoV"] = varianteB.ToString(), ["Detalle[1].Cantidad"] = "3", ["Detalle[1].PrecioUnitario"] = "20.00" };
    }
    private static Dictionary<string, string> OrdenCampos()
    {
        return new() { ["ID_Proveedor"] = proveedorProceso.ToString(), ["accion"] = "guardar",
            ["Detalle[0].ID_ProductoV"] = varianteA.ToString(), ["Detalle[0].Cantidad"] = "5",
            ["Detalle[1].ID_ProductoV"] = varianteB.ToString(), ["Detalle[1].Cantidad"] = "7" };
    }
    private static Dictionary<string, string> RecepcionCampos(int orden, DataTable lineas, int cantidadA, int cantidadB)
    {
        return new() { ["ID_OrdenC"] = orden.ToString(), ["ID_UbicacionA"] = ubicacionProceso.ToString(),
            ["FechaIngreso"] = DateTime.Now.AddMinutes(-5).ToString("yyyy-MM-ddTHH:mm:ss"),
            ["Detalle[0].ID_DetalleOC"] = lineas.Rows[0]["ID_DetalleOC"].ToString()!, ["Detalle[0].CantidadRecibida"] = cantidadA.ToString(),
            ["Detalle[0].CostoUnitario"] = "3.50", ["Detalle[0].NumeroLote"] = Prefijo + "-A",
            ["Detalle[1].ID_DetalleOC"] = lineas.Rows[1]["ID_DetalleOC"].ToString()!, ["Detalle[1].CantidadRecibida"] = cantidadB.ToString(),
            ["Detalle[1].CostoUnitario"] = cantidadB == 0 ? "0" : "5.00", ["Detalle[1].NumeroLote"] = "" };
    }
    private static async Task<int> GuardarDocumento(HttpClient http, string modulo, Dictionary<string, string> campos)
    {
        using var respuesta = await Enviar(http, $"/{modulo}/Crear", $"/{modulo}/Crear", campos);
        Exigir(respuesta.StatusCode == System.Net.HttpStatusCode.Redirect, "No guardó documento " + modulo + ": " + respuesta.StatusCode);
        string ruta = respuesta.Headers.Location!.ToString();
        var coincidencia = System.Text.RegularExpressions.Regex.Match(ruta, @"/Detalle/(\d+)");
        Exigir(coincidencia.Success, "Redirección inesperada: " + ruta);
        await Pagina(http, ruta);
        return int.Parse(coincidencia.Groups[1].Value);
    }
    private static async Task RechazarDocumento(HttpClient http, string modulo, Dictionary<string, string> campos, string caso)
    {
        string antes = FotoProceso();
        using var respuesta = await Enviar(http, $"/{modulo}/Crear", $"/{modulo}/Crear", campos);
        string html = await respuesta.Content.ReadAsStringAsync();
        Exigir(respuesta.StatusCode == System.Net.HttpStatusCode.OK &&
            (html.Contains("validation-summary-errors") || html.Contains("field-validation-error")), "No validó " + caso + " en " + modulo);
        Exigir(FotoProceso() == antes, "La operación inválida cambió documentos/inventario: " + caso);
        Console.WriteLine("OK rechazo sin cambios: " + caso);
    }
    private static void PrepararProcesos(UsuarioSesion gerente)
    {
        Clientes.Guardar(new Cliente { TipoCliente = "Particular", NombreRazonSocial = Prefijo + " cliente" });
        clienteProceso = Clientes.Listar().Single(c => c.NombreRazonSocial == Prefijo + " cliente").ID_Cliente;
        Proveedores.Guardar(new Proveedor { Nombre_Proveedor = Prefijo + " proveedor" });
        proveedorProceso = Proveedores.Listar().Single(p => p.Nombre_Proveedor == Prefijo + " proveedor").ID_Proveedor;
        Productos.Guardar(new Producto { NombreProducto = Prefijo + " producto", Descripcion = "Prueba procesos Web",
            ID_CategoriaP = Categorias.Listar().First().ID_Categoria, ID_Marca = Marcas.Listar().First().ID_Marca }, true);
        productoProceso = Productos.Listar().Single(p => p.NombreProducto == Prefijo + " producto").ID_Producto;
        var presentaciones = Presentaciones.Listar().Take(2).ToList();
        Exigir(presentaciones.Count == 2, "Faltan presentaciones para verificar el caso.");
        foreach (var p in presentaciones) Productos.GuardarVariante(new ProductoVariante { ID_Producto = productoProceso, ID_Presentacion = p.ID_Presentacion, PrecioVenta = 10.50m }, true);
        var variantes = Productos.ListarVariantes(productoProceso);
        varianteA = variantes[0].ID_ProductoV; varianteB = variantes[1].ID_ProductoV;
        var ubicaciones = new UbicacionNegocio();
        ubicaciones.Guardar(new Ubicacion { NombreUbicacion = Prefijo + " ubicacion", Descripcion = "Prueba procesos Web" });
        ubicacionProceso = ubicaciones.Listar().Single(u => u.NombreUbicacion == Prefijo + " ubicacion").ID_UbicacionA;
    }
    private static async Task SeguridadProcesos(HttpClient anonimo, HttpClient gerente, HttpClient cajero, HttpClient digitador)
    {
        foreach (string modulo in new[] { "Cotizaciones", "OrdenesCompra", "IngresosBodega" })
        {
            using (var r = await anonimo.GetAsync($"/{modulo}"))
                Exigir(r.StatusCode == System.Net.HttpStatusCode.Redirect && r.Headers.Location!.ToString().Contains("/Account/Login"), "No exige autenticación.");
            foreach (var otro in new[] { gerente, cajero })
            {
                foreach (string accion in new[] { "Index", "Crear", "Detalle/1" })
                {
                    using var r = await otro.GetAsync($"/{modulo}/{accion}");
                    Exigir(r.StatusCode == System.Net.HttpStatusCode.Redirect && r.Headers.Location!.ToString().Contains("/Account/AccesoDenegado"), "Permiso GET incorrecto: " + modulo);
                }
                using var rPost = await otro.PostAsync($"/{modulo}/Crear", new FormUrlEncodedContent(new Dictionary<string, string>()));
                Exigir(rPost.StatusCode == System.Net.HttpStatusCode.Redirect && rPost.Headers.Location!.ToString().Contains("/Account/AccesoDenegado"), "Permiso POST incorrecto.");
            }
            await Pagina(digitador, $"/{modulo}/Index");
            await Pagina(digitador, $"/{modulo}/Crear");
            using (var r = await digitador.GetAsync($"/{modulo}/Detalle/{int.MaxValue}"))
                Exigir(r.StatusCode == System.Net.HttpStatusCode.NotFound, "No devuelve 404 para documento inexistente.");
            using (var r = await digitador.PostAsync($"/{modulo}/Crear", new FormUrlEncodedContent(new Dictionary<string, string>())))
                Exigir(r.StatusCode == System.Net.HttpStatusCode.BadRequest, "No exige antiforgery.");
            Console.WriteLine("OK listados, formularios, permisos GET/POST, antiforgery y 404: " + modulo);
        }
        foreach (string accion in new[] { "Confirmar", "Cancelar" })
        {
            using var get = await digitador.GetAsync($"/Cotizaciones/{accion}/1");
            Exigir(get.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed, "Permite cambiar estado por GET.");
            using var post = await digitador.PostAsync($"/Cotizaciones/{accion}/1", new FormUrlEncodedContent(new Dictionary<string, string>()));
            Exigir(post.StatusCode == System.Net.HttpStatusCode.BadRequest, "Cambio de estado sin antiforgery.");
            using var prohibido = await gerente.PostAsync($"/Cotizaciones/{accion}/1", new FormUrlEncodedContent(new Dictionary<string, string>()));
            Exigir(prohibido.StatusCode == System.Net.HttpStatusCode.Redirect && prohibido.Headers.Location!.ToString().Contains("AccesoDenegado"), "Gerente puede cambiar cotización.");
        }
        string inicio = await Pagina(digitador, "/Home/Index");
        foreach (string modulo in new[] { "Cotizaciones", "OrdenesCompra", "IngresosBodega" })
            Exigir(inicio.Contains("href=\"/" + modulo + "\""), "Falta enlace de proceso.");
        foreach (string mantenimiento in Modulos) await Pagina(digitador, $"/{mantenimiento}");
        Exigir((await Pagina(anonimo, "/health")).Contains("Healthy"), "No responde health.");
        Console.WriteLine("OK menú, mantenimiento existente y /health conservados.");
    }
    private static async Task CotizacionesYOrdenes(HttpClient http, UsuarioSesion digitador)
    {
        var borrador = CotizacionCampos(); borrador.Remove("Detalle[1].ID_ProductoV"); borrador.Remove("Detalle[1].Cantidad"); borrador.Remove("Detalle[1].PrecioUnitario"); borrador["accion"] = "agregar";
        string antes = FotoProceso();
        using (var r = await Enviar(http, "/Cotizaciones/Crear", "/Cotizaciones/Crear", borrador))
            Exigir(r.IsSuccessStatusCode && (await r.Content.ReadAsStringAsync()).Contains("name=\"Detalle[1].ID_ProductoV\""), "No agrega línea.");
        borrador = CotizacionCampos(); borrador["accion"] = "quitar:0";
        using (var r = await Enviar(http, "/Cotizaciones/Crear", "/Cotizaciones/Crear", borrador))
            Exigir(r.IsSuccessStatusCode && !(await r.Content.ReadAsStringAsync()).Contains("name=\"Detalle[1].ID_ProductoV\""), "No quita/reindexa línea.");
        Exigir(antes == FotoProceso(), "Editar borrador creó un documento.");
        var invalida = CotizacionCampos(); invalida["Detalle[1].ID_ProductoV"] = varianteA.ToString();
        await RechazarDocumento(http, "Cotizaciones", invalida, "variantes repetidas en cotización");
        invalida = CotizacionCampos(); invalida["Detalle[0].PrecioUnitario"] = "1.234";
        await RechazarDocumento(http, "Cotizaciones", invalida, "precio con tres decimales");
        invalida = CotizacionCampos(); invalida["ID_Cliente"] = int.MaxValue.ToString();
        await RechazarDocumento(http, "Cotizaciones", invalida, "cliente inexistente");
        invalida = CotizacionCampos(); invalida["Detalle[0].Cantidad"] = "0";
        await RechazarDocumento(http, "Cotizaciones", invalida, "cantidad no positiva");
        var valida = CotizacionCampos(); valida["Total"] = "0.01"; valida["Estado"] = "Facturada";
        int cotizacion = await GuardarDocumento(http, "Cotizaciones", valida);
        var proceso = new ProcesoNegocio(digitador);
        var cabecera = proceso.Cotizaciones().AsEnumerable().Single(r => r.Field<int>("ID_Cotizacion") == cotizacion);
        Exigir(cabecera.Field<decimal>("Total") == 81m && cabecera.Field<string>("Estado") == "Pendiente" && proceso.DetalleCotizacion(cotizacion).Count == 2, "No calcula total/estado en servidor.");
        await Redireccion(http, $"/Cotizaciones/Confirmar/{cotizacion}", $"/Cotizaciones/Detalle/{cotizacion}", new());
        Exigir(proceso.Cotizaciones().AsEnumerable().Single(r => r.Field<int>("ID_Cotizacion") == cotizacion).Field<string>("Estado") == "Confirmada", "No confirma.");
        await Redireccion(http, $"/Cotizaciones/Cancelar/{cotizacion}", $"/Cotizaciones/Detalle/{cotizacion}", new());
        await Redireccion(http, $"/Cotizaciones/Confirmar/{cotizacion}", $"/Cotizaciones/Detalle/{cotizacion}", new());
        Exigir(proceso.Cotizaciones().AsEnumerable().Single(r => r.Field<int>("ID_Cotizacion") == cotizacion).Field<string>("Estado") == "Cancelada", "Confirma una cancelada.");
        Console.WriteLine("OK cotización de dos líneas: Q81.00, total/estado inalterables desde POST, confirmar y cancelar; cancelada no se confirma. Agregar/quitar sin guardar.");

        var ordenInvalida = OrdenCampos(); ordenInvalida["Detalle[1].ID_ProductoV"] = varianteA.ToString();
        await RechazarDocumento(http, "OrdenesCompra", ordenInvalida, "variantes repetidas en compra");
        ordenInvalida = OrdenCampos(); ordenInvalida["ID_Proveedor"] = int.MaxValue.ToString();
        await RechazarDocumento(http, "OrdenesCompra", ordenInvalida, "proveedor inexistente");
        int orden = await GuardarDocumento(http, "OrdenesCompra", OrdenCampos());
        int otra = await GuardarDocumento(http, "OrdenesCompra", OrdenCampos());
        await Recepciones(http, digitador, orden, otra);
    }
    private static async Task Recepciones(HttpClient http, UsuarioSesion digitador, int orden, int otraOrden)
    {
        var lineas = Pendientes(digitador, orden);
        Exigir(lineas.Rows.Count == 2 && (int)lineas.Rows[0]["Pendiente"] == 5 && (int)lineas.Rows[1]["Pendiente"] == 7, "Pendientes iniciales incorrectos.");
        string crear = await Pagina(http, "/IngresosBodega/Crear?orden=" + orden);
        Exigir(crear.Contains(Prefijo + " producto") && crear.Contains("name=\"Detalle[0].ID_DetalleOC\""), "No carga las líneas de orden.");
        int parcial = await GuardarDocumento(http, "IngresosBodega", RecepcionCampos(orden, lineas, 2, 0));
        var proceso = new ProcesoNegocio(digitador);
        Exigir(proceso.Ordenes().AsEnumerable().Single(r => r.Field<int>("ID_OrdenC") == orden).Field<string>("Estado") == "Parcial", "No marca Parcial.");
        lineas = Pendientes(digitador, orden);
        Exigir((int)lineas.Rows[0]["CantidadRecibida"] == 2 && (int)lineas.Rows[0]["Pendiente"] == 3 && (int)lineas.Rows[1]["Pendiente"] == 7, "Recepción parcial incorrecta.");
        var invalidos = new List<(string Campo, string Valor, string Caso)> {
            ("Detalle[0].CantidadRecibida", "4", "sobre-recepción"),
            ("Detalle[0].CantidadRecibida", "-1", "cantidad negativa"),
            ("Detalle[0].CantidadRecibida", "abc", "cantidad no numérica"),
            ("Detalle[0].CostoUnitario", "0", "costo no positivo"),
            ("Detalle[0].CostoUnitario", "2.345", "costo con tres decimales"),
            ("Detalle[0].NumeroLote", new string('L',51), "lote demasiado largo"),
            ("ID_UbicacionA", int.MaxValue.ToString(), "ubicación inexistente"),
            ("FechaIngreso", DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss"), "fecha futura"),
            ("FechaIngreso", "", "fecha ausente") };
        foreach (var caso in invalidos)
        {
            var campos = RecepcionCampos(orden, lineas, 1, 0); campos[caso.Campo] = caso.Valor;
            campos["Detalle[0].Pendiente"] = "999999";
            await RechazarDocumento(http, "IngresosBodega", campos, caso.Caso);
        }
        await RechazarDocumento(http, "IngresosBodega", RecepcionCampos(orden, lineas, 0, 0), "ninguna unidad a recibir");
        var otraLinea = RecepcionCampos(orden, lineas, 1, 0); otraLinea["Detalle[0].ID_DetalleOC"] = Pendientes(digitador, otraOrden).Rows[0]["ID_DetalleOC"].ToString()!;
        await RechazarDocumento(http, "IngresosBodega", otraLinea, "línea de otra orden");
        otraLinea = RecepcionCampos(orden, lineas, 1, 1); otraLinea["Detalle[1].ID_DetalleOC"] = otraLinea["Detalle[0].ID_DetalleOC"];
        await RechazarDocumento(http, "IngresosBodega", otraLinea, "detalle de recepción duplicado");
        var completo = RecepcionCampos(orden, lineas, 3, 7); completo["Detalle[0].NumeroLote"] = Prefijo + "-B"; completo["Detalle[1].NumeroLote"] = Prefijo + "-C";
        int final = await GuardarDocumento(http, "IngresosBodega", completo);
        Exigir(proceso.Ordenes().AsEnumerable().Single(r => r.Field<int>("ID_OrdenC") == orden).Field<string>("Estado") == "Completa", "No completa orden.");
        Exigir(Pendientes(digitador, orden).AsEnumerable().All(r => r.Field<int>("Pendiente") == 0), "Quedaron pendientes.");
        await RechazarDocumento(http, "IngresosBodega", completo, "orden ya completa / segundo envío");
        var comprobacion = TablaProceso(@"SELECT
          (SELECT SUM(CantidadDisponible) FROM dbo.tbl_LoteInventario WHERE ID_ProductoV=@a) StockA,
          (SELECT SUM(CantidadDisponible) FROM dbo.tbl_LoteInventario WHERE ID_ProductoV=@b) StockB,
          (SELECT COUNT(*) FROM dbo.tbl_LoteInventario WHERE ID_ProductoV IN(@a,@b)) Lotes,
          (SELECT COUNT(*) FROM dbo.tbl_MovimientoInventario WHERE ID_Usuario=@actor AND TipoMovimiento='ENTRADA_COMPRA') Movimientos,
          (SELECT COUNT(*) FROM dbo.tbl_Auditoria WHERE ID_Usuario=@actor AND Accion='INGRESO BODEGA') Auditorias,
          (SELECT COUNT(*) FROM dbo.tbl_IngresoBodega WHERE ID_IngresoB IN(@parcial,@final) AND ID_Usuario=@actor) Responsable",
          new SqlParameter("@a", varianteA), new SqlParameter("@b", varianteB), new SqlParameter("@actor", actor),
          new SqlParameter("@parcial", parcial), new SqlParameter("@final", final)).Rows[0];
        Exigir((int)comprobacion["StockA"] == 5 && (int)comprobacion["StockB"] == 7 && (int)comprobacion["Lotes"] == 3 &&
            (int)comprobacion["Movimientos"] == 3 && (int)comprobacion["Auditorias"] == 2 && (int)comprobacion["Responsable"] == 2, "Stock, lotes, movimientos, auditoría o responsable incorrectos.");
        Exigir(proceso.Ingresos().AsEnumerable().Count(r => r.Field<int>("ID_OrdenC") == orden) == 3, "No conserva detalles de ambos ingresos.");
        await Pagina(http, "/IngresosBodega/Index?orden=" + orden);
        await Pagina(http, "/OrdenesCompra/Detalle/" + orden);
        Console.WriteLine($"OK orden {orden}: Pendiente → Parcial (2/0) → Completa (3/7). Stock 5/7; tres lotes, tres entradas, dos auditorías y responsable de sesión correcto. Ingresos {parcial}/{final}.");
    }
    private static void LimpiarProcesos(UsuarioSesion gerente)
    {
        using var conexion = new ProyectoBD.Datos.ConexionBD().CrearConexion();
        conexion.Open();
        using var comando = new SqlCommand(@"SET XACT_ABORT ON; BEGIN TRANSACTION;
          DECLARE @ordenes TABLE(ID int PRIMARY KEY);
          INSERT @ordenes SELECT ID_OrdenC FROM dbo.tbl_OrdenCompra WHERE ID_Proveedor=@proveedor;
          DECLARE @ingresos TABLE(ID int PRIMARY KEY);
          INSERT @ingresos SELECT ID_IngresoB FROM dbo.tbl_IngresoBodega WHERE ID_Usuario=@actor AND ID_OrdenC IN(SELECT ID FROM @ordenes);
          DECLARE @lotes TABLE(ID int PRIMARY KEY);
          INSERT @lotes SELECT l.ID_Lote FROM dbo.tbl_LoteInventario l JOIN dbo.tbl_DetalleIngresoBodega d ON d.ID_DetalleIB=l.ID_DetalleIB WHERE d.ID_IngresoB IN(SELECT ID FROM @ingresos);
          DELETE dbo.tbl_MovimientoInventario WHERE ID_Usuario=@actor AND ID_Lote IN(SELECT ID FROM @lotes);
          DELETE dbo.tbl_Auditoria WHERE ID_Usuario=@actor AND TablaAfectada='tbl_IngresoBodega' AND RegistroAfectado IN(SELECT CONVERT(nvarchar(60),ID) FROM @ingresos);
          DELETE dbo.tbl_LoteInventario WHERE ID_Lote IN(SELECT ID FROM @lotes);
          DELETE dbo.tbl_DetalleIngresoBodega WHERE ID_IngresoB IN(SELECT ID FROM @ingresos);
          DELETE dbo.tbl_IngresoBodega WHERE ID_IngresoB IN(SELECT ID FROM @ingresos);
          DELETE dbo.tbl_DetalleOrdenCompra WHERE ID_OrdenC IN(SELECT ID FROM @ordenes);
          DELETE dbo.tbl_OrdenCompra WHERE ID_OrdenC IN(SELECT ID FROM @ordenes);
          DELETE d FROM dbo.tbl_DetalleCotizacion d JOIN dbo.tbl_Cotizacion c ON c.ID_Cotizacion=d.ID_Cotizacion WHERE c.ID_Cliente=@cliente AND c.ID_Usuario=@actor;
          DELETE dbo.tbl_Cotizacion WHERE ID_Cliente=@cliente AND ID_Usuario=@actor;
          DELETE dbo.tbl_ProductoVariante WHERE ID_Producto=@producto;
          DELETE dbo.tbl_Producto WHERE ID_Producto=@producto;
          DELETE dbo.tbl_Clientes WHERE ID_Cliente=@cliente;
          DELETE dbo.tbl_Proveedor WHERE ID_Proveedor=@proveedor;
          DELETE dbo.tbl_UbicacionAlmacen WHERE ID_UbicacionA=@ubicacion;
          COMMIT TRANSACTION;", conexion);
        comando.Parameters.AddWithValue("@actor", actor); comando.Parameters.AddWithValue("@cliente", clienteProceso);
        comando.Parameters.AddWithValue("@proveedor", proveedorProceso); comando.Parameters.AddWithValue("@producto", productoProceso);
        comando.Parameters.AddWithValue("@ubicacion", ubicacionProceso);
        comando.ExecuteNonQuery();
        if (actor > 0) Usuarios.Eliminar(actor, gerente);
        Console.WriteLine("OK limpieza de documentos, lotes y catálogos exclusivamente propios de la prueba " + Prefijo + ".");
    }
    private static async Task<int> ProbarProcesosWeb(string? url)
    {
        UsuarioSesion? gerente = null;
        try
        {
            servidor = url ?? "http://127.0.0.1:5188";
            string claveGerente = Environment.GetEnvironmentVariable("PRUEBA_WEB_GERENTE_PASSWORD") ?? throw new InvalidOperationException("Falta contraseña de prueba en variable de entorno.");
            string claveCajero = Environment.GetEnvironmentVariable("PRUEBA_WEB_CAJERO_PASSWORD") ?? throw new InvalidOperationException("Falta contraseña de prueba en variable de entorno.");
            gerente = Usuarios.IniciarSesion("gerente", claveGerente) ?? throw new InvalidOperationException("No autentica Gerente.");
            string claveTemporal = Guid.NewGuid().ToString("N") + "A1!";
            int rol = new RolNegocio().ListarRoles().Single(r => r.NombreRol == "Digitador").ID_Rol;
            Usuarios.Guardar(new Usuario { NombreUsuario = Prefijo, Nombres = "Prueba procesos", Apellidos = "Temporal", ID_Rol = rol }, claveTemporal, gerente, true);
            actor = Usuarios.Listar(gerente).Single(u => u.NombreUsuario == Prefijo).ID_Usuario;
            UsuarioSesion digitador = Usuarios.IniciarSesion(Prefijo, claveTemporal)!;
            PrepararProcesos(gerente);
            using var anonimo = ClienteHttp(); using var gerenteHttp = ClienteHttp(); using var cajero = ClienteHttp(); using var http = ClienteHttp();
            await Entrar(gerenteHttp, "gerente", claveGerente); await Entrar(cajero, "cajero", claveCajero); await Entrar(http, Prefijo, claveTemporal);
            Console.WriteLine("PROCESOS WEB | " + DateTime.Now.ToString("s"));
            await SeguridadProcesos(anonimo, gerenteHttp, cajero, http);
            await CotizacionesYOrdenes(http, digitador);
            Console.WriteLine("RESULTADO: cotizaciones, compras e ingresos parciales/completos verificados por HTTP y SQL Server.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
        finally { if (gerente != null) LimpiarProcesos(gerente); }
    }
}
