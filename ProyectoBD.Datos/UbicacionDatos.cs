using System.Data;
using ProyectoBD.Entidades;
using static ProyectoBD.Datos.OperacionDatos;
namespace ProyectoBD.Datos;

public class UbicacionDatos
{
    public List<Ubicacion> Listar() => Tabla("SELECT ID_UbicacionA,NombreUbicacion,Descripcion FROM dbo.tbl_UbicacionAlmacen ORDER BY ID_UbicacionA").AsEnumerable().Select(r => new Ubicacion { ID_UbicacionA = r.Field<int>("ID_UbicacionA"), NombreUbicacion = r.Field<string>("NombreUbicacion")!, Descripcion = r.Field<string>("Descripcion")! }).ToList();
    public bool Insertar(Ubicacion item) => Ejecutar("INSERT dbo.tbl_UbicacionAlmacen(NombreUbicacion,Descripcion) VALUES(@NombreUbicacion,@Descripcion)", Parametro("@NombreUbicacion", item.NombreUbicacion), Parametro("@Descripcion", item.Descripcion)) > 0;
    public bool Actualizar(Ubicacion item) => Ejecutar("UPDATE dbo.tbl_UbicacionAlmacen SET NombreUbicacion=@NombreUbicacion,Descripcion=@Descripcion WHERE ID_UbicacionA=@id", Parametro("@id", item.ID_UbicacionA), Parametro("@NombreUbicacion", item.NombreUbicacion), Parametro("@Descripcion", item.Descripcion)) > 0;
    public bool Eliminar(int id) => Ejecutar("DELETE dbo.tbl_UbicacionAlmacen WHERE ID_UbicacionA=@id", Parametro("@id", id)) > 0;
}
