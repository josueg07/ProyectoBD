using System.Data;
using ProyectoBD.Entidades;
using static ProyectoBD.Datos.OperacionDatos;
namespace ProyectoBD.Datos;

public class MetodoPagoDatos
{
    public List<MetodoPago> Listar() => Tabla("SELECT ID_MetodoP,NombreMetodo FROM dbo.tbl_MetodoPago ORDER BY ID_MetodoP").AsEnumerable().Select(r => new MetodoPago { ID_MetodoP = r.Field<int>("ID_MetodoP"), NombreMetodo = r.Field<string>("NombreMetodo")! }).ToList();
    public bool Insertar(MetodoPago item) => Ejecutar("INSERT dbo.tbl_MetodoPago(NombreMetodo) VALUES(@NombreMetodo)", Parametro("@NombreMetodo", item.NombreMetodo)) > 0;
    public bool Actualizar(MetodoPago item) => Ejecutar("UPDATE dbo.tbl_MetodoPago SET NombreMetodo=@NombreMetodo WHERE ID_MetodoP=@id", Parametro("@id", item.ID_MetodoP), Parametro("@NombreMetodo", item.NombreMetodo)) > 0;
    public bool Eliminar(int id) => Ejecutar("DELETE dbo.tbl_MetodoPago WHERE ID_MetodoP=@id", Parametro("@id", id)) > 0;
}
