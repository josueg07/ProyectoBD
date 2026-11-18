using ProyectoBD.Entidades;
using ProyectoBD.Datos;
namespace ProyectoBD.Negocio;

public class ColorProductoNegocio
{
    private readonly ColorProductoDatos datos = new();
    public List<ColorProducto> Listar() => datos.Listar();
    private static void Validar(ColorProducto item)
    {
        if (string.IsNullOrWhiteSpace(item.NombreColor) || item.NombreColor.Trim().Length > 45)
            throw new ArgumentException("NombreColor: ingrese entre 1 y 45 caracteres.");
        item.NombreColor = item.NombreColor.Trim();
    }
    public bool Guardar(ColorProducto item)
    {
        Validar(item);
        return datos.Insertar(item);
    }
    public bool Actualizar(ColorProducto item)
    {
        Validar(item);
        if (item.ID_Color <= 0)
            throw new ArgumentException("Seleccione un registro.");
        return datos.Actualizar(item);
    }
    public bool Eliminar(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Seleccione un registro.");
        return datos.Eliminar(id);
    }
}
