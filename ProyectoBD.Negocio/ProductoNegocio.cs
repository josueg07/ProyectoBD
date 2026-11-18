using ProyectoBD.Entidades;
using ProyectoBD.Datos;
namespace ProyectoBD.Negocio;

public class ProductoNegocio
{
    private readonly ProductoDatos datos = new();
    public List<Producto> Listar() => datos.Listar();
    public List<ProductoVariante> ListarVariantes(int? producto = null) => datos.ListarVariantes(producto);
    public bool Guardar(Producto p, bool nuevo)
    {
        if (string.IsNullOrWhiteSpace(p.NombreProducto) || p.NombreProducto.Trim().Length > 45 || string.IsNullOrWhiteSpace(p.Descripcion) || p.Descripcion.Trim().Length > 45 || p.ID_CategoriaP <= 0 || p.ID_Marca <= 0 || (!nuevo && p.ID_Producto <= 0))
            throw new ArgumentException("Seleccione categoría y marca; nombre y descripción requieren entre 1 y 45 caracteres.");
        p.NombreProducto = p.NombreProducto.Trim();
        p.Descripcion = p.Descripcion.Trim();
        return datos.Guardar(p, nuevo);
    }
    public bool GuardarVariante(ProductoVariante v, bool nuevo)
    {
        if (v.ID_Producto <= 0 || v.ID_Presentacion <= 0 || v.PrecioVenta <= 0 || v.PrecioVenta > 9999999999.99m || decimal.Round(v.PrecioVenta, 2) != v.PrecioVenta || (!nuevo && v.ID_ProductoV <= 0))
            throw new ArgumentException("Seleccione producto y presentación e ingrese un precio positivo con dos decimales.");
        return datos.GuardarVariante(v, nuevo);
    }
    public bool Eliminar(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Seleccione un producto.");
        return datos.Eliminar(id);
    }
    public bool EliminarVariante(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Seleccione una variante.");
        return datos.EliminarVariante(id);
    }
}
