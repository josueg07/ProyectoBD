using ProyectoBD.Entidades;
using ProyectoBD.Datos;
namespace ProyectoBD.Negocio;

public class MetodoPagoNegocio
{
    private readonly MetodoPagoDatos datos = new();
    public List<MetodoPago> Listar() => datos.Listar();
    private static void Validar(MetodoPago item)
    {
        if (string.IsNullOrWhiteSpace(item.NombreMetodo) || item.NombreMetodo.Trim().Length > 45)
            throw new ArgumentException("NombreMetodo: ingrese entre 1 y 45 caracteres.");
        item.NombreMetodo = item.NombreMetodo.Trim();
    }
    public bool Guardar(MetodoPago item)
    {
        Validar(item);
        return datos.Insertar(item);
    }
    public bool Actualizar(MetodoPago item)
    {
        Validar(item);
        if (item.ID_MetodoP <= 0)
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
