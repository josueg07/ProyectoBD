using System.Data;
using ProyectoBD.Entidades;
using static ProyectoBD.Datos.OperacionDatos;
namespace ProyectoBD.Datos;

public class CategoriaDatos
{
    public List<Categoria> Listar() => Tabla("SELECT ID_Categoria,NombreCategoria,Descripcion FROM dbo.tbl_CategoriaProducto ORDER BY ID_Categoria").AsEnumerable().Select(r => new Categoria { ID_Categoria = r.Field<int>("ID_Categoria"), NombreCategoria = r.Field<string>("NombreCategoria")!, Descripcion = r.Field<string>("Descripcion")! }).ToList();
    public bool Insertar(Categoria item) => Ejecutar("INSERT dbo.tbl_CategoriaProducto(NombreCategoria,Descripcion) VALUES(@NombreCategoria,@Descripcion)", Parametro("@NombreCategoria", item.NombreCategoria), Parametro("@Descripcion", item.Descripcion)) > 0;
    public bool Actualizar(Categoria item) => Ejecutar("UPDATE dbo.tbl_CategoriaProducto SET NombreCategoria=@NombreCategoria,Descripcion=@Descripcion WHERE ID_Categoria=@id", Parametro("@id", item.ID_Categoria), Parametro("@NombreCategoria", item.NombreCategoria), Parametro("@Descripcion", item.Descripcion)) > 0;
    public bool Eliminar(int id) => Ejecutar("DELETE dbo.tbl_CategoriaProducto WHERE ID_Categoria=@id", Parametro("@id", id)) > 0;
}
