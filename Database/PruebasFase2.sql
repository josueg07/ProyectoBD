USE Proyecto_BD;
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
BEGIN TRAN;

DECLARE @rol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Digitador'),@cajeroRol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Cajero');
DECLARE @s nvarchar(36)=CONVERT(nvarchar(36),NEWID());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Prueba','Temporal',@s,(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@rol); DECLARE @actor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Caja','Temporal',@s+'c',(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@cajeroRol); DECLARE @cajero int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Marca(NombreMarca) VALUES('PRUEBA');DECLARE @marca int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_CategoriaProducto(NombreCategoria,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @categoria int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Presentacion(NombrePresentacion,UnidadMedida) VALUES('PRUEBA','Unidad');DECLARE @presentacion int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES('PRUEBA','PRUEBA',@categoria,@marca);DECLARE @producto int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(10,@producto,@presentacion,NULL);DECLARE @variante int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Proveedor(Nombre_Proveedor) VALUES('PRUEBA');DECLARE @proveedor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES('Particular','PRUEBA');DECLARE @cliente int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_UbicacionAlmacen(NombreUbicacion,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @ubicacion int=CONVERT(int,SCOPE_IDENTITY());
DECLARE @resultado TABLE(id int);DECLARE @json nvarchar(max)=CONCAT('[{"ID_ProductoV":',@variante,',"Cantidad":90,"PrecioUnitario":10}]');
INSERT @resultado EXEC dbo.usp_GuardarOrdenCompra @proveedor,@actor,@json;DECLARE @orden int=(SELECT id FROM @resultado);DELETE @resultado;
DECLARE @doc int=(SELECT ID_DetalleOC FROM dbo.tbl_DetalleOrdenCompra WHERE ID_OrdenC=@orden),@fecha datetime2(0)=DATEADD(day,-2,SYSDATETIME());
SET @json=CONCAT('[{"ID_DetalleOC":',@doc,',"CantidadRecibida":50,"CostoUnitario":5}]');
-- No INSERT EXEC here: SQL Server prevents ROLLBACK within INSERT EXEC on failure paths.
EXEC dbo.usp_RegistrarIngresoBodega @orden,@actor,@ubicacion,@fecha,@json;
DECLARE @lote1 int=(SELECT l.ID_Lote FROM dbo.tbl_LoteInventario l WHERE l.ID_ProductoV=@variante);

IF (SELECT Estado FROM dbo.tbl_OrdenCompra WHERE ID_OrdenC=@orden)<>'Parcial' THROW 51000,'No actualizó estado Parcial.',1;
SET @fecha=DATEADD(day,-1,SYSDATETIME());
SET @json=CONCAT('[{"ID_DetalleOC":',@doc,',"CantidadRecibida":40,"CostoUnitario":6,"NumeroLote":"FISICO-2"}]');
EXEC dbo.usp_RegistrarIngresoBodega @orden,@actor,@ubicacion,@fecha,@json;
DECLARE @lote2 int=(SELECT MAX(ID_Lote) FROM dbo.tbl_LoteInventario WHERE ID_ProductoV=@variante);
IF (SELECT Estado FROM dbo.tbl_OrdenCompra WHERE ID_OrdenC=@orden)<>'Completa' THROW 51000,'No actualizó estado Completa.',1;
IF (SELECT CantidadSolicitada FROM dbo.tbl_DetalleOrdenCompra WHERE ID_DetalleOC=@doc)<>90 THROW 51000,'Alteró la cantidad solicitada.',1;
SET @json=CONCAT('[{"ID_ProductoV":',@variante,',"Cantidad":60,"PrecioUnitario":10}]');
INSERT @resultado EXEC dbo.usp_FacturarPEPS @cliente,@actor,NULL,@json;DECLARE @factura int=(SELECT id FROM @resultado);DELETE @resultado;
IF (SELECT CantidadDisponible FROM dbo.tbl_LoteInventario WHERE ID_Lote=@lote1)<>0 OR (SELECT CantidadDisponible FROM dbo.tbl_LoteInventario WHERE ID_Lote=@lote2)<>30 THROW 51000,'PEPS incorrecto.',1;
IF (SELECT SUM(fl.CantidadSalida) FROM dbo.tbl_DetalleFacturaLote fl JOIN dbo.tbl_DetalleFactura f ON f.ID_DetalleF=fl.ID_DetalleF WHERE f.ID_Factura=@factura)<>60 THROW 51000,'Asignación a lotes incorrecta.',1;
IF (SELECT COUNT(*) FROM dbo.tbl_MovimientoInventario WHERE ID_ProductoV=@variante AND TipoMovimiento='SALIDA_VENTA')<>2 THROW 51000,'No registró los dos movimientos.',1;
INSERT dbo.tbl_MetodoPago(NombreMetodo) VALUES(@s);DECLARE @metodo int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_MetodoPago(NombreMetodo) VALUES(@s+'2');DECLARE @metodo2 int=CONVERT(int,SCOPE_IDENTITY());
SET @json=CONCAT('[{"ID_MetodoP":',@metodo,',"Monto":300}]');EXEC dbo.usp_RegistrarPagos @factura,@cajero,@json;
IF (SELECT Estado FROM dbo.tbl_Factura WHERE ID_Factura=@factura)<>'Pendiente' THROW 51000,'Un pago parcial marcó la factura pagada.',1;
SET @json=CONCAT('[{"ID_MetodoP":',@metodo2,',"Monto":200},{"ID_MetodoP":',@metodo,',"Monto":100}]');EXEC dbo.usp_RegistrarPagos @factura,@cajero,@json;
IF (SELECT Estado FROM dbo.tbl_Factura WHERE ID_Factura=@factura)<>'Pagada' THROW 51000,'La suma exacta no marcó la factura pagada.',1;
SET @json=CONCAT('[{"ID_ProductoV":',@variante,',"Cantidad":10,"PrecioUnitario":10}]');INSERT @resultado EXEC dbo.usp_GuardarCotizacion @cliente,@actor,@json;DECLARE @cot int=(SELECT id FROM @resultado);DELETE @resultado;
EXEC dbo.usp_EstadoCotizacion @cot,'Confirmada',@actor;
EXEC dbo.usp_FacturarPEPS @cliente,@cajero,@cot,NULL;
IF (SELECT Estado FROM dbo.tbl_Cotizacion WHERE ID_Cotizacion=@cot)<>'Facturada' THROW 51000,'La cotización no se convirtió a factura.',1;
IF (SELECT CantidadDisponible FROM dbo.tbl_LoteInventario WHERE ID_Lote=@lote2)<>20 THROW 51000,'No descontó inventario de la cotización.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Auditoria WHERE TablaAfectada='tbl_Factura' AND ID_Usuario=@actor) OR NOT EXISTS(SELECT 1 FROM dbo.tbl_Auditoria WHERE TablaAfectada='tbl_IngresoBodega' AND ID_Usuario=@actor) THROW 51000,'Falta auditoría.',1;
ROLLBACK;
PRINT 'OK: recepción parcial/completa, lote NULL, venta directa PEPS 50+10, movimientos, pago parcial y mixto exacto, conversión de cotización y auditoría.';
END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;
GO
DECLARE @antes int=(SELECT COUNT(*) FROM dbo.tbl_Factura),@lotesAntes int=(SELECT COUNT(*) FROM dbo.tbl_LoteInventario),@pagosAntes int=(SELECT COUNT(*) FROM dbo.tbl_PagoFactura);
BEGIN TRY BEGIN TRAN;

DECLARE @rol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Digitador'),@cajeroRol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Cajero');
DECLARE @s nvarchar(36)=CONVERT(nvarchar(36),NEWID());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Prueba','Temporal',@s,(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@rol); DECLARE @actor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Caja','Temporal',@s+'c',(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@cajeroRol); DECLARE @cajero int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Marca(NombreMarca) VALUES('PRUEBA');DECLARE @marca int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_CategoriaProducto(NombreCategoria,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @categoria int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Presentacion(NombrePresentacion,UnidadMedida) VALUES('PRUEBA','Unidad');DECLARE @presentacion int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES('PRUEBA','PRUEBA',@categoria,@marca);DECLARE @producto int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(10,@producto,@presentacion,NULL);DECLARE @variante int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Proveedor(Nombre_Proveedor) VALUES('PRUEBA');DECLARE @proveedor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES('Particular','PRUEBA');DECLARE @cliente int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_UbicacionAlmacen(NombreUbicacion,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @ubicacion int=CONVERT(int,SCOPE_IDENTITY());
DECLARE @resultado TABLE(id int);DECLARE @json nvarchar(max)=CONCAT('[{"ID_ProductoV":',@variante,',"Cantidad":90,"PrecioUnitario":10}]');
INSERT @resultado EXEC dbo.usp_GuardarOrdenCompra @proveedor,@actor,@json;DECLARE @orden int=(SELECT id FROM @resultado);DELETE @resultado;
DECLARE @doc int=(SELECT ID_DetalleOC FROM dbo.tbl_DetalleOrdenCompra WHERE ID_OrdenC=@orden),@fecha datetime2(0)=DATEADD(day,-2,SYSDATETIME());
SET @json=CONCAT('[{"ID_DetalleOC":',@doc,',"CantidadRecibida":50,"CostoUnitario":5}]');
-- No INSERT EXEC here: SQL Server prevents ROLLBACK within INSERT EXEC on failure paths.
EXEC dbo.usp_RegistrarIngresoBodega @orden,@actor,@ubicacion,@fecha,@json;
DECLARE @lote1 int=(SELECT l.ID_Lote FROM dbo.tbl_LoteInventario l WHERE l.ID_ProductoV=@variante);
SET @json=CONCAT('[{"ID_DetalleOC":',@doc,',"CantidadRecibida":41,"CostoUnitario":6}]');EXEC dbo.usp_RegistrarIngresoBodega @orden,@actor,@ubicacion,@fecha,@json;
ROLLBACK; THROW 51000,'Se aceptó una operación inválida.',1;
END TRY BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 IF ERROR_NUMBER()<>50001 THROW;
 IF (SELECT COUNT(*) FROM dbo.tbl_Factura)<>@antes OR (SELECT COUNT(*) FROM dbo.tbl_LoteInventario)<>@lotesAntes OR (SELECT COUNT(*) FROM dbo.tbl_PagoFactura)<>@pagosAntes THROW 51000,'El rollback no revirtió los registros de prueba.',1;
 PRINT 'OK: sobre-recepción.';
END CATCH;
GO
DECLARE @antes int=(SELECT COUNT(*) FROM dbo.tbl_Factura),@lotesAntes int=(SELECT COUNT(*) FROM dbo.tbl_LoteInventario),@pagosAntes int=(SELECT COUNT(*) FROM dbo.tbl_PagoFactura);
BEGIN TRY BEGIN TRAN;

DECLARE @rol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Digitador'),@cajeroRol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Cajero');
DECLARE @s nvarchar(36)=CONVERT(nvarchar(36),NEWID());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Prueba','Temporal',@s,(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@rol); DECLARE @actor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Caja','Temporal',@s+'c',(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@cajeroRol); DECLARE @cajero int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Marca(NombreMarca) VALUES('PRUEBA');DECLARE @marca int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_CategoriaProducto(NombreCategoria,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @categoria int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Presentacion(NombrePresentacion,UnidadMedida) VALUES('PRUEBA','Unidad');DECLARE @presentacion int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES('PRUEBA','PRUEBA',@categoria,@marca);DECLARE @producto int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(10,@producto,@presentacion,NULL);DECLARE @variante int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Proveedor(Nombre_Proveedor) VALUES('PRUEBA');DECLARE @proveedor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES('Particular','PRUEBA');DECLARE @cliente int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_UbicacionAlmacen(NombreUbicacion,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @ubicacion int=CONVERT(int,SCOPE_IDENTITY());
DECLARE @resultado TABLE(id int);DECLARE @json nvarchar(max)=CONCAT('[{"ID_ProductoV":',@variante,',"Cantidad":90,"PrecioUnitario":10}]');
INSERT @resultado EXEC dbo.usp_GuardarOrdenCompra @proveedor,@actor,@json;DECLARE @orden int=(SELECT id FROM @resultado);DELETE @resultado;
DECLARE @doc int=(SELECT ID_DetalleOC FROM dbo.tbl_DetalleOrdenCompra WHERE ID_OrdenC=@orden),@fecha datetime2(0)=DATEADD(day,-2,SYSDATETIME());
SET @json=CONCAT('[{"ID_DetalleOC":',@doc,',"CantidadRecibida":50,"CostoUnitario":5}]');
-- No INSERT EXEC here: SQL Server prevents ROLLBACK within INSERT EXEC on failure paths.
EXEC dbo.usp_RegistrarIngresoBodega @orden,@actor,@ubicacion,@fecha,@json;
DECLARE @lote1 int=(SELECT l.ID_Lote FROM dbo.tbl_LoteInventario l WHERE l.ID_ProductoV=@variante);
SET @json=CONCAT('[{"ID_ProductoV":',@variante,',"Cantidad":60,"PrecioUnitario":10}]');EXEC dbo.usp_FacturarPEPS @cliente,@actor,NULL,@json;
ROLLBACK; THROW 51000,'Se aceptó una operación inválida.',1;
END TRY BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 IF ERROR_NUMBER()<>50001 THROW;
 IF (SELECT COUNT(*) FROM dbo.tbl_Factura)<>@antes OR (SELECT COUNT(*) FROM dbo.tbl_LoteInventario)<>@lotesAntes OR (SELECT COUNT(*) FROM dbo.tbl_PagoFactura)<>@pagosAntes THROW 51000,'El rollback no revirtió los registros de prueba.',1;
 PRINT 'OK: inventario insuficiente revierte factura y consumo parcial.';
END CATCH;
GO
DECLARE @antes int=(SELECT COUNT(*) FROM dbo.tbl_Factura),@lotesAntes int=(SELECT COUNT(*) FROM dbo.tbl_LoteInventario),@pagosAntes int=(SELECT COUNT(*) FROM dbo.tbl_PagoFactura);
BEGIN TRY BEGIN TRAN;

DECLARE @rol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Digitador'),@cajeroRol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Cajero');
DECLARE @s nvarchar(36)=CONVERT(nvarchar(36),NEWID());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Prueba','Temporal',@s,(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@rol); DECLARE @actor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Caja','Temporal',@s+'c',(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@cajeroRol); DECLARE @cajero int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Marca(NombreMarca) VALUES('PRUEBA');DECLARE @marca int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_CategoriaProducto(NombreCategoria,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @categoria int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Presentacion(NombrePresentacion,UnidadMedida) VALUES('PRUEBA','Unidad');DECLARE @presentacion int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES('PRUEBA','PRUEBA',@categoria,@marca);DECLARE @producto int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(10,@producto,@presentacion,NULL);DECLARE @variante int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Proveedor(Nombre_Proveedor) VALUES('PRUEBA');DECLARE @proveedor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES('Particular','PRUEBA');DECLARE @cliente int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_UbicacionAlmacen(NombreUbicacion,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @ubicacion int=CONVERT(int,SCOPE_IDENTITY());
DECLARE @resultado TABLE(id int);DECLARE @json nvarchar(max)=CONCAT('[{"ID_ProductoV":',@variante,',"Cantidad":90,"PrecioUnitario":10}]');
INSERT @resultado EXEC dbo.usp_GuardarOrdenCompra @proveedor,@actor,@json;DECLARE @orden int=(SELECT id FROM @resultado);DELETE @resultado;
DECLARE @doc int=(SELECT ID_DetalleOC FROM dbo.tbl_DetalleOrdenCompra WHERE ID_OrdenC=@orden),@fecha datetime2(0)=DATEADD(day,-2,SYSDATETIME());
SET @json=CONCAT('[{"ID_DetalleOC":',@doc,',"CantidadRecibida":50,"CostoUnitario":5}]');
-- No INSERT EXEC here: SQL Server prevents ROLLBACK within INSERT EXEC on failure paths.
EXEC dbo.usp_RegistrarIngresoBodega @orden,@actor,@ubicacion,@fecha,@json;
DECLARE @lote1 int=(SELECT l.ID_Lote FROM dbo.tbl_LoteInventario l WHERE l.ID_ProductoV=@variante);
SET @json=CONCAT('[{"ID_ProductoV":',@variante,',"Cantidad":1,"PrecioUnitario":10}]');INSERT @resultado EXEC dbo.usp_GuardarCotizacion @cliente,@actor,@json;DECLARE @cot int=(SELECT id FROM @resultado);EXEC dbo.usp_FacturarPEPS @cliente,@actor,@cot,NULL;
ROLLBACK; THROW 51000,'Se aceptó una operación inválida.',1;
END TRY BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 IF ERROR_NUMBER()<>50001 THROW;
 IF (SELECT COUNT(*) FROM dbo.tbl_Factura)<>@antes OR (SELECT COUNT(*) FROM dbo.tbl_LoteInventario)<>@lotesAntes OR (SELECT COUNT(*) FROM dbo.tbl_PagoFactura)<>@pagosAntes THROW 51000,'El rollback no revirtió los registros de prueba.',1;
 PRINT 'OK: cotización pendiente no se factura.';
END CATCH;
GO
DECLARE @antes int=(SELECT COUNT(*) FROM dbo.tbl_Factura),@lotesAntes int=(SELECT COUNT(*) FROM dbo.tbl_LoteInventario),@pagosAntes int=(SELECT COUNT(*) FROM dbo.tbl_PagoFactura);
BEGIN TRY BEGIN TRAN;

DECLARE @rol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Digitador'),@cajeroRol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Cajero');
DECLARE @s nvarchar(36)=CONVERT(nvarchar(36),NEWID());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Prueba','Temporal',@s,(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@rol); DECLARE @actor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Caja','Temporal',@s+'c',(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@cajeroRol); DECLARE @cajero int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Marca(NombreMarca) VALUES('PRUEBA');DECLARE @marca int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_CategoriaProducto(NombreCategoria,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @categoria int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Presentacion(NombrePresentacion,UnidadMedida) VALUES('PRUEBA','Unidad');DECLARE @presentacion int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES('PRUEBA','PRUEBA',@categoria,@marca);DECLARE @producto int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(10,@producto,@presentacion,NULL);DECLARE @variante int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Proveedor(Nombre_Proveedor) VALUES('PRUEBA');DECLARE @proveedor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES('Particular','PRUEBA');DECLARE @cliente int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_UbicacionAlmacen(NombreUbicacion,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @ubicacion int=CONVERT(int,SCOPE_IDENTITY());
DECLARE @resultado TABLE(id int);DECLARE @json nvarchar(max)=CONCAT('[{"ID_ProductoV":',@variante,',"Cantidad":90,"PrecioUnitario":10}]');
INSERT @resultado EXEC dbo.usp_GuardarOrdenCompra @proveedor,@actor,@json;DECLARE @orden int=(SELECT id FROM @resultado);DELETE @resultado;
DECLARE @doc int=(SELECT ID_DetalleOC FROM dbo.tbl_DetalleOrdenCompra WHERE ID_OrdenC=@orden),@fecha datetime2(0)=DATEADD(day,-2,SYSDATETIME());
SET @json=CONCAT('[{"ID_DetalleOC":',@doc,',"CantidadRecibida":50,"CostoUnitario":5}]');
-- No INSERT EXEC here: SQL Server prevents ROLLBACK within INSERT EXEC on failure paths.
EXEC dbo.usp_RegistrarIngresoBodega @orden,@actor,@ubicacion,@fecha,@json;
DECLARE @lote1 int=(SELECT l.ID_Lote FROM dbo.tbl_LoteInventario l WHERE l.ID_ProductoV=@variante);
EXEC dbo.usp_GuardarOrdenCompra @proveedor,@cajero,@json;
ROLLBACK; THROW 51000,'Se aceptó una operación inválida.',1;
END TRY BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 IF ERROR_NUMBER()<>50001 THROW;
 IF (SELECT COUNT(*) FROM dbo.tbl_Factura)<>@antes OR (SELECT COUNT(*) FROM dbo.tbl_LoteInventario)<>@lotesAntes OR (SELECT COUNT(*) FROM dbo.tbl_PagoFactura)<>@pagosAntes THROW 51000,'El rollback no revirtió los registros de prueba.',1;
 PRINT 'OK: rol Cajero no puede crear compras.';
END CATCH;
GO
DECLARE @antes int=(SELECT COUNT(*) FROM dbo.tbl_Factura),@lotesAntes int=(SELECT COUNT(*) FROM dbo.tbl_LoteInventario),@pagosAntes int=(SELECT COUNT(*) FROM dbo.tbl_PagoFactura);
BEGIN TRY BEGIN TRAN;

DECLARE @rol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Digitador'),@cajeroRol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Cajero');
DECLARE @s nvarchar(36)=CONVERT(nvarchar(36),NEWID());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Prueba','Temporal',@s,(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@rol); DECLARE @actor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Caja','Temporal',@s+'c',(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@cajeroRol); DECLARE @cajero int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Marca(NombreMarca) VALUES('PRUEBA');DECLARE @marca int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_CategoriaProducto(NombreCategoria,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @categoria int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Presentacion(NombrePresentacion,UnidadMedida) VALUES('PRUEBA','Unidad');DECLARE @presentacion int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES('PRUEBA','PRUEBA',@categoria,@marca);DECLARE @producto int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(10,@producto,@presentacion,NULL);DECLARE @variante int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Proveedor(Nombre_Proveedor) VALUES('PRUEBA');DECLARE @proveedor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES('Particular','PRUEBA');DECLARE @cliente int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_UbicacionAlmacen(NombreUbicacion,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @ubicacion int=CONVERT(int,SCOPE_IDENTITY());
DECLARE @resultado TABLE(id int);DECLARE @json nvarchar(max)=CONCAT('[{"ID_ProductoV":',@variante,',"Cantidad":90,"PrecioUnitario":10}]');
INSERT @resultado EXEC dbo.usp_GuardarOrdenCompra @proveedor,@actor,@json;DECLARE @orden int=(SELECT id FROM @resultado);DELETE @resultado;
DECLARE @doc int=(SELECT ID_DetalleOC FROM dbo.tbl_DetalleOrdenCompra WHERE ID_OrdenC=@orden),@fecha datetime2(0)=DATEADD(day,-2,SYSDATETIME());
SET @json=CONCAT('[{"ID_DetalleOC":',@doc,',"CantidadRecibida":50,"CostoUnitario":5}]');
-- No INSERT EXEC here: SQL Server prevents ROLLBACK within INSERT EXEC on failure paths.
EXEC dbo.usp_RegistrarIngresoBodega @orden,@actor,@ubicacion,@fecha,@json;
DECLARE @lote1 int=(SELECT l.ID_Lote FROM dbo.tbl_LoteInventario l WHERE l.ID_ProductoV=@variante);
SET @json=CONCAT('[{"ID_ProductoV":',@variante,',"Cantidad":1,"PrecioUnitario":10}]');INSERT @resultado EXEC dbo.usp_FacturarPEPS @cliente,@actor,NULL,@json;DECLARE @fac int=(SELECT id FROM @resultado);INSERT dbo.tbl_MetodoPago(NombreMetodo) VALUES(@s);DECLARE @metodo int=CONVERT(int,SCOPE_IDENTITY());SET @json=CONCAT('[{"ID_MetodoP":',@metodo,',"Monto":11}]');EXEC dbo.usp_RegistrarPagos @fac,@cajero,@json;
ROLLBACK; THROW 51000,'Se aceptó una operación inválida.',1;
END TRY BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 IF ERROR_NUMBER()<>50001 THROW;
 IF (SELECT COUNT(*) FROM dbo.tbl_Factura)<>@antes OR (SELECT COUNT(*) FROM dbo.tbl_LoteInventario)<>@lotesAntes OR (SELECT COUNT(*) FROM dbo.tbl_PagoFactura)<>@pagosAntes THROW 51000,'El rollback no revirtió los registros de prueba.',1;
 PRINT 'OK: sobrepago revierte pagos.';
END CATCH;
GO
DECLARE @antes int=(SELECT COUNT(*) FROM dbo.tbl_Factura),@lotesAntes int=(SELECT COUNT(*) FROM dbo.tbl_LoteInventario),@pagosAntes int=(SELECT COUNT(*) FROM dbo.tbl_PagoFactura);
BEGIN TRY BEGIN TRAN;

DECLARE @rol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Digitador'),@cajeroRol int=(SELECT ID_Rol FROM dbo.tbl_Rol WHERE NombreRol='Cajero');
DECLARE @s nvarchar(36)=CONVERT(nvarchar(36),NEWID());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Prueba','Temporal',@s,(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@rol); DECLARE @actor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Usuario(Nombres,Apellidos,NombreUsuario,PasswordHash,ID_Rol) VALUES('Caja','Temporal',@s+'c',(SELECT TOP(1) PasswordHash FROM dbo.tbl_Usuario),@cajeroRol); DECLARE @cajero int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Marca(NombreMarca) VALUES('PRUEBA');DECLARE @marca int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_CategoriaProducto(NombreCategoria,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @categoria int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Presentacion(NombrePresentacion,UnidadMedida) VALUES('PRUEBA','Unidad');DECLARE @presentacion int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca) VALUES('PRUEBA','PRUEBA',@categoria,@marca);DECLARE @producto int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(10,@producto,@presentacion,NULL);DECLARE @variante int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Proveedor(Nombre_Proveedor) VALUES('PRUEBA');DECLARE @proveedor int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_Clientes(TipoCliente,NombreRazonSocial) VALUES('Particular','PRUEBA');DECLARE @cliente int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.tbl_UbicacionAlmacen(NombreUbicacion,Descripcion) VALUES('PRUEBA','PRUEBA');DECLARE @ubicacion int=CONVERT(int,SCOPE_IDENTITY());
DECLARE @resultado TABLE(id int);DECLARE @json nvarchar(max)=CONCAT('[{"ID_ProductoV":',@variante,',"Cantidad":90,"PrecioUnitario":10}]');
INSERT @resultado EXEC dbo.usp_GuardarOrdenCompra @proveedor,@actor,@json;DECLARE @orden int=(SELECT id FROM @resultado);DELETE @resultado;
DECLARE @doc int=(SELECT ID_DetalleOC FROM dbo.tbl_DetalleOrdenCompra WHERE ID_OrdenC=@orden),@fecha datetime2(0)=DATEADD(day,-2,SYSDATETIME());
SET @json=CONCAT('[{"ID_DetalleOC":',@doc,',"CantidadRecibida":50,"CostoUnitario":5}]');
-- No INSERT EXEC here: SQL Server prevents ROLLBACK within INSERT EXEC on failure paths.
EXEC dbo.usp_RegistrarIngresoBodega @orden,@actor,@ubicacion,@fecha,@json;
DECLARE @lote1 int=(SELECT l.ID_Lote FROM dbo.tbl_LoteInventario l WHERE l.ID_ProductoV=@variante);
SET @json=CONCAT('[{"ID_ProductoV":',@variante,',"Cantidad":1,"PrecioUnitario":10}]');INSERT @resultado EXEC dbo.usp_GuardarCotizacion @cliente,@actor,@json;DECLARE @cot int=(SELECT id FROM @resultado);EXEC dbo.usp_EstadoCotizacion @cot,'Confirmada',@actor;EXEC dbo.usp_FacturarPEPS @cliente,@actor,@cot,NULL;EXEC dbo.usp_FacturarPEPS @cliente,@actor,@cot,NULL;
ROLLBACK; THROW 51000,'Se aceptó una operación inválida.',1;
END TRY BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 IF ERROR_NUMBER()<>50001 THROW;
 IF (SELECT COUNT(*) FROM dbo.tbl_Factura)<>@antes OR (SELECT COUNT(*) FROM dbo.tbl_LoteInventario)<>@lotesAntes OR (SELECT COUNT(*) FROM dbo.tbl_PagoFactura)<>@pagosAntes THROW 51000,'El rollback no revirtió los registros de prueba.',1;
 PRINT 'OK: cotización no se factura dos veces.';
END CATCH;
GO

