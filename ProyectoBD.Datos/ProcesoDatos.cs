using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using ProyectoBD.Entidades;
using static ProyectoBD.Datos.OperacionDatos;
namespace ProyectoBD.Datos;

public class ProcesoDatos
{
    private static int Procedimiento(string nombre, params SqlParameter[] parametros)
    {
        try
        {
            using var c = new ConexionBD().CrearConexion();
            c.Open();
            using var cmd = new SqlCommand(nombre, c) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddRange(parametros);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        catch (SqlException ex) when (ex.Number >= 50000) { throw new InvalidOperationException(ex.Message, ex); }
        catch (SqlException ex) when (ex.Number == 547) { throw new InvalidOperationException("El cliente, producto u otro dato relacionado ya no existe.", ex); }
    }
    public int GuardarCotizacion(Cotizacion c, int actor) => Procedimiento("dbo.usp_GuardarCotizacion", Parametro("@ID_Cliente", c.ID_Cliente), Parametro("@ID_Usuario", actor), Parametro("@Detalle", JsonSerializer.Serialize(c.Detalle)));
    public void EstadoCotizacion(int id, string estado, int actor) => Procedimiento("dbo.usp_EstadoCotizacion", Parametro("@ID_Cotizacion", id), Parametro("@Estado", estado), Parametro("@ID_Usuario", actor));
    public int GuardarOrden(OrdenCompra o, int actor) => Procedimiento("dbo.usp_GuardarOrdenCompra", Parametro("@ID_Proveedor", o.ID_Proveedor), Parametro("@ID_Usuario", actor), Parametro("@Detalle", JsonSerializer.Serialize(o.Detalle)));
    public int Ingresar(IngresoBodega i, int actor) => Procedimiento("dbo.usp_RegistrarIngresoBodega", Parametro("@ID_OrdenC", i.ID_OrdenC), Parametro("@ID_Usuario", actor), Parametro("@ID_UbicacionA", i.ID_UbicacionA), Parametro("@FechaIngreso", i.FechaIngreso), Parametro("@Detalle", JsonSerializer.Serialize(i.Detalle)));
    public int Facturar(Factura f, int actor) => Procedimiento("dbo.usp_FacturarPEPS", Parametro("@ID_Cliente", f.ID_Cliente), Parametro("@ID_Usuario", actor), new SqlParameter("@ID_Cotizacion", SqlDbType.Int) { Value = (object?)f.ID_Cotizacion ?? DBNull.Value }, Parametro("@Detalle", JsonSerializer.Serialize(f.Detalle)));
    public void Pagar(int factura, List<PagoFactura> pagos, int actor) => Procedimiento("dbo.usp_RegistrarPagos", Parametro("@ID_Factura", factura), Parametro("@ID_Usuario", actor), Parametro("@Pagos", JsonSerializer.Serialize(pagos)));
    public DataTable Cotizaciones() => Tabla("SELECT q.ID_Cotizacion,q.FechaCotizacion,q.Estado,q.Total,q.ID_Cliente,c.NombreRazonSocial FROM dbo.tbl_Cotizacion q JOIN dbo.tbl_Clientes c ON c.ID_Cliente=q.ID_Cliente ORDER BY q.ID_Cotizacion DESC");
    public DataTable DetalleCotizacion(int id) => Tabla(@"SELECT d.ID_ProductoV,CONCAT(p.NombreProducto,' / ',pr.NombrePresentacion,' / ',COALESCE(c.NombreColor,'Sin color')) Producto,d.Cantidad,d.PrecioUnitario,d.Subtotal FROM dbo.tbl_DetalleCotizacion d JOIN dbo.tbl_ProductoVariante v ON v.ID_ProductoV=d.ID_ProductoV JOIN dbo.tbl_Producto p ON p.ID_Producto=v.ID_Producto JOIN dbo.tbl_Presentacion pr ON pr.ID_Presentacion=v.ID_Presentacion LEFT JOIN dbo.tbl_Color c ON c.ID_Color=v.ID_Color WHERE d.ID_Cotizacion=@id", Parametro("@id", id));
    public DataTable Ordenes() => Tabla("SELECT o.ID_OrdenC,o.FechaOrden,o.Estado,p.Nombre_Proveedor FROM dbo.tbl_OrdenCompra o JOIN dbo.tbl_Proveedor p ON p.ID_Proveedor=o.ID_Proveedor ORDER BY o.ID_OrdenC DESC");
    public DataTable PendienteOrden(int id) => Tabla(@"SELECT oc.ID_DetalleOC,oc.ID_ProductoV,CONCAT(p.NombreProducto,' / ',pr.NombrePresentacion,' / ',COALESCE(c.NombreColor,'Sin color')) Producto,oc.CantidadSolicitada,ISNULL(SUM(ib.CantidadRecibida),0) CantidadRecibida,oc.CantidadSolicitada-ISNULL(SUM(ib.CantidadRecibida),0) Pendiente FROM dbo.tbl_DetalleOrdenCompra oc JOIN dbo.tbl_ProductoVariante v ON v.ID_ProductoV=oc.ID_ProductoV JOIN dbo.tbl_Producto p ON p.ID_Producto=v.ID_Producto JOIN dbo.tbl_Presentacion pr ON pr.ID_Presentacion=v.ID_Presentacion LEFT JOIN dbo.tbl_Color c ON c.ID_Color=v.ID_Color LEFT JOIN dbo.tbl_DetalleIngresoBodega ib ON ib.ID_DetalleOC=oc.ID_DetalleOC WHERE oc.ID_OrdenC=@id GROUP BY oc.ID_DetalleOC,oc.ID_ProductoV,p.NombreProducto,pr.NombrePresentacion,c.NombreColor,oc.CantidadSolicitada ORDER BY oc.ID_DetalleOC", Parametro("@id", id));
    public DataTable Facturas() => Tabla(@"SELECT f.ID_Factura,f.NumeroFactura,f.FechaFactura,c.NombreRazonSocial Cliente,CONCAT(u.Nombres,' ',u.Apellidos) Usuario,f.Total,f.Estado,f.ID_Cotizacion,ISNULL((SELECT SUM(Monto) FROM dbo.tbl_PagoFactura WHERE ID_Factura=f.ID_Factura),0) Pagado,CASE WHEN f.Estado='Anulada' THEN CONVERT(decimal(12,2),0) ELSE f.Total-ISNULL((SELECT SUM(Monto) FROM dbo.tbl_PagoFactura WHERE ID_Factura=f.ID_Factura),0) END Saldo FROM dbo.tbl_Factura f JOIN dbo.tbl_Clientes c ON c.ID_Cliente=f.ID_Cliente JOIN dbo.tbl_Usuario u ON u.ID_Usuario=f.ID_Usuario ORDER BY f.ID_Factura DESC");
    public DataTable DetalleFactura(int id) => Tabla(@"SELECT CONCAT(p.NombreProducto,' / ',pr.NombrePresentacion,' / ',COALESCE(c.NombreColor,'Sin color')) Producto,d.Cantidad,d.PrecioUnitario,d.Subtotal FROM dbo.tbl_DetalleFactura d JOIN dbo.tbl_ProductoVariante v ON v.ID_ProductoV=d.ID_ProductoV JOIN dbo.tbl_Producto p ON p.ID_Producto=v.ID_Producto JOIN dbo.tbl_Presentacion pr ON pr.ID_Presentacion=v.ID_Presentacion LEFT JOIN dbo.tbl_Color c ON c.ID_Color=v.ID_Color WHERE d.ID_Factura=@id ORDER BY d.ID_DetalleF", Parametro("@id", id));
    public DataTable Pagos(int id) => Tabla("SELECT m.NombreMetodo,p.Monto,p.Referencia FROM dbo.tbl_PagoFactura p JOIN dbo.tbl_MetodoPago m ON m.ID_MetodoP=p.ID_MetodoP WHERE p.ID_Factura=@id ORDER BY p.ID_PagoF", Parametro("@id", id));
    public DataTable Inventario() => Tabla(@"SELECT l.ID_Lote,l.NumeroLote,l.FechaIngreso,p.NombreProducto,pr.NombrePresentacion,c.NombreColor,l.CantidadInicial,l.CantidadDisponible,l.CostoUnitario,u.NombreUbicacion FROM dbo.tbl_LoteInventario l JOIN dbo.tbl_ProductoVariante v ON v.ID_ProductoV=l.ID_ProductoV JOIN dbo.tbl_Producto p ON p.ID_Producto=v.ID_Producto JOIN dbo.tbl_Presentacion pr ON pr.ID_Presentacion=v.ID_Presentacion LEFT JOIN dbo.tbl_Color c ON c.ID_Color=v.ID_Color JOIN dbo.tbl_UbicacionAlmacen u ON u.ID_UbicacionA=l.ID_UbicacionA ORDER BY l.FechaIngreso,l.ID_Lote");
    public DataTable Ingresos() => Tabla("SELECT ib.ID_IngresoB,ib.FechaIngreso,ib.ID_OrdenC,CONCAT(u.Nombres,' ',u.Apellidos) Usuario,d.ID_DetalleOC,d.CantidadRecibida,d.CostoUnitario FROM dbo.tbl_IngresoBodega ib JOIN dbo.tbl_DetalleIngresoBodega d ON d.ID_IngresoB=ib.ID_IngresoB JOIN dbo.tbl_Usuario u ON u.ID_Usuario=ib.ID_Usuario ORDER BY ib.ID_IngresoB DESC");
}

