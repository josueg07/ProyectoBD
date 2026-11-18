namespace ProyectoBD.Entidades;

public class Categoria
{
    public int ID_Categoria
    {
        get; set;
    }
    public string NombreCategoria { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
