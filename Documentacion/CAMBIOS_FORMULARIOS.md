# Revisión de formularios

Se ajustó exclusivamente la presentación y su utilidad de verificación. No se modificaron las capas Entidades, Negocio o Datos, el SQL ni ProyectoBD.Web en esta revisión. Se conserva el comportamiento funcional de las Fases 1 y 2.

## Diseño y código

- Los controles se definen en InitializeComponent de cada .Designer.cs, con sus campos y conexiones a eventos. Los .cs contienen constructores, Load, Click, SelectionChanged y métodos de carga/guardado.
- Se utilizan GroupBox para separar datos, productos/variantes e historial. Se quitaron TabControl y los generadores dinámicos de controles.
- Los formularios usan DarkCyan como Menu, con títulos y botones de estilo similar al diseño original. Login sólo recibió el mismo color de fondo; su autenticación no cambió.
- FormBorderStyle.None elimina la barra superior. Volver cierra el formulario; el menú se conserva, se oculta durante la navegación y vuelve a mostrarse al regresar. Se verificó el evento del botón en las pantallas de prueba.
- Las acciones se conectan con métodos normales, por ejemplo btnGuardar_Click, en vez de concentrarlas en expresiones lambda dentro del constructor.
- Los controles del detalle de cotización, compra y factura pertenecen directamente a cada formulario. Se retiraron Vista y EditorDetalle, porque ya no se necesitan para construir la interfaz.
- Los constructores sin parámetros permiten que el diseñador cree las pantallas. Los constructores con UsuarioSesion continúan usando la sesión al ejecutarlas; no se carga SQL en modo diseño.
- Las consultas de reportes también tienen un formulario Reportes con su diseñador y Volver.

## Nombres actuales

| Nombre anterior | Nombre actual |
|---|---|
| FrmUsuarios | Usuarios |
| FrmCotizacion | Cotizacion |
| FrmFactura | Factura |
| FrmOrdenCompra | OrdenCompra |
| FrmIngresoBodega | IngresoBodega |
| FrmPagos | Pagos |
| FrmMetodosPago | MetodosPago |
| FrmUbicaciones | Ubicaciones |

Los formularios originales conservan sus nombres: Roles, Marcas, Proveedores, Clientes, Productos, Categorias, Presentaciones, Colores, Menu y Login. Los nombres singulares de las entidades no se cambiaron; en los formularios con el mismo nombre se utiliza ProyectoBD.Entidades para evitar ambigüedad.

## Cómo revisar en Visual Studio

1. Abrir ProyectoBD2.sln y, si permanecía abierta una pestaña Frm..., cerrarla y abrir el archivo con su nombre actual.
2. En ProyectoBD.Presentacion, hacer clic derecho sobre Marcas.cs, Productos.cs, Usuarios.cs, Cotizacion.cs u otro formulario y elegir **Ver diseñador** (Shift+F7).
3. Seleccionar controles, modificar Text/Location/Size o abrir sus eventos desde Propiedades. El .Designer.cs mantiene la parte visual y el .cs las acciones.
4. Ejecutar el proyecto. Entrar según el rol, abrir un mantenimiento o proceso y regresar con Volver. Productos y variantes ahora se ven al mismo tiempo; creación e historial de procesos también.

## Archivos nuevos de esta revisión

- `ProyectoBD.Presentacion/Cotizacion.Designer.cs`
- `ProyectoBD.Presentacion/Cotizacion.cs`
- `ProyectoBD.Presentacion/Cotizacion.resx`
- `ProyectoBD.Presentacion/Factura.Designer.cs`
- `ProyectoBD.Presentacion/Factura.cs`
- `ProyectoBD.Presentacion/Factura.resx`
- `ProyectoBD.Presentacion/IngresoBodega.Designer.cs`
- `ProyectoBD.Presentacion/IngresoBodega.cs`
- `ProyectoBD.Presentacion/IngresoBodega.resx`
- `ProyectoBD.Presentacion/MetodosPago.Designer.cs`
- `ProyectoBD.Presentacion/MetodosPago.cs`
- `ProyectoBD.Presentacion/MetodosPago.resx`
- `ProyectoBD.Presentacion/OrdenCompra.Designer.cs`
- `ProyectoBD.Presentacion/OrdenCompra.cs`
- `ProyectoBD.Presentacion/OrdenCompra.resx`
- `ProyectoBD.Presentacion/Pagos.Designer.cs`
- `ProyectoBD.Presentacion/Pagos.cs`
- `ProyectoBD.Presentacion/Pagos.resx`
- `ProyectoBD.Presentacion/Reportes.Designer.cs`
- `ProyectoBD.Presentacion/Reportes.cs`
- `ProyectoBD.Presentacion/Reportes.resx`
- `ProyectoBD.Presentacion/Ubicaciones.Designer.cs`
- `ProyectoBD.Presentacion/Ubicaciones.cs`
- `ProyectoBD.Presentacion/Ubicaciones.resx`
- `ProyectoBD.Presentacion/Usuarios.Designer.cs`
- `ProyectoBD.Presentacion/Usuarios.cs`
- `ProyectoBD.Presentacion/Usuarios.resx`

## Archivos modificados de esta revisión

- `ProyectoBD.Presentacion/Categorias.Designer.cs`
- `ProyectoBD.Presentacion/Categorias.cs`
- `ProyectoBD.Presentacion/Clientes.Designer.cs`
- `ProyectoBD.Presentacion/Clientes.cs`
- `ProyectoBD.Presentacion/Colores.Designer.cs`
- `ProyectoBD.Presentacion/Colores.cs`
- `ProyectoBD.Presentacion/Login.Designer.cs`
- `ProyectoBD.Presentacion/Marcas.Designer.cs`
- `ProyectoBD.Presentacion/Marcas.cs`
- `ProyectoBD.Presentacion/Menu.Designer.cs`
- `ProyectoBD.Presentacion/Menu.cs`
- `ProyectoBD.Presentacion/Presentaciones.Designer.cs`
- `ProyectoBD.Presentacion/Presentaciones.cs`
- `ProyectoBD.Presentacion/Productos.Designer.cs`
- `ProyectoBD.Presentacion/Productos.cs`
- `ProyectoBD.Presentacion/Proveedores.Designer.cs`
- `ProyectoBD.Presentacion/Proveedores.cs`
- `ProyectoBD.Presentacion/ProyectoBD.Presentacion.csproj.user`
- `ProyectoBD.Presentacion/Roles.Designer.cs`
- `ProyectoBD.Presentacion/Roles.cs`
- `Verificacion/Program.cs`

## Archivos anteriores retirados o renombrados

- `ProyectoBD.Presentacion/EditorDetalle.cs`
- `ProyectoBD.Presentacion/FrmCotizacion.cs`
- `ProyectoBD.Presentacion/FrmFactura.cs`
- `ProyectoBD.Presentacion/FrmFactura.resx`
- `ProyectoBD.Presentacion/FrmIngresoBodega.cs`
- `ProyectoBD.Presentacion/FrmMetodosPago.cs`
- `ProyectoBD.Presentacion/FrmOrdenCompra.cs`
- `ProyectoBD.Presentacion/FrmPagos.cs`
- `ProyectoBD.Presentacion/FrmUbicaciones.cs`
- `ProyectoBD.Presentacion/FrmUsuarios.cs`
- `ProyectoBD.Presentacion/Vista.cs`

Los .cs de Frm... se reemplazaron por los nombres actuales. Vista.cs y EditorDetalle.cs se retiraron al pasar los controles a sus formularios. Se actualizaron las referencias en Menu, en los metadatos de Visual Studio y en la verificación. Los archivos de compilación bin/obj no forman parte de este inventario.

## Verificación

- La utilidad --diseno construye los formularios, comprueba controles antes de Load, color DarkCyan, ausencia de barra y pestañas, y renderiza con un sitio de modo diseño sin consultar la BD. Es una prueba de construcción/diseño; no automatiza el IDE de Visual Studio.
- --ui abre las pantallas y los menús de los tres roles. Comprueba la visibilidad y posición de Volver, pulsa el botón y verifica que cierre el formulario.
- --integracion-ui prueba los CRUD y BCrypt, compra/ingresos/PEPS/pagos/cotización, concurrencia y carga de datos. También pulsa Agregar y Quitar línea, comprueba la cantidad y verifica que una cotización confirmada bloquee la edición del detalle.
- Se generó la previsualización de factura sin imprimir físicamente. Las pruebas limpian únicamente sus propios registros temporales.
- Registros: ResultadoDiseno.txt, ResultadoUIFormularios.txt, ResultadoCSharpFormularios.txt y ResultadoCompilacion.txt. Capturas: Documentacion/CapturasFormularios.
- Compilación completa de ProyectoBD2.sln: 0 errores y 0 advertencias.

Comandos desde la raíz:

```powershell
dotnet run --project Verificacion/ProyectoBD.Verificacion.csproj -- --diseno
dotnet run --project Verificacion/ProyectoBD.Verificacion.csproj -- --ui
dotnet run --project Verificacion/ProyectoBD.Verificacion.csproj -- --integracion-ui
dotnet build ProyectoBD2.sln --nologo --no-incremental
```

Copia de fuentes anterior a esta revisión: %TEMP%/ProyectoBD2-antes-diseno-20261020.zip.
