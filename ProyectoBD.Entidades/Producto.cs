namespace ProyectoBD.Entidades;

public class Producto
{
    public int ID_Producto
    {
        get; set;
    }
    public string NombreProducto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int ID_CategoriaP
    {
        get; set;
    }
    public int ID_Marca
    {
        get; set;
    }
}
public class ProductoVariante
{
    public int ID_ProductoV
    {
        get; set;
    }
    public decimal PrecioVenta
    {
        get; set;
    }
    public int ID_Producto
    {
        get; set;
    }
    public int ID_Presentacion
    {
        get; set;
    }
    public int? ID_Color
    {
        get; set;
    }
    public string DescripcionVariante { get; set; } = string.Empty;
}
