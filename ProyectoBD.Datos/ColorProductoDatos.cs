using System.Data;
using ProyectoBD.Entidades;
using static ProyectoBD.Datos.OperacionDatos;
namespace ProyectoBD.Datos;

public class ColorProductoDatos
{
    public List<ColorProducto> Listar() => Tabla("SELECT ID_Color,NombreColor FROM dbo.tbl_Color ORDER BY ID_Color").AsEnumerable().Select(r => new ColorProducto { ID_Color = r.Field<int>("ID_Color"), NombreColor = r.Field<string>("NombreColor")! }).ToList();
    public bool Insertar(ColorProducto item) => Ejecutar("INSERT dbo.tbl_Color(NombreColor) VALUES(@NombreColor)", Parametro("@NombreColor", item.NombreColor)) > 0;
    public bool Actualizar(ColorProducto item) => Ejecutar("UPDATE dbo.tbl_Color SET NombreColor=@NombreColor WHERE ID_Color=@id", Parametro("@id", item.ID_Color), Parametro("@NombreColor", item.NombreColor)) > 0;
    public bool Eliminar(int id) => Ejecutar("DELETE dbo.tbl_Color WHERE ID_Color=@id", Parametro("@id", id)) > 0;
}
