# Corrección de inicio de sesión con LocalDB

El error real se registró en `%LOCALAPPDATA%\ProyectoBD\login.log`: SqlException al abrir la conexión, proveedor SQL Network Interfaces, error 50, LocalDB no pudo iniciar su proceso. Fallaba antes de verificar BCrypt. Las credenciales y roles de cajero y gerente sí validaron en las comprobaciones independientes.

Se modificó la clase existente ConexionBD, sin crear otra conexión o base de datos. Consulta `SqlLocalDB info MSSQLLocalDB` para obtener el canal actual de la misma instancia y usa esa dirección con la misma base Proyecto_BD y autenticación integrada. Si la instancia está detenida, solicita su inicio y vuelve a consultar su dirección. Si la utilidad no está disponible, conserva la cadena original por nombre de instancia. No se guarda un identificador de canal fijo.

La conexión por canal de LocalDB está documentada por Microsoft; su nombre cambia con cada arranque, por eso debe resolverse dinámicamente: [SQL Server Express LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb).

Login conserva la validación y el sistema de sesión. Su catch ahora registra la excepción y la etapa en un archivo local, sin escribir el usuario introducido, la contraseña ni el hash. También vuelve a mostrar el formulario si falló la apertura del menú. Se corrigió la codificación del archivo a UTF-8 para conservar los acentos de los mensajes.

Verificación ejecutada: autenticación correcta e incorrecta de cajero y gerente, apertura de sus menús WinForms, conexión por el canal resuelto y comprobación de DB_NAME=Proyecto_BD. Resultado en ResultadoLogin.txt. Compilación completa: 0 errores, 0 advertencias. No se modificaron cuentas, contraseñas, documentos ni datos de prueba.

Verificación adicional realizada el 22/10/2026 dentro del depurador del Visual Studio abierto con esta solución, mediante su automatización DTE: Debugger.IsAttached=True; conexión a Proyecto_BD; autenticación, apertura del menú WinForms y rechazo de contraseña incorrecta para cajero y gerente. Evidencia en ResultadoLoginVisualStudio.txt. Se utilizó temporalmente un arranque de diagnóstico y después se restauró Program.cs; la versión final vuelve a iniciar normalmente en Login, sin pruebas automáticas ni credenciales incorporadas en su arranque. Se recompiló la solución y se inició nuevamente desde Visual Studio. La diferencia exacta entre las ejecuciones fallidas anteriores y esta ejecución correcta no quedó demostrada; no se atribuye el error a las contraseñas. Se conserva un registro de fallos de la utilidad LocalDB y el identificador del ensamblado de datos en los errores de Login para distinguir versiones si vuelve a ocurrir.

