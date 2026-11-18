# Mantenimientos de ProyectoBD.Web — 17 de noviembre de 2026

## Estado inicial

Se inspeccionaron los proyectos, las entidades, las firmas de Negocio/Datos, los controladores, las vistas, la configuración y el formulario de Productos de WinForms antes de implementar. La compilación inicial tuvo 0 errores y 0 advertencias (`EstadoInicialWeb.txt`).

La Web tenía autenticación, Usuarios, cambio de contraseña y Roles. Los ocho mantenimientos de esta entrega no tenían controladores ni vistas; el menú mostraba cuatro enlaces deshabilitados. Las capas compartidas ya ofrecían toda la funcionalidad necesaria.

Se reutilizaron las firmas reales `Listar`, `Guardar`, `Actualizar`, `Eliminar`, `Guardar(Producto, bool)`, `ListarVariantes(int?)`, `GuardarVariante(ProductoVariante, bool)` y `EliminarVariante`. No se modificó ninguna clase compartida ni WinForms. Se conservó la arquitectura Web → Negocio → Datos → SQL Server, con Entidades compartido.

## Archivos creados

En `ProyectoBD.Web/Controllers`:

- `ClientesController.cs`
- `ProveedoresController.cs`
- `MarcasController.cs`
- `CategoriasController.cs`
- `PresentacionesController.cs`
- `ColoresController.cs`
- `ProductosController.cs`
- `VariantesController.cs`

En `ProyectoBD.Web/Models`:

- `ClienteViewModel.cs`
- `ProveedorViewModel.cs`
- `MarcaViewModel.cs`
- `CategoriaViewModel.cs`
- `PresentacionViewModel.cs`
- `ColorViewModel.cs`
- `ProductoViewModel.cs` (entrada y fila del listado de productos).
- `ProductoVarianteViewModel.cs` (entrada y fila del listado de variantes).

En cada directorio `Views/Clientes`, `Views/Proveedores`, `Views/Marcas`, `Views/Categorias`, `Views/Presentaciones`, `Views/Colores`, `Views/Productos` y `Views/Variantes`:

- `Index.cshtml`
- `Crear.cshtml`
- `Editar.cshtml`
- `_Formulario.cshtml`

Además:

- `ProyectoBD.Web/Views/Shared/_MensajesMantenimiento.cshtml`.
- `VerificacionWeb/ProyectoBD.VerificacionWeb.csproj` y `Program.cs`: herramienta de pruebas HTTP, separada de la aplicación.
- Este documento y los archivos de resultados indicados abajo.

## Archivos modificados

- `ProyectoBD.Web/Program.cs`: siete registros de Negocio y corrección de `/Account/AccessoDenegado` a `/Account/AccesoDenegado`.
- `ProyectoBD.Web/Views/Shared/_Layout.cshtml`: enlaces de mantenimientos de Digitador.
- Preferencia local de Visual Studio: proyecto de inicio Web y perfil HTTP; no cambia la configuración de despliegue.

Se verificaron por hashes 125 archivos protegidos de Presentación, Negocio, Datos, Entidades, scripts Database y controladores de autenticación/Usuarios/Roles y helper de sesión. **0 archivos protegidos modificados**. Evidencia: `IntegridadMantenimientosWeb.txt`.

## Módulos terminados

- [x] Clientes: Particular/Empresa y nombre o razón social.
- [x] Proveedores: nombre real de `Proveedor`.
- [x] Marcas: nombre.
- [x] Categorías: nombre y descripción.
- [x] Presentaciones: nombre y unidad de medida existente.
- [x] Colores: nombre.
- [x] Productos: nombre, descripción, categoría y marca.
- [x] Variantes: producto, presentación, color opcional y precio de venta.

Cada módulo tiene Index, Crear, Editar y Eliminar. Las vistas usan Bootstrap existente, validación mediante DataAnnotations/ModelState, mensajes TempData, antiforgery y confirmación JavaScript para eliminar. Los catálogos de los formularios se vuelven a cargar cuando hay errores.

Los listados de Productos y Variantes muestran nombres de relaciones. Productos enlaza a las variantes filtradas; Variantes también permite ver todos los productos y filtrar mediante un select. Una variante se crea eligiendo producto; al editar conserva su producto original, como exige la implementación existente de Datos. Se admite `ID_Color = null` mediante «Sin color». Negocio sigue validando precio positivo, límite y máximo de dos decimales.

No se añadieron ubicaciones: las entidades Producto y ProductoVariante no dependen de una ubicación; esa relación pertenece al inventario y queda fuera del alcance.

## Program.cs

Servicios añadidos una sola vez:

```csharp
builder.Services.AddScoped<ClienteNegocio>();
builder.Services.AddScoped<ProveedorNegocio>();
builder.Services.AddScoped<MarcaNegocio>();
builder.Services.AddScoped<CategoriaNegocio>();
builder.Services.AddScoped<PresentacionNegocio>();
builder.Services.AddScoped<ColorProductoNegocio>();
builder.Services.AddScoped<ProductoNegocio>();
```

Variantes reutiliza `ProductoNegocio`; no existe ni se inventó otro negocio de variantes. Se conservaron los registros existentes de Usuario/Rol, cookies, claims, Login y `/health`. No se cambió la lógica de `PROYECTOBD_CONNECTION_STRING` ni el fallback LocalDB.

## Menú

Digitador → Mantenimientos: Productos, Clientes, Proveedores, Marcas, Categorías, Presentaciones, Colores y Variantes de producto. Se conservaron Administración de Gerente, Usuarios/Roles, Caja de Cajero y los procesos aún pendientes.

## Seguridad

Los ocho controladores nuevos tienen `[Authorize(Roles = "Digitador")]`. Todas las escrituras requieren POST y `[ValidateAntiForgeryToken]`; no se elimina mediante GET. Se usan ViewModels con mapeo explícito a entidades. Se comprueban las relaciones seleccionadas en Productos/Variantes y la coincidencia entre ID de ruta y modelo al editar.

No hay comandos ni conexiones SQL en los controladores. Los errores de relaciones se traducen en «No se puede eliminar porque el registro está siendo utilizado por otras operaciones». Otros errores inesperados se registran en el servidor y se presentan al usuario mediante un mensaje genérico.

Se conservaron las reglas existentes para duplicados: los catálogos actuales no tienen índices únicos sobre nombres. No se impuso una restricción nueva. Cuando Datos recibe un rechazo por unicidad, conserva su traducción a un mensaje entendible, que los formularios controlan.

## Compilación

Solución completa: **0 errores, 0 advertencias** (`CompilacionFinalWeb.txt`). Se compiló al completar los mantenimientos, después de corregir el precio de Variantes y en la revisión final. La herramienta de pruebas también compila con 0 errores y 0 advertencias (`CompilacionPruebasWeb.txt`).

## Pruebas ejecutadas realmente

Se inició la Web en un puerto local de prueba y se realizaron peticiones HTTP reales con cookies y tokens antiforgery contra `Proyecto_BD`, pasando por los controladores MVC y las capas existentes:

- Autenticación de Gerente y Cajero, más un Digitador temporal con contraseña aleatoria. No se cambió ninguna contraseña existente.
- Los ocho listados y formularios de crear/editar; GET de registros inexistentes devuelve 404.
- Crear, editar y eliminar registros temporales en los ocho módulos; comprobación de persistencia mediante Negocio.
- Validaciones de obligatorios, longitudes, tipo de cliente y FK inválidas. Catálogos disponibles al volver a mostrar formularios con error.
- Variante con color nulo, cambio a color válido y precio actualizado. Rechazo de precio con más de dos decimales y de reasignación a otro producto.
- Productos/Variantes muestran los nombres de categoría, marca, presentación y color. El filtro de variantes funciona.
- Rechazo de eliminaciones por FK en los ocho módulos: registros temporales relacionados para Productos/Marcas/Categorías/Presentaciones/Colores y referencias existentes comprobadas de facturas, órdenes y lotes para Clientes/Proveedores/Variantes. Se verificó que los registros protegidos siguieran existiendo y que no se expusieran excepciones SQL al navegador.
- Anónimos redirigidos a Login. Gerente y Cajero bloqueados en Index/Crear/Editar y en los POST de Crear/Editar/Eliminar de los ocho controladores.
- POST sin token devuelve 400; GET de eliminación devuelve 405.
- Enlaces del menú según rol; Usuarios y Roles existentes accesibles para Gerente; ruta y vista de acceso denegado correctas.
- `/health` responde `Healthy`.

La primera ejecución detectó un detalle de interpolación Razor en el precio de Variantes; se corrigió y se repitió el recorrido completo, que terminó correctamente. Los registros temporales y el usuario temporal fueron eliminados al finalizar, incluso en la ejecución que detectó el detalle. No se eliminaron documentos ni registros operativos existentes, ni se ejecutaron migraciones o cambios de esquema/procedimientos.

Resultado completo: `ResultadoMantenimientosWeb.txt`.

Para repetir las pruebas, ejecutar la Web en un puerto local y proporcionar las contraseñas de las cuentas existentes mediante variables de entorno del proceso `PRUEBA_WEB_GERENTE_PASSWORD` y `PRUEBA_WEB_CAJERO_PASSWORD`. Después:

```powershell
dotnet build VerificacionWeb/ProyectoBD.VerificacionWeb.csproj
dotnet VerificacionWeb/bin/Debug/net8.0/ProyectoBD.VerificacionWeb.dll http://127.0.0.1:5187
```

La herramienta crea únicamente registros con un prefijo aleatorio propio y los limpia en orden de dependencias. No contiene contraseñas existentes en el código ni modifica el inicio de sesión de la aplicación.

## Pendiente después de este trabajo

Cotizaciones, Compras, Ingreso a bodega, Facturación/PEPS, Cobros, Reportes, PDF y Anulación Web. No se implementaron en esta entrega. Tampoco se modificaron Docker, Kubernetes o Dokploy.
