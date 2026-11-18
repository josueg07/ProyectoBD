using ProyectoBD.Entidades;
using ProyectoBD.Datos;
namespace ProyectoBD.Negocio;

public class MarcaNegocio
{
    private readonly MarcaDatos datos = new();
    public List<Marca> Listar() => datos.Listar();
    private static void Validar(Marca item)
    {
        if (string.IsNullOrWhiteSpace(item.NombreMarca) || item.NombreMarca.Trim().Length > 45)
            throw new ArgumentException("NombreMarca: ingrese entre 1 y 45 caracteres.");
        item.NombreMarca = item.NombreMarca.Trim();
    }
    public bool Guardar(Marca item)
    {
        Validar(item);
        return datos.Insertar(item);
    }
    public bool Actualizar(Marca item)
    {
        Validar(item);
        if (item.ID_Marca <= 0)
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
