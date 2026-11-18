#!/usr/bin/env bash
set -euo pipefail
SQLCMD=/opt/mssql-tools18/bin/sqlcmd
for attempt in {1..90}; do
  if "$SQLCMD" -S sqlserver,1433 -U sa -C -b -Q 'SELECT 1' -o /dev/null; then break; fi
  if [ "$attempt" = 90 ]; then echo 'SQL Server no disponible' >&2; exit 1; fi
  sleep 2
done
exists=$("$SQLCMD" -S sqlserver,1433 -U sa -C -b -h -1 -W -Q "SET NOCOUNT ON; SELECT CASE WHEN DB_ID(N'Proyecto_BD') IS NULL THEN 0 ELSE 1 END" | tr -d '\r\n ')
if [ "$exists" = 1 ]; then
  # Una importación incompleta se detiene para revisión, nunca se borra ni se reintenta destructivamente.
  "$SQLCMD" -S sqlserver,1433 -U sa -C -b -d Proyecto_BD -Q "IF OBJECT_ID(N'dbo.__DeploymentInitialization',N'U') IS NULL THROW 51000,'Base existente sin marca de importación: revisar manualmente; no se modificarán datos.',1;"
  echo 'Base inicializada previamente; se conserva sin cambios.'
  exit 0
fi
"$SQLCMD" -S sqlserver,1433 -U sa -C -b -i /init/Proyecto_BD_Final.sql
"$SQLCMD" -S sqlserver,1433 -U sa -C -b -d Proyecto_BD -Q "IF OBJECT_ID(N'dbo.usp_AnularFactura',N'P') IS NULL OR OBJECT_ID(N'dbo.tbl_Usuario',N'U') IS NULL THROW 51000,'Importación incompleta.',1; CREATE TABLE dbo.__DeploymentInitialization (Version int NOT NULL, Completado datetime2 NOT NULL); INSERT dbo.__DeploymentInitialization VALUES(1,SYSDATETIME());"
echo 'SQL final importado correctamente.'
