using System.Data;
using ProyectoBD.Entidades;
using static ProyectoBD.Datos.OperacionDatos;
namespace ProyectoBD.Datos;

public class PresentacionDatos
{
    public List<Presentacion> Listar() => Tabla("SELECT ID_Presentacion,NombrePresentacion,UnidadMedida FROM dbo.tbl_Presentacion ORDER BY ID_Presentacion").AsEnumerable().Select(r => new Presentacion { ID_Presentacion = r.Field<int>("ID_Presentacion"), NombrePresentacion = r.Field<string>("NombrePresentacion")!, UnidadMedida = r.Field<string>("UnidadMedida")! }).ToList();
    public bool Insertar(Presentacion item) => Ejecutar("INSERT dbo.tbl_Presentacion(NombrePresentacion,UnidadMedida) VALUES(@NombrePresentacion,@UnidadMedida)", Parametro("@NombrePresentacion", item.NombrePresentacion), Parametro("@UnidadMedida", item.UnidadMedida)) > 0;
    public bool Actualizar(Presentacion item) => Ejecutar("UPDATE dbo.tbl_Presentacion SET NombrePresentacion=@NombrePresentacion,UnidadMedida=@UnidadMedida WHERE ID_Presentacion=@id", Parametro("@id", item.ID_Presentacion), Parametro("@NombrePresentacion", item.NombrePresentacion), Parametro("@UnidadMedida", item.UnidadMedida)) > 0;
    public bool Eliminar(int id) => Ejecutar("DELETE dbo.tbl_Presentacion WHERE ID_Presentacion=@id", Parametro("@id", id)) > 0;
}
