-- Fase final. Ejecutar en la base existente después de Fase2.sql.
USE Proyecto_BD;
GO
CREATE OR ALTER PROCEDURE dbo.usp_ReporteFacturacionFormasPago
 @FechaInicial date, @FechaFinal date
AS
BEGIN
 SET NOCOUNT ON;
 IF @FechaInicial IS NULL OR @FechaFinal IS NULL OR @FechaInicial>@FechaFinal
  THROW 50001,N'El período de fechas no es válido.',1;
 -- El período corresponde a la emisión de las facturas; los pagos no tienen fecha propia.
 SELECT ISNULL(SUM(f.Total),0) TotalFacturado,
        ISNULL(SUM(p.Pagado),0) TotalPagado,
        ISNULL(SUM(f.Total-ISNULL(p.Pagado,0)),0) SaldoPorCobrar
 FROM dbo.tbl_Factura f
 OUTER APPLY(SELECT SUM(Monto) Pagado FROM dbo.tbl_PagoFactura WHERE ID_Factura=f.ID_Factura) p
 WHERE f.Estado<>N'Anulada' AND CONVERT(date,f.FechaFactura) BETWEEN @FechaInicial AND @FechaFinal;
 ;WITH Catalogo AS (
  SELECT ID_MetodoP,CASE
   WHEN NombreMetodo=N'Efectivo' OR NombreMetodo LIKE N'Efectivo (%' THEN N'Efectivo'
   WHEN NombreMetodo=N'Cheque' OR NombreMetodo LIKE N'Cheque (%' THEN N'Cheque'
   WHEN NombreMetodo=N'Tarjeta' OR NombreMetodo LIKE N'Tarjeta (%' THEN N'Tarjeta'
   ELSE NombreMetodo END NombreMetodo FROM dbo.tbl_MetodoPago
 ), Metodos AS (
  SELECT NombreMetodo FROM Catalogo
  UNION SELECT N'Efectivo' UNION SELECT N'Cheque' UNION SELECT N'Tarjeta'
 ), Pagos AS (
  SELECT m.NombreMetodo,SUM(p.Monto) TotalPagado
  FROM dbo.tbl_PagoFactura p JOIN Catalogo m ON m.ID_MetodoP=p.ID_MetodoP
  JOIN dbo.tbl_Factura f ON f.ID_Factura=p.ID_Factura
  WHERE f.Estado<>N'Anulada' AND CONVERT(date,f.FechaFactura) BETWEEN @FechaInicial AND @FechaFinal
  GROUP BY m.NombreMetodo
 )
 SELECT m.NombreMetodo MetodoPago,ISNULL(p.TotalPagado,0) TotalPagado
 FROM Metodos m LEFT JOIN Pagos p ON p.NombreMetodo=m.NombreMetodo ORDER BY m.NombreMetodo;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_ReporteProductosMayoresIngresos
 @FechaInicial date, @FechaFinal date
AS
BEGIN
 SET NOCOUNT ON;
 IF @FechaInicial IS NULL OR @FechaFinal IS NULL OR @FechaInicial>@FechaFinal
  THROW 50001,N'El período de fechas no es válido.',1;
 SELECT p.NombreProducto Producto,pr.NombrePresentacion Presentacion,COALESCE(c.NombreColor,N'Sin color') Color,
        SUM(CONVERT(bigint,d.Cantidad)) CantidadVendida,SUM(d.Subtotal) IngresoTotal
 FROM dbo.tbl_DetalleFactura d JOIN dbo.tbl_Factura f ON f.ID_Factura=d.ID_Factura
 JOIN dbo.tbl_ProductoVariante v ON v.ID_ProductoV=d.ID_ProductoV
 JOIN dbo.tbl_Producto p ON p.ID_Producto=v.ID_Producto
 JOIN dbo.tbl_Presentacion pr ON pr.ID_Presentacion=v.ID_Presentacion
 LEFT JOIN dbo.tbl_Color c ON c.ID_Color=v.ID_Color
 WHERE f.Estado<>N'Anulada' AND CONVERT(date,f.FechaFactura) BETWEEN @FechaInicial AND @FechaFinal
 GROUP BY v.ID_ProductoV,p.NombreProducto,pr.NombrePresentacion,c.NombreColor
 ORDER BY IngresoTotal DESC,CantidadVendida DESC,Producto,Presentacion,Color;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_ReporteProductosMasVendidos
 @FechaInicial date, @FechaFinal date
AS
BEGIN
 SET NOCOUNT ON;
 IF @FechaInicial IS NULL OR @FechaFinal IS NULL OR @FechaInicial>@FechaFinal
  THROW 50001,N'El período de fechas no es válido.',1;
 SELECT p.NombreProducto Producto,pr.NombrePresentacion Presentacion,COALESCE(c.NombreColor,N'Sin color') Color,
        SUM(CONVERT(bigint,d.Cantidad)) CantidadVendida
 FROM dbo.tbl_DetalleFactura d JOIN dbo.tbl_Factura f ON f.ID_Factura=d.ID_Factura
 JOIN dbo.tbl_ProductoVariante v ON v.ID_ProductoV=d.ID_ProductoV
 JOIN dbo.tbl_Producto p ON p.ID_Producto=v.ID_Producto
 JOIN dbo.tbl_Presentacion pr ON pr.ID_Presentacion=v.ID_Presentacion
 LEFT JOIN dbo.tbl_Color c ON c.ID_Color=v.ID_Color
 WHERE f.Estado<>N'Anulada' AND CONVERT(date,f.FechaFactura) BETWEEN @FechaInicial AND @FechaFinal
 GROUP BY v.ID_ProductoV,p.NombreProducto,pr.NombrePresentacion,c.NombreColor
 ORDER BY CantidadVendida DESC,Producto,Presentacion,Color;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_ReporteInventarioActual
AS
BEGIN
 SET NOCOUNT ON;
 SELECT p.NombreProducto Producto,pr.NombrePresentacion Presentacion,COALESCE(c.NombreColor,N'Sin color') Color,
        COALESCE(l.NumeroLote,CONCAT(N'Lote ',l.ID_Lote)) Lote,u.NombreUbicacion Ubicacion,
        l.FechaIngreso,l.CantidadDisponible
 FROM dbo.tbl_LoteInventario l JOIN dbo.tbl_ProductoVariante v ON v.ID_ProductoV=l.ID_ProductoV
 JOIN dbo.tbl_Producto p ON p.ID_Producto=v.ID_Producto
 JOIN dbo.tbl_Presentacion pr ON pr.ID_Presentacion=v.ID_Presentacion
 LEFT JOIN dbo.tbl_Color c ON c.ID_Color=v.ID_Color
 JOIN dbo.tbl_UbicacionAlmacen u ON u.ID_UbicacionA=l.ID_UbicacionA
 WHERE l.CantidadDisponible>0
 ORDER BY Producto,Presentacion,Color,l.FechaIngreso,l.ID_Lote;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_ReporteProductosMenosVendidos
 @FechaInicial date, @FechaFinal date
AS
BEGIN
 SET NOCOUNT ON;
 IF @FechaInicial IS NULL OR @FechaFinal IS NULL OR @FechaInicial>@FechaFinal
  THROW 50001,N'El período de fechas no es válido.',1;
 ;WITH Ventas AS (
  SELECT d.ID_ProductoV,SUM(CONVERT(bigint,d.Cantidad)) CantidadVendida
  FROM dbo.tbl_DetalleFactura d JOIN dbo.tbl_Factura f ON f.ID_Factura=d.ID_Factura
  WHERE f.Estado<>N'Anulada' AND CONVERT(date,f.FechaFactura) BETWEEN @FechaInicial AND @FechaFinal
  GROUP BY d.ID_ProductoV
 )
 SELECT p.NombreProducto Producto,pr.NombrePresentacion Presentacion,COALESCE(c.NombreColor,N'Sin color') Color,
        ISNULL(s.CantidadVendida,0) CantidadVendida
 FROM dbo.tbl_ProductoVariante v JOIN dbo.tbl_Producto p ON p.ID_Producto=v.ID_Producto
 JOIN dbo.tbl_Presentacion pr ON pr.ID_Presentacion=v.ID_Presentacion
 LEFT JOIN dbo.tbl_Color c ON c.ID_Color=v.ID_Color LEFT JOIN Ventas s ON s.ID_ProductoV=v.ID_ProductoV
 ORDER BY CantidadVendida,Producto,Presentacion,Color;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_ReporteProductosSinExistencia
AS
BEGIN
 SET NOCOUNT ON;
 SELECT p.NombreProducto Producto,pr.NombrePresentacion Presentacion,COALESCE(c.NombreColor,N'Sin color') Color,
        ISNULL(SUM(CONVERT(bigint,l.CantidadDisponible)),0) CantidadDisponible
 FROM dbo.tbl_ProductoVariante v JOIN dbo.tbl_Producto p ON p.ID_Producto=v.ID_Producto
 JOIN dbo.tbl_Presentacion pr ON pr.ID_Presentacion=v.ID_Presentacion
 LEFT JOIN dbo.tbl_Color c ON c.ID_Color=v.ID_Color LEFT JOIN dbo.tbl_LoteInventario l ON l.ID_ProductoV=v.ID_ProductoV
 GROUP BY v.ID_ProductoV,p.NombreProducto,pr.NombrePresentacion,c.NombreColor
 HAVING ISNULL(SUM(CONVERT(bigint,l.CantidadDisponible)),0)=0
 ORDER BY Producto,Presentacion,Color;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_ReporteDetalleFactura @NumeroFactura int
AS
BEGIN
 SET NOCOUNT ON;
 IF @NumeroFactura IS NULL OR @NumeroFactura<=0 THROW 50001,N'Ingrese un número de factura positivo.',1;
 DECLARE @id int=(SELECT ID_Factura FROM dbo.tbl_Factura WHERE NumeroFactura=@NumeroFactura);
 SELECT f.NumeroFactura,f.FechaFactura,c.NombreRazonSocial Cliente,f.Estado,f.Total,
        CONCAT(u.Nombres,N' ',u.Apellidos) Empleado,u.NombreUsuario Usuario,
        ISNULL((SELECT SUM(Subtotal) FROM dbo.tbl_DetalleFactura WHERE ID_Factura=f.ID_Factura),0) TotalOriginal,
        ISNULL((SELECT SUM(Monto) FROM dbo.tbl_PagoFactura WHERE ID_Factura=f.ID_Factura),0) PagosHistoricos,
        CASE WHEN f.Estado=N'Anulada' THEN CONVERT(decimal(12,2),0)
             ELSE f.Total-ISNULL((SELECT SUM(Monto) FROM dbo.tbl_PagoFactura WHERE ID_Factura=f.ID_Factura),0) END SaldoPorCobrar
 FROM dbo.tbl_Factura f JOIN dbo.tbl_Clientes c ON c.ID_Cliente=f.ID_Cliente
 JOIN dbo.tbl_Usuario u ON u.ID_Usuario=f.ID_Usuario WHERE f.ID_Factura=@id;
 SELECT p.NombreProducto Producto,pr.NombrePresentacion Presentacion,COALESCE(c.NombreColor,N'Sin color') Color,
        d.Cantidad,d.PrecioUnitario,d.Subtotal
 FROM dbo.tbl_DetalleFactura d JOIN dbo.tbl_ProductoVariante v ON v.ID_ProductoV=d.ID_ProductoV
 JOIN dbo.tbl_Producto p ON p.ID_Producto=v.ID_Producto
 JOIN dbo.tbl_Presentacion pr ON pr.ID_Presentacion=v.ID_Presentacion
 LEFT JOIN dbo.tbl_Color c ON c.ID_Color=v.ID_Color WHERE d.ID_Factura=@id ORDER BY d.ID_DetalleF;
 SELECT m.NombreMetodo MetodoPago,p.Monto,p.Referencia FROM dbo.tbl_PagoFactura p
 JOIN dbo.tbl_MetodoPago m ON m.ID_MetodoP=p.ID_MetodoP WHERE p.ID_Factura=@id ORDER BY p.ID_PagoF;
 SELECT a.FechaHora,a.Accion,CONCAT(u.Nombres,N' ',u.Apellidos) Responsable,a.DatosAnteriores,a.DatosNuevos
 FROM dbo.tbl_Auditoria a JOIN dbo.tbl_Usuario u ON u.ID_Usuario=a.ID_Usuario
 WHERE a.TablaAfectada=N'tbl_Factura' AND a.RegistroAfectado=CONVERT(nvarchar(60),@id)
 ORDER BY a.FechaHora,a.ID_Auditoria;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_AnularFactura @NumeroFactura int,@ID_Usuario int
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
  BEGIN TRANSACTION;
  IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol
                WHERE u.ID_Usuario=@ID_Usuario AND r.NombreRol=N'Gerente')
   THROW 50001,N'Sólo el Gerente puede anular facturas.',1;
  IF @NumeroFactura IS NULL OR @NumeroFactura<=0 THROW 50001,N'Ingrese un número de factura positivo.',1;
  -- Mismo orden de bloqueo que la facturación PEPS. No se modifica su algoritmo.
  DECLARE @bloqueo int;
  EXEC @bloqueo=sys.sp_getapplock @Resource='ProyectoBD_FacturacionPEPS',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000;
  IF @bloqueo<0 THROW 50001,N'No se pudo obtener el bloqueo de inventario; intente nuevamente.',1;
  DECLARE @id int,@estado nvarchar(45),@total decimal(12,2),@antes nvarchar(max);
  SELECT @id=ID_Factura,@estado=Estado,@total=Total FROM dbo.tbl_Factura WITH(UPDLOCK,HOLDLOCK) WHERE NumeroFactura=@NumeroFactura;
  IF @id IS NULL THROW 50001,N'La factura no existe.',1;
  IF @estado=N'Anulada' THROW 50001,N'La factura ya está anulada.',1;
  IF @estado NOT IN(N'Pendiente',N'Pagada') THROW 50001,N'El estado de la factura no permite su anulación.',1;
  -- Una devolución previa requiere conciliación para evitar reintegrar dos veces.
  IF EXISTS(SELECT 1 FROM dbo.tbl_DevolucionVenta WITH(UPDLOCK,HOLDLOCK) WHERE ID_Factura=@id)
   THROW 50001,N'La factura tiene devoluciones registradas y requiere conciliación antes de anularse.',1;
  IF NOT EXISTS(SELECT 1 FROM dbo.tbl_DetalleFactura WITH(UPDLOCK,HOLDLOCK) WHERE ID_Factura=@id)
   THROW 50001,N'La factura no tiene detalles; no se modificó el inventario.',1;
  IF EXISTS(
   SELECT 1 FROM dbo.tbl_DetalleFactura d WITH(UPDLOCK,HOLDLOCK)
   OUTER APPLY(SELECT SUM(CONVERT(bigint,fl.CantidadSalida)) Salida FROM dbo.tbl_DetalleFacturaLote fl WITH(UPDLOCK,HOLDLOCK) WHERE fl.ID_DetalleF=d.ID_DetalleF) s
   WHERE d.ID_Factura=@id AND (d.Cantidad<=0 OR d.PrecioUnitario<=0 OR d.Subtotal<>CONVERT(decimal(18,2),d.Cantidad)*d.PrecioUnitario OR ISNULL(s.Salida,0)<>d.Cantidad))
   THROW 50001,N'El detalle o sus cantidades por lote son inconsistentes.',1;
  IF @total<>(SELECT SUM(Subtotal) FROM dbo.tbl_DetalleFactura WHERE ID_Factura=@id)
   THROW 50001,N'El total no coincide con el detalle de la factura.',1;
  IF EXISTS(SELECT 1 FROM dbo.tbl_DetalleFactura d JOIN dbo.tbl_DetalleFacturaLote fl ON fl.ID_DetalleF=d.ID_DetalleF
             LEFT JOIN dbo.tbl_LoteInventario l WITH(UPDLOCK,HOLDLOCK) ON l.ID_Lote=fl.ID_Lote
             WHERE d.ID_Factura=@id AND (fl.CantidadSalida<=0 OR l.ID_Lote IS NULL OR l.ID_ProductoV<>d.ID_ProductoV))
   THROW 50001,N'Los lotes de la factura no son válidos.',1;
  DECLARE @lotes TABLE(ID_Lote int PRIMARY KEY,Cantidad int,DisponibleAntes int,ID_ProductoV int,ID_UbicacionA int);
  INSERT @lotes
   SELECT l.ID_Lote,SUM(fl.CantidadSalida),l.CantidadDisponible,l.ID_ProductoV,l.ID_UbicacionA
   FROM dbo.tbl_DetalleFactura d JOIN dbo.tbl_DetalleFacturaLote fl ON fl.ID_DetalleF=d.ID_DetalleF
   JOIN dbo.tbl_LoteInventario l WITH(UPDLOCK,HOLDLOCK) ON l.ID_Lote=fl.ID_Lote WHERE d.ID_Factura=@id
   GROUP BY l.ID_Lote,l.CantidadDisponible,l.ID_ProductoV,l.ID_UbicacionA;
  IF EXISTS(SELECT 1 FROM @lotes r JOIN dbo.tbl_LoteInventario l ON l.ID_Lote=r.ID_Lote
            WHERE r.DisponibleAntes<0 OR CONVERT(bigint,r.DisponibleAntes)+r.Cantidad>l.CantidadInicial)
   THROW 50001,N'El reintegro superaría la cantidad inicial de un lote.',1;
  SET @antes=(SELECT @NumeroFactura NumeroFactura,@estado Estado,@total Total,
                     JSON_QUERY((SELECT * FROM @lotes FOR JSON PATH)) Lotes FOR JSON PATH,WITHOUT_ARRAY_WRAPPER);
  UPDATE l SET CantidadDisponible=l.CantidadDisponible+r.Cantidad
   FROM dbo.tbl_LoteInventario l JOIN @lotes r ON r.ID_Lote=l.ID_Lote;
  INSERT dbo.tbl_MovimientoInventario(TipoMovimiento,Cantidad,FechaHora,ID_Referencia,TipoReferencia,ID_ProductoV,ID_Lote,ID_UbicacionA,ID_Usuario)
   SELECT N'ENTRADA_ANULACION',Cantidad,SYSDATETIME(),@id,N'FACTURA',ID_ProductoV,ID_Lote,ID_UbicacionA,@ID_Usuario FROM @lotes;
  UPDATE dbo.tbl_Factura SET Estado=N'Anulada',Total=0 WHERE ID_Factura=@id;
  INSERT dbo.tbl_Auditoria(FechaHora,TablaAfectada,RegistroAfectado,Accion,DatosAnteriores,DatosNuevos,ID_Usuario)
   VALUES(SYSDATETIME(),N'tbl_Factura',CONVERT(nvarchar(60),@id),N'ANULAR FACTURA',@antes,
          (SELECT @NumeroFactura NumeroFactura,N'Anulada' Estado,CONVERT(decimal(12,2),0) Total,
                  JSON_QUERY((SELECT ID_Lote,Cantidad Reintegrada,DisponibleAntes+Cantidad DisponibleDespues FROM @lotes FOR JSON PATH)) Lotes
           FOR JSON PATH,WITHOUT_ARRAY_WRAPPER),@ID_Usuario);
  COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
  THROW;
 END CATCH;
END;
GO
