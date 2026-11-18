-- Datos de ejemplo para la BD existente Proyecto_BD.
-- Ejecutar mediante Herramientas/DatosEjemplo: proporciona hashes BCrypt como parámetros.
-- No borra ni actualiza registros existentes. Las fechas de las compras son de ejemplo.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @bloqueo int;
 EXEC @bloqueo=sys.sp_getapplock @Resource='ProyectoBD_DatosEjemplo_V1',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000;
 IF @bloqueo<0 THROW 50001,'No se pudo bloquear la carga de ejemplo.',1;
 IF EXISTS(SELECT 1 FROM dbo.tbl_Auditoria WHERE TablaAfectada='DATOS_EJEMPLO' AND RegistroAfectado='BD2_EJEMPLO_V1' AND Accion='CARGA EJEMPLO')
 BEGIN
  COMMIT; PRINT 'La carga V1 ya existe. No se duplicaron registros.'; RETURN;
 END;
 IF EXISTS(SELECT 1 FROM dbo.tbl_Usuario WHERE NombreUsuario IN('cajero','gerente'))
  THROW 50001,'Ya existe un usuario sin el marcador de carga. No se sobrescribió ninguna cuenta.',1;
 IF @HashCajero NOT LIKE '$2%' OR @HashGerente NOT LIKE '$2%'
  THROW 50001,'La herramienta debe proporcionar hashes BCrypt.',1;
 DECLARE @rolD int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Digitador');
 DECLARE @rolC int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Cajero');
 DECLARE @rolG int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Gerente');
 IF @rolD IS NULL OR @rolC IS NULL OR @rolG IS NULL THROW 50001,'Faltan los roles oficiales.',1;
 DECLARE @digitador int=(SELECT ID_Usuario FROM dbo.tbl_Usuario WHERE NombreUsuario='admin' AND ID_Rol=@rolD);
 IF @digitador IS NULL THROW 50001,'Falta la cuenta admin Digitador existente.',1;
 INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES(N'Luis',N'Morales','cajero',@HashCajero,@rolC);
 DECLARE @cajero int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES(N'Daniela',N'Gómez','gerente',@HashGerente,@rolG);
 DECLARE @gerente int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Auditoria(FechaHora,TablaAfectada,RegistroAfectado,Accion,DatosNuevos,ID_Usuario)
 SELECT SYSDATETIME(),'tbl_Usuario',CONVERT(nvarchar(60),u.ID_Usuario),'ALTA USUARIO',
 (SELECT u.Nombres,u.Apellidos,u.NombreUsuario,u.ID_Rol FOR JSON PATH,WITHOUT_ARRAY_WRAPPER),@gerente
 FROM dbo.tbl_Usuario u WHERE u.ID_Usuario IN(@cajero,@gerente);

 INSERT dbo.tbl_CategoriaProducto(NombreCategoria,Descripcion) VALUES(N'Pinturas',N'Catálogo de ejemplo: Pinturas');
 DECLARE @cat1 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_CategoriaProducto(NombreCategoria,Descripcion) VALUES(N'Ferretería',N'Catálogo de ejemplo: Ferretería');
 DECLARE @cat2 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_CategoriaProducto(NombreCategoria,Descripcion) VALUES(N'Material eléctrico',N'Catálogo de ejemplo: Material eléctrico');
 DECLARE @cat3 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_CategoriaProducto(NombreCategoria,Descripcion) VALUES(N'Plomería',N'Catálogo de ejemplo: Plomería');
 DECLARE @cat4 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Marca(NombreMarca) VALUES(N'ColorGuate');
 DECLARE @marca1 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Marca(NombreMarca) VALUES(N'Construmax');
 DECLARE @marca2 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Marca(NombreMarca) VALUES(N'ElectroGT');
 DECLARE @marca3 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Presentacion(NombrePresentacion,UnidadMedida) VALUES(N'Galón',N'gal');
 DECLARE @pres1 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Presentacion(NombrePresentacion,UnidadMedida) VALUES(N'Cubeta',N'5 gal');
 DECLARE @pres2 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Presentacion(NombrePresentacion,UnidadMedida) VALUES(N'Unidad',N'unidad');
 DECLARE @pres3 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Presentacion(NombrePresentacion,UnidadMedida) VALUES(N'Caja',N'100 unidades');
 DECLARE @pres4 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Color(NombreColor) VALUES(N'Blanco');
 DECLARE @color1 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Color(NombreColor) VALUES(N'Azul');
 DECLARE @color2 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Color(NombreColor) VALUES(N'Negro');
 DECLARE @color3 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Color(NombreColor) VALUES(N'Rojo');
 DECLARE @color4 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Proveedor(Nombre_Proveedor) VALUES(N'Distribuidora Central');
 DECLARE @prov1 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Proveedor(Nombre_Proveedor) VALUES(N'Suministros del Norte');
 DECLARE @prov2 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Proveedor(Nombre_Proveedor) VALUES(N'Electro y PVC Guatemala');
 DECLARE @prov3 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES(N'Particular',N'Ana López');
 DECLARE @cli1 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES(N'Particular',N'Carlos Méndez');
 DECLARE @cli2 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES(N'Particular',N'Marta Pérez');
 DECLARE @cli3 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES(N'Empresa',N'Constructora Los Pinos');
 DECLARE @cli4 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES(N'Empresa',N'Ferretería El Progreso');
 DECLARE @cli5 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES(N'Empresa',N'Inversiones El Roble');
 DECLARE @cli6 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_UbicacionAlmacen(NombreUbicacion,Descripcion) VALUES(N'Bodega principal',N'Ubicación de ejemplo');
 DECLARE @ubi1 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_UbicacionAlmacen(NombreUbicacion,Descripcion) VALUES(N'Estante de exhibición',N'Ubicación de ejemplo');
 DECLARE @ubi2 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_MetodoPago(NombreMetodo) VALUES(N'Efectivo');
 DECLARE @met1 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_MetodoPago(NombreMetodo) VALUES(N'Tarjeta');
 DECLARE @met2 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_MetodoPago(NombreMetodo) VALUES(N'Cheque');
 DECLARE @met3 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES(N'Pintura látex interior',N'Pintura para paredes interiores',@cat1,@marca1);
 DECLARE @p1 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES(N'Esmalte anticorrosivo',N'Protección y acabado de metal',@cat1,@marca1);
 DECLARE @p2 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES(N'Brocha profesional',N'Brocha de cerdas de tres pulgadas',@cat2,@marca2);
 DECLARE @p3 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES(N'Rodillo para pintar',N'Rodillo de nueve pulgadas',@cat2,@marca2);
 DECLARE @p4 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES(N'Tornillo para madera',N'Caja de tornillos de una pulgada',@cat2,@marca2);
 DECLARE @p5 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES(N'Cable eléctrico THHN',N'Cable eléctrico calibre doce por metro',@cat3,@marca3);
 DECLARE @p6 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES(N'Tomacorriente doble',N'Tomacorriente doble polarizado',@cat3,@marca3);
 DECLARE @p7 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES(N'Tubo PVC de media pulgada',N'Tubo PVC de tres metros',@cat4,@marca2);
 DECLARE @p8 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(120,@p1,@pres1,@color1);
 DECLARE @v1 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(135,@p1,@pres1,@color2);
 DECLARE @v2 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(550,@p1,@pres2,@color1);
 DECLARE @v3 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(160,@p2,@pres1,@color3);
 DECLARE @v4 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(165,@p2,@pres1,@color4);
 DECLARE @v5 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(35,@p3,@pres3,NULL);
 DECLARE @v6 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(55,@p4,@pres3,NULL);
 DECLARE @v7 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(40,@p5,@pres4,NULL);
 DECLARE @v8 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(8.5,@p6,@pres3,@color4);
 DECLARE @v9 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(8.5,@p6,@pres3,@color3);
 DECLARE @v10 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(25,@p7,@pres3,@color1);
 DECLARE @v11 int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(30,@p8,@pres3,NULL);
 DECLARE @v12 int=CONVERT(int,SCOPE_IDENTITY());
 DECLARE @detalle nvarchar(max),@fecha datetime2(0);
 SET @detalle=(SELECT d.ID_ProductoV,d.Cantidad FROM (VALUES (@v1,90)) d(ID_ProductoV,Cantidad) FOR JSON PATH);
 EXEC dbo.usp_GuardarOrdenCompra @ID_Proveedor=@prov1,@ID_Usuario=@digitador,@Detalle=@detalle;
 DECLARE @oc1 int=(SELECT MAX(ID_OrdenC) FROM dbo.tbl_OrdenCompra WHERE ID_Proveedor=@prov1);
 UPDATE dbo.tbl_OrdenCompra SET FechaOrden=CONVERT(date,DATEADD(day,-25,SYSDATETIME())) WHERE ID_OrdenC=@oc1;
 SET @detalle=(SELECT oc.ID_DetalleOC,d.CantidadRecibida,d.CostoUnitario,d.NumeroLote
 FROM (VALUES (@v1,50,80,NULL)) d(ID_ProductoV,CantidadRecibida,CostoUnitario,NumeroLote)
 JOIN dbo.tbl_DetalleOrdenCompra oc ON oc.ID_ProductoV=d.ID_ProductoV AND oc.ID_OrdenC=@oc1 FOR JSON PATH);
 SET @fecha=DATEADD(day,-20,SYSDATETIME());
 EXEC dbo.usp_RegistrarIngresoBodega @ID_OrdenC=@oc1,@ID_Usuario=@digitador,@ID_UbicacionA=@ubi1,@FechaIngreso=@fecha,@Detalle=@detalle;
 SET @detalle=(SELECT oc.ID_DetalleOC,d.CantidadRecibida,d.CostoUnitario,d.NumeroLote
 FROM (VALUES (@v1,40,85,N'PINT-BLANCO-02')) d(ID_ProductoV,CantidadRecibida,CostoUnitario,NumeroLote)
 JOIN dbo.tbl_DetalleOrdenCompra oc ON oc.ID_ProductoV=d.ID_ProductoV AND oc.ID_OrdenC=@oc1 FOR JSON PATH);
 SET @fecha=DATEADD(day,-10,SYSDATETIME());
 EXEC dbo.usp_RegistrarIngresoBodega @ID_OrdenC=@oc1,@ID_Usuario=@digitador,@ID_UbicacionA=@ubi1,@FechaIngreso=@fecha,@Detalle=@detalle;
 SET @detalle=(SELECT d.ID_ProductoV,d.Cantidad FROM (VALUES (@v2,60),(@v3,15),(@v4,30),(@v5,20),(@v6,100),(@v7,80),(@v8,200),(@v9,500),(@v10,500),(@v11,100),(@v12,60)) d(ID_ProductoV,Cantidad) FOR JSON PATH);
 EXEC dbo.usp_GuardarOrdenCompra @ID_Proveedor=@prov1,@ID_Usuario=@digitador,@Detalle=@detalle;
 DECLARE @oc2 int=(SELECT MAX(ID_OrdenC) FROM dbo.tbl_OrdenCompra WHERE ID_Proveedor=@prov1);
 UPDATE dbo.tbl_OrdenCompra SET FechaOrden=CONVERT(date,DATEADD(day,-23,SYSDATETIME())) WHERE ID_OrdenC=@oc2;
 SET @detalle=(SELECT oc.ID_DetalleOC,d.CantidadRecibida,d.CostoUnitario,d.NumeroLote
 FROM (VALUES (@v2,25,90,N'PINT-AZUL-01'),(@v3,15,390,NULL),(@v4,30,110,NULL),(@v5,20,112,NULL),(@v6,100,20,NULL),(@v7,80,32,NULL),(@v8,200,25,NULL),(@v9,500,5,NULL),(@v10,500,5,NULL),(@v11,100,15,NULL),(@v12,60,18,NULL)) d(ID_ProductoV,CantidadRecibida,CostoUnitario,NumeroLote)
 JOIN dbo.tbl_DetalleOrdenCompra oc ON oc.ID_ProductoV=d.ID_ProductoV AND oc.ID_OrdenC=@oc2 FOR JSON PATH);
 SET @fecha=DATEADD(day,-16,SYSDATETIME());
 EXEC dbo.usp_RegistrarIngresoBodega @ID_OrdenC=@oc2,@ID_Usuario=@digitador,@ID_UbicacionA=@ubi1,@FechaIngreso=@fecha,@Detalle=@detalle;
 SET @detalle=(SELECT oc.ID_DetalleOC,d.CantidadRecibida,d.CostoUnitario,d.NumeroLote
 FROM (VALUES (@v2,35,95,N'PINT-AZUL-02')) d(ID_ProductoV,CantidadRecibida,CostoUnitario,NumeroLote)
 JOIN dbo.tbl_DetalleOrdenCompra oc ON oc.ID_ProductoV=d.ID_ProductoV AND oc.ID_OrdenC=@oc2 FOR JSON PATH);
 SET @fecha=DATEADD(day,-6,SYSDATETIME());
 EXEC dbo.usp_RegistrarIngresoBodega @ID_OrdenC=@oc2,@ID_Usuario=@digitador,@ID_UbicacionA=@ubi1,@FechaIngreso=@fecha,@Detalle=@detalle;
 SET @detalle=(SELECT d.ID_ProductoV,d.Cantidad FROM (VALUES (@v6,100),(@v7,50)) d(ID_ProductoV,Cantidad) FOR JSON PATH);
 EXEC dbo.usp_GuardarOrdenCompra @ID_Proveedor=@prov2,@ID_Usuario=@digitador,@Detalle=@detalle;
 DECLARE @oc3 int=(SELECT MAX(ID_OrdenC) FROM dbo.tbl_OrdenCompra WHERE ID_Proveedor=@prov2);
 UPDATE dbo.tbl_OrdenCompra SET FechaOrden=CONVERT(date,DATEADD(day,-8,SYSDATETIME())) WHERE ID_OrdenC=@oc3;
 SET @detalle=(SELECT oc.ID_DetalleOC,d.CantidadRecibida,d.CostoUnitario,d.NumeroLote
 FROM (VALUES (@v6,75,21,NULL),(@v7,20,33,NULL)) d(ID_ProductoV,CantidadRecibida,CostoUnitario,NumeroLote)
 JOIN dbo.tbl_DetalleOrdenCompra oc ON oc.ID_ProductoV=d.ID_ProductoV AND oc.ID_OrdenC=@oc3 FOR JSON PATH);
 SET @fecha=DATEADD(day,-3,SYSDATETIME());
 EXEC dbo.usp_RegistrarIngresoBodega @ID_OrdenC=@oc3,@ID_Usuario=@digitador,@ID_UbicacionA=@ubi2,@FechaIngreso=@fecha,@Detalle=@detalle;
 SET @detalle=(SELECT d.ID_ProductoV,d.Cantidad FROM (VALUES (@v9,300),(@v11,60),(@v12,80)) d(ID_ProductoV,Cantidad) FOR JSON PATH);
 EXEC dbo.usp_GuardarOrdenCompra @ID_Proveedor=@prov3,@ID_Usuario=@digitador,@Detalle=@detalle;
 DECLARE @oc4 int=(SELECT MAX(ID_OrdenC) FROM dbo.tbl_OrdenCompra WHERE ID_Proveedor=@prov3);
 UPDATE dbo.tbl_OrdenCompra SET FechaOrden=CONVERT(date,DATEADD(day,-1,SYSDATETIME())) WHERE ID_OrdenC=@oc4;
 SET @detalle=(SELECT d.ID_ProductoV,d.Cantidad,d.PrecioUnitario FROM (VALUES (@v6,5,35),(@v7,2,55)) d(ID_ProductoV,Cantidad,PrecioUnitario) FOR JSON PATH);
 EXEC dbo.usp_GuardarCotizacion @ID_Cliente=@cli2,@ID_Usuario=@digitador,@Detalle=@detalle;
 DECLARE @cot1 int=(SELECT MAX(ID_Cotizacion) FROM dbo.tbl_Cotizacion WHERE ID_Cliente=@cli2);
 SET @detalle=(SELECT d.ID_ProductoV,d.Cantidad,d.PrecioUnitario FROM (VALUES (@v4,3,160),(@v8,4,40)) d(ID_ProductoV,Cantidad,PrecioUnitario) FOR JSON PATH);
 EXEC dbo.usp_GuardarCotizacion @ID_Cliente=@cli3,@ID_Usuario=@digitador,@Detalle=@detalle;
 DECLARE @cot2 int=(SELECT MAX(ID_Cotizacion) FROM dbo.tbl_Cotizacion WHERE ID_Cliente=@cli3);
 EXEC dbo.usp_EstadoCotizacion @ID_Cotizacion=@cot2,@Estado=N'Confirmada',@ID_Usuario=@digitador;
 SET @detalle=(SELECT d.ID_ProductoV,d.Cantidad,d.PrecioUnitario FROM (VALUES (@v3,2,550)) d(ID_ProductoV,Cantidad,PrecioUnitario) FOR JSON PATH);
 EXEC dbo.usp_GuardarCotizacion @ID_Cliente=@cli4,@ID_Usuario=@digitador,@Detalle=@detalle;
 DECLARE @cot3 int=(SELECT MAX(ID_Cotizacion) FROM dbo.tbl_Cotizacion WHERE ID_Cliente=@cli4);
 EXEC dbo.usp_EstadoCotizacion @ID_Cotizacion=@cot3,@Estado=N'Cancelada',@ID_Usuario=@digitador;
 SET @detalle=(SELECT d.ID_ProductoV,d.Cantidad,d.PrecioUnitario FROM (VALUES (@v6,10,35),(@v7,5,55),(@v11,4,25)) d(ID_ProductoV,Cantidad,PrecioUnitario) FOR JSON PATH);
 EXEC dbo.usp_GuardarCotizacion @ID_Cliente=@cli5,@ID_Usuario=@digitador,@Detalle=@detalle;
 DECLARE @cot4 int=(SELECT MAX(ID_Cotizacion) FROM dbo.tbl_Cotizacion WHERE ID_Cliente=@cli5);
 EXEC dbo.usp_EstadoCotizacion @ID_Cotizacion=@cot4,@Estado=N'Confirmada',@ID_Usuario=@digitador;
 SET @detalle=(SELECT @v1 ID_ProductoV,60 Cantidad,CAST(120 AS decimal(12,2)) PrecioUnitario FOR JSON PATH);
 EXEC dbo.usp_FacturarPEPS @ID_Cliente=@cli1,@ID_Usuario=@cajero,@ID_Cotizacion=NULL,@Detalle=@detalle;
 DECLARE @f1 int=(SELECT MAX(ID_Factura) FROM dbo.tbl_Factura WHERE ID_Cliente=@cli1);
 SET @detalle=(SELECT d.ID_MetodoP,d.Monto,d.Referencia FROM (VALUES(@met1,3000,N'EFECTIVO'),(@met2,3000,N'TARJETA-001'),(@met3,1200,N'CHEQUE-001')) d(ID_MetodoP,Monto,Referencia) FOR JSON PATH);
 EXEC dbo.usp_RegistrarPagos @ID_Factura=@f1,@ID_Usuario=@cajero,@Pagos=@detalle;
 EXEC dbo.usp_FacturarPEPS @ID_Cliente=@cli5,@ID_Usuario=@cajero,@ID_Cotizacion=@cot4,@Detalle=NULL;
 DECLARE @f2 int=(SELECT MAX(ID_Factura) FROM dbo.tbl_Factura WHERE ID_Cliente=@cli5);
 SET @detalle=(SELECT @met1 ID_MetodoP,CAST(200 AS decimal(12,2)) Monto,N'ABONO-001' Referencia FOR JSON PATH);
 EXEC dbo.usp_RegistrarPagos @ID_Factura=@f2,@ID_Usuario=@cajero,@Pagos=@detalle;
 SET @detalle=(SELECT d.ID_ProductoV,d.Cantidad,d.PrecioUnitario FROM (VALUES(@v12,3,CAST(30 AS decimal(12,2))),(@v9,20,CAST(8.5 AS decimal(12,2)))) d(ID_ProductoV,Cantidad,PrecioUnitario) FOR JSON PATH);
 EXEC dbo.usp_FacturarPEPS @ID_Cliente=@cli6,@ID_Usuario=@cajero,@ID_Cotizacion=NULL,@Detalle=@detalle;
 DECLARE @f3 int=(SELECT MAX(ID_Factura) FROM dbo.tbl_Factura WHERE ID_Cliente=@cli6);
 -- Verificaciones previas al COMMIT: todos los documentos y existencias deben ser consistentes.
 IF (SELECT Estado FROM dbo.tbl_OrdenCompra WHERE ID_OrdenC=@oc1)<>'Completa' OR (SELECT Estado FROM dbo.tbl_OrdenCompra WHERE ID_OrdenC=@oc2)<>'Completa' OR (SELECT Estado FROM dbo.tbl_OrdenCompra WHERE ID_OrdenC=@oc3)<>'Parcial' OR (SELECT Estado FROM dbo.tbl_OrdenCompra WHERE ID_OrdenC=@oc4)<>'Pendiente'
  THROW 50001,'Los estados de compra no son los esperados.',1;
 IF (SELECT SUM(CantidadDisponible) FROM dbo.tbl_LoteInventario WHERE ID_ProductoV=@v1)<>30 OR (SELECT SUM(CantidadDisponible) FROM dbo.tbl_LoteInventario WHERE ID_ProductoV=@v2)<>60
  THROW 50001,'Las existencias de pintura no son las esperadas.',1;
 IF (SELECT COUNT(*) FROM dbo.tbl_DetalleFacturaLote fl JOIN dbo.tbl_DetalleFactura f ON f.ID_DetalleF=fl.ID_DetalleF WHERE f.ID_Factura=@f1 AND fl.CantidadSalida IN(50,10))<>2
  THROW 50001,'La factura no registró PEPS 50+10.',1;
 IF (SELECT SUM(Monto) FROM dbo.tbl_PagoFactura WHERE ID_Factura=@f1)<>7200 OR (SELECT Estado FROM dbo.tbl_Factura WHERE ID_Factura=@f1)<>'Pagada'
  THROW 50001,'El pago mixto no está completo.',1;
 IF (SELECT Total FROM dbo.tbl_Factura WHERE ID_Factura=@f2)<>725 OR (SELECT SUM(Monto) FROM dbo.tbl_PagoFactura WHERE ID_Factura=@f2)<>200 OR (SELECT Estado FROM dbo.tbl_Factura WHERE ID_Factura=@f2)<>'Pendiente'
  THROW 50001,'La factura con abono es incorrecta.',1;
 IF EXISTS(SELECT 1 FROM dbo.tbl_LoteInventario WHERE ID_ProductoV IN(@v1,@v2,@v3,@v4,@v5,@v6,@v7,@v8,@v9,@v10,@v11,@v12) AND CantidadDisponible<0)
  THROW 50001,'Se detectaron existencias negativas.',1;
 DECLARE @manifiesto nvarchar(max)=(SELECT @digitador UsuarioDigitador,@cajero UsuarioCajero,@gerente UsuarioGerente,@oc1 OrdenCompletaPEPS,@oc2 OrdenCompletaStock,@oc3 OrdenParcial,@oc4 OrdenPendiente,@cot1 CotizacionPendiente,@cot2 CotizacionConfirmada,@cot3 CotizacionCancelada,@cot4 CotizacionFacturada,@f1 FacturaPagadaPEPS,@f2 FacturaAbono,@f3 FacturaSinPagos,@v1 VariantePinturaBlanca,@v2 VariantePinturaAzul FOR JSON PATH,WITHOUT_ARRAY_WRAPPER);
 INSERT dbo.tbl_Auditoria(FechaHora,TablaAfectada,RegistroAfectado,Accion,DatosNuevos,ID_Usuario)
 VALUES(SYSDATETIME(),'DATOS_EJEMPLO','BD2_EJEMPLO_V1','CARGA EJEMPLO',@manifiesto,@gerente);
 COMMIT TRANSACTION;
 PRINT 'Carga V1 completada y validada. Los registros anteriores se conservaron.';
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;
