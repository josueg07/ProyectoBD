using System.Data;
using ProyectoBD.Entidades;
using static ProyectoBD.Datos.OperacionDatos;
namespace ProyectoBD.Datos;

public class ProveedorDatos
{
    public List<Proveedor> Listar() => Tabla("SELECT ID_Proveedor,Nombre_Proveedor FROM dbo.tbl_Proveedor ORDER BY ID_Proveedor").AsEnumerable().Select(r => new Proveedor { ID_Proveedor = r.Field<int>("ID_Proveedor"), Nombre_Proveedor = r.Field<string>("Nombre_Proveedor")! }).ToList();
    public bool Insertar(Proveedor item) => Ejecutar("INSERT dbo.tbl_Proveedor(Nombre_Proveedor) VALUES(@Nombre_Proveedor)", Parametro("@Nombre_Proveedor", item.Nombre_Proveedor)) > 0;
    public bool Actualizar(Proveedor item) => Ejecutar("UPDATE dbo.tbl_Proveedor SET Nombre_Proveedor=@Nombre_Proveedor WHERE ID_Proveedor=@id", Parametro("@id", item.ID_Proveedor), Parametro("@Nombre_Proveedor", item.Nombre_Proveedor)) > 0;
    public bool Eliminar(int id) => Ejecutar("DELETE dbo.tbl_Proveedor WHERE ID_Proveedor=@id", Parametro("@id", id)) > 0;
}
