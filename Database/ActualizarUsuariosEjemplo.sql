SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @admin int=(SELECT u.ID_Usuario FROM dbo.tbl_Usuario u WITH(UPDLOCK,HOLDLOCK) JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.NombreUsuario='admin' AND r.NombreRol='Digitador');
 DECLARE @marca int,@json nvarchar(max);
 SELECT @marca=ID_Auditoria,@json=DatosNuevos FROM dbo.tbl_Auditoria WITH(UPDLOCK,HOLDLOCK) WHERE TablaAfectada='DATOS_EJEMPLO' AND RegistroAfectado='BD2_EJEMPLO_V1' AND Accion='CARGA EJEMPLO';
 IF @admin IS NULL OR @marca IS NULL THROW 50001,'No existe admin Digitador o el manifiesto de datos.',1;
 DECLARE @cajero int=CONVERT(int,JSON_VALUE(@json,'$.UsuarioCajero')),@gerente int=CONVERT(int,JSON_VALUE(@json,'$.UsuarioGerente'));
 DECLARE @anterior int=(SELECT ID_Usuario FROM dbo.tbl_Usuario WITH(UPDLOCK,HOLDLOCK) WHERE NombreUsuario='digitador');
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@cajero AND u.NombreUsuario IN('cajero','cajero') AND r.NombreRol='Cajero')
  THROW 50001,'No se encontró la cuenta Cajero esperada.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@gerente AND u.NombreUsuario IN('gerente','gerente') AND r.NombreRol='Gerente')
  THROW 50001,'No se encontró la cuenta Gerente esperada.',1;
 IF EXISTS(SELECT 1 FROM dbo.tbl_Usuario WHERE NombreUsuario='cajero' AND ID_Usuario<>@cajero) OR EXISTS(SELECT 1 FROM dbo.tbl_Usuario WHERE NombreUsuario='gerente' AND ID_Usuario<>@gerente)
  THROW 50001,'El nombre cajero o gerente ya pertenece a otra cuenta.',1;
 DECLARE @antesC nvarchar(max)=(SELECT ID_Usuario,NombreUsuario,ID_Rol FROM dbo.tbl_Usuario WHERE ID_Usuario=@cajero FOR JSON PATH,WITHOUT_ARRAY_WRAPPER);
 DECLARE @antesG nvarchar(max)=(SELECT ID_Usuario,NombreUsuario,ID_Rol FROM dbo.tbl_Usuario WHERE ID_Usuario=@gerente FOR JSON PATH,WITHOUT_ARRAY_WRAPPER);
 UPDATE dbo.tbl_Usuario SET NombreUsuario='cajero',PasswordHash=@HashCajero WHERE ID_Usuario=@cajero;
 UPDATE dbo.tbl_Usuario SET NombreUsuario='gerente',PasswordHash=@HashGerente WHERE ID_Usuario=@gerente;
 INSERT dbo.tbl_Auditoria(FechaHora,TablaAfectada,RegistroAfectado,Accion,DatosAnteriores,DatosNuevos,ID_Usuario)
 VALUES(SYSDATETIME(),'tbl_Usuario',CONVERT(nvarchar(60),@cajero),'CAMBIO DE USUARIO',@antesC,(SELECT ID_Usuario,NombreUsuario,ID_Rol FROM dbo.tbl_Usuario WHERE ID_Usuario=@cajero FOR JSON PATH,WITHOUT_ARRAY_WRAPPER),@gerente),
       (SYSDATETIME(),'tbl_Usuario',CONVERT(nvarchar(60),@gerente),'CAMBIO DE USUARIO',@antesG,(SELECT ID_Usuario,NombreUsuario,ID_Rol FROM dbo.tbl_Usuario WHERE ID_Usuario=@gerente FOR JSON PATH,WITHOUT_ARRAY_WRAPPER),@gerente);
 IF @anterior IS NOT NULL
 BEGIN
  DECLARE @antesD nvarchar(max)=(SELECT ID_Usuario,Nombres,Apellidos,NombreUsuario,ID_Rol FROM dbo.tbl_Usuario WHERE ID_Usuario=@anterior FOR JSON PATH,WITHOUT_ARRAY_WRAPPER);
  DECLARE @auditorias nvarchar(max)=(SELECT ID_Auditoria FROM dbo.tbl_Auditoria WHERE ID_Usuario=@anterior FOR JSON PATH);
  DECLARE @ingresos int,@movimientos int,@cotizaciones int,@facturas int,@devoluciones int;
  UPDATE dbo.tbl_IngresoBodega SET ID_Usuario=@admin WHERE ID_Usuario=@anterior; SET @ingresos=@@ROWCOUNT;
  UPDATE dbo.tbl_MovimientoInventario SET ID_Usuario=@admin WHERE ID_Usuario=@anterior; SET @movimientos=@@ROWCOUNT;
  UPDATE dbo.tbl_Cotizacion SET ID_Usuario=@admin WHERE ID_Usuario=@anterior; SET @cotizaciones=@@ROWCOUNT;
  UPDATE dbo.tbl_Factura SET ID_Usuario=@admin WHERE ID_Usuario=@anterior; SET @facturas=@@ROWCOUNT;
  UPDATE dbo.tbl_DevolucionVenta SET ID_Usuario=@admin WHERE ID_Usuario=@anterior; SET @devoluciones=@@ROWCOUNT;
  UPDATE dbo.tbl_Auditoria SET ID_Usuario=@admin WHERE ID_Usuario=@anterior;
  INSERT dbo.tbl_Auditoria(FechaHora,TablaAfectada,RegistroAfectado,Accion,DatosAnteriores,DatosNuevos,ID_Usuario)
  VALUES(SYSDATETIME(),'tbl_Usuario',CONVERT(nvarchar(60),@anterior),'REASIGNAR USUARIO',@antesD,
  (SELECT @admin UsuarioDestino,N'admin' NombreUsuarioDestino,@ingresos Ingresos,@movimientos Movimientos,@cotizaciones Cotizaciones,@facturas Facturas,@devoluciones Devoluciones,JSON_QUERY(@auditorias) AuditoriasConActorAnterior,N'Reasignación solicitada para eliminar la cuenta sin borrar documentos; actor anterior conservado en DatosAnteriores.' Motivo FOR JSON PATH,WITHOUT_ARRAY_WRAPPER),@gerente);
  DELETE dbo.tbl_Usuario WHERE ID_Usuario=@anterior;
 END;
 SET @json=JSON_MODIFY(@json,'$.UsuarioDigitador',@admin);
 SET @json=JSON_MODIFY(@json,'$.NombreDigitador',N'admin');
 SET @json=JSON_MODIFY(@json,'$.NombreCajero',N'cajero');
 SET @json=JSON_MODIFY(@json,'$.NombreGerente',N'gerente');
 UPDATE dbo.tbl_Auditoria SET DatosNuevos=@json WHERE ID_Auditoria=@marca;
 COMMIT;
 PRINT 'Usuarios actualizados. admin conservado; digitador eliminado sin borrar documentos.';
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 THROW;
END CATCH;
