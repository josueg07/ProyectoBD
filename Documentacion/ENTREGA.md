# Entrega — Windows Forms, Fases 1 y 2

Fecha: 20 de octubre de 2026. Proyecto principal: `ProyectoBD.Presentacion` (.NET 8). Se mantuvo la cadena de dependencias Presentación → Negocio → Datos → SQL Server, con Entidades compartidas. La capa Presentación no contiene SQL ni referencias directas a SqlClient. No se modificó código ni configuración de ProyectoBD.Web.

Los componentes y eventos usan nombres por defecto de WinForms, como button1, comboBox1 y textBox1. La correspondencia de nombres está en `NOMBRES_CONTROLES.md`.

## Diseño de formularios actualizado

Los controles están en los `.Designer.cs` y se pueden seleccionar, mover y editar con el diseñador de Visual Studio. Cada formulario tiene constructor sin parámetros para el diseñador y eventos separados (Load, Click y SelectionChanged). Se usa el color DarkCyan del Menu, sin barra superior; Volver cierra la pantalla y permite regresar al menú. Productos y procesos muestran secciones fijas con GroupBox; no usan pestañas internas. Se retiró el prefijo Frm de archivos, clases y referencias.

Para abrir el diseñador: Explorador de soluciones → ProyectoBD.Presentacion → clic derecho en Marcas.cs, Productos.cs, Usuarios.cs u otro formulario → Ver diseñador (Shift+F7). El archivo principal contiene las acciones; .Designer.cs contiene controles/posiciones/eventos y .resx los recursos. No es necesario ejecutar el programa para ver el diseño.

## Estado final

FASE 1

- [OK] Login: se conserva autenticación BCrypt y UsuarioSesion; se corrigió únicamente el retorno al cerrar sesión.
- [OK] Roles: se completó el formulario existente `Roles`, con su diseñador y controles originales.
- [OK] Usuarios: `Usuarios`, CRUD, selección de rol y cambio de contraseña.
- [OK] Marcas: formulario existente `Marcas`, CRUD.
- [OK] Proveedores: formulario existente `Proveedores`, CRUD.
- [OK] Clientes: formulario existente `Clientes`, CRUD y tipo Particular/Empresa.
- [OK] Productos: formulario existente `Productos`, CRUD del producto y CRUD separado de variantes; color opcional.

FASE 2

- [OK] Cotización: creación con varios detalles, historial, confirmación, cancelación y conversión a factura.
- [OK] Orden de compra: varios productos y seguimiento de cantidades solicitadas/recibidas/pendientes.
- [OK] Ingreso a bodega: completo o parcial; genera lotes, movimientos y auditoría.
- [OK] Facturación: venta directa o desde cotización confirmada, con responsable y numeración.
- [OK] PEPS: orden por FechaIngreso e ID_Lote; cada salida se vincula con DetalleFacturaLote.
- [OK] Pagos: varios métodos y pagos parciales; Pagada sólo con suma exacta del total.
- [OK] Impresión: PrintDocument, previsualización y botón de impresión; incluye detalle y pagos, con paginación.

## Instalación y acceso

En este equipo los procedimientos ya están instalados en la BD existente. En otro equipo que tenga el mismo esquema, abrir `Database/Fase2.sql` en SSMS y ejecutarlo contra `(localdb)\MSSQLLocalDB`. También se puede ejecutar desde PowerShell en la raíz:

```powershell
sqlcmd -S '(localdb)\MSSQLLocalDB' -d Proyecto_BD -E -b -i Database/Fase2.sql
```

El script usa CREATE OR ALTER y no crea otra BD, no borra datos y no reemplaza tablas. Requiere SQL Server con OPENJSON habilitado (compatibilidad 130 o superior). El esquema real utiliza `ID_Categoria`, `ID_OrdenC`, `ID_IngresoB` e `ID_MetodoP`; no se cambiaron esos nombres. Los importes son decimal(12,2) y las cantidades son enteras.

Abrir `ProyectoBD2.sln` en Visual Studio 2022 con soporte .NET 8 y establecer **ProyectoBD.Presentacion** como proyecto de inicio. Ejecutar con F5. Alternativa:

```powershell
dotnet run --project ProyectoBD.Presentacion
```

La cuenta existente `admin` tiene rol **Digitador**. No se cambió su contraseña ni su rol. La BD revisada no tiene ningún Gerente, por lo que se incluyó una utilidad local para crear exclusivamente el primer Gerente. No se ejecutó esa opción ni se dejó una contraseña predeterminada:

```powershell
dotnet run --project Verificacion/ProyectoBD.Verificacion.csproj -- --crear-gerente
```

Escribir nombres, apellidos y usuario; introducir y repetir una contraseña elegida por usted (no se muestra en consola). La utilidad reutiliza UsuarioNegocio y BCrypt, y se niega a crear otra cuenta si ya existe un Gerente. Después entrar en Windows Forms con esa cuenta y crear las demás cuentas desde Usuarios. Esta utilidad requiere acceso local a la BD con la identidad de Windows del desarrollador; no es parte del menú del usuario final.

## Pruebas manuales de Fase 1

1. **Login:** entrar con una cuenta existente; probar una contraseña incorrecta y luego la correcta. Verificar nombre y rol en el menú. Archivo → Cerrar sesión debe regresar al mismo login y limpiar la contraseña.
2. **Permisos:** Digitador ve Mantenimientos y Procesos; Cajero ve Caja; Gerente ve Usuarios y Reportes. Facturación está disponible al Digitador en Procesos y al Cajero en Caja → Cobros. Negocio y los SP también validan los roles de los procesos.
3. **Roles (Gerente):** Usuarios → Gestión de roles. Nuevo → escribir un nombre → Guardar. Seleccionar la fila, cambiar el texto y pulsar Editar. Seleccionar y Eliminar un rol sin usuarios. Intentar eliminar un rol asignado: debe aparecer un mensaje claro. Mantener los nombres oficiales Digitador, Cajero y Gerente, porque definen los perfiles del sistema; un rol personalizado no recibe permisos automáticamente.
4. **Usuarios (Gerente):** Usuarios → Gestión de usuarios. Nuevo → completar nombres/apellidos/usuario, contraseña de al menos 6 caracteres y rol → Guardar. El listado muestra ID, usuario, nombre completo y rol, sin hash. Seleccionar y Editar conserva la contraseña. Para reemplazarla, escribir una nueva y pulsar Cambiar contraseña; probar el nuevo login. Eliminar una cuenta sin referencias. No se permite eliminar la propia sesión ni cambiar su propio rol desde este formulario. Las contraseñas nuevas admiten hasta 72 bytes UTF-8 por el límite de BCrypt.
5. **Marcas (Digitador):** Mantenimientos → Marcas → Nuevo → nombre → Guardar. Seleccionar, cambiar nombre, Editar; luego Eliminar. Un registro referenciado por productos no se puede borrar.
6. **Proveedores (Digitador):** repetir Nuevo/Guardar/seleccionar/Editar/Eliminar en Mantenimientos → Proveedores. Un proveedor utilizado en compras no se puede borrar.
7. **Clientes (Digitador):** Mantenimientos → Clientes. Seleccionar Particular o Empresa, escribir NombreRazonSocial y Guardar. Seleccionar la fila, cambiar datos y Editar. Eliminar un cliente sin cotizaciones/facturas.
8. **Catálogos (Digitador):** crear categoría con descripción, marca, presentación con unidad de medida y, si se desea, un color. También crear una Ubicación de almacén desde Mantenimientos. Se reutilizaron los formularios existentes de categorías, presentaciones y colores.
9. **Productos (Digitador):** Mantenimientos → Productos → sección Producto general → Nuevo; completar nombre y descripción (hasta 45 caracteres), categoría y marca; Guardar. Seleccionar el producto, cambiar datos y Editar. En la sección Variantes del producto seleccionado; seleccionar presentación, color o Sin color y precio positivo; Guardar variante. Seleccionar una variante y probar Editar variante. Puede haber varias variantes de un mismo producto. Eliminar primero una variante sin referencias y luego el producto. No se borra inventario o historial para permitir eliminaciones.

## Prueba manual integral de Fase 2

Preparar un proveedor, un cliente, una ubicación y un producto con al menos una variante. Se pueden crear métodos Efectivo, Tarjeta y Cheque desde Caja → Medios de pago entrando como Cajero.

1. **Orden:** entrar como Digitador → Procesos → Compras. Elegir proveedor; agregar variante y cantidad 90; Guardar orden. En la sección de historial verificar estado Pendiente y solicitado 90. Se admiten varios productos y se impide duplicar una variante en el mismo documento.
2. **Primera recepción:** Procesos → Ingreso a bodega. Elegir orden y ubicación. Cargar pendientes; escribir Recibir=50 y Costo Q=5; dejar lote vacío y usar una fecha anterior a la siguiente recepción. Registrar ingreso. La orden debe quedar Parcial y conservar solicitado 90; pendiente 40. Sólo las columnas de recepción/costo/lote se pueden editar; cero omite una línea.
3. **Segunda recepción:** volver a cargar la misma orden; Recibir=40 y costo=6, con fecha posterior (no futura). Puede escribir un número de lote o dejarlo vacío. Registrar. La orden pasa a Completa y deja de aparecer entre las pendientes. También se puede usar Recibir todo, completar los costos y registrar. Probar una cantidad mayor a lo pendiente: debe rechazarse sin guardar el ingreso.
4. **Venta directa/PEPS:** Procesos → Facturación → Sin cotización. Elegir cliente, agregar 60 unidades a precio 10 y Facturar. Debe guardarse una factura Pendiente por Q600 y descontar 50 del lote más antiguo y 10 del segundo. Como Gerente, Reportes → Inventario permite verificar disponibles 0 y 30. La relación detallada está en tbl_DetalleFacturaLote y los movimientos en tbl_MovimientoInventario.
5. **Rollback:** intentar otra venta de 31 unidades cuando quedan 30. Debe informar inventario insuficiente y no guardar factura, detalles, salidas, auditoría de venta ni cambios de existencias. Las pruebas automatizadas también verifican este caso y la concurrencia.
6. **Cotización:** Digitador → Procesos → Cotizaciones. Seleccionar cliente, agregar variantes/cantidades/precios y Guardar. En la sección Historial seleccionar la cotización y Confirmar. Se pueden cancelar cotizaciones pendientes o confirmadas que aún no estén facturadas. Una cotización no reserva inventario.
7. **Convertir cotización:** abrir Facturación y seleccionar la cotización confirmada. El cliente y las líneas quedan bloqueados; el SP toma las líneas guardadas en la BD. Facturar. Se descuenta inventario por PEPS y la cotización cambia a Facturada. No puede convertirse dos veces ni facturarse estando Pendiente o Cancelada. La venta desde cotización puede realizarla también el Cajero desde Caja → Cobros.
8. **Pagos:** entrar como Cajero → Caja → Medios de pago y crear los métodos si faltan. Caja → Cobros → sección Facturas guardadas / cobros / impresión → seleccionar la factura de Q600 → Cobrar. Agregar Efectivo=300, Tarjeta=200 y Cheque=100 (referencia opcional de hasta 100 caracteres). Registrar pagos. Debe quedar Pagada. Si sólo se registran 300, sigue Pendiente con saldo 300. Un pago que supere el saldo se rechaza. Registrar los pagos es una transacción independiente del alta de factura; una factura pendiente ya representa una venta y ya descontó inventario.
9. **Impresión:** seleccionar factura → Previsualizar / imprimir. Revisar número, fecha, cliente, responsable, detalle, total, métodos y saldo. Usar el botón de impresora del diálogo después de revisar; el botón usa la impresora predeterminada de Windows. Para generar un PDF, configurar Microsoft Print to PDF como predeterminada antes de abrir la previsualización. Se probó el render de previsualización; no se envió una impresión física.
10. **Auditoría/reportes:** Gerente → Reportes para ventas, inventario por lote e ingresos. En SSMS consultar tbl_Auditoria: FACTURA e INGRESO BODEGA incluyen actor, fecha, referencia y detalles JSON. Las altas/ediciones/bajas de usuarios y cambios de contraseña también dejan historial. Las modificaciones de usuarios guardan datos anteriores/nuevos sin contraseñas ni hashes.

## Procedimientos creados

Todos están en `Database/Fase2.sql` y se instalaron en la base existente:

- `dbo.usp_GuardarCotizacion`
- `dbo.usp_EstadoCotizacion`
- `dbo.usp_GuardarOrdenCompra`
- `dbo.usp_RegistrarIngresoBodega`
- `dbo.usp_FacturarPEPS`
- `dbo.usp_RegistrarPagos`

Se eligió JSON para transportar listas sin agregar UDT o tablas de staging. Cada procedimiento valida entradas y aplica TRY/CATCH, XACT_ABORT y una transacción. Facturación serializa la numeración y las ventas mediante sp_getapplock; los lotes se bloquean con UPDLOCK/HOLDLOCK. Las recepciones bloquean la orden para evitar sobre-recepciones simultáneas, y los cobros bloquean la factura para evitar sobrepagos. La secuencia de NumeroFactura es MAX+1 dentro del bloqueo transaccional de facturación; las identidades de SQL pueden tener saltos normales.

## Verificación reproducible

Desde la raíz del repositorio:

```powershell
# SP, estados, PEPS, pagos, errores y rollback. Usa transacciones que revierten las filas de prueba.
sqlcmd -S '(localdb)\MSSQLLocalDB' -d Proyecto_BD -E -b -i Database/PruebasFase2.sql

# CRUD, login BCrypt, cambio de contraseña, auditoría y procesos reales C#→SQL,
# incluyendo dos ventas simultáneas. Limpia las identidades creadas por la prueba.
dotnet run --project Verificacion/ProyectoBD.Verificacion.csproj

# La misma integración más formularios cargados con datos y render de factura,
# sin enviar trabajos a una impresora física.
dotnet run --project Verificacion/ProyectoBD.Verificacion.csproj -- --integracion-ui

# Construcción con controles antes de cargar datos y comprobación de diseño sin BD.
dotnet run --project Verificacion/ProyectoBD.Verificacion.csproj -- --diseno

# Apertura/carga de los formularios y menú de cada rol, sin crear registros.
dotnet run --project Verificacion/ProyectoBD.Verificacion.csproj -- --ui

# Compilación completa de los cinco proyectos originales.
dotnet build ProyectoBD2.sln --nologo --no-incremental
```

`Verificacion` es una utilidad de comprobación separada, sin paquetes adicionales de pruebas y sin modificar la solución original. Las capturas de formularios y factura están en `Documentacion/CapturasFormularios`; representan datos temporales de prueba. Las pruebas no dejan cuentas ni documentos de ejemplo; los contadores IDENTITY sí avanzan, incluso con rollback, como es normal en SQL Server.

Resultados: `ResultadoSQL.txt`, `ResultadoCSharp.txt`, `ResultadoUI.txt` y `ResultadoCompilacion.txt`. Inventario inicial: `CAMBIOS.md`. Revisión actual de presentación y renombres: `CAMBIOS_FORMULARIOS.md`. Auditoría previa: `AUDITORIA.md`.

## Alcance y limitaciones

No quedaron módulos solicitados pendientes de implementar. La impresión física depende del controlador/impresora del equipo; se verificó la generación de páginas con el controlador de previsualización. No se agregaron devoluciones o anulación de facturas porque no formaban parte de los módulos pedidos; los movimientos implementados son ENTRADA_COMPRA y SALIDA_VENTA. Los reportes agregados son consultas básicas en grillas. No se alteró la estructura de tablas ni se crearon credenciales permanentes para probar.

