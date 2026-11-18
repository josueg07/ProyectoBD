namespace ProyectoBD.Entidades;

public class LineaDocumento
{
    public int ID_ProductoV
    {
        get; set;
    }
    public string Producto { get; set; } = string.Empty;
    public int Cantidad
    {
        get; set;
    }
    public decimal PrecioUnitario
    {
        get; set;
    }
    public decimal Subtotal => Cantidad * PrecioUnitario;
}
public class Cotizacion
{
    public int ID_Cliente
    {
        get; set;
    }
    public List<LineaDocumento> Detalle { get; set; } = new();
}
public class OrdenCompra
{
    public int ID_Proveedor
    {
        get; set;
    }
    public List<LineaDocumento> Detalle { get; set; } = new();
}
public class LineaIngreso
{
    public int ID_DetalleOC
    {
        get; set;
    }
    public int CantidadRecibida
    {
        get; set;
    }
    public decimal CostoUnitario
    {
        get; set;
    }
    public string? NumeroLote
    {
        get; set;
    }
}
public class IngresoBodega
{
    public int ID_OrdenC
    {
        get; set;
    }
    public int ID_UbicacionA
    {
        get; set;
    }
    public DateTime FechaIngreso { get; set; } = DateTime.Now;
    public List<LineaIngreso> Detalle { get; set; } = new();
}
public class Factura
{
    public int ID_Cliente
    {
        get; set;
    }
    public int? ID_Cotizacion
    {
        get; set;
    }
    public List<LineaDocumento> Detalle { get; set; } = new();
}
public class PagoFactura
{
    public int ID_MetodoP
    {
        get; set;
    }
    public string Metodo { get; set; } = string.Empty;
    public decimal Monto
    {
        get; set;
    }
    public string Referencia { get; set; } = string.Empty;
}
