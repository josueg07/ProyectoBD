using ProyectoBD.Entidades;
using ProyectoBD.Datos;
namespace ProyectoBD.Negocio;

public class CategoriaNegocio
{
    private readonly CategoriaDatos datos = new();
    public List<Categoria> Listar() => datos.Listar();
    private static void Validar(Categoria item)
    {
        if (string.IsNullOrWhiteSpace(item.NombreCategoria) || item.NombreCategoria.Trim().Length > 45)
            throw new ArgumentException("NombreCategoria: ingrese entre 1 y 45 caracteres.");
        item.NombreCategoria = item.NombreCategoria.Trim();
        if (string.IsNullOrWhiteSpace(item.Descripcion) || item.Descripcion.Trim().Length > 60)
            throw new ArgumentException("Descripcion: ingrese entre 1 y 60 caracteres.");
        item.Descripcion = item.Descripcion.Trim();
    }
    public bool Guardar(Categoria item)
    {
        Validar(item);
        return datos.Insertar(item);
    }
    public bool Actualizar(Categoria item)
    {
        Validar(item);
        if (item.ID_Categoria <= 0)
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
