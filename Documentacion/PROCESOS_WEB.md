# Cotizaciones, órdenes de compra e ingreso a bodega Web

## Inspección y estado inicial

Se inspeccionó la solución antes de editar archivos: entidades, firmas públicas, controladores y vistas existentes, formularios WinForms equivalentes, consultas de `ProcesoDatos` y los procedimientos de `Database/Fase2.sql`. La compilación inicial terminó con **0 errores y 0 advertencias** (`EstadoInicialProcesosWeb.txt`).

La Web ya tenía autenticación, Usuarios/Roles y los ocho mantenimientos. Los tres procesos estaban pendientes y sus opciones de menú estaban deshabilitadas.

Las entidades reales están en `Documentos.cs`: `Cotizacion` y `OrdenCompra` contienen listas de `LineaDocumento`; `IngresoBodega` contiene listas de `LineaIngreso`. No existen entidades independientes de detalle de cotización/orden que fuera necesario inventar. Se respetaron los campos disponibles.

## Reutilización de las capas

Los nuevos controladores obtienen la sesión mediante `User.ObtenerSesion()` y construyen el `ProcesoNegocio` existente con ella. Se reutilizan:

- `GuardarCotizacion(Cotizacion)` y `EstadoCotizacion(int, string)`.
- `GuardarOrden(OrdenCompra)`.
- `Ingresar(IngresoBodega)`.
- `Cotizaciones()`, `DetalleCotizacion(int)`, `Ordenes()`, `PendienteOrden(int)` e `Ingresos()`.
- `ClienteNegocio`, `ProveedorNegocio`, `ProductoNegocio` y `UbicacionNegocio` para los catálogos.

Las operaciones siguen llegando a `usp_GuardarCotizacion`, `usp_EstadoCotizacion`, `usp_GuardarOrdenCompra` y `usp_RegistrarIngresoBodega` a través de Datos. **No se modificaron sus firmas, implementaciones, el algoritmo PEPS ni los procedimientos de Fase Final.** Tampoco se modificaron Negocio, Datos, Entidades o WinForms.

## Funciones terminadas

### Cotizaciones

- Listado y consulta de encabezado/detalle.
- Cliente seleccionado desde el catálogo y variantes con descripción de producto, presentación y color.
- Agregar y quitar líneas mediante el formulario, sin guardar documentos intermedios.
- Cantidades enteras positivas, precio sugerido editable, subtotales y total.
- El importe definitivo se calcula en las capas existentes; no se acepta un total o estado enviado por el navegador.
- Guardado como Pendiente, confirmación y cancelación por POST con confirmación visual.
- Las transiciones admitidas son las del procedimiento existente: Pendiente → Confirmada/Cancelada, Confirmada → Cancelada. Las canceladas/facturadas no admiten esos cambios.

### Órdenes de compra

- Crear con proveedor y variantes/cantidades sin repetir.
- Listado por estado y consulta del detalle.
- Cantidades solicitadas, recibidas y pendientes obtenidas de la consulta real.
- Enlaces para registrar un ingreso y consultar las recepciones de la orden.
- No se inventaron precios o totales de compra: el modelo existente de orden sólo registra cantidades solicitadas.

### Ingreso a bodega

- Selección de órdenes Pendiente/Parcial y consulta de los pendientes reales.
- Selección de ubicación, fecha/hora, cantidades, costos y número de lote opcional.
- Recepción parcial dejando otras líneas en cero; botón para llenar todas las cantidades pendientes.
- Se vuelven a consultar las líneas y pendientes en cada POST. No se confía en descripciones, cantidades pendientes ni estado enviados por el cliente.
- Se rechazan líneas de otra orden, detalles duplicados, sobre-recepciones, cantidades negativas/no numéricas, costos inválidos, lotes demasiado largos, ubicación inválida y fechas futuras o ausentes.
- El procedimiento original realiza el registro transaccional, crea lotes/movimientos, registra la auditoría y actualiza la orden a Parcial o Completa.
- Listado de ingresos y detalle de las cantidades/costos recibidos, con el usuario real que registró el ingreso.
- Se conserva lo introducido al mostrar errores; se actualizan los pendientes. Tras guardar se redirige al detalle del ingreso.

El JavaScript sólo mejora la sugerencia de precios, los totales visibles y el llenado de cantidades. Agregar/quitar líneas y registrar documentos funcionan mediante POST del servidor; los cálculos y validaciones definitivos no dependen de JavaScript.

## Archivos creados

En `ProyectoBD.Web`:

- `Controllers/CotizacionesController.cs`.
- `Controllers/OrdenesCompraController.cs`.
- `Controllers/IngresosBodegaController.cs`.
- `Models/CotizacionProcesoViewModel.cs`.
- `Models/OrdenCompraProcesoViewModel.cs`.
- `Models/IngresoBodegaProcesoViewModel.cs`.
- `Views/Cotizaciones/Index.cshtml`, `Crear.cshtml`, `Detalle.cshtml`.
- `Views/OrdenesCompra/Index.cshtml`, `Crear.cshtml`, `Detalle.cshtml`.
- `Views/IngresosBodega/Index.cshtml`, `Crear.cshtml`, `Detalle.cshtml`.
- `wwwroot/js/detalle-proceso.js`.

Fuera de la aplicación:

- `VerificacionWeb/Procesos.cs`, pruebas HTTP con comprobaciones contra la BD.
- Este documento y los archivos de resultados de compilación, pruebas e integridad.

## Archivos modificados

- `ProyectoBD.Web/Program.cs`: se registra únicamente `UbicacionNegocio`, que faltaba. `ProcesoNegocio` se construye con la sesión de cada petición; no se registra una sesión global ni se reconstruyen claims.
- `ProyectoBD.Web/Views/Shared/_Layout.cshtml`: se activan los tres enlaces bajo Digitador → Procesos. Facturación y los demás módulos pendientes continúan deshabilitados.
- `VerificacionWeb/Program.cs`: la clase de pruebas pasa a `partial` y admite el argumento `--procesos`. El recorrido anterior de mantenimientos se conserva.

## Seguridad y compatibilidad

Los tres controladores tienen `[Authorize(Roles = "Digitador")]`. Crear, confirmar y cancelar requieren POST y antiforgery; GET sólo consulta. La sesión vuelve a ser validada por Negocio y por los procedimientos existentes. Las excepciones inesperadas se registran en el servidor y se muestra un mensaje entendible, sin detalles SQL en la página.

Se conservaron autenticación, cookies, claims, BCrypt, Usuarios/Roles, mantenimientos, `/health`, `PROYECTOBD_CONNECTION_STRING` y LocalDB. No hay acceso SQL desde los nuevos controladores ni se añadieron tablas, migraciones, librerías frontend, EF, Dapper o DbContext.

La revisión por hashes de **191 archivos protegidos** terminó con **0 cambios** (`IntegridadProcesosWeb.txt`). También se compararon las definiciones instaladas de todos los procedimientos: **0 cambios**.

## Compilación

Se compiló durante la implementación de cotizaciones/compras, ingreso a bodega, vistas y al finalizar la solución completa. Resultado final: **0 errores, 0 advertencias** (`CompilacionFinalProcesosWeb.txt`). La herramienta de pruebas también compila sin errores ni advertencias (`CompilacionPruebasProcesosWeb.txt`).

## Pruebas ejecutadas

Se ejecutó la aplicación en un puerto local aislado y se probaron peticiones HTTP reales con cookies y tokens antiforgery, pasando por los controladores y la BD existente:

- Listados, creación, detalles y 404 para documentos inexistentes.
- Anónimos redirigidos a Login; Gerente y Cajero bloqueados en GET y POST. Cambios de estado bloqueados por GET y sin token.
- Menú por rol, acceso a los mantenimientos anteriores y `/health` conservados.
- Cotización de dos variantes: 2 × Q10.50 + 3 × Q20.00 = Q81.00. Los intentos de imponer Total/Estado desde el navegador no alteraron los valores reales.
- Confirmar y cancelar; una cotización cancelada no pudo confirmarse. Agregar/quitar líneas no creó documentos.
- Rechazo de variantes repetidas, cliente/proveedor inexistentes, cantidad no positiva y precio con tres decimales.
- Orden de prueba con cantidades 5 y 7: Pendiente → Parcial al recibir 2/0 → Completa al recibir 3/7. Se comprobaron pendientes 3/7 después del parcial y cero al terminar.
- Existencias reales finales 5/7, tres lotes, tres movimientos `ENTRADA_COMPRA`, dos auditorías de ingreso y responsable igual al usuario autenticado.
- Se verificó que los casos inválidos no cambiaran documentos, lotes, stock, movimientos ni auditoría: sobre-recepción, cantidad negativa/no numérica, costo cero/con tres decimales, lote de 51 caracteres, ubicación inexistente, fecha futura/ausente, ninguna unidad, línea de otra orden, detalle duplicado y segundo envío de una orden completa.
- Consulta de los dos ingresos y del estado final de la orden por HTTP.

Los documentos, lotes, catálogos y el usuario temporales se limpiaron exclusivamente por sus identificadores propios. No se eliminaron datos operativos existentes ni se cambiaron las contraseñas existentes. Resultado completo: `ResultadoProcesosWeb.txt`.

La herramienta de verificación usa SQL exclusivamente para comprobar y limpiar sus propios datos de prueba; no forma parte de los controladores ni de la arquitectura de producción.

También se comprobó en un navegador real el JavaScript mediante un formulario local aislado: precios sugeridos, edición de cantidades/precios, totales, conservación de identificadores y recepción de todo lo pendiente con costo obligatorio únicamente cuando se reciben unidades. Resultado: `ResultadoJavaScriptProcesosWeb.txt`.

Al entregar se inició el proyecto Web con F5 desde Visual Studio, perfil `http`, y se abrió su pantalla de inicio de sesión. `/health` en `http://localhost:5096` respondió HTTP 200, `Healthy` (`InicioVisualStudioProcesosWeb.txt`). Entrar con el usuario Digitador existente y abrir el menú **Procesos**.

Para repetir las pruebas, iniciar la Web en un puerto local y proporcionar al proceso las variables `PRUEBA_WEB_GERENTE_PASSWORD` y `PRUEBA_WEB_CAJERO_PASSWORD` con las contraseñas existentes. Después:

```powershell
dotnet build VerificacionWeb/ProyectoBD.VerificacionWeb.csproj
dotnet VerificacionWeb/bin/Debug/net8.0/ProyectoBD.VerificacionWeb.dll --procesos http://127.0.0.1:5188
```

## Fuera de esta etapa

Facturación/PEPS Web, Cobros, Reportes, consulta/anulación, PDF, Docker, Kubernetes y Dokploy siguen pendientes y no se implementaron aquí.
