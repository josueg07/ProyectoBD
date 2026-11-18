-- Consulta de lectura de los documentos y lotes de ejemplo.
SET NOCOUNT ON;
DECLARE @m nvarchar(max)=(SELECT TOP(1) DatosNuevos FROM dbo.tbl_Auditoria WHERE TablaAfectada='DATOS_EJEMPLO' AND RegistroAfectado='BD2_EJEMPLO_V1' AND Accion='CARGA EJEMPLO');
IF @m IS NULL THROW 50001,'No se ha ejecutado la carga V1.',1;
DECLARE @f1 int=CONVERT(int,JSON_VALUE(@m,'$.FacturaPagadaPEPS')),@f2 int=CONVERT(int,JSON_VALUE(@m,'$.FacturaAbono')),@f3 int=CONVERT(int,JSON_VALUE(@m,'$.FacturaSinPagos'));
DECLARE @v1 int=CONVERT(int,JSON_VALUE(@m,'$.VariantePinturaBlanca')),@v2 int=CONVERT(int,JSON_VALUE(@m,'$.VariantePinturaAzul'));
SELECT u.ID_Usuario,u.NombreUsuario,r.NombreRol FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.NombreUsuario IN('admin','cajero','gerente');
SELECT f.ID_Factura,f.NumeroFactura,c.NombreRazonSocial,f.Total,f.Estado,ISNULL(p.Pagado,0) Pagado,f.Total-ISNULL(p.Pagado,0) Saldo
FROM dbo.tbl_Factura f JOIN dbo.tbl_Clientes c ON c.ID_Cliente=f.ID_Cliente
OUTER APPLY(SELECT SUM(Monto) Pagado FROM dbo.tbl_PagoFactura WHERE ID_Factura=f.ID_Factura) p WHERE f.ID_Factura IN(@f1,@f2,@f3) ORDER BY f.NumeroFactura;
SELECT o.ID_OrdenC,o.Estado,p.Nombre_Proveedor,d.ID_ProductoV,d.CantidadSolicitada,ISNULL(SUM(i.CantidadRecibida),0) Recibido,d.CantidadSolicitada-ISNULL(SUM(i.CantidadRecibida),0) Pendiente
FROM dbo.tbl_OrdenCompra o JOIN dbo.tbl_Proveedor p ON p.ID_Proveedor=o.ID_Proveedor JOIN dbo.tbl_DetalleOrdenCompra d ON d.ID_OrdenC=o.ID_OrdenC
LEFT JOIN dbo.tbl_DetalleIngresoBodega i ON i.ID_DetalleOC=d.ID_DetalleOC
WHERE o.ID_OrdenC IN(CONVERT(int,JSON_VALUE(@m,'$.OrdenParcial')),CONVERT(int,JSON_VALUE(@m,'$.OrdenPendiente')))
GROUP BY o.ID_OrdenC,o.Estado,p.Nombre_Proveedor,d.ID_ProductoV,d.CantidadSolicitada ORDER BY o.ID_OrdenC,d.ID_ProductoV;
SELECT c.ID_Cotizacion,c.Estado,cl.NombreRazonSocial,c.Total FROM dbo.tbl_Cotizacion c JOIN dbo.tbl_Clientes cl ON cl.ID_Cliente=c.ID_Cliente
WHERE c.ID_Cotizacion IN(CONVERT(int,JSON_VALUE(@m,'$.CotizacionPendiente')),CONVERT(int,JSON_VALUE(@m,'$.CotizacionConfirmada')),CONVERT(int,JSON_VALUE(@m,'$.CotizacionCancelada')),CONVERT(int,JSON_VALUE(@m,'$.CotizacionFacturada'))) ORDER BY c.ID_Cotizacion;
SELECT l.ID_Lote,l.ID_ProductoV,l.NumeroLote,l.FechaIngreso,l.CantidadInicial,l.CantidadDisponible,l.CostoUnitario FROM dbo.tbl_LoteInventario l WHERE l.ID_ProductoV IN(@v1,@v2) ORDER BY l.ID_ProductoV,l.FechaIngreso,l.ID_Lote;
SELECT d.ID_Factura,fl.ID_Lote,l.FechaIngreso,fl.CantidadSalida,l.CantidadDisponible FROM dbo.tbl_DetalleFacturaLote fl JOIN dbo.tbl_DetalleFactura d ON d.ID_DetalleF=fl.ID_DetalleF JOIN dbo.tbl_LoteInventario l ON l.ID_Lote=fl.ID_Lote WHERE d.ID_Factura=@f1 ORDER BY l.FechaIngreso,l.ID_Lote;
