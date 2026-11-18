USE [master]
GO
/****** Objeto: Database [Proyecto_BD] Fecha de script: 16/11/2026 10:06:19 ******/
IF DB_ID(N'Proyecto_BD') IS NULL
BEGIN
    CREATE DATABASE Proyecto_BD;
END;
GO

USE Proyecto_BD;
GO
/****** Objeto: Table [dbo].[tbl_Auditoria] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Auditoria](
	[ID_Auditoria] [int] IDENTITY(1,1) NOT NULL,
	[FechaHora] [datetime2](0) NOT NULL,
	[TablaAfectada] [nvarchar](45) NOT NULL,
	[RegistroAfectado] [nvarchar](60) NOT NULL,
	[Accion] [nvarchar](45) NOT NULL,
	[DatosAnteriores] [nvarchar](max) NULL,
	[DatosNuevos] [nvarchar](max) NULL,
	[ID_Usuario] [int] NOT NULL,
 CONSTRAINT [PK_tbl_Auditoria] PRIMARY KEY CLUSTERED 
(
	[ID_Auditoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_CategoriaProducto] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_CategoriaProducto](
	[ID_Categoria] [int] IDENTITY(1,1) NOT NULL,
	[NombreCategoria] [nvarchar](45) NOT NULL,
	[Descripcion] [nvarchar](60) NOT NULL,
 CONSTRAINT [PK_tbl_CategoriaProducto] PRIMARY KEY CLUSTERED 
(
	[ID_Categoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_Clientes] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Clientes](
	[ID_Cliente] [int] IDENTITY(1,1) NOT NULL,
	[TipoCliente] [nvarchar](45) NOT NULL,
	[NombreRazonSocial] [nvarchar](120) NOT NULL,
 CONSTRAINT [PK_tbl_Clientes] PRIMARY KEY CLUSTERED 
(
	[ID_Cliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_Color] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Color](
	[ID_Color] [int] IDENTITY(1,1) NOT NULL,
	[NombreColor] [nvarchar](45) NOT NULL,
 CONSTRAINT [PK_tbl_Color] PRIMARY KEY CLUSTERED 
(
	[ID_Color] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_Cotizacion] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Cotizacion](
	[ID_Cotizacion] [int] IDENTITY(1,1) NOT NULL,
	[FechaCotizacion] [date] NOT NULL,
	[Estado] [nvarchar](45) NOT NULL,
	[Total] [decimal](12, 2) NOT NULL,
	[ID_Cliente] [int] NOT NULL,
	[ID_Usuario] [int] NOT NULL,
 CONSTRAINT [PK_tbl_Cotizacion] PRIMARY KEY CLUSTERED 
(
	[ID_Cotizacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_DetalleCotizacion] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DetalleCotizacion](
	[ID_DetalleC] [int] IDENTITY(1,1) NOT NULL,
	[Cantidad] [int] NOT NULL,
	[PrecioUnitario] [decimal](12, 2) NOT NULL,
	[Subtotal] [decimal](12, 2) NOT NULL,
	[ID_Cotizacion] [int] NOT NULL,
	[ID_ProductoV] [int] NOT NULL,
 CONSTRAINT [PK_tbl_DetalleCotizacion] PRIMARY KEY CLUSTERED 
(
	[ID_DetalleC] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_DetalleDevolucionVenta] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DetalleDevolucionVenta](
	[ID_DetalleDV] [int] IDENTITY(1,1) NOT NULL,
	[CantidadDevuelta] [int] NOT NULL,
	[ID_DevolucionV] [int] NOT NULL,
	[ID_DetalleF] [int] NOT NULL,
 CONSTRAINT [PK_tbl_DetalleDevolucionVenta] PRIMARY KEY CLUSTERED 
(
	[ID_DetalleDV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_DetalleFactura] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DetalleFactura](
	[ID_DetalleF] [int] IDENTITY(1,1) NOT NULL,
	[Cantidad] [int] NOT NULL,
	[PrecioUnitario] [decimal](12, 2) NOT NULL,
	[Subtotal] [decimal](12, 2) NOT NULL,
	[ID_Factura] [int] NOT NULL,
	[ID_ProductoV] [int] NOT NULL,
 CONSTRAINT [PK_tbl_DetalleFactura] PRIMARY KEY CLUSTERED 
(
	[ID_DetalleF] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_DetalleFacturaLote] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DetalleFacturaLote](
	[ID_DetalleFL] [int] IDENTITY(1,1) NOT NULL,
	[CantidadSalida] [int] NOT NULL,
	[ID_Lote] [int] NOT NULL,
	[ID_DetalleF] [int] NOT NULL,
 CONSTRAINT [PK_tbl_DetalleFacturaLote] PRIMARY KEY CLUSTERED 
(
	[ID_DetalleFL] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_DetalleIngresoBodega] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DetalleIngresoBodega](
	[ID_DetalleIB] [int] IDENTITY(1,1) NOT NULL,
	[CantidadRecibida] [int] NOT NULL,
	[CostoUnitario] [decimal](12, 2) NOT NULL,
	[ID_IngresoB] [int] NOT NULL,
	[ID_DetalleOC] [int] NOT NULL,
 CONSTRAINT [PK_tbl_DetalleIngresoBodega] PRIMARY KEY CLUSTERED 
(
	[ID_DetalleIB] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_DetalleOrdenCompra] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DetalleOrdenCompra](
	[ID_DetalleOC] [int] IDENTITY(1,1) NOT NULL,
	[CantidadSolicitada] [int] NOT NULL,
	[ID_ProductoV] [int] NOT NULL,
	[ID_OrdenC] [int] NOT NULL,
 CONSTRAINT [PK_tbl_DetalleOrdenCompra] PRIMARY KEY CLUSTERED 
(
	[ID_DetalleOC] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_DevolucionVenta] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DevolucionVenta](
	[ID_DevolucionV] [int] IDENTITY(1,1) NOT NULL,
	[FechaDevolucion] [date] NOT NULL,
	[ID_Factura] [int] NOT NULL,
	[ID_Usuario] [int] NOT NULL,
 CONSTRAINT [PK_tbl_DevolucionVenta] PRIMARY KEY CLUSTERED 
(
	[ID_DevolucionV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_Factura] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Factura](
	[ID_Factura] [int] IDENTITY(1,1) NOT NULL,
	[NumeroFactura] [int] NOT NULL,
	[FechaFactura] [datetime2](0) NOT NULL,
	[Total] [decimal](12, 2) NOT NULL,
	[Estado] [nvarchar](45) NOT NULL,
	[ID_Cliente] [int] NOT NULL,
	[ID_Cotizacion] [int] NULL,
	[ID_Usuario] [int] NOT NULL,
 CONSTRAINT [PK_tbl_Factura] PRIMARY KEY CLUSTERED 
(
	[ID_Factura] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_IngresoBodega] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_IngresoBodega](
	[ID_IngresoB] [int] IDENTITY(1,1) NOT NULL,
	[FechaIngreso] [datetime2](0) NULL,
	[ID_OrdenC] [int] NOT NULL,
	[ID_Usuario] [int] NOT NULL,
 CONSTRAINT [PK_tbl_IngresoBodega] PRIMARY KEY CLUSTERED 
(
	[ID_IngresoB] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_LoteInventario] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_LoteInventario](
	[ID_Lote] [int] IDENTITY(1,1) NOT NULL,
	[NumeroLote] [nvarchar](50) NULL,
	[FechaIngreso] [datetime2](0) NOT NULL,
	[CantidadInicial] [int] NOT NULL,
	[CantidadDisponible] [int] NOT NULL,
	[CostoUnitario] [decimal](12, 2) NOT NULL,
	[ID_DetalleIB] [int] NOT NULL,
	[ID_ProductoV] [int] NOT NULL,
	[ID_UbicacionA] [int] NOT NULL,
 CONSTRAINT [PK_tbl_LoteInventario] PRIMARY KEY CLUSTERED 
(
	[ID_Lote] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_Marca] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Marca](
	[ID_Marca] [int] IDENTITY(1,1) NOT NULL,
	[NombreMarca] [nvarchar](45) NOT NULL,
 CONSTRAINT [PK_tbl_Marca] PRIMARY KEY CLUSTERED 
(
	[ID_Marca] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_MetodoPago] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_MetodoPago](
	[ID_MetodoP] [int] IDENTITY(1,1) NOT NULL,
	[NombreMetodo] [nvarchar](45) NOT NULL,
 CONSTRAINT [PK_tbl_MetodoPago] PRIMARY KEY CLUSTERED 
(
	[ID_MetodoP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_MovimientoInventario] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_MovimientoInventario](
	[ID_MovimientoI] [int] IDENTITY(1,1) NOT NULL,
	[TipoMovimiento] [nvarchar](60) NOT NULL,
	[Cantidad] [int] NOT NULL,
	[FechaHora] [datetime2](0) NOT NULL,
	[ID_Referencia] [int] NULL,
	[TipoReferencia] [nvarchar](45) NOT NULL,
	[ID_ProductoV] [int] NOT NULL,
	[ID_Lote] [int] NOT NULL,
	[ID_UbicacionA] [int] NOT NULL,
	[ID_Usuario] [int] NOT NULL,
 CONSTRAINT [PK_tbl_MovimientoInventario] PRIMARY KEY CLUSTERED 
(
	[ID_MovimientoI] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_OrdenCompra] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_OrdenCompra](
	[ID_OrdenC] [int] IDENTITY(1,1) NOT NULL,
	[FechaOrden] [date] NOT NULL,
	[Estado] [nvarchar](45) NOT NULL,
	[ID_Proveedor] [int] NOT NULL,
 CONSTRAINT [PK_tbl_OrdenCompra] PRIMARY KEY CLUSTERED 
(
	[ID_OrdenC] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_PagoFactura] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_PagoFactura](
	[ID_PagoF] [int] IDENTITY(1,1) NOT NULL,
	[Monto] [decimal](12, 2) NOT NULL,
	[Referencia] [nvarchar](100) NULL,
	[ID_Factura] [int] NOT NULL,
	[ID_MetodoP] [int] NOT NULL,
 CONSTRAINT [PK_tbl_PagoFactura] PRIMARY KEY CLUSTERED 
(
	[ID_PagoF] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_Presentacion] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Presentacion](
	[ID_Presentacion] [int] IDENTITY(1,1) NOT NULL,
	[NombrePresentacion] [nvarchar](45) NOT NULL,
	[UnidadMedida] [nvarchar](45) NOT NULL,
 CONSTRAINT [PK_tbl_Presentacion] PRIMARY KEY CLUSTERED 
(
	[ID_Presentacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_Producto] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Producto](
	[ID_Producto] [int] IDENTITY(1,1) NOT NULL,
	[NombreProducto] [nvarchar](45) NOT NULL,
	[Descripcion] [nvarchar](45) NOT NULL,
	[ID_CategoriaP] [int] NOT NULL,
	[ID_Marca] [int] NOT NULL,
 CONSTRAINT [PK_tbl_Producto] PRIMARY KEY CLUSTERED 
(
	[ID_Producto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_ProductoVariante] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ProductoVariante](
	[ID_ProductoV] [int] IDENTITY(1,1) NOT NULL,
	[PrecioVenta] [decimal](12, 2) NOT NULL,
	[ID_Producto] [int] NOT NULL,
	[ID_Presentacion] [int] NOT NULL,
	[ID_Color] [int] NULL,
 CONSTRAINT [PK_tbl_ProductoVariante] PRIMARY KEY CLUSTERED 
(
	[ID_ProductoV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_Proveedor] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Proveedor](
	[ID_Proveedor] [int] IDENTITY(1,1) NOT NULL,
	[Nombre_Proveedor] [nvarchar](45) NOT NULL,
 CONSTRAINT [PK_tbl_Proveedor] PRIMARY KEY CLUSTERED 
(
	[ID_Proveedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_Rol] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Rol](
	[ID_Rol] [int] IDENTITY(1,1) NOT NULL,
	[NombreRol] [nvarchar](45) NOT NULL,
 CONSTRAINT [PK_tbl_Rol] PRIMARY KEY CLUSTERED 
(
	[ID_Rol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_UbicacionAlmacen] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_UbicacionAlmacen](
	[ID_UbicacionA] [int] IDENTITY(1,1) NOT NULL,
	[NombreUbicacion] [nvarchar](45) NOT NULL,
	[Descripcion] [nvarchar](45) NOT NULL,
 CONSTRAINT [PK_tbl_UbicacionAlmacen] PRIMARY KEY CLUSTERED 
(
	[ID_UbicacionA] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[tbl_Usuario] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Usuario](
	[ID_Usuario] [int] IDENTITY(1,1) NOT NULL,
	[Nombres] [nvarchar](45) NOT NULL,
	[Apellidos] [nvarchar](45) NOT NULL,
	[NombreUsuario] [nvarchar](45) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[ID_Rol] [int] NOT NULL,
 CONSTRAINT [PK_tbl_Usuario] PRIMARY KEY CLUSTERED 
(
	[ID_Usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[tbl_Auditoria] ON 

INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (128, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'tbl_Usuario', N'53', N'ALTA USUARIO', NULL, N'{"Nombres":"Sofía","Apellidos":"Ramírez","NombreUsuario":"digitador","ID_Rol":1}', 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (129, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'tbl_Usuario', N'54', N'ALTA USUARIO', NULL, N'{"Nombres":"Luis","Apellidos":"Morales","NombreUsuario":"cajero","ID_Rol":2}', 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (130, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'tbl_Usuario', N'55', N'ALTA USUARIO', NULL, N'{"Nombres":"Daniela","Apellidos":"Gómez","NombreUsuario":"gerente","ID_Rol":3}', 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (131, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'tbl_IngresoBodega', N'30', N'INGRESO BODEGA', NULL, N'[{"ID_DetalleOC":39,"CantidadRecibida":50,"CostoUnitario":80}]', 1)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (132, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'tbl_IngresoBodega', N'31', N'INGRESO BODEGA', NULL, N'[{"ID_DetalleOC":39,"CantidadRecibida":40,"CostoUnitario":85,"NumeroLote":"PINT-BLANCO-02"}]', 1)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (133, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'tbl_IngresoBodega', N'32', N'INGRESO BODEGA', NULL, N'[{"ID_DetalleOC":40,"CantidadRecibida":25,"CostoUnitario":90,"NumeroLote":"PINT-AZUL-01"},{"ID_DetalleOC":41,"CantidadRecibida":15,"CostoUnitario":390},{"ID_DetalleOC":42,"CantidadRecibida":30,"CostoUnitario":110},{"ID_DetalleOC":43,"CantidadRecibida":20,"CostoUnitario":112},{"ID_DetalleOC":44,"CantidadRecibida":100,"CostoUnitario":20},{"ID_DetalleOC":45,"CantidadRecibida":80,"CostoUnitario":32},{"ID_DetalleOC":46,"CantidadRecibida":200,"CostoUnitario":25},{"ID_DetalleOC":47,"CantidadRecibida":500,"CostoUnitario":5},{"ID_DetalleOC":48,"CantidadRecibida":500,"CostoUnitario":5},{"ID_DetalleOC":49,"CantidadRecibida":100,"CostoUnitario":15},{"ID_DetalleOC":50,"CantidadRecibida":60,"CostoUnitario":18}]', 1)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (134, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'tbl_IngresoBodega', N'33', N'INGRESO BODEGA', NULL, N'[{"ID_DetalleOC":40,"CantidadRecibida":35,"CostoUnitario":95,"NumeroLote":"PINT-AZUL-02"}]', 1)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (135, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'tbl_IngresoBodega', N'34', N'INGRESO BODEGA', NULL, N'[{"ID_DetalleOC":51,"CantidadRecibida":75,"CostoUnitario":21},{"ID_DetalleOC":52,"CantidadRecibida":20,"CostoUnitario":33}]', 1)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (136, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'tbl_Factura', N'41', N'FACTURA', NULL, N'[{"ID_DetalleF":44,"Cantidad":60,"PrecioUnitario":120.00,"Subtotal":7200.00,"ID_Factura":41,"ID_ProductoV":29}]', 54)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (137, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'tbl_Factura', N'41', N'PAGO', NULL, N'[{"ID_MetodoP":15,"Monto":3000,"Referencia":"EFECTIVO"},{"ID_MetodoP":16,"Monto":3000,"Referencia":"TARJETA-001"},{"ID_MetodoP":17,"Monto":1200,"Referencia":"CHEQUE-001"}]', 54)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (138, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'tbl_Factura', N'42', N'FACTURA', NULL, N'[{"ID_DetalleF":45,"Cantidad":10,"PrecioUnitario":35.00,"Subtotal":350.00,"ID_Factura":42,"ID_ProductoV":34},{"ID_DetalleF":46,"Cantidad":5,"PrecioUnitario":55.00,"Subtotal":275.00,"ID_Factura":42,"ID_ProductoV":35},{"ID_DetalleF":47,"Cantidad":4,"PrecioUnitario":25.00,"Subtotal":100.00,"ID_Factura":42,"ID_ProductoV":39}]', 54)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (139, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'tbl_Factura', N'42', N'PAGO', NULL, N'[{"ID_MetodoP":15,"Monto":200.00,"Referencia":"ABONO-001"}]', 54)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (140, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'tbl_Factura', N'43', N'FACTURA', NULL, N'[{"ID_DetalleF":48,"Cantidad":20,"PrecioUnitario":8.50,"Subtotal":170.00,"ID_Factura":43,"ID_ProductoV":37},{"ID_DetalleF":49,"Cantidad":3,"PrecioUnitario":30.00,"Subtotal":90.00,"ID_Factura":43,"ID_ProductoV":40}]', 54)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (141, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), N'DATOS_EJEMPLO', N'BD2_EJEMPLO_V1', N'CARGA EJEMPLO', NULL, N'{"UsuarioDigitador":1,"UsuarioCajero":54,"UsuarioGerente":55,"OrdenCompletaPEPS":26,"OrdenCompletaStock":27,"OrdenParcial":28,"OrdenPendiente":29,"CotizacionPendiente":22,"CotizacionConfirmada":23,"CotizacionCancelada":24,"CotizacionFacturada":25,"FacturaPagadaPEPS":41,"FacturaAbono":42,"FacturaSinPagos":43,"VariantePinturaBlanca":29,"VariantePinturaAzul":30,"NombreDigitador":"admin","NombreCajero":"cajero","NombreGerente":"gerente"}', 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (142, CAST(N'2026-10-22T00:14:13.0000000' AS DateTime2), N'tbl_Usuario', N'54', N'CAMBIO DE USUARIO', N'{"ID_Usuario":54,"NombreUsuario":"cajero","ID_Rol":2}', N'{"ID_Usuario":54,"NombreUsuario":"cajero","ID_Rol":2}', 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (143, CAST(N'2026-10-22T00:14:13.0000000' AS DateTime2), N'tbl_Usuario', N'55', N'CAMBIO DE USUARIO', N'{"ID_Usuario":55,"NombreUsuario":"gerente","ID_Rol":3}', N'{"ID_Usuario":55,"NombreUsuario":"gerente","ID_Rol":3}', 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (144, CAST(N'2026-10-22T00:14:13.0000000' AS DateTime2), N'tbl_Usuario', N'53', N'REASIGNAR USUARIO', N'{"ID_Usuario":53,"Nombres":"Sofía","Apellidos":"Ramírez","NombreUsuario":"digitador","ID_Rol":1}', N'{"UsuarioDestino":1,"NombreUsuarioDestino":"admin","Ingresos":5,"Movimientos":16,"Cotizaciones":4,"Facturas":0,"Devoluciones":0,"AuditoriasConActorAnterior":[{"ID_Auditoria":131},{"ID_Auditoria":132},{"ID_Auditoria":133},{"ID_Auditoria":134},{"ID_Auditoria":135}],"Motivo":"Reasignación solicitada para eliminar la cuenta sin borrar documentos; actor anterior conservado en DatosAnteriores."}', 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (145, CAST(N'2026-10-27T08:58:01.0000000' AS DateTime2), N'tbl_Factura', N'44', N'FACTURA', NULL, N'[{"ID_DetalleF":50,"Cantidad":3,"PrecioUnitario":160.00,"Subtotal":480.00,"ID_Factura":44,"ID_ProductoV":32},{"ID_DetalleF":51,"Cantidad":4,"PrecioUnitario":40.00,"Subtotal":160.00,"ID_Factura":44,"ID_ProductoV":36}]', 54)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (146, CAST(N'2026-10-27T08:59:27.0000000' AS DateTime2), N'tbl_Factura', N'44', N'PAGO', NULL, N'[{"ID_MetodoP":15,"Metodo":"Efectivo","Monto":44.00,"Referencia":"11845"}]', 54)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (147, CAST(N'2026-10-27T09:01:47.0000000' AS DateTime2), N'tbl_Factura', N'45', N'FACTURA', NULL, N'[{"ID_DetalleF":52,"Cantidad":2,"PrecioUnitario":165.00,"Subtotal":330.00,"ID_Factura":45,"ID_ProductoV":33},{"ID_DetalleF":53,"Cantidad":4,"PrecioUnitario":35.00,"Subtotal":140.00,"ID_Factura":45,"ID_ProductoV":34},{"ID_DetalleF":54,"Cantidad":2,"PrecioUnitario":40.00,"Subtotal":80.00,"ID_Factura":45,"ID_ProductoV":36}]', 54)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (1145, CAST(N'2026-11-08T01:23:15.0000000' AS DateTime2), N'tbl_Usuario', N'56', N'CAMBIO DE USUARIO', NULL, N'{"ID_Usuario":56,"Nombres":"Usuario","Apellidos":"Prueba","NombreUsuario":"pruebaweb","ID_Rol":1}', 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (1146, CAST(N'2026-11-08T01:39:30.0000000' AS DateTime2), N'tbl_Usuario', N'56', N'CAMBIO DE USUARIO', N'{"ID_Usuario":56,"Nombres":"Usuario","Apellidos":"Prueba","NombreUsuario":"pruebaweb","ID_Rol":1}', N'{"ID_Usuario":56,"Nombres":"Usuario Editado","Apellidos":"Prueba","NombreUsuario":"pruebaweb","ID_Rol":2}', 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (1147, CAST(N'2026-11-08T01:56:02.0000000' AS DateTime2), N'tbl_Usuario', N'55', N'CAMBIO DE USUARIO', N'{"ID_Usuario":55,"Nombres":"Daniela","Apellidos":"Gómez","NombreUsuario":"gerente","ID_Rol":3}', N'{"ID_Usuario":55,"Nombres":"Daniela","Apellidos":"Gómez","NombreUsuario":"gerente","ID_Rol":3}', 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (1148, CAST(N'2026-11-08T02:04:24.0000000' AS DateTime2), N'tbl_Usuario', N'56', N'CAMBIO PASSWORD', NULL, NULL, 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (1149, CAST(N'2026-11-08T02:08:01.0000000' AS DateTime2), N'tbl_Usuario', N'56', N'ELIMINAR USUARIO', N'{"ID_Usuario":56,"Nombres":"Usuario Editado","Apellidos":"Prueba","NombreUsuario":"pruebaweb","ID_Rol":2}', NULL, 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (1153, CAST(N'2026-11-16T02:35:14.0000000' AS DateTime2), N'tbl_IngresoBodega', N'35', N'INGRESO BODEGA', NULL, N'[{"ID_DetalleOC":56,"CantidadRecibida":2,"CostoUnitario":5,"NumeroLote":"FINAL-A-1116023513"}]', 1)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (1154, CAST(N'2026-11-16T02:35:14.0000000' AS DateTime2), N'tbl_IngresoBodega', N'36', N'INGRESO BODEGA', NULL, N'[{"ID_DetalleOC":56,"CantidadRecibida":3,"CostoUnitario":6,"NumeroLote":"FINAL-B-1116023513"}]', 1)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (1155, CAST(N'2026-11-16T02:35:14.0000000' AS DateTime2), N'tbl_Factura', N'1044', N'FACTURA', NULL, N'[{"ID_DetalleF":1050,"Cantidad":4,"PrecioUnitario":10.00,"Subtotal":40.00,"ID_Factura":1044,"ID_ProductoV":41}]', 54)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (1156, CAST(N'2026-11-16T02:35:14.0000000' AS DateTime2), N'tbl_Factura', N'1044', N'PAGO', NULL, N'[{"ID_MetodoP":15,"Monto":7.00,"Referencia":"PRUEBA FINAL EFECTIVO"},{"ID_MetodoP":16,"Monto":5.00,"Referencia":"PRUEBA FINAL TARJETA"},{"ID_MetodoP":17,"Monto":3.00,"Referencia":"PRUEBA FINAL CHEQUE"},{"ID_MetodoP":18,"Monto":2.00,"Referencia":"PRUEBA FINAL TRANSFERENCIA"}]', 54)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (2157, CAST(N'2026-11-16T02:35:17.0000000' AS DateTime2), N'tbl_Factura', N'1044', N'ANULAR FACTURA', N'{"NumeroFactura":6,"Estado":"Pendiente","Total":40.00,"Lotes":[{"ID_Lote":57,"Cantidad":2,"DisponibleAntes":0,"ID_ProductoV":41,"ID_UbicacionA":18},{"ID_Lote":58,"Cantidad":2,"DisponibleAntes":1,"ID_ProductoV":41,"ID_UbicacionA":18}]}', N'{"NumeroFactura":6,"Estado":"Anulada","Total":0.00,"Lotes":[{"ID_Lote":57,"Reintegrada":2,"DisponibleDespues":2},{"ID_Lote":58,"Reintegrada":2,"DisponibleDespues":3}]}', 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (2158, CAST(N'2026-11-16T09:43:17.0000000' AS DateTime2), N'CATALOGOS', N'ETIQUETAS', N'LIMPIAR ETIQUETAS', NULL, N'[{"Tabla":"tbl_Auditoria","Columna":"Accion","CampoActualizado":true},{"Tabla":"tbl_Auditoria","Columna":"DatosAnteriores","CampoActualizado":true},{"Tabla":"tbl_Auditoria","Columna":"DatosNuevos","CampoActualizado":true},{"Tabla":"tbl_Auditoria","Columna":"RegistroAfectado","CampoActualizado":true},{"Tabla":"tbl_Auditoria","Columna":"TablaAfectada","CampoActualizado":true},{"Tabla":"tbl_CategoriaProducto","Columna":"Descripcion","CampoActualizado":true},{"Tabla":"tbl_CategoriaProducto","Columna":"NombreCategoria","CampoActualizado":true},{"Tabla":"tbl_Clientes","Columna":"NombreRazonSocial","CampoActualizado":true},{"Tabla":"tbl_Color","Columna":"NombreColor","CampoActualizado":true},{"Tabla":"tbl_LoteInventario","Columna":"NumeroLote","CampoActualizado":true},{"Tabla":"tbl_Marca","Columna":"NombreMarca","CampoActualizado":true},{"Tabla":"tbl_MetodoPago","Columna":"NombreMetodo","CampoActualizado":true},{"Tabla":"tbl_PagoFactura","Columna":"Referencia","CampoActualizado":true},{"Tabla":"tbl_Presentacion","Columna":"NombrePresentacion","CampoActualizado":true},{"Tabla":"tbl_Producto","Columna":"NombreProducto","CampoActualizado":true},{"Tabla":"tbl_Proveedor","Columna":"Nombre_Proveedor","CampoActualizado":true},{"Tabla":"tbl_UbicacionAlmacen","Columna":"Descripcion","CampoActualizado":true},{"Tabla":"tbl_UbicacionAlmacen","Columna":"NombreUbicacion","CampoActualizado":true}]', 55)
INSERT [dbo].[tbl_Auditoria] ([ID_Auditoria], [FechaHora], [TablaAfectada], [RegistroAfectado], [Accion], [DatosAnteriores], [DatosNuevos], [ID_Usuario]) VALUES (2159, CAST(N'2026-11-16T09:55:33.0000000' AS DateTime2), N'tbl_Factura', N'41', N'ANULAR FACTURA', N'{"NumeroFactura":1,"Estado":"Pagada","Total":7200.00,"Lotes":[{"ID_Lote":41,"Cantidad":50,"DisponibleAntes":0,"ID_ProductoV":29,"ID_UbicacionA":18},{"ID_Lote":42,"Cantidad":10,"DisponibleAntes":30,"ID_ProductoV":29,"ID_UbicacionA":18}]}', N'{"NumeroFactura":1,"Estado":"Anulada","Total":0.00,"Lotes":[{"ID_Lote":41,"Reintegrada":50,"DisponibleDespues":50},{"ID_Lote":42,"Reintegrada":10,"DisponibleDespues":40}]}', 55)
SET IDENTITY_INSERT [dbo].[tbl_Auditoria] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_CategoriaProducto] ON 

INSERT [dbo].[tbl_CategoriaProducto] ([ID_Categoria], [NombreCategoria], [Descripcion]) VALUES (21, N'Pinturas', N'Catálogo de ejemplo: Pinturas')
INSERT [dbo].[tbl_CategoriaProducto] ([ID_Categoria], [NombreCategoria], [Descripcion]) VALUES (22, N'Ferretería', N'Catálogo de ejemplo: Ferretería')
INSERT [dbo].[tbl_CategoriaProducto] ([ID_Categoria], [NombreCategoria], [Descripcion]) VALUES (23, N'Material eléctrico', N'Catálogo de ejemplo: Material eléctrico')
INSERT [dbo].[tbl_CategoriaProducto] ([ID_Categoria], [NombreCategoria], [Descripcion]) VALUES (24, N'Plomería', N'Catálogo de ejemplo: Plomería')
SET IDENTITY_INSERT [dbo].[tbl_CategoriaProducto] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Clientes] ON 

INSERT [dbo].[tbl_Clientes] ([ID_Cliente], [TipoCliente], [NombreRazonSocial]) VALUES (23, N'Particular', N'Ana López')
INSERT [dbo].[tbl_Clientes] ([ID_Cliente], [TipoCliente], [NombreRazonSocial]) VALUES (24, N'Particular', N'Carlos Méndez')
INSERT [dbo].[tbl_Clientes] ([ID_Cliente], [TipoCliente], [NombreRazonSocial]) VALUES (25, N'Particular', N'Marta Pérez')
INSERT [dbo].[tbl_Clientes] ([ID_Cliente], [TipoCliente], [NombreRazonSocial]) VALUES (26, N'Empresa', N'Constructora Los Pinos')
INSERT [dbo].[tbl_Clientes] ([ID_Cliente], [TipoCliente], [NombreRazonSocial]) VALUES (27, N'Empresa', N'Ferretería El Progreso')
INSERT [dbo].[tbl_Clientes] ([ID_Cliente], [TipoCliente], [NombreRazonSocial]) VALUES (28, N'Empresa', N'Inversiones El Roble')
SET IDENTITY_INSERT [dbo].[tbl_Clientes] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Color] ON 

INSERT [dbo].[tbl_Color] ([ID_Color], [NombreColor]) VALUES (6, N'Gris')
INSERT [dbo].[tbl_Color] ([ID_Color], [NombreColor]) VALUES (15, N'Blanco')
INSERT [dbo].[tbl_Color] ([ID_Color], [NombreColor]) VALUES (16, N'Azul')
INSERT [dbo].[tbl_Color] ([ID_Color], [NombreColor]) VALUES (17, N'Negro')
INSERT [dbo].[tbl_Color] ([ID_Color], [NombreColor]) VALUES (18, N'Rojo')
SET IDENTITY_INSERT [dbo].[tbl_Color] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Cotizacion] ON 

INSERT [dbo].[tbl_Cotizacion] ([ID_Cotizacion], [FechaCotizacion], [Estado], [Total], [ID_Cliente], [ID_Usuario]) VALUES (22, CAST(N'2026-10-21' AS Date), N'Pendiente', CAST(285.00 AS Decimal(12, 2)), 24, 1)
INSERT [dbo].[tbl_Cotizacion] ([ID_Cotizacion], [FechaCotizacion], [Estado], [Total], [ID_Cliente], [ID_Usuario]) VALUES (23, CAST(N'2026-10-21' AS Date), N'Facturada', CAST(640.00 AS Decimal(12, 2)), 25, 1)
INSERT [dbo].[tbl_Cotizacion] ([ID_Cotizacion], [FechaCotizacion], [Estado], [Total], [ID_Cliente], [ID_Usuario]) VALUES (24, CAST(N'2026-10-21' AS Date), N'Cancelada', CAST(1100.00 AS Decimal(12, 2)), 26, 1)
INSERT [dbo].[tbl_Cotizacion] ([ID_Cotizacion], [FechaCotizacion], [Estado], [Total], [ID_Cliente], [ID_Usuario]) VALUES (25, CAST(N'2026-10-21' AS Date), N'Facturada', CAST(725.00 AS Decimal(12, 2)), 27, 1)
SET IDENTITY_INSERT [dbo].[tbl_Cotizacion] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_DetalleCotizacion] ON 

INSERT [dbo].[tbl_DetalleCotizacion] ([ID_DetalleC], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Cotizacion], [ID_ProductoV]) VALUES (26, 5, CAST(35.00 AS Decimal(12, 2)), CAST(175.00 AS Decimal(12, 2)), 22, 34)
INSERT [dbo].[tbl_DetalleCotizacion] ([ID_DetalleC], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Cotizacion], [ID_ProductoV]) VALUES (27, 2, CAST(55.00 AS Decimal(12, 2)), CAST(110.00 AS Decimal(12, 2)), 22, 35)
INSERT [dbo].[tbl_DetalleCotizacion] ([ID_DetalleC], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Cotizacion], [ID_ProductoV]) VALUES (28, 3, CAST(160.00 AS Decimal(12, 2)), CAST(480.00 AS Decimal(12, 2)), 23, 32)
INSERT [dbo].[tbl_DetalleCotizacion] ([ID_DetalleC], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Cotizacion], [ID_ProductoV]) VALUES (29, 4, CAST(40.00 AS Decimal(12, 2)), CAST(160.00 AS Decimal(12, 2)), 23, 36)
INSERT [dbo].[tbl_DetalleCotizacion] ([ID_DetalleC], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Cotizacion], [ID_ProductoV]) VALUES (30, 2, CAST(550.00 AS Decimal(12, 2)), CAST(1100.00 AS Decimal(12, 2)), 24, 31)
INSERT [dbo].[tbl_DetalleCotizacion] ([ID_DetalleC], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Cotizacion], [ID_ProductoV]) VALUES (31, 10, CAST(35.00 AS Decimal(12, 2)), CAST(350.00 AS Decimal(12, 2)), 25, 34)
INSERT [dbo].[tbl_DetalleCotizacion] ([ID_DetalleC], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Cotizacion], [ID_ProductoV]) VALUES (32, 5, CAST(55.00 AS Decimal(12, 2)), CAST(275.00 AS Decimal(12, 2)), 25, 35)
INSERT [dbo].[tbl_DetalleCotizacion] ([ID_DetalleC], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Cotizacion], [ID_ProductoV]) VALUES (33, 4, CAST(25.00 AS Decimal(12, 2)), CAST(100.00 AS Decimal(12, 2)), 25, 39)
SET IDENTITY_INSERT [dbo].[tbl_DetalleCotizacion] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_DetalleFactura] ON 

INSERT [dbo].[tbl_DetalleFactura] ([ID_DetalleF], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Factura], [ID_ProductoV]) VALUES (44, 60, CAST(120.00 AS Decimal(12, 2)), CAST(7200.00 AS Decimal(12, 2)), 41, 29)
INSERT [dbo].[tbl_DetalleFactura] ([ID_DetalleF], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Factura], [ID_ProductoV]) VALUES (45, 10, CAST(35.00 AS Decimal(12, 2)), CAST(350.00 AS Decimal(12, 2)), 42, 34)
INSERT [dbo].[tbl_DetalleFactura] ([ID_DetalleF], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Factura], [ID_ProductoV]) VALUES (46, 5, CAST(55.00 AS Decimal(12, 2)), CAST(275.00 AS Decimal(12, 2)), 42, 35)
INSERT [dbo].[tbl_DetalleFactura] ([ID_DetalleF], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Factura], [ID_ProductoV]) VALUES (47, 4, CAST(25.00 AS Decimal(12, 2)), CAST(100.00 AS Decimal(12, 2)), 42, 39)
INSERT [dbo].[tbl_DetalleFactura] ([ID_DetalleF], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Factura], [ID_ProductoV]) VALUES (48, 20, CAST(8.50 AS Decimal(12, 2)), CAST(170.00 AS Decimal(12, 2)), 43, 37)
INSERT [dbo].[tbl_DetalleFactura] ([ID_DetalleF], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Factura], [ID_ProductoV]) VALUES (49, 3, CAST(30.00 AS Decimal(12, 2)), CAST(90.00 AS Decimal(12, 2)), 43, 40)
INSERT [dbo].[tbl_DetalleFactura] ([ID_DetalleF], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Factura], [ID_ProductoV]) VALUES (50, 3, CAST(160.00 AS Decimal(12, 2)), CAST(480.00 AS Decimal(12, 2)), 44, 32)
INSERT [dbo].[tbl_DetalleFactura] ([ID_DetalleF], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Factura], [ID_ProductoV]) VALUES (51, 4, CAST(40.00 AS Decimal(12, 2)), CAST(160.00 AS Decimal(12, 2)), 44, 36)
INSERT [dbo].[tbl_DetalleFactura] ([ID_DetalleF], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Factura], [ID_ProductoV]) VALUES (52, 2, CAST(165.00 AS Decimal(12, 2)), CAST(330.00 AS Decimal(12, 2)), 45, 33)
INSERT [dbo].[tbl_DetalleFactura] ([ID_DetalleF], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Factura], [ID_ProductoV]) VALUES (53, 4, CAST(35.00 AS Decimal(12, 2)), CAST(140.00 AS Decimal(12, 2)), 45, 34)
INSERT [dbo].[tbl_DetalleFactura] ([ID_DetalleF], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Factura], [ID_ProductoV]) VALUES (54, 2, CAST(40.00 AS Decimal(12, 2)), CAST(80.00 AS Decimal(12, 2)), 45, 36)
INSERT [dbo].[tbl_DetalleFactura] ([ID_DetalleF], [Cantidad], [PrecioUnitario], [Subtotal], [ID_Factura], [ID_ProductoV]) VALUES (1050, 4, CAST(10.00 AS Decimal(12, 2)), CAST(40.00 AS Decimal(12, 2)), 1044, 41)
SET IDENTITY_INSERT [dbo].[tbl_DetalleFactura] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_DetalleFacturaLote] ON 

INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (54, 50, 41, 44)
INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (55, 10, 42, 44)
INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (56, 10, 47, 45)
INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (57, 5, 48, 46)
INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (58, 4, 52, 47)
INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (59, 20, 50, 48)
INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (60, 3, 53, 49)
INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (61, 3, 45, 50)
INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (62, 4, 49, 51)
INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (63, 2, 46, 52)
INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (64, 4, 47, 53)
INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (65, 2, 49, 54)
INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (1061, 2, 57, 1050)
INSERT [dbo].[tbl_DetalleFacturaLote] ([ID_DetalleFL], [CantidadSalida], [ID_Lote], [ID_DetalleF]) VALUES (1062, 2, 58, 1050)
SET IDENTITY_INSERT [dbo].[tbl_DetalleFacturaLote] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_DetalleIngresoBodega] ON 

INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (41, 50, CAST(80.00 AS Decimal(12, 2)), 30, 39)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (42, 40, CAST(85.00 AS Decimal(12, 2)), 31, 39)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (43, 25, CAST(90.00 AS Decimal(12, 2)), 32, 40)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (44, 15, CAST(390.00 AS Decimal(12, 2)), 32, 41)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (45, 30, CAST(110.00 AS Decimal(12, 2)), 32, 42)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (46, 20, CAST(112.00 AS Decimal(12, 2)), 32, 43)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (47, 100, CAST(20.00 AS Decimal(12, 2)), 32, 44)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (48, 80, CAST(32.00 AS Decimal(12, 2)), 32, 45)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (49, 200, CAST(25.00 AS Decimal(12, 2)), 32, 46)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (50, 500, CAST(5.00 AS Decimal(12, 2)), 32, 47)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (51, 500, CAST(5.00 AS Decimal(12, 2)), 32, 48)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (52, 100, CAST(15.00 AS Decimal(12, 2)), 32, 49)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (53, 60, CAST(18.00 AS Decimal(12, 2)), 32, 50)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (54, 35, CAST(95.00 AS Decimal(12, 2)), 33, 40)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (55, 75, CAST(21.00 AS Decimal(12, 2)), 34, 51)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (56, 20, CAST(33.00 AS Decimal(12, 2)), 34, 52)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (57, 2, CAST(5.00 AS Decimal(12, 2)), 35, 56)
INSERT [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB], [CantidadRecibida], [CostoUnitario], [ID_IngresoB], [ID_DetalleOC]) VALUES (58, 3, CAST(6.00 AS Decimal(12, 2)), 36, 56)
SET IDENTITY_INSERT [dbo].[tbl_DetalleIngresoBodega] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_DetalleOrdenCompra] ON 

INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (39, 90, 29, 26)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (40, 60, 30, 27)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (41, 15, 31, 27)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (42, 30, 32, 27)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (43, 20, 33, 27)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (44, 100, 34, 27)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (45, 80, 35, 27)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (46, 200, 36, 27)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (47, 500, 37, 27)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (48, 500, 38, 27)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (49, 100, 39, 27)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (50, 60, 40, 27)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (51, 100, 34, 28)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (52, 50, 35, 28)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (53, 300, 37, 29)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (54, 60, 39, 29)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (55, 80, 40, 29)
INSERT [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC], [CantidadSolicitada], [ID_ProductoV], [ID_OrdenC]) VALUES (56, 5, 41, 30)
SET IDENTITY_INSERT [dbo].[tbl_DetalleOrdenCompra] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Factura] ON 

INSERT [dbo].[tbl_Factura] ([ID_Factura], [NumeroFactura], [FechaFactura], [Total], [Estado], [ID_Cliente], [ID_Cotizacion], [ID_Usuario]) VALUES (41, 1, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), CAST(0.00 AS Decimal(12, 2)), N'Anulada', 23, NULL, 54)
INSERT [dbo].[tbl_Factura] ([ID_Factura], [NumeroFactura], [FechaFactura], [Total], [Estado], [ID_Cliente], [ID_Cotizacion], [ID_Usuario]) VALUES (42, 2, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), CAST(725.00 AS Decimal(12, 2)), N'Pendiente', 27, 25, 54)
INSERT [dbo].[tbl_Factura] ([ID_Factura], [NumeroFactura], [FechaFactura], [Total], [Estado], [ID_Cliente], [ID_Cotizacion], [ID_Usuario]) VALUES (43, 3, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), CAST(260.00 AS Decimal(12, 2)), N'Pendiente', 28, NULL, 54)
INSERT [dbo].[tbl_Factura] ([ID_Factura], [NumeroFactura], [FechaFactura], [Total], [Estado], [ID_Cliente], [ID_Cotizacion], [ID_Usuario]) VALUES (44, 4, CAST(N'2026-10-27T08:58:01.0000000' AS DateTime2), CAST(640.00 AS Decimal(12, 2)), N'Pendiente', 25, 23, 54)
INSERT [dbo].[tbl_Factura] ([ID_Factura], [NumeroFactura], [FechaFactura], [Total], [Estado], [ID_Cliente], [ID_Cotizacion], [ID_Usuario]) VALUES (45, 5, CAST(N'2026-10-27T09:01:47.0000000' AS DateTime2), CAST(550.00 AS Decimal(12, 2)), N'Pendiente', 27, NULL, 54)
INSERT [dbo].[tbl_Factura] ([ID_Factura], [NumeroFactura], [FechaFactura], [Total], [Estado], [ID_Cliente], [ID_Cotizacion], [ID_Usuario]) VALUES (1044, 6, CAST(N'2026-11-16T02:35:14.0000000' AS DateTime2), CAST(0.00 AS Decimal(12, 2)), N'Anulada', 23, NULL, 54)
SET IDENTITY_INSERT [dbo].[tbl_Factura] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_IngresoBodega] ON 

INSERT [dbo].[tbl_IngresoBodega] ([ID_IngresoB], [FechaIngreso], [ID_OrdenC], [ID_Usuario]) VALUES (30, CAST(N'2026-10-01T23:59:55.0000000' AS DateTime2), 26, 1)
INSERT [dbo].[tbl_IngresoBodega] ([ID_IngresoB], [FechaIngreso], [ID_OrdenC], [ID_Usuario]) VALUES (31, CAST(N'2026-10-11T23:59:55.0000000' AS DateTime2), 26, 1)
INSERT [dbo].[tbl_IngresoBodega] ([ID_IngresoB], [FechaIngreso], [ID_OrdenC], [ID_Usuario]) VALUES (32, CAST(N'2026-10-05T23:59:55.0000000' AS DateTime2), 27, 1)
INSERT [dbo].[tbl_IngresoBodega] ([ID_IngresoB], [FechaIngreso], [ID_OrdenC], [ID_Usuario]) VALUES (33, CAST(N'2026-10-15T23:59:55.0000000' AS DateTime2), 27, 1)
INSERT [dbo].[tbl_IngresoBodega] ([ID_IngresoB], [FechaIngreso], [ID_OrdenC], [ID_Usuario]) VALUES (34, CAST(N'2026-10-18T23:59:55.0000000' AS DateTime2), 28, 1)
INSERT [dbo].[tbl_IngresoBodega] ([ID_IngresoB], [FechaIngreso], [ID_OrdenC], [ID_Usuario]) VALUES (35, CAST(N'2026-11-14T02:35:13.0000000' AS DateTime2), 30, 1)
INSERT [dbo].[tbl_IngresoBodega] ([ID_IngresoB], [FechaIngreso], [ID_OrdenC], [ID_Usuario]) VALUES (36, CAST(N'2026-11-15T02:35:14.0000000' AS DateTime2), 30, 1)
SET IDENTITY_INSERT [dbo].[tbl_IngresoBodega] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_LoteInventario] ON 

INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (41, NULL, CAST(N'2026-10-01T23:59:55.0000000' AS DateTime2), 50, 50, CAST(80.00 AS Decimal(12, 2)), 41, 29, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (42, N'PINT-BLANCO-02', CAST(N'2026-10-11T23:59:55.0000000' AS DateTime2), 40, 40, CAST(85.00 AS Decimal(12, 2)), 42, 29, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (43, N'PINT-AZUL-01', CAST(N'2026-10-05T23:59:55.0000000' AS DateTime2), 25, 25, CAST(90.00 AS Decimal(12, 2)), 43, 30, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (44, NULL, CAST(N'2026-10-05T23:59:55.0000000' AS DateTime2), 15, 15, CAST(390.00 AS Decimal(12, 2)), 44, 31, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (45, NULL, CAST(N'2026-10-05T23:59:55.0000000' AS DateTime2), 30, 27, CAST(110.00 AS Decimal(12, 2)), 45, 32, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (46, NULL, CAST(N'2026-10-05T23:59:55.0000000' AS DateTime2), 20, 18, CAST(112.00 AS Decimal(12, 2)), 46, 33, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (47, NULL, CAST(N'2026-10-05T23:59:55.0000000' AS DateTime2), 100, 86, CAST(20.00 AS Decimal(12, 2)), 47, 34, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (48, NULL, CAST(N'2026-10-05T23:59:55.0000000' AS DateTime2), 80, 75, CAST(32.00 AS Decimal(12, 2)), 48, 35, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (49, NULL, CAST(N'2026-10-05T23:59:55.0000000' AS DateTime2), 200, 194, CAST(25.00 AS Decimal(12, 2)), 49, 36, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (50, NULL, CAST(N'2026-10-05T23:59:55.0000000' AS DateTime2), 500, 480, CAST(5.00 AS Decimal(12, 2)), 50, 37, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (51, NULL, CAST(N'2026-10-05T23:59:55.0000000' AS DateTime2), 500, 500, CAST(5.00 AS Decimal(12, 2)), 51, 38, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (52, NULL, CAST(N'2026-10-05T23:59:55.0000000' AS DateTime2), 100, 96, CAST(15.00 AS Decimal(12, 2)), 52, 39, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (53, NULL, CAST(N'2026-10-05T23:59:55.0000000' AS DateTime2), 60, 57, CAST(18.00 AS Decimal(12, 2)), 53, 40, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (54, N'PINT-AZUL-02', CAST(N'2026-10-15T23:59:55.0000000' AS DateTime2), 35, 35, CAST(95.00 AS Decimal(12, 2)), 54, 30, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (55, NULL, CAST(N'2026-10-18T23:59:55.0000000' AS DateTime2), 75, 75, CAST(21.00 AS Decimal(12, 2)), 55, 34, 19)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (56, NULL, CAST(N'2026-10-18T23:59:55.0000000' AS DateTime2), 20, 20, CAST(33.00 AS Decimal(12, 2)), 56, 35, 19)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (57, N'FINAL-A-1116023513', CAST(N'2026-11-14T02:35:13.0000000' AS DateTime2), 2, 2, CAST(5.00 AS Decimal(12, 2)), 57, 41, 18)
INSERT [dbo].[tbl_LoteInventario] ([ID_Lote], [NumeroLote], [FechaIngreso], [CantidadInicial], [CantidadDisponible], [CostoUnitario], [ID_DetalleIB], [ID_ProductoV], [ID_UbicacionA]) VALUES (58, N'FINAL-B-1116023513', CAST(N'2026-11-15T02:35:14.0000000' AS DateTime2), 3, 3, CAST(6.00 AS Decimal(12, 2)), 58, 41, 18)
SET IDENTITY_INSERT [dbo].[tbl_LoteInventario] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Marca] ON 

INSERT [dbo].[tbl_Marca] ([ID_Marca], [NombreMarca]) VALUES (20, N'ColorGuate')
INSERT [dbo].[tbl_Marca] ([ID_Marca], [NombreMarca]) VALUES (21, N'Construmax')
INSERT [dbo].[tbl_Marca] ([ID_Marca], [NombreMarca]) VALUES (22, N'ElectroGT')
SET IDENTITY_INSERT [dbo].[tbl_Marca] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_MetodoPago] ON 

INSERT [dbo].[tbl_MetodoPago] ([ID_MetodoP], [NombreMetodo]) VALUES (17, N'Cheque')
INSERT [dbo].[tbl_MetodoPago] ([ID_MetodoP], [NombreMetodo]) VALUES (15, N'Efectivo')
INSERT [dbo].[tbl_MetodoPago] ([ID_MetodoP], [NombreMetodo]) VALUES (16, N'Tarjeta')
INSERT [dbo].[tbl_MetodoPago] ([ID_MetodoP], [NombreMetodo]) VALUES (18, N'Transferencia (prueba final)')
SET IDENTITY_INSERT [dbo].[tbl_MetodoPago] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_MovimientoInventario] ON 

INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (94, N'ENTRADA_COMPRA', 50, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 30, N'INGRESO BODEGA', 29, 41, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (95, N'ENTRADA_COMPRA', 40, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 31, N'INGRESO BODEGA', 29, 42, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (96, N'ENTRADA_COMPRA', 25, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 32, N'INGRESO BODEGA', 30, 43, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (97, N'ENTRADA_COMPRA', 15, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 32, N'INGRESO BODEGA', 31, 44, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (98, N'ENTRADA_COMPRA', 30, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 32, N'INGRESO BODEGA', 32, 45, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (99, N'ENTRADA_COMPRA', 20, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 32, N'INGRESO BODEGA', 33, 46, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (100, N'ENTRADA_COMPRA', 100, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 32, N'INGRESO BODEGA', 34, 47, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (101, N'ENTRADA_COMPRA', 80, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 32, N'INGRESO BODEGA', 35, 48, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (102, N'ENTRADA_COMPRA', 200, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 32, N'INGRESO BODEGA', 36, 49, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (103, N'ENTRADA_COMPRA', 500, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 32, N'INGRESO BODEGA', 37, 50, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (104, N'ENTRADA_COMPRA', 500, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 32, N'INGRESO BODEGA', 38, 51, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (105, N'ENTRADA_COMPRA', 100, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 32, N'INGRESO BODEGA', 39, 52, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (106, N'ENTRADA_COMPRA', 60, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 32, N'INGRESO BODEGA', 40, 53, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (107, N'ENTRADA_COMPRA', 35, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 33, N'INGRESO BODEGA', 30, 54, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (108, N'ENTRADA_COMPRA', 75, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 34, N'INGRESO BODEGA', 34, 55, 19, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (109, N'ENTRADA_COMPRA', 20, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 34, N'INGRESO BODEGA', 35, 56, 19, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (110, N'SALIDA_VENTA', 50, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 41, N'FACTURA', 29, 41, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (111, N'SALIDA_VENTA', 10, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 41, N'FACTURA', 29, 42, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (112, N'SALIDA_VENTA', 10, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 42, N'FACTURA', 34, 47, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (113, N'SALIDA_VENTA', 5, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 42, N'FACTURA', 35, 48, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (114, N'SALIDA_VENTA', 4, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 42, N'FACTURA', 39, 52, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (115, N'SALIDA_VENTA', 20, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 43, N'FACTURA', 37, 50, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (116, N'SALIDA_VENTA', 3, CAST(N'2026-10-21T23:59:55.0000000' AS DateTime2), 43, N'FACTURA', 40, 53, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (117, N'SALIDA_VENTA', 3, CAST(N'2026-10-27T08:58:01.0000000' AS DateTime2), 44, N'FACTURA', 32, 45, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (118, N'SALIDA_VENTA', 4, CAST(N'2026-10-27T08:58:01.0000000' AS DateTime2), 44, N'FACTURA', 36, 49, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (119, N'SALIDA_VENTA', 2, CAST(N'2026-10-27T09:01:47.0000000' AS DateTime2), 45, N'FACTURA', 33, 46, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (120, N'SALIDA_VENTA', 4, CAST(N'2026-10-27T09:01:47.0000000' AS DateTime2), 45, N'FACTURA', 34, 47, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (121, N'SALIDA_VENTA', 2, CAST(N'2026-10-27T09:01:47.0000000' AS DateTime2), 45, N'FACTURA', 36, 49, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (1117, N'ENTRADA_COMPRA', 2, CAST(N'2026-11-16T02:35:14.0000000' AS DateTime2), 35, N'INGRESO BODEGA', 41, 57, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (1118, N'ENTRADA_COMPRA', 3, CAST(N'2026-11-16T02:35:14.0000000' AS DateTime2), 36, N'INGRESO BODEGA', 41, 58, 18, 1)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (1119, N'SALIDA_VENTA', 2, CAST(N'2026-11-16T02:35:14.0000000' AS DateTime2), 1044, N'FACTURA', 41, 57, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (1120, N'SALIDA_VENTA', 2, CAST(N'2026-11-16T02:35:14.0000000' AS DateTime2), 1044, N'FACTURA', 41, 58, 18, 54)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (1123, N'ENTRADA_ANULACION', 2, CAST(N'2026-11-16T02:35:17.0000000' AS DateTime2), 1044, N'FACTURA', 41, 57, 18, 55)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (1124, N'ENTRADA_ANULACION', 2, CAST(N'2026-11-16T02:35:17.0000000' AS DateTime2), 1044, N'FACTURA', 41, 58, 18, 55)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (1125, N'ENTRADA_ANULACION', 50, CAST(N'2026-11-16T09:55:33.0000000' AS DateTime2), 41, N'FACTURA', 29, 41, 18, 55)
INSERT [dbo].[tbl_MovimientoInventario] ([ID_MovimientoI], [TipoMovimiento], [Cantidad], [FechaHora], [ID_Referencia], [TipoReferencia], [ID_ProductoV], [ID_Lote], [ID_UbicacionA], [ID_Usuario]) VALUES (1126, N'ENTRADA_ANULACION', 10, CAST(N'2026-11-16T09:55:33.0000000' AS DateTime2), 41, N'FACTURA', 29, 42, 18, 55)
SET IDENTITY_INSERT [dbo].[tbl_MovimientoInventario] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_OrdenCompra] ON 

INSERT [dbo].[tbl_OrdenCompra] ([ID_OrdenC], [FechaOrden], [Estado], [ID_Proveedor]) VALUES (26, CAST(N'2026-09-26' AS Date), N'Completa', 20)
INSERT [dbo].[tbl_OrdenCompra] ([ID_OrdenC], [FechaOrden], [Estado], [ID_Proveedor]) VALUES (27, CAST(N'2026-09-28' AS Date), N'Completa', 20)
INSERT [dbo].[tbl_OrdenCompra] ([ID_OrdenC], [FechaOrden], [Estado], [ID_Proveedor]) VALUES (28, CAST(N'2026-10-13' AS Date), N'Parcial', 21)
INSERT [dbo].[tbl_OrdenCompra] ([ID_OrdenC], [FechaOrden], [Estado], [ID_Proveedor]) VALUES (29, CAST(N'2026-10-20' AS Date), N'Pendiente', 22)
INSERT [dbo].[tbl_OrdenCompra] ([ID_OrdenC], [FechaOrden], [Estado], [ID_Proveedor]) VALUES (30, CAST(N'2026-11-16' AS Date), N'Completa', 20)
SET IDENTITY_INSERT [dbo].[tbl_OrdenCompra] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_PagoFactura] ON 

INSERT [dbo].[tbl_PagoFactura] ([ID_PagoF], [Monto], [Referencia], [ID_Factura], [ID_MetodoP]) VALUES (32, CAST(3000.00 AS Decimal(12, 2)), N'EFECTIVO', 41, 15)
INSERT [dbo].[tbl_PagoFactura] ([ID_PagoF], [Monto], [Referencia], [ID_Factura], [ID_MetodoP]) VALUES (33, CAST(3000.00 AS Decimal(12, 2)), N'TARJETA-001', 41, 16)
INSERT [dbo].[tbl_PagoFactura] ([ID_PagoF], [Monto], [Referencia], [ID_Factura], [ID_MetodoP]) VALUES (34, CAST(1200.00 AS Decimal(12, 2)), N'CHEQUE-001', 41, 17)
INSERT [dbo].[tbl_PagoFactura] ([ID_PagoF], [Monto], [Referencia], [ID_Factura], [ID_MetodoP]) VALUES (35, CAST(200.00 AS Decimal(12, 2)), N'ABONO-001', 42, 15)
INSERT [dbo].[tbl_PagoFactura] ([ID_PagoF], [Monto], [Referencia], [ID_Factura], [ID_MetodoP]) VALUES (36, CAST(44.00 AS Decimal(12, 2)), N'11845', 44, 15)
INSERT [dbo].[tbl_PagoFactura] ([ID_PagoF], [Monto], [Referencia], [ID_Factura], [ID_MetodoP]) VALUES (1036, CAST(7.00 AS Decimal(12, 2)), N'PRUEBA FINAL EFECTIVO', 1044, 15)
INSERT [dbo].[tbl_PagoFactura] ([ID_PagoF], [Monto], [Referencia], [ID_Factura], [ID_MetodoP]) VALUES (1037, CAST(5.00 AS Decimal(12, 2)), N'PRUEBA FINAL TARJETA', 1044, 16)
INSERT [dbo].[tbl_PagoFactura] ([ID_PagoF], [Monto], [Referencia], [ID_Factura], [ID_MetodoP]) VALUES (1038, CAST(3.00 AS Decimal(12, 2)), N'PRUEBA FINAL CHEQUE', 1044, 17)
INSERT [dbo].[tbl_PagoFactura] ([ID_PagoF], [Monto], [Referencia], [ID_Factura], [ID_MetodoP]) VALUES (1039, CAST(2.00 AS Decimal(12, 2)), N'PRUEBA FINAL TRANSFERENCIA', 1044, 18)
SET IDENTITY_INSERT [dbo].[tbl_PagoFactura] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Presentacion] ON 

INSERT [dbo].[tbl_Presentacion] ([ID_Presentacion], [NombrePresentacion], [UnidadMedida]) VALUES (21, N'Galón', N'gal')
INSERT [dbo].[tbl_Presentacion] ([ID_Presentacion], [NombrePresentacion], [UnidadMedida]) VALUES (22, N'Cubeta', N'5 gal')
INSERT [dbo].[tbl_Presentacion] ([ID_Presentacion], [NombrePresentacion], [UnidadMedida]) VALUES (23, N'Unidad', N'unidad')
INSERT [dbo].[tbl_Presentacion] ([ID_Presentacion], [NombrePresentacion], [UnidadMedida]) VALUES (24, N'Caja', N'100 unidades')
SET IDENTITY_INSERT [dbo].[tbl_Presentacion] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Producto] ON 

INSERT [dbo].[tbl_Producto] ([ID_Producto], [NombreProducto], [Descripcion], [ID_CategoriaP], [ID_Marca]) VALUES (25, N'Pintura látex interior', N'Pintura para paredes interiores', 21, 20)
INSERT [dbo].[tbl_Producto] ([ID_Producto], [NombreProducto], [Descripcion], [ID_CategoriaP], [ID_Marca]) VALUES (26, N'Esmalte anticorrosivo', N'Protección y acabado de metal', 21, 20)
INSERT [dbo].[tbl_Producto] ([ID_Producto], [NombreProducto], [Descripcion], [ID_CategoriaP], [ID_Marca]) VALUES (27, N'Brocha profesional', N'Brocha de cerdas de tres pulgadas', 22, 21)
INSERT [dbo].[tbl_Producto] ([ID_Producto], [NombreProducto], [Descripcion], [ID_CategoriaP], [ID_Marca]) VALUES (28, N'Rodillo para pintar', N'Rodillo de nueve pulgadas', 22, 21)
INSERT [dbo].[tbl_Producto] ([ID_Producto], [NombreProducto], [Descripcion], [ID_CategoriaP], [ID_Marca]) VALUES (29, N'Tornillo para madera', N'Caja de tornillos de una pulgada', 22, 21)
INSERT [dbo].[tbl_Producto] ([ID_Producto], [NombreProducto], [Descripcion], [ID_CategoriaP], [ID_Marca]) VALUES (30, N'Cable eléctrico THHN', N'Cable eléctrico calibre doce por metro', 23, 22)
INSERT [dbo].[tbl_Producto] ([ID_Producto], [NombreProducto], [Descripcion], [ID_CategoriaP], [ID_Marca]) VALUES (31, N'Tomacorriente doble', N'Tomacorriente doble polarizado', 23, 22)
INSERT [dbo].[tbl_Producto] ([ID_Producto], [NombreProducto], [Descripcion], [ID_CategoriaP], [ID_Marca]) VALUES (32, N'Tubo PVC de media pulgada', N'Tubo PVC de tres metros', 24, 21)
INSERT [dbo].[tbl_Producto] ([ID_Producto], [NombreProducto], [Descripcion], [ID_CategoriaP], [ID_Marca]) VALUES (33, N'Prueba final 1116023513', N'Caso PEPS y anulación', 21, 20)
INSERT [dbo].[tbl_Producto] ([ID_Producto], [NombreProducto], [Descripcion], [ID_CategoriaP], [ID_Marca]) VALUES (34, N'Sin ventas final 1116023513', N'Caso sin ventas ni stock', 21, 20)
SET IDENTITY_INSERT [dbo].[tbl_Producto] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_ProductoVariante] ON 

INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (29, CAST(120.00 AS Decimal(12, 2)), 25, 21, 15)
INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (30, CAST(135.00 AS Decimal(12, 2)), 25, 21, 16)
INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (31, CAST(550.00 AS Decimal(12, 2)), 25, 22, 15)
INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (32, CAST(160.00 AS Decimal(12, 2)), 26, 21, 17)
INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (33, CAST(165.00 AS Decimal(12, 2)), 26, 21, 18)
INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (34, CAST(35.00 AS Decimal(12, 2)), 27, 23, NULL)
INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (35, CAST(55.00 AS Decimal(12, 2)), 28, 23, NULL)
INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (36, CAST(40.00 AS Decimal(12, 2)), 29, 24, NULL)
INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (37, CAST(8.50 AS Decimal(12, 2)), 30, 23, 18)
INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (38, CAST(8.50 AS Decimal(12, 2)), 30, 23, 17)
INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (39, CAST(25.00 AS Decimal(12, 2)), 31, 23, 15)
INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (40, CAST(30.00 AS Decimal(12, 2)), 32, 23, NULL)
INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (41, CAST(10.00 AS Decimal(12, 2)), 33, 21, NULL)
INSERT [dbo].[tbl_ProductoVariante] ([ID_ProductoV], [PrecioVenta], [ID_Producto], [ID_Presentacion], [ID_Color]) VALUES (42, CAST(10.00 AS Decimal(12, 2)), 34, 21, NULL)
SET IDENTITY_INSERT [dbo].[tbl_ProductoVariante] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Proveedor] ON 

INSERT [dbo].[tbl_Proveedor] ([ID_Proveedor], [Nombre_Proveedor]) VALUES (20, N'Distribuidora Central')
INSERT [dbo].[tbl_Proveedor] ([ID_Proveedor], [Nombre_Proveedor]) VALUES (21, N'Suministros del Norte')
INSERT [dbo].[tbl_Proveedor] ([ID_Proveedor], [Nombre_Proveedor]) VALUES (22, N'Electro y PVC Guatemala')
SET IDENTITY_INSERT [dbo].[tbl_Proveedor] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Rol] ON 

INSERT [dbo].[tbl_Rol] ([ID_Rol], [NombreRol]) VALUES (2, N'Cajero')
INSERT [dbo].[tbl_Rol] ([ID_Rol], [NombreRol]) VALUES (1, N'Digitador')
INSERT [dbo].[tbl_Rol] ([ID_Rol], [NombreRol]) VALUES (3, N'Gerente')
SET IDENTITY_INSERT [dbo].[tbl_Rol] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_UbicacionAlmacen] ON 

INSERT [dbo].[tbl_UbicacionAlmacen] ([ID_UbicacionA], [NombreUbicacion], [Descripcion]) VALUES (18, N'Bodega principal', N'Ubicación de ejemplo')
INSERT [dbo].[tbl_UbicacionAlmacen] ([ID_UbicacionA], [NombreUbicacion], [Descripcion]) VALUES (19, N'Estante de exhibición', N'Ubicación de ejemplo')
SET IDENTITY_INSERT [dbo].[tbl_UbicacionAlmacen] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Usuario] ON 

INSERT [dbo].[tbl_Usuario] ([ID_Usuario], [Nombres], [Apellidos], [NombreUsuario], [PasswordHash], [ID_Rol]) VALUES (1, N'Administrador', N'Principal', N'admin', N'$2a$11$0LW3osHa/waSIfuWIY8ynOq8Cf6Dn1NVmZr9YZ0qwrHYD5x3GmQ0C', 1)
INSERT [dbo].[tbl_Usuario] ([ID_Usuario], [Nombres], [Apellidos], [NombreUsuario], [PasswordHash], [ID_Rol]) VALUES (54, N'Luis', N'Morales', N'cajero', N'$2a$11$xMFbWYOODwXtBG9.HF5CSefR9FH9gy0iXDDTjGpHtC6fJmStKQkZS', 2)
INSERT [dbo].[tbl_Usuario] ([ID_Usuario], [Nombres], [Apellidos], [NombreUsuario], [PasswordHash], [ID_Rol]) VALUES (55, N'Daniela', N'Gómez', N'gerente', N'$2a$11$UfQ02ZGTKR9jjihDlyK9pe5PT7Yhs5SH85t5MJmRB..csFEIR8gmW', 3)
SET IDENTITY_INSERT [dbo].[tbl_Usuario] OFF
GO
/****** Objeto: Index [IX_tbl_Auditoria_ID_Usuario] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_Auditoria_ID_Usuario] ON [dbo].[tbl_Auditoria]
(
	[ID_Usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_Cotizacion_ID_Cliente] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_Cotizacion_ID_Cliente] ON [dbo].[tbl_Cotizacion]
(
	[ID_Cliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_Cotizacion_ID_Usuario] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_Cotizacion_ID_Usuario] ON [dbo].[tbl_Cotizacion]
(
	[ID_Usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DetalleCotizacion_ID_Cotizacion] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DetalleCotizacion_ID_Cotizacion] ON [dbo].[tbl_DetalleCotizacion]
(
	[ID_Cotizacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DetalleCotizacion_ID_ProductoV] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DetalleCotizacion_ID_ProductoV] ON [dbo].[tbl_DetalleCotizacion]
(
	[ID_ProductoV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DetalleDevolucionVenta_ID_DetalleF] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DetalleDevolucionVenta_ID_DetalleF] ON [dbo].[tbl_DetalleDevolucionVenta]
(
	[ID_DetalleF] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DetalleDevolucionVenta_ID_DevolucionV] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DetalleDevolucionVenta_ID_DevolucionV] ON [dbo].[tbl_DetalleDevolucionVenta]
(
	[ID_DevolucionV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DetalleFactura_ID_Factura] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DetalleFactura_ID_Factura] ON [dbo].[tbl_DetalleFactura]
(
	[ID_Factura] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DetalleFactura_ID_ProductoV] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DetalleFactura_ID_ProductoV] ON [dbo].[tbl_DetalleFactura]
(
	[ID_ProductoV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DetalleFacturaLote_ID_DetalleF] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DetalleFacturaLote_ID_DetalleF] ON [dbo].[tbl_DetalleFacturaLote]
(
	[ID_DetalleF] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DetalleFacturaLote_ID_Lote] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DetalleFacturaLote_ID_Lote] ON [dbo].[tbl_DetalleFacturaLote]
(
	[ID_Lote] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DetalleIngresoBodega_ID_DetalleOC] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DetalleIngresoBodega_ID_DetalleOC] ON [dbo].[tbl_DetalleIngresoBodega]
(
	[ID_DetalleOC] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DetalleIngresoBodega_ID_IngresoB] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DetalleIngresoBodega_ID_IngresoB] ON [dbo].[tbl_DetalleIngresoBodega]
(
	[ID_IngresoB] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DetalleOrdenCompra_ID_OrdenC] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DetalleOrdenCompra_ID_OrdenC] ON [dbo].[tbl_DetalleOrdenCompra]
(
	[ID_OrdenC] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DetalleOrdenCompra_ID_ProductoV] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DetalleOrdenCompra_ID_ProductoV] ON [dbo].[tbl_DetalleOrdenCompra]
(
	[ID_ProductoV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DevolucionVenta_ID_Factura] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DevolucionVenta_ID_Factura] ON [dbo].[tbl_DevolucionVenta]
(
	[ID_Factura] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_DevolucionVenta_ID_Usuario] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_DevolucionVenta_ID_Usuario] ON [dbo].[tbl_DevolucionVenta]
(
	[ID_Usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [UQ_tbl_Factura_NumeroFactura] Fecha de script: 16/11/2026 10:06:20 ******/
ALTER TABLE [dbo].[tbl_Factura] ADD  CONSTRAINT [UQ_tbl_Factura_NumeroFactura] UNIQUE NONCLUSTERED 
(
	[NumeroFactura] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_Factura_ID_Cliente] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_Factura_ID_Cliente] ON [dbo].[tbl_Factura]
(
	[ID_Cliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_Factura_ID_Cotizacion] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_Factura_ID_Cotizacion] ON [dbo].[tbl_Factura]
(
	[ID_Cotizacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_Factura_ID_Usuario] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_Factura_ID_Usuario] ON [dbo].[tbl_Factura]
(
	[ID_Usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_IngresoBodega_ID_OrdenC] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_IngresoBodega_ID_OrdenC] ON [dbo].[tbl_IngresoBodega]
(
	[ID_OrdenC] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_IngresoBodega_ID_Usuario] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_IngresoBodega_ID_Usuario] ON [dbo].[tbl_IngresoBodega]
(
	[ID_Usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_LoteInventario_ID_DetalleIB] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_LoteInventario_ID_DetalleIB] ON [dbo].[tbl_LoteInventario]
(
	[ID_DetalleIB] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_LoteInventario_ID_ProductoV] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_LoteInventario_ID_ProductoV] ON [dbo].[tbl_LoteInventario]
(
	[ID_ProductoV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_LoteInventario_ID_UbicacionA] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_LoteInventario_ID_UbicacionA] ON [dbo].[tbl_LoteInventario]
(
	[ID_UbicacionA] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [UQ_tbl_MetodoPago_NombreMetodo] Fecha de script: 16/11/2026 10:06:20 ******/
ALTER TABLE [dbo].[tbl_MetodoPago] ADD  CONSTRAINT [UQ_tbl_MetodoPago_NombreMetodo] UNIQUE NONCLUSTERED 
(
	[NombreMetodo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_MovimientoInventario_ID_Lote] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_MovimientoInventario_ID_Lote] ON [dbo].[tbl_MovimientoInventario]
(
	[ID_Lote] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_MovimientoInventario_ID_ProductoV] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_MovimientoInventario_ID_ProductoV] ON [dbo].[tbl_MovimientoInventario]
(
	[ID_ProductoV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_MovimientoInventario_ID_UbicacionA] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_MovimientoInventario_ID_UbicacionA] ON [dbo].[tbl_MovimientoInventario]
(
	[ID_UbicacionA] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_MovimientoInventario_ID_Usuario] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_MovimientoInventario_ID_Usuario] ON [dbo].[tbl_MovimientoInventario]
(
	[ID_Usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_OrdenCompra_ID_Proveedor] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_OrdenCompra_ID_Proveedor] ON [dbo].[tbl_OrdenCompra]
(
	[ID_Proveedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_PagoFactura_ID_Factura] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_PagoFactura_ID_Factura] ON [dbo].[tbl_PagoFactura]
(
	[ID_Factura] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_PagoFactura_ID_MetodoP] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_PagoFactura_ID_MetodoP] ON [dbo].[tbl_PagoFactura]
(
	[ID_MetodoP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_Producto_ID_CategoriaP] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_Producto_ID_CategoriaP] ON [dbo].[tbl_Producto]
(
	[ID_CategoriaP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_Producto_ID_Marca] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_Producto_ID_Marca] ON [dbo].[tbl_Producto]
(
	[ID_Marca] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_ProductoVariante_ID_Color] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_ProductoVariante_ID_Color] ON [dbo].[tbl_ProductoVariante]
(
	[ID_Color] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_ProductoVariante_ID_Presentacion] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_ProductoVariante_ID_Presentacion] ON [dbo].[tbl_ProductoVariante]
(
	[ID_Presentacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_ProductoVariante_ID_Producto] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_ProductoVariante_ID_Producto] ON [dbo].[tbl_ProductoVariante]
(
	[ID_Producto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [UQ_tbl_Rol_NombreRol] Fecha de script: 16/11/2026 10:06:20 ******/
ALTER TABLE [dbo].[tbl_Rol] ADD  CONSTRAINT [UQ_tbl_Rol_NombreRol] UNIQUE NONCLUSTERED 
(
	[NombreRol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [UQ_tbl_Usuario_NombreUsuario] Fecha de script: 16/11/2026 10:06:20 ******/
ALTER TABLE [dbo].[tbl_Usuario] ADD  CONSTRAINT [UQ_tbl_Usuario_NombreUsuario] UNIQUE NONCLUSTERED 
(
	[NombreUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_tbl_Usuario_ID_Rol] Fecha de script: 16/11/2026 10:06:20 ******/
CREATE NONCLUSTERED INDEX [IX_tbl_Usuario_ID_Rol] ON [dbo].[tbl_Usuario]
(
	[ID_Rol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tbl_Auditoria]  WITH CHECK ADD  CONSTRAINT [FK_tbl_Auditoria_Usuario] FOREIGN KEY([ID_Usuario])
REFERENCES [dbo].[tbl_Usuario] ([ID_Usuario])
GO
ALTER TABLE [dbo].[tbl_Auditoria] CHECK CONSTRAINT [FK_tbl_Auditoria_Usuario]
GO
ALTER TABLE [dbo].[tbl_Cotizacion]  WITH CHECK ADD  CONSTRAINT [FK_tbl_Cotizacion_Clientes] FOREIGN KEY([ID_Cliente])
REFERENCES [dbo].[tbl_Clientes] ([ID_Cliente])
GO
ALTER TABLE [dbo].[tbl_Cotizacion] CHECK CONSTRAINT [FK_tbl_Cotizacion_Clientes]
GO
ALTER TABLE [dbo].[tbl_Cotizacion]  WITH CHECK ADD  CONSTRAINT [FK_tbl_Cotizacion_Usuario] FOREIGN KEY([ID_Usuario])
REFERENCES [dbo].[tbl_Usuario] ([ID_Usuario])
GO
ALTER TABLE [dbo].[tbl_Cotizacion] CHECK CONSTRAINT [FK_tbl_Cotizacion_Usuario]
GO
ALTER TABLE [dbo].[tbl_DetalleCotizacion]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DetalleCotizacion_Cotizacion] FOREIGN KEY([ID_Cotizacion])
REFERENCES [dbo].[tbl_Cotizacion] ([ID_Cotizacion])
GO
ALTER TABLE [dbo].[tbl_DetalleCotizacion] CHECK CONSTRAINT [FK_tbl_DetalleCotizacion_Cotizacion]
GO
ALTER TABLE [dbo].[tbl_DetalleCotizacion]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DetalleCotizacion_ProductoVariante] FOREIGN KEY([ID_ProductoV])
REFERENCES [dbo].[tbl_ProductoVariante] ([ID_ProductoV])
GO
ALTER TABLE [dbo].[tbl_DetalleCotizacion] CHECK CONSTRAINT [FK_tbl_DetalleCotizacion_ProductoVariante]
GO
ALTER TABLE [dbo].[tbl_DetalleDevolucionVenta]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DetalleDevolucionVenta_DetalleFactura] FOREIGN KEY([ID_DetalleF])
REFERENCES [dbo].[tbl_DetalleFactura] ([ID_DetalleF])
GO
ALTER TABLE [dbo].[tbl_DetalleDevolucionVenta] CHECK CONSTRAINT [FK_tbl_DetalleDevolucionVenta_DetalleFactura]
GO
ALTER TABLE [dbo].[tbl_DetalleDevolucionVenta]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DetalleDevolucionVenta_DevolucionVenta] FOREIGN KEY([ID_DevolucionV])
REFERENCES [dbo].[tbl_DevolucionVenta] ([ID_DevolucionV])
GO
ALTER TABLE [dbo].[tbl_DetalleDevolucionVenta] CHECK CONSTRAINT [FK_tbl_DetalleDevolucionVenta_DevolucionVenta]
GO
ALTER TABLE [dbo].[tbl_DetalleFactura]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DetalleFactura_Factura] FOREIGN KEY([ID_Factura])
REFERENCES [dbo].[tbl_Factura] ([ID_Factura])
GO
ALTER TABLE [dbo].[tbl_DetalleFactura] CHECK CONSTRAINT [FK_tbl_DetalleFactura_Factura]
GO
ALTER TABLE [dbo].[tbl_DetalleFactura]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DetalleFactura_ProductoVariante] FOREIGN KEY([ID_ProductoV])
REFERENCES [dbo].[tbl_ProductoVariante] ([ID_ProductoV])
GO
ALTER TABLE [dbo].[tbl_DetalleFactura] CHECK CONSTRAINT [FK_tbl_DetalleFactura_ProductoVariante]
GO
ALTER TABLE [dbo].[tbl_DetalleFacturaLote]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DetalleFacturaLote_DetalleFactura] FOREIGN KEY([ID_DetalleF])
REFERENCES [dbo].[tbl_DetalleFactura] ([ID_DetalleF])
GO
ALTER TABLE [dbo].[tbl_DetalleFacturaLote] CHECK CONSTRAINT [FK_tbl_DetalleFacturaLote_DetalleFactura]
GO
ALTER TABLE [dbo].[tbl_DetalleFacturaLote]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DetalleFacturaLote_LoteInventario] FOREIGN KEY([ID_Lote])
REFERENCES [dbo].[tbl_LoteInventario] ([ID_Lote])
GO
ALTER TABLE [dbo].[tbl_DetalleFacturaLote] CHECK CONSTRAINT [FK_tbl_DetalleFacturaLote_LoteInventario]
GO
ALTER TABLE [dbo].[tbl_DetalleIngresoBodega]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DetalleIngresoBodega_DetalleOrdenCompra] FOREIGN KEY([ID_DetalleOC])
REFERENCES [dbo].[tbl_DetalleOrdenCompra] ([ID_DetalleOC])
GO
ALTER TABLE [dbo].[tbl_DetalleIngresoBodega] CHECK CONSTRAINT [FK_tbl_DetalleIngresoBodega_DetalleOrdenCompra]
GO
ALTER TABLE [dbo].[tbl_DetalleIngresoBodega]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DetalleIngresoBodega_IngresoBodega] FOREIGN KEY([ID_IngresoB])
REFERENCES [dbo].[tbl_IngresoBodega] ([ID_IngresoB])
GO
ALTER TABLE [dbo].[tbl_DetalleIngresoBodega] CHECK CONSTRAINT [FK_tbl_DetalleIngresoBodega_IngresoBodega]
GO
ALTER TABLE [dbo].[tbl_DetalleOrdenCompra]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DetalleOrdenCompra_OrdenCompra] FOREIGN KEY([ID_OrdenC])
REFERENCES [dbo].[tbl_OrdenCompra] ([ID_OrdenC])
GO
ALTER TABLE [dbo].[tbl_DetalleOrdenCompra] CHECK CONSTRAINT [FK_tbl_DetalleOrdenCompra_OrdenCompra]
GO
ALTER TABLE [dbo].[tbl_DetalleOrdenCompra]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DetalleOrdenCompra_ProductoVariante] FOREIGN KEY([ID_ProductoV])
REFERENCES [dbo].[tbl_ProductoVariante] ([ID_ProductoV])
GO
ALTER TABLE [dbo].[tbl_DetalleOrdenCompra] CHECK CONSTRAINT [FK_tbl_DetalleOrdenCompra_ProductoVariante]
GO
ALTER TABLE [dbo].[tbl_DevolucionVenta]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DevolucionVenta_Factura] FOREIGN KEY([ID_Factura])
REFERENCES [dbo].[tbl_Factura] ([ID_Factura])
GO
ALTER TABLE [dbo].[tbl_DevolucionVenta] CHECK CONSTRAINT [FK_tbl_DevolucionVenta_Factura]
GO
ALTER TABLE [dbo].[tbl_DevolucionVenta]  WITH CHECK ADD  CONSTRAINT [FK_tbl_DevolucionVenta_Usuario] FOREIGN KEY([ID_Usuario])
REFERENCES [dbo].[tbl_Usuario] ([ID_Usuario])
GO
ALTER TABLE [dbo].[tbl_DevolucionVenta] CHECK CONSTRAINT [FK_tbl_DevolucionVenta_Usuario]
GO
ALTER TABLE [dbo].[tbl_Factura]  WITH CHECK ADD  CONSTRAINT [FK_tbl_Factura_Clientes] FOREIGN KEY([ID_Cliente])
REFERENCES [dbo].[tbl_Clientes] ([ID_Cliente])
GO
ALTER TABLE [dbo].[tbl_Factura] CHECK CONSTRAINT [FK_tbl_Factura_Clientes]
GO
ALTER TABLE [dbo].[tbl_Factura]  WITH CHECK ADD  CONSTRAINT [FK_tbl_Factura_Cotizacion] FOREIGN KEY([ID_Cotizacion])
REFERENCES [dbo].[tbl_Cotizacion] ([ID_Cotizacion])
GO
ALTER TABLE [dbo].[tbl_Factura] CHECK CONSTRAINT [FK_tbl_Factura_Cotizacion]
GO
ALTER TABLE [dbo].[tbl_Factura]  WITH CHECK ADD  CONSTRAINT [FK_tbl_Factura_Usuario] FOREIGN KEY([ID_Usuario])
REFERENCES [dbo].[tbl_Usuario] ([ID_Usuario])
GO
ALTER TABLE [dbo].[tbl_Factura] CHECK CONSTRAINT [FK_tbl_Factura_Usuario]
GO
ALTER TABLE [dbo].[tbl_IngresoBodega]  WITH CHECK ADD  CONSTRAINT [FK_tbl_IngresoBodega_OrdenCompra] FOREIGN KEY([ID_OrdenC])
REFERENCES [dbo].[tbl_OrdenCompra] ([ID_OrdenC])
GO
ALTER TABLE [dbo].[tbl_IngresoBodega] CHECK CONSTRAINT [FK_tbl_IngresoBodega_OrdenCompra]
GO
ALTER TABLE [dbo].[tbl_IngresoBodega]  WITH CHECK ADD  CONSTRAINT [FK_tbl_IngresoBodega_Usuario] FOREIGN KEY([ID_Usuario])
REFERENCES [dbo].[tbl_Usuario] ([ID_Usuario])
GO
ALTER TABLE [dbo].[tbl_IngresoBodega] CHECK CONSTRAINT [FK_tbl_IngresoBodega_Usuario]
GO
ALTER TABLE [dbo].[tbl_LoteInventario]  WITH CHECK ADD  CONSTRAINT [FK_tbl_LoteInventario_DetalleIngresoBodega] FOREIGN KEY([ID_DetalleIB])
REFERENCES [dbo].[tbl_DetalleIngresoBodega] ([ID_DetalleIB])
GO
ALTER TABLE [dbo].[tbl_LoteInventario] CHECK CONSTRAINT [FK_tbl_LoteInventario_DetalleIngresoBodega]
GO
ALTER TABLE [dbo].[tbl_LoteInventario]  WITH CHECK ADD  CONSTRAINT [FK_tbl_LoteInventario_ProductoVariante] FOREIGN KEY([ID_ProductoV])
REFERENCES [dbo].[tbl_ProductoVariante] ([ID_ProductoV])
GO
ALTER TABLE [dbo].[tbl_LoteInventario] CHECK CONSTRAINT [FK_tbl_LoteInventario_ProductoVariante]
GO
ALTER TABLE [dbo].[tbl_LoteInventario]  WITH CHECK ADD  CONSTRAINT [FK_tbl_LoteInventario_UbicacionAlmacen] FOREIGN KEY([ID_UbicacionA])
REFERENCES [dbo].[tbl_UbicacionAlmacen] ([ID_UbicacionA])
GO
ALTER TABLE [dbo].[tbl_LoteInventario] CHECK CONSTRAINT [FK_tbl_LoteInventario_UbicacionAlmacen]
GO
ALTER TABLE [dbo].[tbl_MovimientoInventario]  WITH CHECK ADD  CONSTRAINT [FK_tbl_MovimientoInventario_LoteInventario] FOREIGN KEY([ID_Lote])
REFERENCES [dbo].[tbl_LoteInventario] ([ID_Lote])
GO
ALTER TABLE [dbo].[tbl_MovimientoInventario] CHECK CONSTRAINT [FK_tbl_MovimientoInventario_LoteInventario]
GO
ALTER TABLE [dbo].[tbl_MovimientoInventario]  WITH CHECK ADD  CONSTRAINT [FK_tbl_MovimientoInventario_ProductoVariante] FOREIGN KEY([ID_ProductoV])
REFERENCES [dbo].[tbl_ProductoVariante] ([ID_ProductoV])
GO
ALTER TABLE [dbo].[tbl_MovimientoInventario] CHECK CONSTRAINT [FK_tbl_MovimientoInventario_ProductoVariante]
GO
ALTER TABLE [dbo].[tbl_MovimientoInventario]  WITH CHECK ADD  CONSTRAINT [FK_tbl_MovimientoInventario_UbicacionAlmacen] FOREIGN KEY([ID_UbicacionA])
REFERENCES [dbo].[tbl_UbicacionAlmacen] ([ID_UbicacionA])
GO
ALTER TABLE [dbo].[tbl_MovimientoInventario] CHECK CONSTRAINT [FK_tbl_MovimientoInventario_UbicacionAlmacen]
GO
ALTER TABLE [dbo].[tbl_MovimientoInventario]  WITH CHECK ADD  CONSTRAINT [FK_tbl_MovimientoInventario_Usuario] FOREIGN KEY([ID_Usuario])
REFERENCES [dbo].[tbl_Usuario] ([ID_Usuario])
GO
ALTER TABLE [dbo].[tbl_MovimientoInventario] CHECK CONSTRAINT [FK_tbl_MovimientoInventario_Usuario]
GO
ALTER TABLE [dbo].[tbl_OrdenCompra]  WITH CHECK ADD  CONSTRAINT [FK_tbl_OrdenCompra_Proveedor] FOREIGN KEY([ID_Proveedor])
REFERENCES [dbo].[tbl_Proveedor] ([ID_Proveedor])
GO
ALTER TABLE [dbo].[tbl_OrdenCompra] CHECK CONSTRAINT [FK_tbl_OrdenCompra_Proveedor]
GO
ALTER TABLE [dbo].[tbl_PagoFactura]  WITH CHECK ADD  CONSTRAINT [FK_tbl_PagoFactura_Factura] FOREIGN KEY([ID_Factura])
REFERENCES [dbo].[tbl_Factura] ([ID_Factura])
GO
ALTER TABLE [dbo].[tbl_PagoFactura] CHECK CONSTRAINT [FK_tbl_PagoFactura_Factura]
GO
ALTER TABLE [dbo].[tbl_PagoFactura]  WITH CHECK ADD  CONSTRAINT [FK_tbl_PagoFactura_MetodoPago] FOREIGN KEY([ID_MetodoP])
REFERENCES [dbo].[tbl_MetodoPago] ([ID_MetodoP])
GO
ALTER TABLE [dbo].[tbl_PagoFactura] CHECK CONSTRAINT [FK_tbl_PagoFactura_MetodoPago]
GO
ALTER TABLE [dbo].[tbl_Producto]  WITH CHECK ADD  CONSTRAINT [FK_tbl_Producto_CategoriaProducto] FOREIGN KEY([ID_CategoriaP])
REFERENCES [dbo].[tbl_CategoriaProducto] ([ID_Categoria])
GO
ALTER TABLE [dbo].[tbl_Producto] CHECK CONSTRAINT [FK_tbl_Producto_CategoriaProducto]
GO
ALTER TABLE [dbo].[tbl_Producto]  WITH CHECK ADD  CONSTRAINT [FK_tbl_Producto_Marca] FOREIGN KEY([ID_Marca])
REFERENCES [dbo].[tbl_Marca] ([ID_Marca])
GO
ALTER TABLE [dbo].[tbl_Producto] CHECK CONSTRAINT [FK_tbl_Producto_Marca]
GO
ALTER TABLE [dbo].[tbl_ProductoVariante]  WITH CHECK ADD  CONSTRAINT [FK_tbl_ProductoVariante_Color] FOREIGN KEY([ID_Color])
REFERENCES [dbo].[tbl_Color] ([ID_Color])
GO
ALTER TABLE [dbo].[tbl_ProductoVariante] CHECK CONSTRAINT [FK_tbl_ProductoVariante_Color]
GO
ALTER TABLE [dbo].[tbl_ProductoVariante]  WITH CHECK ADD  CONSTRAINT [FK_tbl_ProductoVariante_Presentacion] FOREIGN KEY([ID_Presentacion])
REFERENCES [dbo].[tbl_Presentacion] ([ID_Presentacion])
GO
ALTER TABLE [dbo].[tbl_ProductoVariante] CHECK CONSTRAINT [FK_tbl_ProductoVariante_Presentacion]
GO
ALTER TABLE [dbo].[tbl_ProductoVariante]  WITH CHECK ADD  CONSTRAINT [FK_tbl_ProductoVariante_Producto] FOREIGN KEY([ID_Producto])
REFERENCES [dbo].[tbl_Producto] ([ID_Producto])
GO
ALTER TABLE [dbo].[tbl_ProductoVariante] CHECK CONSTRAINT [FK_tbl_ProductoVariante_Producto]
GO
ALTER TABLE [dbo].[tbl_Usuario]  WITH CHECK ADD  CONSTRAINT [FK_tbl_Usuario_Rol] FOREIGN KEY([ID_Rol])
REFERENCES [dbo].[tbl_Rol] ([ID_Rol])
GO
ALTER TABLE [dbo].[tbl_Usuario] CHECK CONSTRAINT [FK_tbl_Usuario_Rol]
GO
/****** Objeto: StoredProcedure [dbo].[usp_AnularFactura] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_AnularFactura] @NumeroFactura int,@ID_Usuario int
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
/****** Objeto: StoredProcedure [dbo].[usp_EstadoCotizacion] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_EstadoCotizacion] @ID_Cotizacion int,@Estado nvarchar(45),@ID_Usuario int
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY BEGIN TRAN;
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@ID_Usuario AND r.NombreRol='Digitador') THROW 50001,'Rol sin permiso para cotizar.',1;
 IF @Estado NOT IN('Confirmada','Cancelada') THROW 50001,'Estado invÃ¡lido.',1;
 UPDATE dbo.tbl_Cotizacion WITH(UPDLOCK,HOLDLOCK) SET Estado=@Estado WHERE ID_Cotizacion=@ID_Cotizacion AND (Estado='Pendiente' OR (Estado='Confirmada' AND @Estado='Cancelada'));
 IF @@ROWCOUNT=0 THROW 50001,'La cotizaciÃ³n no admite ese cambio de estado.',1;
 COMMIT;
 END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;
END;

GO
/****** Objeto: StoredProcedure [dbo].[usp_FacturarPEPS] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_FacturarPEPS]
 @ID_Cliente int,@ID_Usuario int,@ID_Cotizacion int=NULL,@Detalle nvarchar(max)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY BEGIN TRAN;
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@ID_Usuario AND r.NombreRol IN('Digitador','Cajero')) THROW 50001,'Rol sin permiso para facturar.',1;
 -- Serializa numeraciÃ³n y salidas, incluso cuando aÃºn no hay facturas.
 DECLARE @bloqueo int;
 EXEC @bloqueo=sys.sp_getapplock @Resource='ProyectoBD_FacturacionPEPS',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000;
 IF @bloqueo<0 THROW 50001,'No se pudo obtener el bloqueo de facturaciÃ³n; intente de nuevo.',1;
 DECLARE @d TABLE(ID_ProductoV int NOT NULL PRIMARY KEY,Cantidad int NOT NULL,PrecioUnitario decimal(12,2) NOT NULL);
 IF @ID_Cotizacion IS NOT NULL
 BEGIN
   DECLARE @estado nvarchar(45),@cliente int;
   SELECT @estado=Estado,@cliente=ID_Cliente FROM dbo.tbl_Cotizacion WITH(UPDLOCK,HOLDLOCK) WHERE ID_Cotizacion=@ID_Cotizacion;
   IF @estado IS NULL OR @estado<>'Confirmada' OR @cliente<>@ID_Cliente THROW 50001,'La cotizaciÃ³n debe estar confirmada, sin facturar y pertenecer al cliente.',1;
   INSERT @d SELECT ID_ProductoV,Cantidad,PrecioUnitario FROM dbo.tbl_DetalleCotizacion WHERE ID_Cotizacion=@ID_Cotizacion;
 END
 ELSE
 BEGIN
   IF ISJSON(@Detalle)<>1 THROW 50001,'Detalle JSON invÃ¡lido.',1;
   INSERT @d SELECT ID_ProductoV,Cantidad,PrecioUnitario FROM OPENJSON(@Detalle) WITH(ID_ProductoV int,Cantidad int,PrecioUnitario decimal(12,2));
 END;
 IF NOT EXISTS(SELECT 1 FROM @d) OR EXISTS(SELECT 1 FROM @d WHERE Cantidad<=0 OR PrecioUnitario<=0) THROW 50001,'Detalle vacÃ­o o valores invÃ¡lidos.',1;
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
     IF @lote IS NULL THROW 50001,'Inventario insuficiente. No se guardÃ³ la factura ni se descontÃ³ inventario.',1;
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
/****** Objeto: StoredProcedure [dbo].[usp_GuardarCotizacion] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_GuardarCotizacion]
 @ID_Cliente int,@ID_Usuario int,@Detalle nvarchar(max)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRAN;
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@ID_Usuario AND r.NombreRol='Digitador') THROW 50001,'Rol sin permiso para cotizar.',1;
 IF ISJSON(@Detalle)<>1 THROW 50001,'Detalle JSON invÃ¡lido.',1;
 DECLARE @d TABLE(ID_ProductoV int NOT NULL PRIMARY KEY,Cantidad int NOT NULL,PrecioUnitario decimal(12,2) NOT NULL);
 INSERT @d SELECT ID_ProductoV,Cantidad,PrecioUnitario FROM OPENJSON(@Detalle) WITH(ID_ProductoV int,Cantidad int,PrecioUnitario decimal(12,2));
 IF NOT EXISTS(SELECT 1 FROM @d) OR EXISTS(SELECT 1 FROM @d WHERE Cantidad<=0 OR PrecioUnitario<=0) THROW 50001,'Detalle vacÃ­o o cantidades/precios invÃ¡lidos.',1;
 INSERT dbo.tbl_Cotizacion(FechaCotizacion,Estado,Total,ID_Cliente,ID_Usuario) SELECT CONVERT(date,SYSDATETIME()),'Pendiente',SUM(Cantidad*PrecioUnitario),@ID_Cliente,@ID_Usuario FROM @d;
 DECLARE @id int=CONVERT(int,SCOPE_IDENTITY());
 INSERT dbo.tbl_DetalleCotizacion(Cantidad,PrecioUnitario,Subtotal,ID_Cotizacion,ID_ProductoV) SELECT Cantidad,PrecioUnitario,Cantidad*PrecioUnitario,@id,ID_ProductoV FROM @d;
 COMMIT; SELECT @id;
 END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;
END;

GO
/****** Objeto: StoredProcedure [dbo].[usp_GuardarOrdenCompra] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_GuardarOrdenCompra] @ID_Proveedor int,@ID_Usuario int,@Detalle nvarchar(max)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY BEGIN TRAN;
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@ID_Usuario AND r.NombreRol='Digitador') THROW 50001,'Rol sin permiso para compras.',1;
 IF ISJSON(@Detalle)<>1 THROW 50001,'Detalle JSON invÃ¡lido.',1;
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
/****** Objeto: StoredProcedure [dbo].[usp_RegistrarIngresoBodega] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_RegistrarIngresoBodega]
 @ID_OrdenC int,@ID_Usuario int,@ID_UbicacionA int,@FechaIngreso datetime2(0),@Detalle nvarchar(max)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY BEGIN TRAN;
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@ID_Usuario AND r.NombreRol='Digitador') THROW 50001,'Rol sin permiso para ingresos.',1;
 IF @FechaIngreso IS NULL OR @FechaIngreso>SYSDATETIME() THROW 50001,'Fecha de ingreso invÃ¡lida.',1;
 DECLARE @estado nvarchar(45);
 SELECT @estado=Estado FROM dbo.tbl_OrdenCompra WITH(UPDLOCK,HOLDLOCK) WHERE ID_OrdenC=@ID_OrdenC;
 IF @estado IS NULL OR @estado NOT IN('Pendiente','Parcial') THROW 50001,'Orden inexistente o completa.',1;
 IF ISJSON(@Detalle)<>1 THROW 50001,'Detalle JSON invÃ¡lido.',1;
 DECLARE @d TABLE(ID_DetalleOC int NOT NULL PRIMARY KEY,CantidadRecibida int NOT NULL,CostoUnitario decimal(12,2) NOT NULL,NumeroLote nvarchar(50));
 INSERT @d SELECT ID_DetalleOC,CantidadRecibida,CostoUnitario,NumeroLote FROM OPENJSON(@Detalle) WITH(ID_DetalleOC int,CantidadRecibida int,CostoUnitario decimal(12,2),NumeroLote nvarchar(50));
 IF NOT EXISTS(SELECT 1 FROM @d) OR EXISTS(SELECT 1 FROM @d WHERE CantidadRecibida<=0 OR CostoUnitario<=0) THROW 50001,'Detalle o costo invÃ¡lido.',1;
 IF EXISTS(SELECT 1 FROM @d d LEFT JOIN dbo.tbl_DetalleOrdenCompra oc ON oc.ID_DetalleOC=d.ID_DetalleOC AND oc.ID_OrdenC=@ID_OrdenC WHERE oc.ID_DetalleOC IS NULL OR d.CantidadRecibida>oc.CantidadSolicitada-ISNULL((SELECT SUM(ib.CantidadRecibida) FROM dbo.tbl_DetalleIngresoBodega ib WHERE ib.ID_DetalleOC=oc.ID_DetalleOC),0)) THROW 50001,'La recepciÃ³n supera lo pendiente o contiene detalles de otra orden.',1;
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
/****** Objeto: StoredProcedure [dbo].[usp_RegistrarPagos] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_RegistrarPagos] @ID_Factura int,@ID_Usuario int,@Pagos nvarchar(max)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY BEGIN TRAN;
 IF NOT EXISTS(SELECT 1 FROM dbo.tbl_Usuario u JOIN dbo.tbl_Rol r ON r.ID_Rol=u.ID_Rol WHERE u.ID_Usuario=@ID_Usuario AND r.NombreRol IN('Cajero','Digitador')) THROW 50001,'Rol sin permiso para cobros.',1;
 DECLARE @total decimal(12,2),@estado nvarchar(45);
 SELECT @total=Total,@estado=Estado FROM dbo.tbl_Factura WITH(UPDLOCK,HOLDLOCK) WHERE ID_Factura=@ID_Factura;
 IF @total IS NULL OR @estado<>'Pendiente' THROW 50001,'La factura no estÃ¡ pendiente de pago.',1;
 IF ISJSON(@Pagos)<>1 THROW 50001,'Pagos JSON invÃ¡lidos.',1;
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
/****** Objeto: StoredProcedure [dbo].[usp_ReporteDetalleFactura] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_ReporteDetalleFactura] @NumeroFactura int
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
/****** Objeto: StoredProcedure [dbo].[usp_ReporteFacturacionFormasPago] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_ReporteFacturacionFormasPago]
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
/****** Objeto: StoredProcedure [dbo].[usp_ReporteInventarioActual] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_ReporteInventarioActual]
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
/****** Objeto: StoredProcedure [dbo].[usp_ReporteProductosMasVendidos] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_ReporteProductosMasVendidos]
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
/****** Objeto: StoredProcedure [dbo].[usp_ReporteProductosMayoresIngresos] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_ReporteProductosMayoresIngresos]
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
/****** Objeto: StoredProcedure [dbo].[usp_ReporteProductosMenosVendidos] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_ReporteProductosMenosVendidos]
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
/****** Objeto: StoredProcedure [dbo].[usp_ReporteProductosSinExistencia] Fecha de script: 16/11/2026 10:06:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[usp_ReporteProductosSinExistencia]
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
USE [master] 
GO
