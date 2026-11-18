using System.Data;
using ProyectoBD.Entidades;
using static ProyectoBD.Datos.OperacionDatos;
namespace ProyectoBD.Datos;

public class ClienteDatos
{
    public List<Cliente> Listar() => Tabla("SELECT ID_Cliente,TipoCliente,NombreRazonSocial FROM dbo.tbl_Clientes ORDER BY ID_Cliente").AsEnumerable().Select(r => new Cliente { ID_Cliente = r.Field<int>("ID_Cliente"), TipoCliente = r.Field<string>("TipoCliente")!, NombreRazonSocial = r.Field<string>("NombreRazonSocial")! }).ToList();
    public bool Insertar(Cliente item) => Ejecutar("INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES(@TipoCliente,@NombreRazonSocial)", Parametro("@TipoCliente", item.TipoCliente), Parametro("@NombreRazonSocial", item.NombreRazonSocial)) > 0;
    public bool Actualizar(Cliente item) => Ejecutar("UPDATE dbo.tbl_Clientes SET TipoCliente=@TipoCliente,NombreRazonSocial=@NombreRazonSocial WHERE ID_Cliente=@id", Parametro("@id", item.ID_Cliente), Parametro("@TipoCliente", item.TipoCliente), Parametro("@NombreRazonSocial", item.NombreRazonSocial)) > 0;
    public bool Eliminar(int id) => Ejecutar("DELETE dbo.tbl_Clientes WHERE ID_Cliente=@id", Parametro("@id", id)) > 0;
}
