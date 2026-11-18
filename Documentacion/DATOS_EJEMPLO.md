# Datos de ejemplo listos para usar

Los datos ya están insertados en `(localdb)\MSSQLLocalDB`, base `Proyecto_BD`. Se conservaron la cuenta admin, los roles y el color que ya existían. No se cambió código de los formularios ni la estructura de la BD.

Los catálogos, clientes, proveedores y productos usan sus nombres normales, sin etiquetas añadidas. Las operaciones se registraron usando los procedimientos transaccionales existentes; incluyen lotes, movimientos, relaciones PEPS y auditoría. Las fechas anteriores de las órdenes y recepciones son datos ficticios para mostrar la antigüedad de los lotes.

## Acceso

Cerrar sesión si el programa estaba abierto e ingresar con una de estas cuentas:

| Usuario | Contraseña de ejemplo | Rol |
|---|---|---|
| admin | Sin cambios; usar su contraseña existente | Digitador |
| cajero | `cajero123` | Cajero |
| gerente | `gerente 123` | Gerente |

Las contraseñas distinguen mayúsculas y minúsculas. La de gerente contiene un espacio entre gerente y 123. Se guardaron hashes BCrypt; nunca se envió la contraseña original a SQL Server. No se modificaron las credenciales de admin. Se eliminó digitador y se reasignaron sus referencias a admin, dejando constancia del usuario original en auditoría. Las cuentas de caja y gerencia conservan sus identificadores y documentos.

## Qué se cargó

- 4 categorías, 3 marcas, 4 presentaciones y 4 colores nuevos.
- 3 proveedores, 6 clientes, 2 ubicaciones y 3 métodos de pago.
- 8 productos con 12 variantes.
- 4 órdenes: 2 completas, 1 parcial y 1 pendiente.
- 5 ingresos que generaron 16 lotes internos.
- 4 cotizaciones: Pendiente, Confirmada, Cancelada y Facturada.
- 3 facturas: pagada, con abono y sin pagos.
- 4 registros de pago, 23 movimientos y auditoría de las operaciones y de la actualización de cuentas.
- 2 cuentas adicionales: cajero y gerente; se reutiliza admin como Digitador.

Las devoluciones y sus detalles se dejaron vacíos: no hay un formulario de devoluciones implementado en el alcance actual. Las cantidades y saldos de esta guía son el estado inicial de la carga; cambiarán al hacer ventas, cobros o recepciones durante las pruebas.

## Productos y precios

Los nombres de catálogo de esta tabla coinciden con los que aparecen dentro del programa.

| Producto | Presentación | Color | Precio Q |
|---|---|---|---:|
| Pintura látex interior | Galón | Blanco | 120.00 |
| Pintura látex interior | Galón | Azul | 135.00 |
| Pintura látex interior | Cubeta | Blanco | 550.00 |
| Esmalte anticorrosivo | Galón | Negro | 160.00 |
| Esmalte anticorrosivo | Galón | Rojo | 165.00 |
| Brocha profesional | Unidad | Sin color | 35.00 |
| Rodillo para pintar | Unidad | Sin color | 55.00 |
| Tornillo para madera | Caja | Sin color | 40.00 |
| Cable eléctrico THHN | Unidad | Rojo | 8.50 |
| Cable eléctrico THHN | Unidad | Negro | 8.50 |
| Tomacorriente doble | Unidad | Blanco | 25.00 |
| Tubo PVC de media pulgada | Unidad | Sin color | 30.00 |

## Recorrido sugerido para exponer

### 1. Mantenimientos y permisos

Entrar como **admin**. Mostrar productos y variantes, clientes, proveedores y catálogos. Los datos ya están relacionados; para demostrar una eliminación exitosa, crear un registro nuevo sin referencias, editarlo y eliminarlo.

Entrar como **gerente** para mostrar Usuarios, Roles y Reportes. Como **cajero**, mostrar Caja. Los permisos del menú conservan la distribución de los perfiles.

### 2. Compras y recepción parcial

Como Digitador → Procesos → Compras, revisar:

| ID de orden | Estado | Caso |
|---:|---|---|
| 26 | Completa | Pintura blanca, solicitado 90; ingresos de 50 y 40 |
| 27 | Completa | Stock inicial de las otras variantes, incluida pintura azul en dos lotes |
| 28 | Parcial | Brochas: solicitado 100, recibido 75, pendiente 25. Rodillos: solicitado 50, recibido 20, pendiente 30 |
| 29 | Pendiente | Cable rojo 300, tomacorrientes 60 y tubos PVC 80, todavía sin recepción |

Para una ejemplo corta, abrir **Ingreso a bodega**, seleccionar la orden **28** y una ubicación. Recibir las **25 brochas** restantes, con costo unitario Q21; dejar Recibir=0 en rodillos. La orden debe seguir Parcial, pendiente de 30 rodillos. Después recibir esos 30 a costo Q33 y verificar que quede Completa.

La orden **29** permite demostrar una recepción desde cero, completa o parcial. No cambiar las cantidades solicitadas.

### 3. PEPS ya registrado

Como Cajero → Caja → Cobros, seleccionar **factura n.º 1**, ID interno **41**, de **Ana López**:

- Venta de 60 galones de pintura blanca a Q120 = **Q7,200**.
- El lote antiguo tenía 50 y el siguiente 40.
- Se consumieron **50 del primero + 10 del segundo**; quedaron 0 y 30.
- Pagos: Efectivo Q3,000 + Tarjeta Q3,000 + Cheque Q1,200.
- Estado: **Pagada**, saldo Q0.

Previsualizar la factura para mostrar detalle, responsable y los tres medios de pago. Como Gerente, Reportes → Inventario muestra los lotes; `Database/ConsultarDatosEjemplo.sql` permite mostrar en SQL las dos relaciones de DetalleFacturaLote.

### 4. Una venta PEPS nueva desde la interfaz

La variante **Pintura látex interior / Galón / Azul**, ID interno de variante **30**, tiene dos lotes sin consumir:

| Lote interno | Disponible inicial | Antigüedad |
|---:|---:|---|
| 43 | 25 | Más antiguo |
| 54 | 35 | Más reciente |

Como Cajero → Caja → Cobros, crear una factura **Sin cotización**, elegir un cliente y agregar **30** unidades de esa variante a Q135. Facturar por **Q4,050**.

Debe consumir **25 + 5**, dejando **0 y 30**. Cobrar con Efectivo Q2,000 y Tarjeta Q2,050. Previsualizar. Esta venta es nueva y sus identificadores se asignarán al guardarla.

Antes de vender, también se puede intentar facturar **61** unidades cuando sólo hay 60 disponibles. Debe rechazarse y conservar las existencias y documentos. Después hacer la venta válida de 30.

### 5. Cotizaciones

Como Digitador → Procesos → Cotizaciones:

| ID | Estado inicial | Cliente | Total Q |
|---:|---|---|---:|
| 22 | Pendiente | Carlos Méndez | 285.00 |
| 23 | Confirmada | Marta Pérez | 640.00 |
| 24 | Cancelada | Constructora Los Pinos | 1,100.00 |
| 25 | Facturada | Ferretería El Progreso | 725.00 |

Mostrar la pendiente y confirmarla, o convertir directamente la **23** desde Facturación: seleccionar esa cotización, revisar sus líneas bloqueadas y facturar. Debe descontar inventario y pasar a Facturada; no podrá convertirse una segunda vez. La 24 permite mostrar un documento cancelado.

### 6. Completar un cobro

Como Cajero, abrir la **factura n.º 2**, ID **42**, de **Ferretería El Progreso**:

- Total Q725.
- Ya tiene un abono en efectivo de Q200.
- Saldo pendiente **Q525**.

Agregar Tarjeta Q300 y Efectivo Q225, luego Registrar pagos. Debe pasar de Pendiente a Pagada. Un intento superior a Q525 debe rechazarse antes de registrar el cobro correcto.

La **factura n.º 3**, ID **43**, de **Inversiones El Roble** tiene total **Q260** y ningún pago: se puede demostrar también un cobro completo desde cero.

### 7. Auditoría

En SSMS consultar tbl_Auditoria: hay altas de cuentas, entradas de bodega, facturas, pagos y la reasignación del Digitador a admin con responsable, fecha y referencia. El registro DATOS_EJEMPLO / BD2_EJEMPLO_V1 conserva un manifiesto JSON con los identificadores de esta carga, sin contraseñas ni hashes.

## Herramientas entregadas

La carga ya se ejecutó; no necesitas volver a insertarla. Si necesitas consultar los identificadores:

```powershell
dotnet run --project Herramientas/DatosEjemplo/DatosEjemplo.csproj -- --resumen
```

Para usar la carga en otra copia de la BD con el mismo esquema y los SP instalados:

```powershell
dotnet run --project Herramientas/DatosEjemplo/DatosEjemplo.csproj
```

La herramienta ejecuta `Database/DatosEjemplo.sql`, generando hashes BCrypt como parámetros. El SQL no se debe pegar suelto en SSMS sin esos parámetros. Se hace todo en una transacción: si falla una verificación, no queda una carga parcial.

La segunda ejecución reconoce el marcador y **no duplica datos ni reinicia escenarios**. Se comprobó esa condición. Si después modificas los datos para demostrar una venta o un pago, ejecutar la carga de nuevo no deshace esa operación.

Consulta de sólo lectura para revisar estados, pagos, pendientes y lotes:

```powershell
sqlcmd -S '(localdb)\MSSQLLocalDB' -d Proyecto_BD -E -b -i Database/ConsultarDatosEjemplo.sql
```

Resultado inicial de esa consulta: `Documentacion/VerificacionDatosEjemplo.txt`. Registro inicial de carga: `Documentacion/ResultadoCargaEjemplo.txt`. Verificación de los nombres y accesos actuales: `Documentacion/ResultadoUsuariosEjemplo.txt`.

