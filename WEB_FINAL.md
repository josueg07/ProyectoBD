# ProyectoBD.Web — cierre funcional

Entrega: 18/11/2026.

La Web queda funcionalmente terminada para el alcance solicitado: facturación directa y desde cotización, PEPS, cobros, siete reportes con previsualización y PDF, consulta y anulación de factura. No se implementó infraestructura de despliegue.

## Estado inicial

Antes de modificar archivos se ejecutó `dotnet build ProyectoBD2.sln`: **0 errores, 0 advertencias**. Se inspeccionaron `ProcesoNegocio`, `ProcesoDatos`, `ReporteNegocio`, `ReporteDatos`, las entidades de documentos/pagos, los formularios WinForms `Factura`, `Pagos`, `Reportes` y `ConsultaFactura`, los menús Web y los scripts de Fase 2/Fase Final.

La Web ya tenía autenticación, Usuarios/Roles, mantenimientos, cotizaciones, compras y recepciones. Esas implementaciones se conservaron.

Firmas reutilizadas, leídas de la solución:

```csharp
ProcesoNegocio(UsuarioSesion sesion)
int Facturar(Factura f)
void Pagar(int factura, List<PagoFactura> pagos)
DataTable Facturas()
DataTable DetalleFactura(int id)
DataTable Pagos(int id)

ReporteNegocio(UsuarioSesion sesion)
DataSet ReporteFacturacion(DateTime inicial, DateTime final)
DataTable ReporteProductosMayoresIngresos(DateTime inicial, DateTime final)
DataTable ReporteProductosMasVendidos(DateTime inicial, DateTime final)
DataTable ReporteInventario()
DataTable ReporteProductosMenosVendidos(DateTime inicial, DateTime final)
DataTable ReporteProductosSinExistencia()
DataSet BuscarFactura(int numero)
void AnularFactura(int numero)
```

También se reutilizaron los catálogos de clientes, variantes y métodos de pago. No fue necesario modificar ninguna capa compartida ni procedimiento.

## Archivos creados

Dentro de `ProyectoBD.Web`:

| Grupo | Archivos |
|---|---|
| Controladores | `Controllers/FacturasController.cs`, `CobrosController.cs`, `ReportesController.cs`, `AnulacionController.cs`, `MediosPagoController.cs` |
| Modelos de vista | `Models/FacturaWebViewModel.cs`, `ReporteWebViewModel.cs`, `MedioPagoViewModel.cs` |
| Adaptación de datos para vistas | `Helpers/FacturaWeb.cs`, `ReporteWeb.cs` |
| Presentación PDF y copias temporales | `Services/ReportePdf.cs`, `ReportesMemoria.cs` |
| Facturación | `Views/Facturas/Index.cshtml`, `Crear.cshtml`, `Detalle.cshtml` |
| Cobros | `Views/Cobros/Index.cshtml`, `Cobrar.cshtml`, `Detalle.cshtml` |
| Reportes/consulta/anulación | `Views/Reportes/Index.cshtml`, `Previsualizar.cshtml`, `Consulta.cshtml` |
| Medios de pago | `Views/MediosPago/Index.cshtml`, `Crear.cshtml`, `Editar.cshtml`, `_Formulario.cshtml` |
| Vistas compartidas nuevas | `Views/Shared/_FacturaDetalle.cshtml`, `_ReporteContenido.cshtml` |
| JavaScript | `wwwroot/js/cobros.js` |
| Fuentes portables | `Resources/Fonts/NotoSans-Regular.ttf`, `NotoSans-Bold.ttf`, `OFL.txt` |

Fuera de la Web: `VerificacionWeb/Final.cs`, este informe y las evidencias en `Documentacion`.

## Archivos modificados

Sólo tres archivos de producción existentes:

1. `ProyectoBD.Web/Program.cs`: registro de `MetodoPagoNegocio`, caché en memoria y `ReportesMemoria`. No se modificó la autenticación, las cookies ni la configuración de conexión/health.
2. `ProyectoBD.Web/ProyectoBD.Web.csproj`: dependencia PDF, fuentes incrustadas y copia de su licencia al publicar.
3. `ProyectoBD.Web/Views/Shared/_Layout.cshtml`: enlaces finales de los menús por rol.

En la herramienta de pruebas, `VerificacionWeb/Program.cs` incorpora la opción `--final`. Conserva las opciones y recorridos anteriores.

## Facturación

- [x] Listado.
- [x] Crear.
- [x] Cotización.
- [x] PEPS.
- [x] Inventario.
- [x] Detalle.
- [x] Seguridad.

`/Facturas` muestra número, fecha, cliente, estado, total, pagos históricos y saldo. `/Facturas/Crear` permite venta directa o cargar una cotización confirmada y sin facturar, como WinForms. En la venta directa se pueden agregar/quitar líneas; en la cotización se vuelven a obtener cliente, productos, cantidades y precios desde Negocio, ignorando importes o cliente manipulados en el POST.

La operación es:

```text
FacturasController
→ ProcesoNegocio(User.ObtenerSesion()).Facturar(Factura)
→ ProcesoDatos.Facturar(Factura, actor)
→ dbo.usp_FacturarPEPS
→ lotes ordenados por FechaIngreso e ID_Lote
→ tbl_DetalleFacturaLote
→ descuento de CantidadDisponible
→ movimientos SALIDA_VENTA y auditoría
```

No se implementó PEPS ni actualización de inventario en C#. El SP conserva sus bloqueos y transacción; una venta sin inventario suficiente no guarda factura ni altera existencias.

El detalle muestra empleado, cliente, cantidades, precios, subtotales, total, pagos y saldo. El Digitador tiene acceso a Facturación; el Cajero consulta la misma información desde Cobros y dispone allí del enlace para cobrar. Se evita dirigir al Digitador a una operación exclusiva del Cajero.

## Cobros

- [x] Listado.
- [x] Varios métodos.
- [x] Saldo.
- [x] Estado pagada.
- [x] Seguridad Cajero.

`/Cobros` lista facturas Pendientes con saldo positivo. `/Cobros/Cobrar/{id}` muestra la factura, permite agregar/quitar medios de pago y registrar uno o varios en una sola operación. La referencia es opcional, como en las reglas existentes; admite hasta 100 caracteres.

Se comprueba que los métodos sigan existiendo y que los pagos no superen el saldo consultado. `ProcesoNegocio.Pagar` valida importes positivos de hasta dos decimales; `usp_RegistrarPagos` vuelve a bloquear y comprobar la factura/saldo y decide el estado. Los pagos parciales conservan `Pendiente`; al cubrir el total el estado pasa a `Pagada`.

Las facturas Pagadas y Anuladas no muestran formulario de cobro y tampoco admiten un POST de pago. No se escriben pagos directamente en tablas.

Se agregó `/MediosPago`, pues no existía mantenimiento Web de este catálogo: crear, editar y eliminar con `MetodoPagoNegocio`, restringido al Cajero. La eliminación de un método utilizado devuelve un mensaje comprensible.

## Reportes

- [x] Reporte 1: facturación y formas de pago.
- [x] Reporte 2: productos con mayores ingresos.
- [x] Reporte 3: productos más vendidos por cantidad.
- [x] Reporte 4: inventario actual por lote y ubicación.
- [x] Reporte 5: productos con menos ventas, incluidos los de cero ventas.
- [x] Reporte 6: variantes sin existencias.
- [x] Reporte 7: consulta detallada por número de factura.
- [x] Previsualización.
- [x] PDF.

Todos invocan los métodos de `ReporteNegocio` y sus procedimientos existentes. Los reportes 1, 2, 3 y 5 utilizan fechas; 4 y 6 muestran existencias actuales; 7 utiliza `NumeroFactura`, nunca el ID interno.

El reporte 1 muestra total facturado, pagado, saldo y todos los medios devueltos por SQL: efectivo, cheque, tarjeta y otros. Su período corresponde a la emisión de facturas: los pagos históricos no tienen fecha propia en el esquema existente. Las anuladas se excluyen según los SP, sin reescribir sus consultas.

El reporte 7 incluye encabezado, empleado/usuario, total actual y original, pagos históricos, saldo, detalle por producto/presentación/color, pagos con referencias y auditoría. Las anuladas permanecen consultables.

Flujo:

```text
Gerente → Reportes → parámetros → Generar previsualización
→ página HTML con título, generación, parámetros, tablas y totales
→ Generar PDF → descarga en el navegador
```

La previsualización guarda una copia temporal en memoria durante 20 minutos, identificada con un token aleatorio y vinculada al usuario que la generó. HTML y PDF usan esa misma copia, incluso si luego cambia o se anula la factura. Otro Gerente no puede usar ese token. Si vence o se reinicia la aplicación, se solicita generar otra previsualización.

Los PDF se generan en memoria y se devuelven con `File(...)`. La Web no los guarda en disco. Los archivos bajo `Documentacion/EvidenciaWebFinal` son copias descargadas por la herramienta de prueba, no almacenamiento del servidor Web.

## Anulación

- [x] Busca por número.
- [x] Confirmación.
- [x] Estado Anulada.
- [x] Total Q0.
- [x] Inventario restaurado.
- [x] Auditoría.
- [x] Segunda anulación bloqueada.

`/Anulacion` consulta y muestra primero la factura por su número. La advertencia indica que se reintegrará inventario y se conservarán los pagos históricos. La selección inicial es **No, conservar la factura**. El POST requiere elegir expresamente Sí y presentar token antiforgery; sin confirmación no ejecuta Negocio.

La operación llama exclusivamente a `ReporteNegocio.AnularFactura(numero)` → `ReporteDatos.AnularFactura(numero, actor)` → `usp_AnularFactura`. No hay reintegro manual en el controlador. Las restricciones del SP, incluidas devoluciones previas o inconsistencias por lote, se conservan. Tras anular se vuelve a consultar el historial; una segunda anulación es rechazada por el SP.

## Menú final

- Digitador: mantenimientos existentes y Procesos: Cotizaciones, Facturación, Órdenes de compra, Ingreso a bodega.
- Cajero: Caja: Cobros y Medios de pago.
- Gerente: Administración: Usuarios/Roles; Reportes: Reportes gerenciales, Consulta de factura y Anulación de factura.

## Seguridad por rol

| Rutas | Rol Web |
|---|---|
| `/Facturas`, `/Facturas/Crear`, `/Facturas/Detalle` | Digitador |
| `/Cobros`, `/Cobros/Cobrar`, `/Cobros/Detalle`, `/MediosPago` | Cajero |
| `/Reportes`, `/Reportes/Consulta`, `/Reportes/Previsualizar`, `/Reportes/Pdf`, `/Anulacion` | Gerente |

Los controladores usan `[Authorize(Roles = "...")]`. Los procesos y reportes reciben la sesión obtenida por `User.ObtenerSesion()`; Negocio vuelve a validar permisos. Aunque la capa compartida permite al Digitador y Cajero algunas operaciones comunes de WinForms, la Web aplica la separación solicitada sin cambiar esas firmas o permisos compartidos.

Todos los POST de factura, pago, generación de reporte, anulación y catálogo de medios llevan `[ValidateAntiForgeryToken]`. Las consultas/listados no modifican documentos. Se probaron accesos directos por URL y POST de roles ajenos y anónimos, además de ausencia de token.

Se registran errores técnicos con `ILogger`; la interfaz presenta mensajes de negocio o mensajes generales sin SqlException, stack trace ni cadenas de conexión.

## Dependencias nuevas

- `PDFsharp-MigraDoc` **6.2.4**, edición Core, licencia MIT; incluye PDFsharp como dependencia. Genera tablas/documentos PDF con .NET 8 sin GDI/WPF, Office, impresión ni binarios externos en el servidor. [Ficha del paquete](https://www.nuget.org/packages/PDFsharp-MigraDoc/6.2.4).
- Fuentes Noto Sans Regular/Bold incrustadas, licencia SIL OFL 1.1 incluida en `Resources/Fonts/OFL.txt` y en la publicación. Se obtuvieron del [repositorio oficial Noto](https://github.com/notofonts/noto-fonts). El resolutor de fuentes usa recursos de la aplicación, sin consultar fuentes instaladas ni rutas de Windows. [Resolución portable de fuentes](https://docs.pdfsharp.net/PDFsharp/Topics/Fonts/Font-Resolving.html).

No se agregó EF, Dapper ni DbContext. Tampoco dependencias PDF a WinForms, Datos, Negocio o Entidades.

## Compilación

```text
dotnet build ProyectoBD2.sln
Errores: 0
Advertencias: 0

dotnet build VerificacionWeb/ProyectoBD.VerificacionWeb.csproj
Errores: 0
Advertencias: 0
```

Se compiló después de los bloques de controladores/servicios, vistas y al terminar. También se publicó localmente `ProyectoBD.Web` en Release y se ejecutó el recorrido integral sobre esa publicación, en modo Production. Es una comprobación local del artefacto; no constituye despliegue ni ejecución en Linux.

Evidencias: `Documentacion/CompilacionBloqueWebFinal.txt`, `CompilacionFinalWeb.txt`, `CompilacionVerificacionFinalWeb.txt`, `PublicacionLocalWebFinal.txt`.

## Pruebas individuales

Ejecutadas mediante HTTP real, cookies, antiforgery y SQL Server:

- Login de los roles existentes usando contraseñas de variables de entorno; usuario Digitador temporal con contraseña aleatoria. Ninguna contraseña real está en el código de pruebas.
- Menús, URL GET/POST por rol, anónimos, operaciones sin token, detalle inexistente y anulación por GET.
- Mantenimientos y procesos Web previos accesibles con su rol; `/health` conservado.
- Alta, edición y eliminación exclusivamente de un medio de pago temporal.
- Agregar/quitar líneas de factura y pago no escribe documentos.
- Venta directa y desde cotización; cliente, total y estado manipulados en el POST no sustituyen los datos del servidor.
- Rechazo sin cambios de inventario/documentos: cotización ya facturada, inventario insuficiente, cantidad cero, variantes duplicadas y precio con tres decimales.
- Rechazo sin cambios de pagos: sobrepago, cero/negativo, más de dos decimales, método inexistente, referencia de 101 caracteres y segundo cobro de factura pagada/anulada.
- Pago parcial y combinación de tarjeta/cheque en un solo POST; saldo y estado comprobados en BD.
- Siete reportes con HTML y descarga PDF, filtros, número inexistente y fechas invertidas.
- Token de reporte inaccesible para otro Gerente y PDF inexistente sin una copia válida.
- Consulta de anulada con detalle, pagos y auditoría; anulación sin Sí no cambia datos; segunda anulación no modifica stock ni auditoría.
- Descarga real en el navegador desde la previsualización del Gerente, además de las pruebas HTTP automatizadas.
- PDF renderizados con Poppler y examinados visualmente: tablas, saltos de página, cadenas de lote largas y auditoría sin solapamientos. Texto Unicode comprobado con pypdf. Estas herramientas se usaron para verificación local y no son dependencias del servidor.

Resultados: `Documentacion/ResultadoWebFinal.txt`, `ValidacionPdfWebFinal.txt`, `EvidenciaWebFinal/Reporte_1.pdf` hasta `Reporte_7.pdf`, con sus HTML. El PDF 7 conservado corresponde a la consulta posterior a anular.

## Prueba integral

Última ejecución completa: **18/11/2026**, sobre la publicación Release local. Identificador: **PRUEBAWEBFINAL_3ed27a**. Los registros auxiliares del preparador anterior usaron el sufijo propio `wv13d2c0b2f8`.

| Registro temporal | Identificador |
|---|---|
| Digitador | 67 |
| Cliente | 35 |
| Proveedor | 29 |
| Producto | 41 |
| Variantes A/B | 53 / 54 |
| Ubicación | 24 |
| Orden de compra | 36 |
| Ingreso parcial / completo | 45 / 46 |
| Cotización confirmada y facturada | 30 |
| Factura desde cotización | ID 1053, número 7 |
| Factura directa | ID 1055, número 8 |
| Lote A antiguo / reciente | 71 / 72 |

Recorrido comprobado:

1. Proveedor y orden por **5 A + 7 B**. Stock inicial A/B **0/0**.
2. Recepción parcial: **2 A**, lote `PRUEBAWEBFINAL_3ed27a_ANTIGUO`, fecha dos días anterior. Orden Parcial; stock **2/0**.
3. Recepción completa: **3 A + 7 B**, lotes `..._RECIENTE` y `..._B`, fecha un día anterior. Orden Completa, pendientes cero; stock **5/7**.
4. Cliente → cotización de **4 A × Q10 = Q40** → confirmación → factura 7. Se intentó imponer otro cliente, cantidad/precio/total y estado desde POST; la factura usó la cotización real y el usuario autenticado.
5. PEPS asignó **2 unidades del lote 71** y **2 del 72** a `DetalleFacturaLote`. Quedaron **0 y 1** unidades respectivamente. Se verificaron las dos salidas `SALIDA_VENTA`; stock A **1**.
6. Venta directa de **7 B × Q10 = Q70**, factura 8; stock B **0**. Sirvió también para comprobar el reporte de variantes sin existencias.
7. Cobro de factura 7: primer POST **Q10 Efectivo**; estado Pendiente, saldo Q30. Segundo POST simultáneo **Q20 Tarjeta + Q10 Cheque**; tres pagos históricos, estado Pagada, saldo **Q0**. Métodos existentes: Efectivo 15, Tarjeta 16 y Cheque 17; referencias con el identificador de esta prueba.
8. Reporte 1 del día: incremento **Q110 facturado, Q40 pagado y Q70 saldo**, con medios de pago. Reportes 2/3/5 incluyeron **4 A y 7 B**; reporte 4 mostró la unidad A restante por lote; reporte 6 incluyó B agotada; reporte 7 mostró encabezado, detalle, pagos y auditoría. Los siete generaron HTML y PDF descargable.
9. Consulta y anulación de factura 7: estado **Anulada**, total actual **Q0**, original **Q40**; stock A volvió a **5**. Se conservaron detalle, dos asignaciones a lotes, tres pagos históricos y auditoría de anulación.
10. El reporte de facturación dejó de contar Q40 de la anulada y el de cantidades dejó de contar sus cuatro unidades. Una previsualización anterior conservó su total Q110. Segunda anulación y cobro posterior quedaron bloqueados sin cambios.
11. Se anuló también la factura directa de prueba: stock B volvió a **7** y los reportes de ventas del día volvieron a sus valores anteriores al recorrido.

```text
Stock A: 0 → 2 → 5 → 1 → 5
Stock B: 0 → 7 → 0 → 7
```

Finalmente se eliminaron exclusivamente los documentos temporales propios, sus asignaciones/movimientos/pagos/auditoría y los catálogos/usuarios creados para esa prueba. Los DELETE de la herramienta están limitados a los IDs propios y al cliente/actor de la ejecución. No forman parte de la Web. No se tocaron las facturas, lotes ni usuarios anteriores; quedaron los tres usuarios originales y seis facturas operativas existentes. Los IDs anteriores ya no identifican registros activos; se conservan aquí y en las evidencias para documentar lo probado. Las pruebas pueden dejar saltos normales de identidad.

Para repetir, iniciar la Web localmente, proporcionar `PRUEBA_WEB_GERENTE_PASSWORD` y `PRUEBA_WEB_CAJERO_PASSWORD` al proceso y ejecutar desde la raíz:

```powershell
dotnet build VerificacionWeb/ProyectoBD.VerificacionWeb.csproj
dotnet VerificacionWeb/bin/Debug/net8.0/ProyectoBD.VerificacionWeb.dll --final http://127.0.0.1:5190
```

## Integridad

Comparación SHA-256 contra el estado previo a las modificaciones:

- **282 archivos existentes revisados**: 279 sin cambios; los únicos tres modificados son los de producción Web descritos arriba.
- **0 cambios** en WinForms, Entidades, Datos, Negocio y Database.
- **0 cambios** en autenticación, BCrypt, Usuarios/Roles Web, mantenimientos y procesos Web previamente terminados.
- **14 procedimientos instalados revisados**, con **0 cambios** en sus definiciones, incluidos facturación PEPS, pagos, anulación y reportes.
- Ningún SQL directo, EF, Dapper o DbContext en los controladores nuevos.
- Se conservaron `PROYECTOBD_CONNECTION_STRING`, el fallback existente de desarrollo y `/health`.

Evidencias: `Documentacion/IntegridadWebFinal.csv` (hash inicial/final por archivo), `IntegridadWebFinal.txt` y `ProcedimientosWebFinal.txt`.

## Pendiente

No quedan módulos funcionales pendientes dentro del bloque solicitado. Quedan **Docker/Compose, Dokploy, Kubernetes/Ingress, HTTPS, CI/CD e infraestructura de Redes**, expresamente fuera de esta ejecución. La compatibilidad Linux se preparó mediante PDF Core y fuentes incrustadas; su ejecución en el contenedor y la conexión al SQL Server de despliegue se verificarán en esa etapa.
