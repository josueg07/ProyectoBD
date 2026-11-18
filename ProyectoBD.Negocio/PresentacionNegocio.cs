using ProyectoBD.Entidades;
using ProyectoBD.Datos;
namespace ProyectoBD.Negocio;

public class PresentacionNegocio
{
    private readonly PresentacionDatos datos = new();
    public List<Presentacion> Listar() => datos.Listar();
    private static void Validar(Presentacion item)
    {
        if (string.IsNullOrWhiteSpace(item.NombrePresentacion) || item.NombrePresentacion.Trim().Length > 45)
            throw new ArgumentException("NombrePresentacion: ingrese entre 1 y 45 caracteres.");
        item.NombrePresentacion = item.NombrePresentacion.Trim();
        if (string.IsNullOrWhiteSpace(item.UnidadMedida) || item.UnidadMedida.Trim().Length > 45)
            throw new ArgumentException("UnidadMedida: ingrese entre 1 y 45 caracteres.");
        item.UnidadMedida = item.UnidadMedida.Trim();
    }
    public bool Guardar(Presentacion item)
    {
        Validar(item);
        return datos.Insertar(item);
    }
    public bool Actualizar(Presentacion item)
    {
        Validar(item);
        if (item.ID_Presentacion <= 0)
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
