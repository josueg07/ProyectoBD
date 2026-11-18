using ProyectoBD.Entidades;
using ProyectoBD.Datos;
namespace ProyectoBD.Negocio;

public class ProveedorNegocio
{
    private readonly ProveedorDatos datos = new();
    public List<Proveedor> Listar() => datos.Listar();
    private static void Validar(Proveedor item)
    {
        if (string.IsNullOrWhiteSpace(item.Nombre_Proveedor) || item.Nombre_Proveedor.Trim().Length > 45)
            throw new ArgumentException("Nombre_Proveedor: ingrese entre 1 y 45 caracteres.");
        item.Nombre_Proveedor = item.Nombre_Proveedor.Trim();
    }
    public bool Guardar(Proveedor item)
    {
        Validar(item);
        return datos.Insertar(item);
    }
    public bool Actualizar(Proveedor item)
    {
        Validar(item);
        if (item.ID_Proveedor <= 0)
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
