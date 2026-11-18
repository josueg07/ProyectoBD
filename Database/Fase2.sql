-- Fase 2. Ejecutar en la BD existente; no crea ni reemplaza tablas.
USE Proyecto_BD;
GO
CREATE OR ALTER PROCEDURE dbo.usp_GuardarCotizacion
 @ID_Cliente int,@ID_Usuario int,@Detalle nvarchar(max)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRAN;
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@ID_Usuario AND r.NombreRol='Digitador') THROW 50001,'Rol sin permiso para cotizar.',1;
 IF ISJSON(@Detalle)<>1 THROW 50001,'Detalle JSON inválido.',1;
 DECLARE @d TABLE(ID_ProductoV int NOT NULL PRIMARY KEY,Cantidad int NOT NULL,PrecioUnitario decimal(12,2) NOT NULL);
 INSERT @d SELECT ID_ProductoV,Cantidad,PrecioUnitario FROM OPENJSON(@Detalle) WITH(ID_ProductoV int,Cantidad int,PrecioUnitario decimal(12,2));
 IF NOT EXISTS(SELECT 1 FROM @d) OR EXISTS(SELECT 1 FROM @d WHERE Cantidad<=0 OR PrecioUnitario<=0) THROW 50001,'Detalle vacío o cantidades/precios inválidos.',1;
 INSERT dbo.tbl_Cotizacion(FechaCotizacion,Estado,Total,ID_Cliente,ID_Usuario) SELECT CONVERT(date,SYSDATETIME()),'Pendiente',SUM(Cantidad*PrecioUnitario),@ID_Cliente,@ID_Usuario FROM @d;
 DECLARE @id int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_DetalleCotizacion(Cantidad,PrecioUnitario,Subtotal,ID_Cotizacion,ID_ProductoV) SELECT Cantidad,PrecioUnitario,Cantidad*PrecioUnitario,@id,ID_ProductoV FROM @d;
 COMMIT; SELECT @id;
 END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_EstadoCotizacion @ID_Cotizacion int,@Estado nvarchar(45),@ID_Usuario int
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY BEGIN TRAN;
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@ID_Usuario AND r.NombreRol='Digitador') THROW 50001,'Rol sin permiso para cotizar.',1;
 IF @Estado NOT IN('Confirmada','Cancelada') THROW 50001,'Estado inválido.',1;
 UPDATE dbo.tbl_Cotizacion WITH(UPDLOCK,HOLDLOCK) SET Estado=@Estado WHERE ID_Cotizacion=@ID_Cotizacion AND (Estado='Pendiente' OR (Estado='Confirmada' AND @Estado='Cancelada'));
 IF @@ROWCOUNT=0 THROW 50001,'La cotización no admite ese cambio de estado.',1;
 COMMIT;
 END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_GuardarOrdenCompra @ID_Proveedor int,@ID_Usuario int,@Detalle nvarchar(max)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY BEGIN TRAN;
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@ID_Usuario AND r.NombreRol='Digitador') THROW 50001,'Rol sin permiso para compras.',1;
 IF ISJSON(@Detalle)<>1 THROW 50001,'Detalle JSON inválido.',1;
 DECLARE @d TABLE(ID_ProductoV int NOT NULL PRIMARY KEY,Cantidad int NOT NULL);
 INSERT @d SELECT ID_ProductoV,Cantidad FROM OPENJSON(@Detalle) WITH(ID_ProductoV int,Cantidad int);
 IF NOT EXISTS(SELECT 1 FROM @d) OR EXISTS(SELECT 1 FROM @d WHERE Cantidad<=0) THROW 50001,'Debe incluir cantidades positivas.',1;
 INSERT dbo.tbl_OrdenCompra(FechaOrden,Estado,ID_Proveedor) VALUES(CONVERT(date,SYSDATETIME()),'Pendiente',@ID_Proveedor);
 DECLARE @id int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_DetalleOrdenCompra(CantidadSolicitada,ID_ProductoV,ID_OrdenC) SELECT Cantidad,ID_ProductoV,@id FROM @d;
 COMMIT; SELECT @id;
 END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_RegistrarIngresoBodega
 @ID_OrdenC int,@ID_Usuario int,@ID_UbicacionA int,@FechaIngreso datetime2(0),@Detalle nvarchar(max)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY BEGIN TRAN;
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@ID_Usuario AND r.NombreRol='Digitador') THROW 50001,'Rol sin permiso para ingresos.',1;
 IF @FechaIngreso IS NULL OR @FechaIngreso>SYSDATETIME() THROW 50001,'Fecha de ingreso inválida.',1;
 DECLARE @estado nvarchar(45);
 SELECT @estado=Estado FROM dbo.tbl_OrdenCompra WITH(UPDLOCK,HOLDLOCK) WHERE ID_OrdenC=@ID_OrdenC;
 IF @estado IS NULL OR @estado NOT IN('Pendiente','Parcial') THROW 50001,'Orden inexistente o completa.',1;
 IF ISJSON(@Detalle)<>1 THROW 50001,'Detalle JSON inválido.',1;
 DECLARE @d TABLE(ID_DetalleOC int NOT NULL PRIMARY KEY,CantidadRecibida int NOT NULL,CostoUnitario decimal(12,2) NOT NULL,NumeroLote nvarchar(50));
 INSERT @d SELECT ID_DetalleOC,CantidadRecibida,CostoUnitario,NumeroLote FROM OPENJSON(@Detalle) WITH(ID_DetalleOC int,CantidadRecibida int,CostoUnitario decimal(12,2),NumeroLote nvarchar(50));
 IF NOT EXISTS(SELECT 1 FROM @d) OR EXISTS(SELECT 1 FROM @d WHERE CantidadRecibida<=0 OR CostoUnitario<=0) THROW 50001,'Detalle o costo inválido.',1;
 IF EXISTS(SELECT 1 FROM @d d LEFT JOIN dbo.tbl_DetalleOrdenCompra oc ON oc.ID_DetalleOC=d.ID_DetalleOC AND oc.ID_OrdenC=@ID_OrdenC WHERE oc.ID_DetalleOC IS NULL OR d.CantidadRecibida>oc.CantidadSolicitada-ISNULL((SELECT SUM(ib.CantidadRecibida) FROM dbo.tbl_DetalleIngresoBodega ib WHERE ib.ID_DetalleOC=oc.ID_DetalleOC),0)) THROW 50001,'La recepción supera lo pendiente o contiene detalles de otra orden.',1;
 INSERT dbo.tbl_IngresoBodega(FechaIngreso,ID_OrdenC,ID_Usuario) VALUES(@FechaIngreso,@ID_OrdenC,@ID_Usuario);
 DECLARE @id int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_DetalleIngresoBodega(CantidadRecibida,CostoUnitario,ID_IngresoB,ID_DetalleOC) SELECT CantidadRecibida,CostoUnitario,@id,ID_DetalleOC FROM @d;
 INSERT dbo.tbl_LoteInventario(NumeroLote,FechaIngreso,CantidadInicial,CantidadDisponible,CostoUnitario,ID_DetalleIB,ID_ProductoV,ID_UbicacionA)
 SELECT NULLIF(d.NumeroLote,''),@FechaIngreso,d.CantidadRecibida,d.CantidadRecibida,d.CostoUnitario,ib.ID_DetalleIB,oc.ID_ProductoV,@ID_UbicacionA FROM @d d JOIN dbo.tbl_DetalleIngresoBodega ib ON ib.ID_DetalleOC=d.ID_DetalleOC AND ib.ID_IngresoB=@id JOIN dbo.tbl_DetalleOrdenCompra oc ON oc.ID_DetalleOC=d.ID_DetalleOC;
 INSERT dbo.tbl_MovimientoInventario(TipoMovimiento,Cantidad,FechaHora,ID_Referencia,TipoReferencia,ID_ProductoV,ID_Lote,ID_UbicacionA,ID_Usuario)
 SELECT 'ENTRADA_COMPRA',l.CantidadInicial,SYSDATETIME(),@id,'INGRESO BODEGA',l.ID_ProductoV,l.ID_Lote,l.ID_UbicacionA,@ID_Usuario FROM dbo.tbl_LoteInventario l JOIN dbo.tbl_DetalleIngresoBodega ib ON ib.ID_DetalleIB=l.ID_DetalleIB WHERE ib.ID_IngresoB=@id;
 UPDATE dbo.tbl_OrdenCompra SET Estado=CASE WHEN EXISTS(SELECT 1 FROM dbo.tbl_DetalleOrdenCompra oc WHERE oc.ID_OrdenC=@ID_OrdenC AND oc.CantidadSolicitada>ISNULL((SELECT SUM(CantidadRecibida) FROM dbo.tbl_DetalleIngresoBodega WHERE ID_DetalleOC=oc.ID_DetalleOC),0)) THEN 'Parcial' ELSE 'Completa' END WHERE ID_OrdenC=@ID_OrdenC;
 INSERT dbo.tbl_Auditoria(FechaHora,TablaAfectada,RegistroAfectado,Accion,DatosNuevos,ID_Usuario) VALUES(SYSDATETIME(),'tbl_IngresoBodega',CONVERT(nvarchar(60),@id),'INGRESO BODEGA',@Detalle,@ID_Usuario);
 COMMIT; SELECT @id;
 END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_FacturarPEPS
 @ID_Cliente int,@ID_Usuario int,@ID_Cotizacion int=NULL,@Detalle nvarchar(max)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY BEGIN TRAN;
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@ID_Usuario AND r.NombreRol IN('Digitador','Cajero')) THROW 50001,'Rol sin permiso para facturar.',1;
 -- Serializa numeración y salidas, incluso cuando aún no hay facturas.
 DECLARE @bloqueo int;
 EXEC @bloqueo=sys.sp_getapplock @Resource='ProyectoBD_FacturacionPEPS',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000;
 IF @bloqueo<0 THROW 50001,'No se pudo obtener el bloqueo de facturación; intente de nuevo.',1;
 DECLARE @d TABLE(ID_ProductoV int NOT NULL PRIMARY KEY,Cantidad int NOT NULL,PrecioUnitario decimal(12,2) NOT NULL);
 IF @ID_Cotizacion IS NOT NULL
 BEGIN
   DECLARE @estado nvarchar(45),@cliente int;
   SELECT @estado=Estado,@cliente=ID_Cliente FROM dbo.tbl_Cotizacion WITH(UPDLOCK,HOLDLOCK) WHERE ID_Cotizacion=@ID_Cotizacion;
   IF @estado IS NULL OR @estado<>'Confirmada' OR @cliente<>@ID_Cliente THROW 50001,'La cotización debe estar confirmada, sin facturar y pertenecer al cliente.',1;
   INSERT @d SELECT ID_ProductoV,Cantidad,PrecioUnitario FROM dbo.tbl_DetalleCotizacion WHERE ID_Cotizacion=@ID_Cotizacion;
 END
 ELSE
 BEGIN
   IF ISJSON(@Detalle)<>1 THROW 50001,'Detalle JSON inválido.',1;
   INSERT @d SELECT ID_ProductoV,Cantidad,PrecioUnitario FROM OPENJSON(@Detalle) WITH(ID_ProductoV int,Cantidad int,PrecioUnitario decimal(12,2));
 END;
 IF NOT EXISTS(SELECT 1 FROM @d) OR EXISTS(SELECT 1 FROM @d WHERE Cantidad<=0 OR PrecioUnitario<=0) THROW 50001,'Detalle vacío o valores inválidos.',1;
 DECLARE @numero int=(SELECT ISNULL(MAX(NumeroFactura),0)+1 FROM dbo.tbl_Factura WITH(UPDLOCK,HOLDLOCK)),@total decimal(12,2)=(SELECT SUM(Cantidad*PrecioUnitario) FROM @d);
 INSERT dbo.tbl_Factura(NumeroFactura,FechaFactura,Total,Estado,ID_Cliente,ID_Cotizacion,ID_Usuario) VALUES(@numero,SYSDATETIME(),@total,'Pendiente',@ID_Cliente,@ID_Cotizacion,@ID_Usuario);
 DECLARE @id int=CONVERT(int,SCOPE_IDENTITY()),@v int,@cantidad int,@precio decimal(12,2),@detalleF int,@lote int,@disponible int,@ubicacion int,@salida int;
 WHILE EXISTS(SELECT 1 FROM @d)
 BEGIN
   SELECT TOP(1) @v=ID_ProductoV,@cantidad=Cantidad,@precio=PrecioUnitario FROM @d ORDER BY ID_ProductoV;
   INSERT dbo.tbl_DetalleFactura(Cantidad,PrecioUnitario,Subtotal,ID_Factura,ID_ProductoV) VALUES(@cantidad,@precio,@cantidad*@precio,@id,@v);
   SET @detalleF=CONVERT(int,SCOPE_IDENTITY());
   WHILE @cantidad>0
   BEGIN
     SET @lote=NULL;
     SELECT TOP(1) @lote=ID_Lote,@disponible=CantidadDisponible,@ubicacion=ID_UbicacionA FROM dbo.tbl_LoteInventario WITH(UPDLOCK,HOLDLOCK) WHERE ID_ProductoV=@v AND CantidadDisponible>0 ORDER BY FechaIngreso ASC,ID_Lote ASC;
     IF @lote IS NULL THROW 50001,'Inventario insuficiente. No se guardó la factura ni se descontó inventario.',1;
     SET @salida=CASE WHEN @cantidad<@disponible THEN @cantidad ELSE @disponible END;
     INSERT dbo.tbl_DetalleFacturaLote(CantidadSalida,ID_Lote,ID_DetalleF) VALUES(@salida,@lote,@detalleF);
     UPDATE dbo.tbl_LoteInventario SET CantidadDisponible=CantidadDisponible-@salida WHERE ID_Lote=@lote;
     INSERT dbo.tbl_MovimientoInventario(TipoMovimiento,Cantidad,FechaHora,ID_Referencia,TipoReferencia,ID_ProductoV,ID_Lote,ID_UbicacionA,ID_Usuario) VALUES('SALIDA_VENTA',@salida,SYSDATETIME(),@id,'FACTURA',@v,@lote,@ubicacion,@ID_Usuario);
     SET @cantidad=@cantidad-@salida;
   END;
   DELETE @d WHERE ID_ProductoV=@v;
 END;
 IF @ID_Cotizacion IS NOT NULL UPDATE dbo.tbl_Cotizacion SET Estado='Facturada' WHERE ID_Cotizacion=@ID_Cotizacion;
 INSERT dbo.tbl_Auditoria(FechaHora,TablaAfectada,RegistroAfectado,Accion,DatosNuevos,ID_Usuario) VALUES(SYSDATETIME(),'tbl_Factura',CONVERT(nvarchar(60),@id),'FACTURA',(SELECT * FROM dbo.tbl_DetalleFactura WHERE ID_Factura=@id FOR JSON PATH),@ID_Usuario);
 COMMIT; SELECT @id;
 END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_RegistrarPagos @ID_Factura int,@ID_Usuario int,@Pagos nvarchar(max)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY BEGIN TRAN;
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@ID_Usuario AND r.NombreRol IN('Cajero','Digitador')) THROW 50001,'Rol sin permiso para cobros.',1;
 DECLARE @total decimal(12,2),@estado nvarchar(45);
 SELECT @total=Total,@estado=Estado FROM dbo.tbl_Factura WITH(UPDLOCK,HOLDLOCK) WHERE ID_Factura=@ID_Factura;
 IF @total IS NULL OR @estado<>'Pendiente' THROW 50001,'La factura no está pendiente de pago.',1;
 IF ISJSON(@Pagos)<>1 THROW 50001,'Pagos JSON inválidos.',1;
 DECLARE @p TABLE(ID_MetodoP int NOT NULL,Monto decimal(12,2) NOT NULL,Referencia nvarchar(100));
 INSERT @p SELECT ID_MetodoP,Monto,Referencia FROM OPENJSON(@Pagos) WITH(ID_MetodoP int,Monto decimal(12,2),Referencia nvarchar(100));
 IF NOT EXISTS(SELECT 1 FROM @p) OR EXISTS(SELECT 1 FROM @p WHERE Monto<=0) THROW 50001,'Ingrese pagos positivos.',1;
 DECLARE @pagado decimal(12,2)=ISNULL((SELECT SUM(Monto) FROM dbo.tbl_PagoFactura WHERE ID_Factura=@ID_Factura),0)+(SELECT SUM(Monto) FROM @p);
 IF @pagado>@total THROW 50001,'Los pagos superan el saldo de la factura.',1;
 INSERT dbo.tbl_PagoFactura(Monto,Referencia,ID_Factura,ID_MetodoP) SELECT Monto,NULLIF(Referencia,''),@ID_Factura,ID_MetodoP FROM @p;
 UPDATE dbo.tbl_Factura SET Estado=CASE WHEN @pagado=@total THEN 'Pagada' ELSE 'Pendiente' END WHERE ID_Factura=@ID_Factura;
 INSERT dbo.tbl_Auditoria(FechaHora,TablaAfectada,RegistroAfectado,Accion,DatosNuevos,ID_Usuario) VALUES(SYSDATETIME(),'tbl_Factura',CONVERT(nvarchar(60),@ID_Factura),'PAGO',@Pagos,@ID_Usuario);
 COMMIT;
 END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;
END;
GO
