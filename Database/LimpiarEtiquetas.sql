-- Limpia las etiquetas de los registros sin borrar documentos ni cambiar importes.
USE Proyecto_BD;
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @bloqueo int;
 EXEC @bloqueo=sys.sp_getapplock @Resource=N'ProyectoBD_DatosEjemplo_V1',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000;
 IF @bloqueo<0 THROW 50001,N'No se pudo obtener el bloqueo para limpiar las etiquetas.',1;

 CREATE TABLE #Invariables(Tabla sysname PRIMARY KEY,Filas bigint,Huella varbinary(32));
 CREATE TABLE #Cambios(Tabla sysname,Columna sysname,Registros int);
 CREATE TABLE #JsonAntes(ID_Auditoria int PRIMARY KEY,Anterior bit,Nuevo bit);
 INSERT #JsonAntes SELECT ID_Auditoria,ISNULL(ISJSON(DatosAnteriores),0),ISNULL(ISJSON(DatosNuevos),0) FROM dbo.tbl_Auditoria;

 DECLARE @esquema sysname,@tabla sysname,@columna sysname,@campos nvarchar(max),@orden sysname,@sql nvarchar(max),@huella varbinary(32),@filas bigint;
 DECLARE tablas CURSOR LOCAL FAST_FORWARD FOR
  SELECT s.name,t.name FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id WHERE t.is_ms_shipped=0;
 OPEN tablas;
 FETCH NEXT FROM tablas INTO @esquema,@tabla;
 WHILE @@FETCH_STATUS=0
 BEGIN
  SELECT @campos=STRING_AGG(CONVERT(nvarchar(max),QUOTENAME(c.name)),N',') WITHIN GROUP(ORDER BY c.column_id)
   FROM sys.columns c JOIN sys.types ty ON ty.user_type_id=c.user_type_id
   WHERE c.object_id=OBJECT_ID(QUOTENAME(@esquema)+N'.'+QUOTENAME(@tabla))
   AND (ty.name NOT IN('nvarchar','varchar','nchar','char','text','ntext') OR c.name=N'PasswordHash');
  SELECT TOP(1) @orden=c.name FROM sys.columns c
   WHERE c.object_id=OBJECT_ID(QUOTENAME(@esquema)+N'.'+QUOTENAME(@tabla)) ORDER BY c.column_id;
  SET @sql=N'SELECT @n=COUNT_BIG(*) FROM '+QUOTENAME(@esquema)+N'.'+QUOTENAME(@tabla)+N' WITH(UPDLOCK,HOLDLOCK);
   SELECT @h=HASHBYTES(''SHA2_256'',(SELECT '+@campos+N' FROM '+QUOTENAME(@esquema)+N'.'+QUOTENAME(@tabla)+N' ORDER BY '+QUOTENAME(@orden)+N' FOR JSON PATH,INCLUDE_NULL_VALUES));';
  EXEC sys.sp_executesql @sql,N'@n bigint OUTPUT,@h varbinary(32) OUTPUT',@n=@filas OUTPUT,@h=@huella OUTPUT;
  INSERT #Invariables VALUES(@tabla,@filas,@huella);
  FETCH NEXT FROM tablas INTO @esquema,@tabla;
 END;
 CLOSE tablas;
 DEALLOCATE tablas;

 DECLARE @reglas TABLE(Orden int PRIMARY KEY,Buscar nvarchar(80),Sustituir nvarchar(80));
 INSERT @reglas VALUES
  (1,N'DATOS_DEMO',N'DATOS_EJEMPLO'),
  (2,N'BD2_DEMO_V1',N'BD2_EJEMPLO_V1'),
  (3,N'CARGA DEMO',N'CARGA EJEMPLO'),
  (4,N'demostraciones',N'ejemplos'),
  (5,N'demostración',N'ejemplo'),
  (6,N'demostracion',N'ejemplo'),
  (7,N' (DEMO)',N''),
  (8,N'(DEMO)',N''),
  (9,N'DEMO-',N''),
  (10,N'demo_',N''),
  (11,N' DEMO',N''),
  (12,N'DEMO ',N''),
  (13,N'DEMO',N'');
 DECLARE @expresion nvarchar(max),@buscar nvarchar(80),@sustituir nvarchar(80);
 DECLARE columnas CURSOR LOCAL FAST_FORWARD FOR
  SELECT s.name,t.name,c.name FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id
  JOIN sys.columns c ON c.object_id=t.object_id JOIN sys.types ty ON ty.user_type_id=c.user_type_id
  WHERE t.is_ms_shipped=0 AND c.is_computed=0 AND ty.name IN('nvarchar','varchar','nchar','char') AND c.name<>N'PasswordHash';
 OPEN columnas;
 FETCH NEXT FROM columnas INTO @esquema,@tabla,@columna;
 WHILE @@FETCH_STATUS=0
 BEGIN
  SET @expresion=N'CONVERT(nvarchar(max),'+QUOTENAME(@columna)+N') COLLATE Latin1_General_100_CI_AI';
  DECLARE reglas CURSOR LOCAL FAST_FORWARD FOR SELECT Buscar,Sustituir FROM @reglas ORDER BY Orden;
  OPEN reglas;
  FETCH NEXT FROM reglas INTO @buscar,@sustituir;
  WHILE @@FETCH_STATUS=0
  BEGIN
   SET @expresion=N'REPLACE('+@expresion+N',N'''+REPLACE(@buscar,N'''',N'''''')+N''',N'''+REPLACE(@sustituir,N'''',N'''''')+N''')';
   FETCH NEXT FROM reglas INTO @buscar,@sustituir;
  END;
  CLOSE reglas;
  DEALLOCATE reglas;
  SET @sql=N'UPDATE '+QUOTENAME(@esquema)+N'.'+QUOTENAME(@tabla)+N' SET '+QUOTENAME(@columna)+N'=LTRIM(RTRIM('+@expresion+N'))
   WHERE CONVERT(nvarchar(max),'+QUOTENAME(@columna)+N') COLLATE Latin1_General_100_CI_AI LIKE N''%DEMO%'';
   DECLARE @afectadas int=@@ROWCOUNT;
   IF @afectadas>0 INSERT #Cambios VALUES(@t,@c,@afectadas);
   IF EXISTS(SELECT 1 FROM '+QUOTENAME(@esquema)+N'.'+QUOTENAME(@tabla)+N' WHERE CONVERT(nvarchar(max),'+QUOTENAME(@columna)+N') COLLATE Latin1_General_100_CI_AI LIKE N''%DEMO%'')
    THROW 50001,N''Quedó una etiqueta pendiente de limpiar.'',1;';
  EXEC sys.sp_executesql @sql,N'@t sysname,@c sysname',@t=@tabla,@c=@columna;
  FETCH NEXT FROM columnas INTO @esquema,@tabla,@columna;
 END;
 CLOSE columnas;
 DEALLOCATE columnas;

 IF EXISTS(SELECT 1 FROM #JsonAntes j JOIN dbo.tbl_Auditoria a ON a.ID_Auditoria=j.ID_Auditoria
           WHERE (j.Anterior=1 AND ISNULL(ISJSON(a.DatosAnteriores),0)<>1) OR (j.Nuevo=1 AND ISNULL(ISJSON(a.DatosNuevos),0)<>1))
  THROW 50001,N'El historial JSON no conservó su formato; se revirtió la limpieza.',1;

 DECLARE verificar CURSOR LOCAL FAST_FORWARD FOR
  SELECT s.name,t.name FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id WHERE t.is_ms_shipped=0;
 OPEN verificar;
 FETCH NEXT FROM verificar INTO @esquema,@tabla;
 WHILE @@FETCH_STATUS=0
 BEGIN
  SELECT @campos=STRING_AGG(CONVERT(nvarchar(max),QUOTENAME(c.name)),N',') WITHIN GROUP(ORDER BY c.column_id)
   FROM sys.columns c JOIN sys.types ty ON ty.user_type_id=c.user_type_id
   WHERE c.object_id=OBJECT_ID(QUOTENAME(@esquema)+N'.'+QUOTENAME(@tabla))
   AND (ty.name NOT IN('nvarchar','varchar','nchar','char','text','ntext') OR c.name=N'PasswordHash');
  SELECT TOP(1) @orden=c.name FROM sys.columns c
   WHERE c.object_id=OBJECT_ID(QUOTENAME(@esquema)+N'.'+QUOTENAME(@tabla)) ORDER BY c.column_id;
  SET @sql=N'SELECT @n=COUNT_BIG(*) FROM '+QUOTENAME(@esquema)+N'.'+QUOTENAME(@tabla)+N';
   SELECT @h=HASHBYTES(''SHA2_256'',(SELECT '+@campos+N' FROM '+QUOTENAME(@esquema)+N'.'+QUOTENAME(@tabla)+N' ORDER BY '+QUOTENAME(@orden)+N' FOR JSON PATH,INCLUDE_NULL_VALUES));';
  EXEC sys.sp_executesql @sql,N'@n bigint OUTPUT,@h varbinary(32) OUTPUT',@n=@filas OUTPUT,@h=@huella OUTPUT;
  IF EXISTS(SELECT 1 FROM #Invariables WHERE Tabla=@tabla AND (Filas<>@filas OR Huella<>@huella))
   THROW 50001,N'Cambiaron identificadores, importes, cantidades o credenciales; se revirtió la limpieza.',1;
  FETCH NEXT FROM verificar INTO @esquema,@tabla;
 END;
 CLOSE verificar;
 DEALLOCATE verificar;

 IF EXISTS(SELECT 1 FROM #Cambios)
 BEGIN
  DECLARE @usuario int=(SELECT TOP(1) u.ID_Usuario FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE r.NombreRol=N'Gerente' ORDER BY u.ID_Usuario);
  IF @usuario IS NULL THROW 50001,N'No existe un Gerente para registrar la limpieza.',1;
  INSERT dbo.tbl_Auditoria(FechaHora,TablaAfectada,RegistroAfectado,Accion,DatosNuevos,ID_Usuario)
   VALUES(SYSDATETIME(),N'CATALOGOS',N'ETIQUETAS',N'LIMPIAR ETIQUETAS',
    (SELECT Tabla,Columna,Registros FROM #Cambios ORDER BY Tabla,Columna FOR JSON PATH),@usuario);
 END;
 COMMIT TRANSACTION;
 SELECT Tabla,Columna,Registros FROM #Cambios ORDER BY Tabla,Columna;
 PRINT N'Limpieza completada. Documentos, cantidades, importes, claves y contraseñas conservados.';
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;
GO
