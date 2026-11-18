using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoBD.Datos;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Presentacion;

internal static partial class Program
{
    private static DataTable TablaFinal(string sql, params SqlParameter[] parametros)
    {
        using var conexion = new ConexionBD().CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddRange(parametros);
        using var adaptador = new SqlDataAdapter(comando);
        var tabla = new DataTable();
        adaptador.Fill(tabla);
        return tabla;
    }
    private static UsuarioSesion SesionFinal(string rol)
    {
        var fila = TablaFinal(@"SELECT TOP(1) u.ID_Usuario,u.NombreUsuario,u.Nombres,u.Apellidos,u.ID_Rol,r.NombreRol
            FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE r.NombreRol=@rol ORDER BY u.ID_Usuario",
            new SqlParameter("@rol", SqlDbType.NVarChar, 45) { Value = rol }).Rows[0];
        return new UsuarioSesion { ID_Usuario = (int)fila["ID_Usuario"], ID_Rol = (int)fila["ID_Rol"],
            NombreRol = rol, NombreUsuario = (string)fila["NombreUsuario"], NombreCompleto = fila["Nombres"] + " " + fila["Apellidos"] };
    }
    private static void DebeFallarFinal(Action accion, string caso)
    {
        bool fallo = false;
        try { accion(); }
        catch (ArgumentException) { fallo = true; }
        catch (InvalidOperationException) { fallo = true; }
        Afirmar(fallo, "No bloqueó: " + caso);
        Console.WriteLine("OK rechazo: " + caso);
    }
    private static decimal NumeroFinal(DataTable tabla, string columna)
    {
        return tabla.AsEnumerable().Sum(f => Convert.ToDecimal(f[columna]));
    }
    private static DataTable LotesFinal(int id)
    {
        return TablaFinal(@"SELECT l.ID_Lote,l.NumeroLote,l.CantidadDisponible,fl.CantidadSalida,d.Cantidad,d.PrecioUnitario,d.Subtotal,p.NombreProducto
            FROM dbo.tbl_DetalleFactura d JOIN dbo.tbl_DetalleFacturaLote fl ON fl.ID_DetalleF=d.ID_DetalleF
            JOIN dbo.tbl_LoteInventario l ON l.ID_Lote=fl.ID_Lote
            JOIN dbo.tbl_ProductoVariante v ON v.ID_ProductoV=d.ID_ProductoV JOIN dbo.tbl_Producto p ON p.ID_Producto=v.ID_Producto
            WHERE d.ID_Factura=@id ORDER BY l.ID_Lote", new SqlParameter("@id", id));
    }
    private static string EstadoFinal(int id)
    {
        var tabla = TablaFinal(@"SELECT f.Estado,f.Total,
            (SELECT SUM(l.CantidadDisponible) FROM dbo.tbl_LoteInventario l WHERE l.ID_ProductoV IN(SELECT ID_ProductoV FROM dbo.tbl_DetalleFactura WHERE ID_Factura=f.ID_Factura)) Stock,
            (SELECT COUNT(*) FROM dbo.tbl_DetalleFactura WHERE ID_Factura=f.ID_Factura) Detalles,
            (SELECT COUNT(*) FROM dbo.tbl_DetalleFacturaLote fl JOIN dbo.tbl_DetalleFactura d ON d.ID_DetalleF=fl.ID_DetalleF WHERE d.ID_Factura=f.ID_Factura) Asignaciones,
            (SELECT COUNT(*) FROM dbo.tbl_PagoFactura WHERE ID_Factura=f.ID_Factura) Pagos,
            (SELECT COUNT(*) FROM dbo.tbl_MovimientoInventario WHERE TipoMovimiento='ENTRADA_ANULACION' AND TipoReferencia='FACTURA' AND ID_Referencia=f.ID_Factura) Reintegros,
            (SELECT COUNT(*) FROM dbo.tbl_Auditoria WHERE TablaAfectada='tbl_Factura' AND RegistroAfectado=CONVERT(nvarchar(60),f.ID_Factura) AND Accion='ANULAR FACTURA') Auditorias
            FROM dbo.tbl_Factura f WHERE ID_Factura=@id", new SqlParameter("@id", id));
        return string.Join("|", tabla.Rows[0].ItemArray);
    }
    private static void ReportesFinal(ReporteNegocio negocio)
    {
        DateTime inicio = new DateTime(2000, 1, 1), fin = DateTime.Today;
        DataSet facturacion = negocio.ReporteFacturacion(inicio, fin);
        DataTable mayores = negocio.ReporteProductosMayoresIngresos(inicio, fin);
        DataTable mas = negocio.ReporteProductosMasVendidos(inicio, fin);
        DataTable inventario = negocio.ReporteInventario();
        DataTable menos = negocio.ReporteProductosMenosVendidos(inicio, fin);
        DataTable sin = negocio.ReporteProductosSinExistencia();
        decimal totalBD = Convert.ToDecimal(TablaFinal(@"SELECT ISNULL(SUM(Total),0) Total FROM dbo.tbl_Factura
            WHERE Estado<>'Anulada' AND CONVERT(date,FechaFactura) BETWEEN '20000101' AND CONVERT(date,SYSDATETIME())").Rows[0][0]);
        Afirmar(NumeroFinal(facturacion.Tables[0], "TotalFacturado") == totalBD, "Total facturado incorrecto.");
        Afirmar(NumeroFinal(mayores, "IngresoTotal") == totalBD, "Ingresos no coinciden con facturación.");
        Afirmar(NumeroFinal(mayores, "CantidadVendida") == NumeroFinal(mas, "CantidadVendida"), "Cantidades distintas entre reportes.");
        Afirmar(NumeroFinal(menos, "CantidadVendida") == NumeroFinal(mas, "CantidadVendida"), "Menos vendidos omite ventas.");
        Afirmar(NumeroFinal(facturacion.Tables[1], "TotalPagado") == NumeroFinal(facturacion.Tables[0], "TotalPagado"), "Pagos duplicados por cruces.");
        Afirmar(NumeroFinal(inventario, "CantidadDisponible") == Convert.ToDecimal(TablaFinal("SELECT ISNULL(SUM(CONVERT(bigint,CantidadDisponible)),0) FROM dbo.tbl_LoteInventario WHERE CantidadDisponible>0").Rows[0][0]), "Inventario distinto de lotes reales.");
        Afirmar(sin.AsEnumerable().All(r => Convert.ToInt64(r["CantidadDisponible"]) == 0), "Reporte sin stock tiene cantidades positivas.");
        Afirmar(mayores.AsEnumerable().Select(r => Convert.ToDecimal(r["IngresoTotal"])).SequenceEqual(mayores.AsEnumerable().Select(r => Convert.ToDecimal(r["IngresoTotal"])).OrderByDescending(x => x)), "Ingresos no ordenados.");
        Afirmar(mas.AsEnumerable().Select(r => Convert.ToInt64(r["CantidadVendida"])).SequenceEqual(mas.AsEnumerable().Select(r => Convert.ToInt64(r["CantidadVendida"])).OrderByDescending(x => x)), "Más vendidos no ordenados.");
        Afirmar(menos.AsEnumerable().Select(r => Convert.ToInt64(r["CantidadVendida"])).SequenceEqual(menos.AsEnumerable().Select(r => Convert.ToInt64(r["CantidadVendida"])).OrderBy(x => x)), "Menos vendidos no ordenados.");
        foreach (DataRow f in TablaFinal("SELECT NumeroFactura FROM dbo.tbl_Factura").Rows)
        {
            DataSet detalle = negocio.BuscarFactura((int)f[0]);
            Afirmar(detalle.Tables.Count == 4 && detalle.Tables[0].Rows.Count == 1 && detalle.Tables[1].Rows.Count > 0, "Consulta detallada incompleta.");
        }
        Console.WriteLine($"OK siete reportes actuales: total Q{totalBD:N2}; ingresos={mayores.Rows.Count}; más vendidos={mas.Rows.Count}; lotes={inventario.Rows.Count}; menos vendidos={menos.Rows.Count}; sin stock={sin.Rows.Count}; todas las facturas consultables.");
        DateTime vacio = new DateTime(1900, 1, 1);
        Afirmar(NumeroFinal(negocio.ReporteFacturacion(vacio, vacio).Tables[0], "TotalFacturado") == 0, "Período vacío incorrecto.");
        Afirmar(negocio.ReporteProductosMayoresIngresos(vacio, vacio).Rows.Count == 0, "Ingresos fuera del período.");
        Afirmar(negocio.ReporteProductosMasVendidos(vacio, vacio).Rows.Count == 0, "Ventas fuera del período.");
        Afirmar(negocio.ReporteProductosMenosVendidos(vacio, vacio).AsEnumerable().All(r => Convert.ToInt64(r["CantidadVendida"]) == 0), "No incluye variantes con cero ventas.");
        Console.WriteLine("OK período vacío, ordenamientos y productos sin ventas.");
    }
    private static void ProbarFaseFinal()
    {
        Console.WriteLine("FASE FINAL | " + DateTime.Now.ToString("s"));
        var gerente = SesionFinal("Gerente");
        var negocio = new ReporteNegocio(gerente);
        ReportesFinal(negocio);
        DebeFallarFinal(() => negocio.ReporteFacturacion(DateTime.Today.AddDays(1), DateTime.Today), "fechas invertidas");
        DebeFallarFinal(() => negocio.BuscarFactura(0), "número inválido");
        DebeFallarFinal(() => negocio.AnularFactura(int.MaxValue), "factura inexistente");
        Afirmar(negocio.BuscarFactura(int.MaxValue).Tables[0].Rows.Count == 0, "Devuelve factura inexistente.");
        foreach (string rol in new[] { "Digitador", "Cajero" })
        {
            var n = new ReporteNegocio(SesionFinal(rol));
            foreach (Action consulta in new Action[] {
                () => n.ReporteFacturacion(DateTime.Today, DateTime.Today), () => n.ReporteProductosMayoresIngresos(DateTime.Today, DateTime.Today),
                () => n.ReporteProductosMasVendidos(DateTime.Today, DateTime.Today), () => n.ReporteInventario(),
                () => n.ReporteProductosMenosVendidos(DateTime.Today, DateTime.Today), () => n.ReporteProductosSinExistencia(),
                () => n.BuscarFactura(1), () => n.AnularFactura(1) })
                DebeFallarFinal(consulta, "permiso " + rol);
        }
        DataRow inicial = negocio.ReporteFacturacion(DateTime.Today, DateTime.Today).Tables[0].Rows[0];
        decimal ventasIniciales = Convert.ToDecimal(inicial["TotalFacturado"]), pagosIniciales = Convert.ToDecimal(inicial["TotalPagado"]);
        var caso = TablaFinal(File.ReadAllText("Database/PruebasFaseFinal.sql"),
            new SqlParameter("@Sufijo", SqlDbType.NVarChar, 12) { Value = DateTime.Now.ToString("MMddHHmmss") }).Rows[0];
        int id = (int)caso["ID_Factura"], numero = (int)caso["NumeroFactura"];
        Console.WriteLine("Caso creado: ID=" + id + "; número=" + numero + "; " + EstadoFinal(id));
        DataTable lotes = LotesFinal(id);
        Afirmar(lotes.Rows.Count == 2 && lotes.AsEnumerable().All(r => (int)r["CantidadSalida"] == 2), "PEPS no utilizó dos lotes (2 + 2).");
        foreach (DataRow lote in lotes.Rows)
            Console.WriteLine($"ANTES: producto={lote["NombreProducto"]}; lote={lote["NumeroLote"]}; disponible={lote["CantidadDisponible"]}; vendido={lote["CantidadSalida"]}; cantidad línea={lote["Cantidad"]}; precio=Q{lote["PrecioUnitario"]}; subtotal=Q{lote["Subtotal"]}");
        DataRow durante = negocio.ReporteFacturacion(DateTime.Today, DateTime.Today).Tables[0].Rows[0];
        Afirmar(Convert.ToDecimal(durante["TotalFacturado"]) == ventasIniciales + 40 && Convert.ToDecimal(durante["TotalPagado"]) == pagosIniciales + 17, "Pagos mixtos duplican facturación.");
        string antes = EstadoFinal(id);
        // El trigger sólo existe dentro de esta transacción de prueba. Su error provoca el rollback completo.
        using (var conexion = new ConexionBD().CrearConexion())
        {
            conexion.Open();
            using var comando = new SqlCommand(@"BEGIN TRANSACTION;
                EXEC(N'CREATE TRIGGER dbo.__PruebaRollbackFaseFinal ON dbo.tbl_Auditoria AFTER INSERT AS
                BEGIN IF EXISTS(SELECT 1 FROM inserted WHERE Accion=N''ANULAR FACTURA'') THROW 51099,N''Error simulado de auditoría'',1; END');
                EXEC dbo.usp_AnularFactura @numero,@usuario;", conexion);
            comando.Parameters.AddWithValue("@numero", numero);
            comando.Parameters.AddWithValue("@usuario", gerente.ID_Usuario);
            bool rollback = false;
            try { comando.ExecuteNonQuery(); }
            catch (SqlException ex) when (ex.Number == 51099) { rollback = true; }
            finally { using var limpiar = new SqlCommand("IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;", conexion); limpiar.ExecuteNonQuery(); }
            Afirmar(rollback, "No se produjo el error simulado.");
        }
        Afirmar(EstadoFinal(id) == antes && LotesFinal(id).AsEnumerable().Select(r => (int)r["CantidadDisponible"]).SequenceEqual(lotes.AsEnumerable().Select(r => (int)r["CantidadDisponible"])), "Rollback no restauró todo.");
        Afirmar(TablaFinal("SELECT OBJECT_ID('dbo.__PruebaRollbackFaseFinal')").Rows[0].IsNull(0), "Quedó trigger de prueba.");
        Console.WriteLine("OK rollback por error DESPUÉS de reintegrar lotes, insertar movimientos y actualizar factura: estado, total, stock, pagos, detalles y auditoría intactos; trigger retirado por rollback.");
        foreach (string rol in new[] { "Digitador", "Cajero" })
            DebeFallarFinal(() => new ReporteDatos().AnularFactura(numero, SesionFinal(rol).ID_Usuario), "permiso SQL " + rol);
        Afirmar(EstadoFinal(id) == antes, "Rechazo de permisos alteró datos.");
        negocio.AnularFactura(numero);
        DataSet anulada = negocio.BuscarFactura(numero);
        DataRow encabezado = anulada.Tables[0].Rows[0];
        Afirmar((string)encabezado["Estado"] == "Anulada" && (decimal)encabezado["Total"] == 0 && (decimal)encabezado["SaldoPorCobrar"] == 0, "Factura no quedó anulada en cero.");
        Afirmar((decimal)encabezado["TotalOriginal"] == 40 && (decimal)encabezado["PagosHistoricos"] == 17 && anulada.Tables[1].Rows.Count == 1 && anulada.Tables[2].Rows.Count == 4, "No conserva historial.");
        DataTable despues = LotesFinal(id);
        for (int i = 0; i < lotes.Rows.Count; i++)
        {
            Afirmar((int)despues.Rows[i]["CantidadDisponible"] == (int)lotes.Rows[i]["CantidadDisponible"] + (int)lotes.Rows[i]["CantidadSalida"], "Reintegro inexacto.");
            Console.WriteLine("DESPUÉS lote " + despues.Rows[i]["NumeroLote"] + ": disponible=" + despues.Rows[i]["CantidadDisponible"]);
        }
        var movimientos = TablaFinal("SELECT ID_Lote,Cantidad,ID_Usuario FROM dbo.tbl_MovimientoInventario WHERE TipoMovimiento='ENTRADA_ANULACION' AND TipoReferencia='FACTURA' AND ID_Referencia=@id ORDER BY ID_Lote", new SqlParameter("@id", id));
        Afirmar(movimientos.Rows.Count == 2 && movimientos.AsEnumerable().All(r => (int)r["Cantidad"] == 2 && (int)r["ID_Usuario"] == gerente.ID_Usuario), "Movimientos de reintegro incorrectos.");
        Afirmar(anulada.Tables[3].AsEnumerable().Count(r => (string)r["Accion"] == "ANULAR FACTURA" && !r.IsNull("DatosAnteriores") && !r.IsNull("DatosNuevos")) == 1, "Falta auditoría anterior/nueva.");
        string estadoAnulado = EstadoFinal(id);
        DebeFallarFinal(() => negocio.AnularFactura(numero), "segunda anulación");
        Afirmar(EstadoFinal(id) == estadoAnulado, "Segunda anulación modificó datos.");
        DataRow final = negocio.ReporteFacturacion(DateTime.Today, DateTime.Today).Tables[0].Rows[0];
        Afirmar((decimal)final["TotalFacturado"] == ventasIniciales && (decimal)final["TotalPagado"] == pagosIniciales, "Reportes incluyen anulada/pagos históricos.");
        Afirmar(!negocio.ReporteProductosMayoresIngresos(DateTime.Today, DateTime.Today).AsEnumerable().Any(r => ((string)r["Producto"]).StartsWith("Prueba final ") && ((string)r["Producto"]) == (string)lotes.Rows[0]["NombreProducto"]), "Ingresos incluyen anulada.");
        Afirmar(negocio.ReporteProductosMenosVendidos(DateTime.Today, DateTime.Today).AsEnumerable().Any(r => (string)r["Producto"] == (string)lotes.Rows[0]["NombreProducto"] && Convert.ToInt64(r["CantidadVendida"]) == 0), "Menos vendidos incluye venta anulada.");
        Console.WriteLine("OK anulación por número, reintegro 2+2, historial, pagos, responsable, auditoría y segunda anulación bloqueada: " + estadoAnulado);
        Afirmar(new ProcesoNegocio(gerente).Facturas().AsEnumerable().Single(r => (int)r["ID_Factura"] == id).Field<decimal>("Saldo") == 0, "Listado anterior muestra saldo negativo.");
        ReportesFinal(negocio);
        ProbarPantallasFinal(gerente, numero);
        Console.WriteLine("RESULTADO: TODAS LAS PRUEBAS DE FASE FINAL CORRECTAS. Se conserva el caso de prueba anulado y sus lotes/documentos como historial.");
    }

    private static T ControlFinal<T>(Form formulario, string nombre) where T : Control
    {
        return (T)formulario.Controls.Find(nombre, true).Single();
    }
    private static void ProbarPantallasFinal(UsuarioSesion gerente, int numero)
    {
        ApplicationConfiguration.Initialize();
        Directory.CreateDirectory("Documentacion/CapturasFaseFinal");
        using var limite = new System.Threading.Timer(_ => Environment.Exit(2), null, 60000, Timeout.Infinite);
        using (var reportes = new Reportes(gerente))
        {
            reportes.StartPosition = FormStartPosition.Manual; reportes.Location = new System.Drawing.Point(-2500, -2500);
            reportes.Show(); Application.DoEvents();
            ControlFinal<DateTimePicker>(reportes, "dateTimePicker1").Value = new DateTime(2000, 1, 1);
            for (int i = 0; i < 6; i++)
            {
                ControlFinal<ComboBox>(reportes, "comboBox1").SelectedIndex = i;
                ControlFinal<Button>(reportes, "button2").PerformClick(); Application.DoEvents();
                Afirmar(ControlFinal<DataGridView>(reportes, "dataGridView1").DataSource != null, "No enlaza reporte " + (i + 1));
                using var imagen = new System.Drawing.Bitmap(reportes.Width, reportes.Height);
                reportes.DrawToBitmap(imagen, new System.Drawing.Rectangle(0, 0, imagen.Width, imagen.Height));
                imagen.Save("Documentacion/CapturasFaseFinal/Reporte" + (i + 1) + ".png");
            }
            ControlFinal<Button>(reportes, "button1").PerformClick();
            Afirmar(!reportes.Visible, "Volver no cierra Reportes.");
        }
        using (var consulta = new ConsultaFactura(gerente, true))
        {
            consulta.StartPosition = FormStartPosition.Manual; consulta.Location = new System.Drawing.Point(-2500, -2500);
            consulta.Show(); Application.DoEvents();
            ControlFinal<TextBox>(consulta, "textBox1").Text = numero.ToString();
            ControlFinal<Button>(consulta, "button1").PerformClick(); Application.DoEvents();
            Afirmar(ControlFinal<Label>(consulta, "label2").Text.Contains("Anulada") && ControlFinal<Label>(consulta, "label4").Text.Contains("Total actual Q0.00"), "No muestra anulación en UI.");
            Afirmar(!ControlFinal<Button>(consulta, "button2").Enabled, "UI permite anular de nuevo.");
            using var imagen = new System.Drawing.Bitmap(consulta.Width, consulta.Height);
            consulta.DrawToBitmap(imagen, new System.Drawing.Rectangle(0, 0, imagen.Width, imagen.Height));
            imagen.Save("Documentacion/CapturasFaseFinal/FacturaAnulada.png");
            ControlFinal<TextBox>(consulta, "textBox1").Text = "1";
            Afirmar(ControlFinal<DataGridView>(consulta, "dataGridView1").DataSource == null, "Conserva selección obsoleta al editar número.");
            ControlFinal<Button>(consulta, "button3").PerformClick();
            Afirmar(!consulta.Visible, "Volver no cierra ConsultaFactura.");
        }
        foreach (string rol in new[] { "Digitador", "Cajero", "Gerente" })
        {
            using var menu = new ProyectoBD.Presentacion.Menu(SesionFinal(rol));
            menu.StartPosition = FormStartPosition.Manual; menu.Location = new System.Drawing.Point(-2500, -2500);
            menu.Show(); Application.DoEvents();
            var archivo = ControlFinal<MenuStrip>(menu, "menuStrip1").Items.OfType<ToolStripMenuItem>().Single(i => i.Name == "toolStripMenuItem2");
            var reportes = archivo.DropDownItems.OfType<ToolStripMenuItem>().Single(i => i.Name == "toolStripMenuItem7");
            Afirmar(reportes.Available == (rol == "Gerente"), "Menú no respeta rol " + rol);
            Afirmar(reportes.DropDownItems.OfType<ToolStripMenuItem>().Count(i => new[] { "toolStripMenuItem29", "toolStripMenuItem30", "toolStripMenuItem31" }.Contains(i.Name)) == 3, "Faltan opciones gerenciales.");
            menu.Close();
        }
        Console.WriteLine("OK WinForms: seis reportes, consulta anulada con pagos/auditoría, botones Volver, selección invalidada al editar, menús por rol; capturas guardadas.");
    }
    private static void MostrarFaseFinal()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new ProyectoBD.Presentacion.Menu(SesionFinal("Gerente")));
    }
    private static void ValidarPantallasFinal()
    {
        var gerente = SesionFinal("Gerente");
        ReportesFinal(new ReporteNegocio(gerente));
        int numero = (int)TablaFinal(@"SELECT TOP(1) f.NumeroFactura FROM dbo.tbl_Factura f
            JOIN dbo.tbl_DetalleFactura d ON d.ID_Factura=f.ID_Factura
            JOIN dbo.tbl_ProductoVariante v ON v.ID_ProductoV=d.ID_ProductoV
            JOIN dbo.tbl_Producto p ON p.ID_Producto=v.ID_Producto
            WHERE f.Estado='Anulada' AND p.NombreProducto LIKE 'Prueba final %' ORDER BY f.ID_Factura DESC").Rows[0][0];
        ProbarPantallasFinal(gerente, numero);
    }
}
