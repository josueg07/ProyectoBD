using System.Data;
using ProyectoBD.Entidades;
using static ProyectoBD.Datos.OperacionDatos;
namespace ProyectoBD.Datos;

public partial class UsuarioDatos
{
    public List<Usuario> Listar() => Tabla("SELECT u.ID_Usuario,u.Nombres,u.Apellidos,u.NombreUsuario,u.ID_Rol,r.NombreRol FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol ORDER BY u.ID_Usuario").AsEnumerable().Select(r => new Usuario { ID_Usuario = r.Field<int>("ID_Usuario"), Nombres = r.Field<string>("Nombres")!, Apellidos = r.Field<string>("Apellidos")!, NombreUsuario = r.Field<string>("NombreUsuario")!, ID_Rol = r.Field<int>("ID_Rol"), NombreRol = r.Field<string>("NombreRol")! }).ToList();
    public bool Guardar(Usuario u, int actor, bool nuevo) => Ejecutar(@"SET XACT_ABORT ON;
BEGIN TRY
 BEGIN TRAN;
 DECLARE @antes nvarchar(max)=(SELECT ID_Usuario,Nombres,Apellidos,NombreUsuario,ID_Rol FROM dbo.tbl_Usuario WITH (UPDLOCK,HOLDLOCK) WHERE ID_Usuario=@id FOR JSON PATH,WITHOUT_ARRAY_WRAPPER);
 IF @nuevo=1 BEGIN
 INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES(@n,@a,@u,@hash,@rol); SET @id=CONVERT(int,SCOPE_IDENTITY()); END
 ELSE BEGIN
 UPDATE dbo.tbl_Usuario SET Nombres=@n,Apellidos=@a,NombreUsuario=@u,ID_Rol=@rol WHERE ID_Usuario=@id;
 IF @@ROWCOUNT=0 THROW 50001,'El usuario ya no existe.',1; END;
 INSERT dbo.tbl_Auditoria(FechaHora,TablaAfectada,RegistroAfectado,Accion,DatosAnteriores,DatosNuevos,ID_Usuario)
 VALUES(SYSDATETIME(),'tbl_Usuario',CONVERT(nvarchar(60),@id),'CAMBIO DE USUARIO',@antes,(SELECT ID_Usuario,Nombres,Apellidos,NombreUsuario,ID_Rol FROM dbo.tbl_Usuario WHERE ID_Usuario=@id FOR JSON PATH,WITHOUT_ARRAY_WRAPPER),@actor);
 COMMIT;
END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;", Parametro("@id", u.ID_Usuario), Parametro("@n", u.Nombres), Parametro("@a", u.Apellidos), Parametro("@u", u.NombreUsuario), Parametro("@hash", u.PasswordHash), Parametro("@rol", u.ID_Rol), Parametro("@nuevo", nuevo ? 1 : 0), Parametro("@actor", actor)) > 0;
    public bool ActualizarPassword(int idUsuario, string nuevoHash, int actor) => Ejecutar(@"SET XACT_ABORT ON; BEGIN TRY BEGIN TRAN;
 UPDATE dbo.tbl_Usuario SET PasswordHash=@hash WHERE ID_Usuario=@id;
 IF @@ROWCOUNT=0 THROW 50001,'El usuario ya no existe.',1;
 INSERT dbo.tbl_Auditoria(FechaHora,TablaAfectada,RegistroAfectado,Accion,ID_Usuario) VALUES(SYSDATETIME(),'tbl_Usuario',CONVERT(nvarchar(60),@id),'CAMBIO PASSWORD',@actor); COMMIT;
 END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;", Parametro("@id", idUsuario), Parametro("@hash", nuevoHash), Parametro("@actor", actor)) > 0;
    public bool Eliminar(int id, int actor) => Ejecutar(@"SET XACT_ABORT ON; BEGIN TRY BEGIN TRAN;
 DECLARE @antes nvarchar(max)=(SELECT ID_Usuario,Nombres,Apellidos,NombreUsuario,ID_Rol FROM dbo.tbl_Usuario WHERE ID_Usuario=@id FOR JSON PATH,WITHOUT_ARRAY_WRAPPER);
 DELETE dbo.tbl_Usuario WHERE ID_Usuario=@id;
 IF @@ROWCOUNT=0 THROW 50001,'El usuario ya no existe.',1;
 INSERT dbo.tbl_Auditoria(FechaHora,TablaAfectada,RegistroAfectado,Accion,DatosAnteriores,ID_Usuario) VALUES(SYSDATETIME(),'tbl_Usuario',CONVERT(nvarchar(60),@id),'ELIMINAR USUARIO',@antes,@actor); COMMIT;
 END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;", Parametro("@id", id), Parametro("@actor", actor)) > 0;
}
