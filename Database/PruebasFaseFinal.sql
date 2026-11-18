-- Preparación de un caso nuevo e identificable; NO anula facturas anteriores.
-- Verificacion --fase-final suministra @Sufijo y verifica los resultados.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @digitador int=(SELECT TOP(1) u.ID_Usuario FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE r.NombreRol=N'Digitador' ORDER BY u.ID_Usuario);
 DECLARE @cajero int=(SELECT TOP(1) u.ID_Usuario FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE r.NombreRol=N'Cajero' ORDER BY u.ID_Usuario);
 DECLARE @cliente int=(SELECT TOP(1) ID_Cliente FROM dbo.tbl_Clientes ORDER BY ID_Cliente);
 DECLARE @proveedor int=(SELECT TOP(1) ID_Proveedor FROM dbo.tbl_Proveedor ORDER BY ID_Proveedor);
 DECLARE @ubicacion int=(SELECT TOP(1) ID_UbicacionA FROM dbo.tbl_UbicacionAlmacen ORDER BY ID_UbicacionA);
 DECLARE @presentacion int=(SELECT TOP(1) ID_Presentacion FROM dbo.tbl_Presentacion ORDER BY ID_Presentacion);
 DECLARE @efectivo int=(SELECT TOP(1) ID_MetodoP FROM dbo.tbl_MetodoPago WHERE NombreMetodo=N'Efectivo' OR NombreMetodo LIKE N'Efectivo (%');
 DECLARE @tarjeta int=(SELECT TOP(1) ID_MetodoP FROM dbo.tbl_MetodoPago WHERE NombreMetodo=N'Tarjeta' OR NombreMetodo LIKE N'Tarjeta (%');
 DECLARE @cheque int=(SELECT TOP(1) ID_MetodoP FROM dbo.tbl_MetodoPago WHERE NombreMetodo=N'Cheque' OR NombreMetodo LIKE N'Cheque (%');
 IF @digitador IS NULL OR @cajero IS NULL OR @efectivo IS NULL OR @tarjeta IS NULL OR @cheque IS NULL
  THROW 50001,N'Faltan los roles o métodos de pago necesarios para la prueba.',1;
 DECLARE @otro int=(SELECT TOP(1) ID_MetodoP FROM dbo.tbl_MetodoPago WHERE NombreMetodo=N'Transferencia (prueba final)');
 IF @otro IS NULL
 BEGIN
  INSERT dbo.tbl_MetodoPago(NombreMetodo) VALUES(N'Transferencia (prueba final)');
  SET @otro=CONVERT(int,SCOPE_IDENTITY());
 END;
 INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca)
  SELECT N'Prueba final '+@Sufijo,N'Caso PEPS y anulación',MIN(ID_Categoria),(SELECT MIN(ID_Marca) FROM dbo.tbl_Marca) FROM dbo.tbl_CategoriaProducto;
 DECLARE @producto int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(10,@producto,@presentacion,NULL);
 DECLARE @variante int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_Producto(NombreProducto,Descripcion,ID_CategoriaP,ID_Marca)
  SELECT N'Sin ventas final '+@Sufijo,N'Caso sin ventas ni stock',MIN(ID_Categoria),(SELECT MIN(ID_Marca) FROM dbo.tbl_Marca) FROM dbo.tbl_CategoriaProducto;
 DECLARE @sinVenta int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_ProductoVariante(PrecioVenta,ID_Producto,ID_Presentacion,ID_Color) VALUES(10,@sinVenta,@presentacion,NULL);
 DECLARE @detalle nvarchar(max)=(SELECT @variante ID_ProductoV,5 Cantidad FOR JSON PATH);
 DECLARE @ids TABLE(ID int);
 INSERT @ids EXEC dbo.usp_GuardarOrdenCompra @proveedor,@digitador,@detalle;
 DECLARE @orden int=(SELECT ID FROM @ids),@detalleOC int;
 SELECT @detalleOC=ID_DetalleOC FROM dbo.tbl_DetalleOrdenCompra WHERE ID_OrdenC=@orden;
 DECLARE @fecha datetime2(0)=DATEADD(day,-2,SYSDATETIME());
 SET @detalle=(SELECT @detalleOC ID_DetalleOC,2 CantidadRecibida,5 CostoUnitario,N'FINAL-A-'+@Sufijo NumeroLote FOR JSON PATH);
 DELETE @ids;
 INSERT @ids EXEC dbo.usp_RegistrarIngresoBodega @orden,@digitador,@ubicacion,@fecha,@detalle;
 SET @fecha=DATEADD(day,-1,SYSDATETIME());
 SET @detalle=(SELECT @detalleOC ID_DetalleOC,3 CantidadRecibida,6 CostoUnitario,N'FINAL-B-'+@Sufijo NumeroLote FOR JSON PATH);
 DELETE @ids;
 INSERT @ids EXEC dbo.usp_RegistrarIngresoBodega @orden,@digitador,@ubicacion,@fecha,@detalle;
 SET @detalle=(SELECT @variante ID_ProductoV,4 Cantidad,10 PrecioUnitario FOR JSON PATH);
 DELETE @ids;
 INSERT @ids EXEC dbo.usp_FacturarPEPS @cliente,@cajero,NULL,@detalle;
 DECLARE @factura int=(SELECT ID FROM @ids);
 DECLARE @pagos nvarchar(max)=(SELECT ID_MetodoP,Monto,Referencia FROM (VALUES
  (@efectivo,CONVERT(decimal(12,2),7),N'PRUEBA FINAL EFECTIVO'),
  (@tarjeta,CONVERT(decimal(12,2),5),N'PRUEBA FINAL TARJETA'),
  (@cheque,CONVERT(decimal(12,2),3),N'PRUEBA FINAL CHEQUE'),
  (@otro,CONVERT(decimal(12,2),2),N'PRUEBA FINAL TRANSFERENCIA')) p(ID_MetodoP,Monto,Referencia) FOR JSON PATH);
 EXEC dbo.usp_RegistrarPagos @factura,@cajero,@pagos;
 COMMIT TRANSACTION;
 SELECT @factura ID_Factura,NumeroFactura,@variante ID_ProductoV,@producto ID_Producto,@sinVenta ID_ProductoSinVenta
 FROM dbo.tbl_Factura WHERE ID_Factura=@factura;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;
