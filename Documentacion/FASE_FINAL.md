# Fase final de ProyectoBD — 16 de noviembre de 2026

## Inspección previa

Se enumeraron los proyectos, entidades, métodos, formularios, consultas y scripts de la solución antes de editar archivos. La compilación inicial terminó con 0 errores y 0 advertencias.

- Ya existían `Reportes` y su diseñador, como visor de una `DataTable`; se ampliaron y se conservó su constructor anterior.
- `ProcesoDatos` y `ProcesoNegocio` ya consultaban facturas, detalles, pagos, inventario e ingresos. Se conservaron; únicamente se adaptó el saldo del listado para que una factura anulada no muestre un saldo negativo por sus pagos históricos.
- `Menu` ya restringía la sección Reportes al Gerente y ofrecía Ventas, Inventario e Ingresos a bodega. Estas opciones siguen disponibles.
- No existían `ReporteDatos`, `ReporteNegocio`, consulta gerencial por número ni procedimientos de reportes/anulación. La base tenía los seis procedimientos de Fase 2.
- Se inspeccionaron las tablas/columnas, claves, índices, restricciones y triggers reales mediante el catálogo SQL Server. `NumeroFactura` es entero y tiene índice único; es distinto de `ID_Factura`.
- `tbl_Factura` tiene `Total` como único importe del encabezado. `tbl_PagoFactura` no tiene fecha propia. `tbl_DetalleFacturaLote` registra `CantidadSalida`; se utiliza esa columna para el reintegro.
- Existen tablas de devolución, pero no un flujo de conciliación implementado en esta fase. Se bloquea la anulación de una factura con devoluciones para evitar un segundo reintegro.

## Uso

Iniciar sesión normalmente con el Gerente y abrir **Archivo → Reportes**:

1. **Reportes gerenciales**: seleccionar uno de los siete reportes. Los reportes 1, 2, 3 y 5 utilizan fechas inclusivas; los de inventario son actuales. El reporte 7 abre la consulta detallada.
2. **Consulta de factura**: introducir el número visible de factura, no el ID interno, y pulsar Buscar o Enter. Muestra encabezado, empleado, detalle, pagos con referencia y auditoría.
3. **Anulación de factura**: buscar, revisar y pulsar Anular factura. El diálogo inicia con **No** como opción predeterminada. Solamente **Sí** ejecuta la operación. Después se vuelve a consultar la factura.

Los controles están en archivos `.Designer.cs`, tienen nombres por defecto y se mantienen el color DarkCyan, los formularios sin barra superior y el botón Volver. Se comprobaron los menús de los tres roles. Negocio valida los permisos en cada método de los nuevos reportes, búsqueda y anulación; SQL vuelve a comprobar que el usuario que anula sea Gerente.

## Reportes

1. Total facturado, pagado y saldo del período, más pagos agrupados por forma de pago. El total facturado se calcula una vez por factura para evitar multiplicarlo por sus pagos. Efectivo, Cheque y Tarjeta aparecen incluso con cero. Otros métodos conservan su nombre y se incluyen automáticamente.
2. Productos/variantes ordenados por ingreso total, con cantidad vendida.
3. Productos/variantes ordenados por cantidad vendida descendente.
4. Lotes con cantidad disponible positiva, producto, presentación, color, ubicación y fecha real de ingreso.
5. Todas las variantes, incluidas las que no vendieron en el período, ordenadas por cantidad ascendente.
6. Variantes con existencia total cero, incluidas las que nunca tuvieron un lote.
7. Consulta detallada por número, incluidos documentos anulados, con total actual, total original del detalle, pagos históricos, saldo y auditoría.

Se excluyen las facturas anuladas de los reportes de ventas y pagos. Como los pagos no tienen fecha propia, el período del reporte 1 corresponde a la **emisión de las facturas**, no a la fecha de cobro. La pantalla lo indica. Los pagos de documentos anulados siguen visibles únicamente como historial, con saldo por cobrar cero.

## Anulación transaccional

`usp_AnularFactura` valida usuario, existencia, estado, ausencia de devoluciones, integridad de los detalles, correspondencia entre variante y lote, cantidades PEPS, total y límites del lote. Usa el mismo bloqueo de aplicación de la facturación para coordinar cambios de inventario y bloquea la factura para coordinarse con pagos. No modifica el algoritmo PEPS ni los procedimientos anteriores.

Dentro de una sola transacción reintegra cada cantidad a su lote original, inserta movimientos `ENTRADA_ANULACION`, cambia Estado a `Anulada` y Total a cero, y registra `ANULAR FACTURA` en auditoría con responsable, fecha, identificador y JSON anterior/nuevo, incluido el número y los lotes. Conserva detalles, asignaciones PEPS, pagos y auditoría previa. Una segunda anulación falla antes de cambiar inventario. Un error provoca `ROLLBACK` y se propaga con `THROW`.

## Instalación

`Database/FaseFinal.sql` ya fue ejecutado en la base local `Proyecto_BD`. Contiene siete procedimientos de reporte y uno de anulación, con `CREATE OR ALTER`; puede volver a aplicarse después de instalar Fase 2. No crea ni elimina tablas.

No se modificaron fuentes de `ProyectoBD.Web` ni definiciones de los seis procedimientos originales. La comprobación por hashes está en `IntegridadFaseFinal.txt`.

## Pruebas ejecutadas

- Los siete reportes con los datos reales: conciliación de ventas, ingresos, cantidades, pagos e inventario; consulta de todas las facturas; ordenamientos; período sin ventas y productos con cero ventas; fechas invertidas y número inválido/inexistente.
- Cada método de Negocio rechazó Digitador/Cajero. El procedimiento de anulación también rechazó ambos usuarios directamente en SQL.
- Se creó exclusivamente un caso nuevo, factura **N.º 6**, ID interno **1044**, con un producto identificado `Prueba final 1116023513`, cuatro unidades a Q10.00, dos lotes y cuatro pagos: Q7 efectivo, Q5 tarjeta, Q3 cheque y Q2 transferencia. Total Q40.00, pagado Q17.00. El caso se conserva como historial; las facturas anteriores no se anularon.
- Antes: lote `FINAL-A-1116023513`, disponible 0, salida 2; lote `FINAL-B-1116023513`, disponible 1, salida 2.
- Rollback: un trigger de prueba provocó error al insertar la auditoría, después de actualizar lotes, movimientos y factura. Todo volvió al estado anterior. El propio rollback retiró el trigger; se verificó que no quedó instalado.
- Después de la anulación correcta: lotes disponibles 2 y 3, Estado Anulada, Total Q0.00 y Saldo Q0.00. Se conservaron 1 detalle, 2 asignaciones PEPS y 4 pagos. Quedaron 2 movimientos de reintegro y 1 auditoría de anulación con responsable y datos anteriores/nuevos.
- El segundo intento falló sin alterar inventario, movimientos ni auditoría. El reporte de ventas/pagos volvió a sus importes previos y la consulta siguió mostrando la factura anulada y sus pagos.
- Pruebas WinForms: generación de los seis reportes tabulares, búsqueda del documento anulado, totales visibles, anulación deshabilitada, limpieza al editar el número, menús por rol y botones Volver. Capturas en `CapturasFaseFinal`.
- Revisión por control de computadora: menú del Gerente → Anulación → factura 1 → confirmación → **No** → Volver. La factura 1 continuó Pagada por Q7,200.00, sin auditoría de anulación; evidencia en `ConfirmacionAnulacionUI.txt`.
- Se abrió `Reportes.cs [Diseño]` en Visual Studio y se comprobaron los controles visibles en el diseñador. Se dejó `ProyectoBD.Presentacion` como proyecto de inicio de esta sesión del IDE.

Resultados detallados: `ResultadoFaseFinal.txt`, `ResultadoPantallasFaseFinal.txt`, `CompilacionFaseFinal.txt` y `CompilacionVerificacionFaseFinal.txt`.

## Repetir las comprobaciones

Desde la raíz del proyecto:

```powershell
dotnet build ProyectoBD2.sln --no-restore
dotnet build Verificacion/ProyectoBD.Verificacion.csproj --no-restore
dotnet Verificacion/bin/Debug/net8.0-windows/ProyectoBD.Verificacion.dll --fase-final-pantallas
```

La última opción consulta los datos y prueba formularios sin crear otra factura. `--fase-final` repite la prueba completa y crea **otro caso nuevo**, que conserva anulado con sus documentos, pagos y lotes. `Database/PruebasFaseFinal.sql` es la preparación parametrizada del caso y la ejecuta esa opción; no se ejecuta automáticamente al iniciar la aplicación.

`--fase-final-ui` abre únicamente el menú dentro de la herramienta de verificación para revisar pantallas con una sesión de prueba basada en el Gerente existente. No guarda contraseñas ni cambia `Program.cs` de Presentación: la aplicación normal conserva su inicio en Login y la autenticación BCrypt.
