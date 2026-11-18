USE Proyecto_BD;
GO
SET NOCOUNT ON;
CREATE TABLE #Pendientes(Tabla sysname,Columna sysname,Registros bigint);
DECLARE @sql nvarchar(max)=N'';
SELECT @sql=@sql+N'INSERT #Pendientes SELECT N'''+REPLACE(t.name,N'''',N'''''')+N''',N'''+REPLACE(c.name,N'''',N'''''')+N''',COUNT_BIG(*)
 FROM '+QUOTENAME(s.name)+N'.'+QUOTENAME(t.name)+N' WHERE CONVERT(nvarchar(max),'+QUOTENAME(c.name)+N') COLLATE Latin1_General_100_CI_AI LIKE N''%DEMO%'';'
FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id
JOIN sys.columns c ON c.object_id=t.object_id JOIN sys.types ty ON ty.user_type_id=c.user_type_id
WHERE t.is_ms_shipped=0 AND ty.name IN('nvarchar','varchar','nchar','char','text','ntext');
EXEC sys.sp_executesql @sql;
SELECT COUNT(*) ColumnasRevisadas,SUM(Registros) CoincidenciasPendientes FROM #Pendientes;
SELECT * FROM #Pendientes WHERE Registros>0;
IF EXISTS(SELECT 1 FROM #Pendientes WHERE Registros>0) THROW 50001,N'Quedaron etiquetas pendientes de limpiar.',1;
SELECT N'Verificación correcta: no quedan etiquetas en los valores de texto.' Resultado;
GO
