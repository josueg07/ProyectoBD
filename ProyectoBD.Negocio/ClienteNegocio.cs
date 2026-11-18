using ProyectoBD.Entidades;
using ProyectoBD.Datos;
namespace ProyectoBD.Negocio;

public class ClienteNegocio
{
    private readonly ClienteDatos datos = new();
    public List<Cliente> Listar() => datos.Listar();
    private static void Validar(Cliente item)
    {
        if (string.IsNullOrWhiteSpace(item.TipoCliente) || item.TipoCliente.Trim().Length > 45)
            throw new ArgumentException("TipoCliente: ingrese entre 1 y 45 caracteres.");
        item.TipoCliente = item.TipoCliente.Trim();
        if (string.IsNullOrWhiteSpace(item.NombreRazonSocial) || item.NombreRazonSocial.Trim().Length > 120)
            throw new ArgumentException("NombreRazonSocial: ingrese entre 1 y 120 caracteres.");
        item.NombreRazonSocial = item.NombreRazonSocial.Trim();
        if (item.TipoCliente != "Particular" && item.TipoCliente != "Empresa")
            throw new ArgumentException("Tipo de cliente inválido.");
    }
    public bool Guardar(Cliente item)
    {
        Validar(item);
        return datos.Insertar(item);
    }
    public bool Actualizar(Cliente item)
    {
        Validar(item);
        if (item.ID_Cliente <= 0)
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
