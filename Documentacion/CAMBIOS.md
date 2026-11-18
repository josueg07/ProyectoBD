# Inventario de la entrega

> Este es el inventario de la primera entrega. La revisión de formularios solicitada después, incluidos renombres y diseñadores, está en `CAMBIOS_FORMULARIOS.md`.

Comparación de fuentes contra la copia anterior a los cambios: `%TEMP%/ProyectoBD2-antes-20261020.zip`. No se incluyen bin/obj. La solución y los csproj originales conservan su contenido; el proyecto de Verificacion es nuevo y está separado. Los Designer y resx existentes se conservan.

## Archivos creados

- `Database/Fase2.sql`
- `Database/PruebasFase2.sql`
- `Documentacion/AUDITORIA.md`
- `Documentacion/ENTREGA.md`
- `Documentacion/ResultadoCompilacion.txt`
- `Documentacion/ResultadoCSharp.txt`
- `Documentacion/ResultadoSQL.txt`
- `Documentacion/ResultadoUI.txt`
- `ProyectoBD.Datos/CategoriaDatos.cs`
- `ProyectoBD.Datos/ClienteDatos.cs`
- `ProyectoBD.Datos/ColorProductoDatos.cs`
- `ProyectoBD.Datos/MarcaDatos.cs`
- `ProyectoBD.Datos/MetodoPagoDatos.cs`
- `ProyectoBD.Datos/OperacionDatos.cs`
- `ProyectoBD.Datos/PresentacionDatos.cs`
- `ProyectoBD.Datos/ProcesoDatos.cs`
- `ProyectoBD.Datos/ProductoDatos.cs`
- `ProyectoBD.Datos/ProveedorDatos.cs`
- `ProyectoBD.Datos/UbicacionDatos.cs`
- `ProyectoBD.Datos/UsuarioDatos.Administracion.cs`
- `ProyectoBD.Entidades/Categoria.cs`
- `ProyectoBD.Entidades/Cliente.cs`
- `ProyectoBD.Entidades/ColorProducto.cs`
- `ProyectoBD.Entidades/Documentos.cs`
- `ProyectoBD.Entidades/Marca.cs`
- `ProyectoBD.Entidades/MetodoPago.cs`
- `ProyectoBD.Entidades/Presentacion.cs`
- `ProyectoBD.Entidades/Producto.cs`
- `ProyectoBD.Entidades/Proveedor.cs`
- `ProyectoBD.Entidades/Ubicacion.cs`
- `ProyectoBD.Negocio/CategoriaNegocio.cs`
- `ProyectoBD.Negocio/ClienteNegocio.cs`
- `ProyectoBD.Negocio/ColorProductoNegocio.cs`
- `ProyectoBD.Negocio/MarcaNegocio.cs`
- `ProyectoBD.Negocio/MetodoPagoNegocio.cs`
- `ProyectoBD.Negocio/PermisosNegocio.cs`
- `ProyectoBD.Negocio/PresentacionNegocio.cs`
- `ProyectoBD.Negocio/ProcesoNegocio.cs`
- `ProyectoBD.Negocio/ProductoNegocio.cs`
- `ProyectoBD.Negocio/ProveedorNegocio.cs`
- `ProyectoBD.Negocio/UbicacionNegocio.cs`
- `ProyectoBD.Negocio/UsuarioNegocio.Administracion.cs`
- `ProyectoBD.Presentacion/EditorDetalle.cs`
- `ProyectoBD.Presentacion/FrmCotizacion.cs`
- `ProyectoBD.Presentacion/FrmFactura.cs`
- `ProyectoBD.Presentacion/FrmIngresoBodega.cs`
- `ProyectoBD.Presentacion/FrmMetodosPago.cs`
- `ProyectoBD.Presentacion/FrmOrdenCompra.cs`
- `ProyectoBD.Presentacion/FrmPagos.cs`
- `ProyectoBD.Presentacion/FrmUbicaciones.cs`
- `ProyectoBD.Presentacion/FrmUsuarios.cs`
- `ProyectoBD.Presentacion/ImpresionFactura.cs`
- `ProyectoBD.Presentacion/Vista.cs`
- `Verificacion/Program.cs`
- `Verificacion/ProyectoBD.Verificacion.csproj`
- `Documentacion/CAMBIOS.md` (este inventario).
- `Documentacion/Capturas/`: 35 imágenes de las verificaciones de formularios y factura.

## Archivos modificados

- `ProyectoBD.Datos/RolDatos.cs`
- `ProyectoBD.Datos/UsuarioDatos.cs`
- `ProyectoBD.Entidades/Rol.cs`
- `ProyectoBD.Entidades/Usuario.cs`
- `ProyectoBD.Negocio/RolNegocio.cs`
- `ProyectoBD.Negocio/UsuarioNegocio.cs`
- `ProyectoBD.Presentacion/Categorias.cs`
- `ProyectoBD.Presentacion/Clientes.cs`
- `ProyectoBD.Presentacion/Colores.cs`
- `ProyectoBD.Presentacion/Login.cs`
- `ProyectoBD.Presentacion/Marcas.cs`
- `ProyectoBD.Presentacion/Menu.cs`
- `ProyectoBD.Presentacion/Presentaciones.cs`
- `ProyectoBD.Presentacion/Productos.cs`
- `ProyectoBD.Presentacion/Proveedores.cs`
- `ProyectoBD.Presentacion/ProyectoBD.Presentacion.csproj.user`
- `ProyectoBD.Presentacion/Roles.cs`

Los cambios de Login.cs se limitan al retorno desde el menú al cerrar sesión; se conserva su verificación BCrypt. UsuarioNegocio.cs conserva IniciarSesion y CrearUsuarioInicial; se volvió partial para agregar administración en un archivo separado. UsuarioDatos.cs conserva búsqueda y creación inicial, con la misma ampliación partial. Usuario.cs agrega NombreCompleto. Rol.cs sólo recibió formato. ConexionBD.cs, UsuarioSesion.cs, Program.cs y los csproj originales no se modificaron.

## Clases agregadas

- `ProyectoBD.Datos/CategoriaDatos.cs`: `CategoriaDatos`.
- `ProyectoBD.Datos/ClienteDatos.cs`: `ClienteDatos`.
- `ProyectoBD.Datos/ColorProductoDatos.cs`: `ColorProductoDatos`.
- `ProyectoBD.Datos/MarcaDatos.cs`: `MarcaDatos`.
- `ProyectoBD.Datos/MetodoPagoDatos.cs`: `MetodoPagoDatos`.
- `ProyectoBD.Datos/OperacionDatos.cs`: `OperacionDatos`.
- `ProyectoBD.Datos/PresentacionDatos.cs`: `PresentacionDatos`.
- `ProyectoBD.Datos/ProcesoDatos.cs`: `ProcesoDatos`.
- `ProyectoBD.Datos/ProductoDatos.cs`: `ProductoDatos`.
- `ProyectoBD.Datos/ProveedorDatos.cs`: `ProveedorDatos`.
- `ProyectoBD.Datos/UbicacionDatos.cs`: `UbicacionDatos`.
- `ProyectoBD.Entidades/Categoria.cs`: `Categoria`.
- `ProyectoBD.Entidades/Cliente.cs`: `Cliente`.
- `ProyectoBD.Entidades/ColorProducto.cs`: `ColorProducto`.
- `ProyectoBD.Entidades/Documentos.cs`: `LineaDocumento`, `Cotizacion`, `OrdenCompra`, `LineaIngreso`, `IngresoBodega`, `Factura`, `PagoFactura`.
- `ProyectoBD.Entidades/Marca.cs`: `Marca`.
- `ProyectoBD.Entidades/MetodoPago.cs`: `MetodoPago`.
- `ProyectoBD.Entidades/Presentacion.cs`: `Presentacion`.
- `ProyectoBD.Entidades/Producto.cs`: `Producto`, `ProductoVariante`.
- `ProyectoBD.Entidades/Proveedor.cs`: `Proveedor`.
- `ProyectoBD.Entidades/Ubicacion.cs`: `Ubicacion`.
- `ProyectoBD.Negocio/CategoriaNegocio.cs`: `CategoriaNegocio`.
- `ProyectoBD.Negocio/ClienteNegocio.cs`: `ClienteNegocio`.
- `ProyectoBD.Negocio/ColorProductoNegocio.cs`: `ColorProductoNegocio`.
- `ProyectoBD.Negocio/MarcaNegocio.cs`: `MarcaNegocio`.
- `ProyectoBD.Negocio/MetodoPagoNegocio.cs`: `MetodoPagoNegocio`.
- `ProyectoBD.Negocio/PermisosNegocio.cs`: `PermisosNegocio`.
- `ProyectoBD.Negocio/PresentacionNegocio.cs`: `PresentacionNegocio`.
- `ProyectoBD.Negocio/ProcesoNegocio.cs`: `ProcesoNegocio`.
- `ProyectoBD.Negocio/ProductoNegocio.cs`: `ProductoNegocio`.
- `ProyectoBD.Negocio/ProveedorNegocio.cs`: `ProveedorNegocio`.
- `ProyectoBD.Negocio/UbicacionNegocio.cs`: `UbicacionNegocio`.
- `ProyectoBD.Presentacion/EditorDetalle.cs`: `EditorDetalle`.
- `ProyectoBD.Presentacion/FrmCotizacion.cs`: `FrmCotizacion`.
- `ProyectoBD.Presentacion/FrmFactura.cs`: `FrmFactura`.
- `ProyectoBD.Presentacion/FrmIngresoBodega.cs`: `FrmIngresoBodega`.
- `ProyectoBD.Presentacion/FrmMetodosPago.cs`: `FrmMetodosPago`.
- `ProyectoBD.Presentacion/FrmOrdenCompra.cs`: `FrmOrdenCompra`.
- `ProyectoBD.Presentacion/FrmPagos.cs`: `FrmPagos`.
- `ProyectoBD.Presentacion/FrmUbicaciones.cs`: `FrmUbicaciones`.
- `ProyectoBD.Presentacion/FrmUsuarios.cs`: `FrmUsuarios`.
- `ProyectoBD.Presentacion/ImpresionFactura.cs`: `ImpresionFactura`.
- `ProyectoBD.Presentacion/Vista.cs`: `Vista`.
- `Verificacion/Program.cs`: `Program`.

UsuarioDatos.Administracion.cs y UsuarioNegocio.Administracion.cs amplían las clases existentes; no son clases duplicadas.

## Métodos agregados

Se listan los métodos nuevos por archivo; también se incluyen auxiliares privados de validación, carga y presentación. Los métodos existentes modificados se describen en el apartado siguiente.

- `ProyectoBD.Datos/CategoriaDatos.cs`: `Listar`, `Insertar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Datos/ClienteDatos.cs`: `Listar`, `Insertar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Datos/ColorProductoDatos.cs`: `Listar`, `Insertar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Datos/MarcaDatos.cs`: `Listar`, `Insertar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Datos/MetodoPagoDatos.cs`: `Listar`, `Insertar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Datos/OperacionDatos.cs`: `Parametro`, `Tabla`, `Ejecutar`.
- `ProyectoBD.Datos/PresentacionDatos.cs`: `Listar`, `Insertar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Datos/ProcesoDatos.cs`: `Procedimiento`, `GuardarCotizacion`, `EstadoCotizacion`, `GuardarOrden`, `Ingresar`, `Facturar`, `Pagar`, `Cotizaciones`, `DetalleCotizacion`, `Ordenes`, `PendienteOrden`, `Facturas`, `DetalleFactura`, `Pagos`, `Inventario`, `Ingresos`.
- `ProyectoBD.Datos/ProductoDatos.cs`: `Listar`, `Guardar`, `Eliminar`, `ListarVariantes`, `GuardarVariante`, `EliminarVariante`.
- `ProyectoBD.Datos/ProveedorDatos.cs`: `Listar`, `Insertar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Datos/UbicacionDatos.cs`: `Listar`, `Insertar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Datos/UsuarioDatos.Administracion.cs`: `Listar`, `Guardar`, `ActualizarPassword`, `Eliminar`.
- `ProyectoBD.Negocio/CategoriaNegocio.cs`: `Listar`, `Validar`, `Guardar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Negocio/ClienteNegocio.cs`: `Listar`, `Validar`, `Guardar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Negocio/ColorProductoNegocio.cs`: `Listar`, `Validar`, `Guardar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Negocio/MarcaNegocio.cs`: `Listar`, `Validar`, `Guardar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Negocio/MetodoPagoNegocio.cs`: `Listar`, `Validar`, `Guardar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Negocio/PermisosNegocio.cs`: `Exigir`.
- `ProyectoBD.Negocio/PresentacionNegocio.cs`: `Listar`, `Validar`, `Guardar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Negocio/ProcesoNegocio.cs`: `Importe`, `Detalle`, `GuardarCotizacion`, `EstadoCotizacion`, `GuardarOrden`, `Ingresar`, `Facturar`, `Pagar`, `Cotizaciones`, `DetalleCotizacion`, `Ordenes`, `PendienteOrden`, `Facturas`, `DetalleFactura`, `Pagos`, `Inventario`, `Ingresos`.
- `ProyectoBD.Negocio/ProductoNegocio.cs`: `Listar`, `ListarVariantes`, `Guardar`, `GuardarVariante`, `Eliminar`, `EliminarVariante`.
- `ProyectoBD.Negocio/ProveedorNegocio.cs`: `Listar`, `Validar`, `Guardar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Negocio/UbicacionNegocio.cs`: `Listar`, `Validar`, `Guardar`, `Actualizar`, `Eliminar`.
- `ProyectoBD.Negocio/UsuarioNegocio.Administracion.cs`: `Listar`, `Guardar`, `ValidarPassword`, `CambiarPassword`, `Eliminar`.
- `ProyectoBD.Presentacion/Categorias.cs`: `Nuevo`, `Cargar`, `Guardar`.
- `ProyectoBD.Presentacion/Clientes.cs`: `Nuevo`, `Cargar`, `Guardar`.
- `ProyectoBD.Presentacion/Colores.cs`: `Nuevo`, `Cargar`, `Guardar`.
- `ProyectoBD.Presentacion/EditorDetalle.cs`: `Cargar`, `Establecer`, `ActualizarTotal`.
- `ProyectoBD.Presentacion/FrmCotizacion.cs`: `Cargar`, `Estado`.
- `ProyectoBD.Presentacion/FrmFactura.cs`: `Seleccionada`, `CargarHistorial`, `Cargar`.
- `ProyectoBD.Presentacion/FrmIngresoBodega.cs`: `CargarOrdenes`, `CargarDetalle`, `Guardar`.
- `ProyectoBD.Presentacion/FrmMetodosPago.cs`: `Nuevo`, `Cargar`, `Guardar`.
- `ProyectoBD.Presentacion/FrmOrdenCompra.cs`: `Cargar`.
- `ProyectoBD.Presentacion/FrmPagos.cs`: `Cargar`, `ActualizarSaldo`.
- `ProyectoBD.Presentacion/FrmUbicaciones.cs`: `Nuevo`, `Cargar`, `Guardar`.
- `ProyectoBD.Presentacion/FrmUsuarios.cs`: `Nuevo`, `Cargar`, `Guardar`.
- `ProyectoBD.Presentacion/ImpresionFactura.cs`: `Previsualizar`, `CrearDocumento`, `Envolver`.
- `ProyectoBD.Presentacion/Marcas.cs`: `Nuevo`, `Cargar`, `Guardar`.
- `ProyectoBD.Presentacion/Menu.cs`: `AbrirProceso`, `MostrarReporte`.
- `ProyectoBD.Presentacion/Presentaciones.cs`: `Nuevo`, `Cargar`, `Guardar`.
- `ProyectoBD.Presentacion/Productos.cs`: `Enlazar`, `Cargar`, `CargarVariantes`, `Guardar`, `GuardarVariante`.
- `ProyectoBD.Presentacion/Proveedores.cs`: `Nuevo`, `Cargar`, `Guardar`.
- `ProyectoBD.Presentacion/Vista.cs`: `Ejecutar`, `Preparar`, `Tabla`, `Boton`, `Campo`, `Combo`, `Numero`, `Confirmar`.
- `Verificacion/Program.cs`: `CrearGerente`, `LeerPassword`, `Afirmar`, `ProbarFase1`, `ProbarProcesos`, `Descendientes`, `Capturar`, `ProbarFormularios`.

## Cambios en clases y formularios existentes

- RolDatos: conserva Listar/Insertar/Actualizar/Eliminar; traduce errores de rol asignado y duplicados a mensajes claros.
- RolNegocio: conserva ListarRoles/Guardar/Actualizar/Eliminar; valida longitud del nombre.
- Roles: conserva controles y diseñador; conecta selección y Editar, completa Eliminar, protege la carga y cierra para regresar al menú sin crear otra sesión.
- Marcas, Proveedores, Clientes: se agregan controles en código al formulario existente (los diseñadores estaban vacíos), con selección, Nuevo, Guardar, Editar, Eliminar y Volver.
- Productos: dos pestañas para el producto general y sus variantes; ComboBox de categoría, marca, presentación y color; precio numérico; CRUD independiente. No se eliminó la estructura de variantes.
- Categorias, Presentaciones y Colores: se completaron como catálogos necesarios para productos; se exigen descripción y unidad de medida según el esquema real.
- ProyectoBD.Presentacion.csproj.user: metadatos de diseñador para reconocer los nuevos formularios y EditorDetalle; no cambia las referencias ni la configuración del csproj.
- Menu: conserva MenuStrip y permisos originales, conecta procesos/cobros/reportes, incorpora ubicaciones y mantiene abierto el menú al cerrar formularios. Cerrar sesión retorna al login.
- FrmUsuarios: crea/edita/elimina usuarios, selecciona rol y cambia contraseña. Oculta hash e ID_Rol y muestra nombre completo.
- FrmCotizacion: nueva cotización y pestaña de historial con confirmar/cancelar y consulta de líneas.
- FrmOrdenCompra: nueva orden y consulta de cantidades recibidas y pendientes.
- FrmIngresoBodega: selección de orden/ubicación/fecha; grilla con cantidades solicitadas y recibidas protegidas; edición sólo de recepción, costo y lote opcional.
- FrmFactura: venta directa o carga bloqueada de cotización confirmada; historial, consulta de detalle, cobro y previsualización.
- FrmPagos: lista de pagos por registrar y consulta de los ya registrados, con saldo y referencias.
- ImpresionFactura: crea PrintDocument y PrintPreviewDialog, envuelve texto largo y pagina; muestra datos, totales y pagos.
- Vista y EditorDetalle: auxiliares pequeños de controles WinForms y captura de líneas, sin SQL.

## Procedimientos almacenados

Archivo `Database/Fase2.sql`: `usp_GuardarCotizacion`, `usp_EstadoCotizacion`, `usp_GuardarOrdenCompra`, `usp_RegistrarIngresoBodega`, `usp_FacturarPEPS`, `usp_RegistrarPagos`. Se instalaron con CREATE OR ALTER. No se añadieron tablas, columnas, UDT ni otra base de datos. Se utiliza JSON como formato de transporte de los detalles y de auditoría.

## Validación y pendientes

Compilación completa y pruebas: ver ENTREGA.md y los archivos Resultado*.txt. Se verificó CRUD, BCrypt, permisos de procesos, recepción parcial/completa, PEPS, rollback, venta desde cotización, cobros, dos ventas concurrentes, carga de formularios y render de factura. No hay módulos requeridos pendientes de código. La impresión física queda para el equipo/impresora del usuario; la previsualización se verificó mediante render. No se probaron todas las combinaciones posibles de drivers, resoluciones o versiones de SQL Server.
