using System.Data;
using ProyectoBD.Entidades;
using static ProyectoBD.Datos.OperacionDatos;
namespace ProyectoBD.Datos;

public class MarcaDatos
{
    public List<Marca> Listar() => Tabla("SELECT ID_Marca,NombreMarca FROM dbo.tbl_Marca ORDER BY ID_Marca").AsEnumerable().Select(r => new Marca { ID_Marca = r.Field<int>("ID_Marca"), NombreMarca = r.Field<string>("NombreMarca")! }).ToList();
    public bool Insertar(Marca item) => Ejecutar("INSERT dbo.tbl_Marca(NombreMarca) VALUES(@NombreMarca)", Parametro("@NombreMarca", item.NombreMarca)) > 0;
    public bool Actualizar(Marca item) => Ejecutar("UPDATE dbo.tbl_Marca SET NombreMarca=@NombreMarca WHERE ID_Marca=@id", Parametro("@id", item.ID_Marca), Parametro("@NombreMarca", item.NombreMarca)) > 0;
    public bool Eliminar(int id) => Ejecutar("DELETE dbo.tbl_Marca WHERE ID_Marca=@id", Parametro("@id", id)) > 0;
}
