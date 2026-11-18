namespace ProyectoBD.Entidades;

public class Cliente
{
    public int ID_Cliente
    {
        get; set;
    }
    public string TipoCliente { get; set; } = string.Empty;
    public string NombreRazonSocial { get; set; } = string.Empty;
}
