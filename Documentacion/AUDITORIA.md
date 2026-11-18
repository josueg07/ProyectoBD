# Auditoría inicial — 20/10/2026

Se revisaron los cinco csproj y las clases y diseñadores de las cuatro capas de escritorio. Web sólo se incluyó en la compilación solicitada; no se modifica su código.

YA EXISTE: conexión ConexionBD, entidades Rol/Usuario/UsuarioSesion, login con BCrypt, menú con visibilidad por los tres roles, RolDatos CRUD parametrizado, creación inicial de usuarios.

INCOMPLETO: RolNegocio y Roles (faltan selección, edición, eliminación, manejo de errores y retorno preservando sesión); menú abre formularios cerrándose y tiene opciones sin eventos.

FALTA: administración de usuarios, CRUD de marcas/proveedores/clientes/productos y variantes, procesos completos de Fase 2, impresión y auditoría.

Compilación inicial: 2 errores (CS1061 RolNegocio.Listar y CS7036 new Menu sin sesión) y una advertencia CS0414. Copia previa de fuentes: %TEMP%\ProyectoBD2-antes-20261020.zip.

BD inspeccionada directamente: Proyecto_BD en (localdb)\MSSQLLocalDB. Existen las 29 tablas indicadas y sus claves foráneas; no hay SP ni triggers. Nombres reales relevantes: ID_Categoria, ID_OrdenC, ID_IngresoB, ID_MetodoP. NumeroFactura es int; cantidades int; importes decimal(12,2). Categoría requiere Descripcion, Presentacion requiere UnidadMedida. Roles oficiales ya registrados. Existe admin con rol Digitador; no se cambian sus credenciales ni rol. Productos iniciales: 0.
