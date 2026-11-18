using ProyectoBD.Entidades;
using ProyectoBD.Datos;
namespace ProyectoBD.Negocio;

public class UbicacionNegocio
{
    private readonly UbicacionDatos datos = new();
    public List<Ubicacion> Listar() => datos.Listar();
    private static void Validar(Ubicacion item)
    {
        if (string.IsNullOrWhiteSpace(item.NombreUbicacion) || item.NombreUbicacion.Trim().Length > 45)
            throw new ArgumentException("NombreUbicacion: ingrese entre 1 y 45 caracteres.");
        item.NombreUbicacion = item.NombreUbicacion.Trim();
        if (string.IsNullOrWhiteSpace(item.Descripcion) || item.Descripcion.Trim().Length > 45)
            throw new ArgumentException("Descripcion: ingrese entre 1 y 45 caracteres.");
        item.Descripcion = item.Descripcion.Trim();
    }
    public bool Guardar(Ubicacion item)
    {
        Validar(item);
        return datos.Insertar(item);
    }
    public bool Actualizar(Ubicacion item)
    {
        Validar(item);
        if (item.ID_UbicacionA <= 0)
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
