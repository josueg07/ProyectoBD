using System.Data;
using ProyectoBD.Entidades;
using ProyectoBD.Datos;
namespace ProyectoBD.Negocio;

public class ProcesoNegocio
{
    private readonly ProcesoDatos datos = new();
    private readonly UsuarioSesion sesion;
    public ProcesoNegocio(UsuarioSesion sesion)
    {
        this.sesion = sesion;
    }
    private static void Importe(decimal importe)
    {
        if (importe <= 0 || importe > 9999999999.99m || decimal.Round(importe, 2) != importe)
            throw new ArgumentException("Ingrese un importe positivo con hasta dos decimales.");
    }
    private static void Detalle(List<LineaDocumento> d, bool precio)
    {
        if (d.Count == 0 || d.Select(x => x.ID_ProductoV).Distinct().Count() != d.Count)
            throw new ArgumentException("Agregue variantes sin duplicarlas.");
        foreach (var l in d)
        {
            if (l.ID_ProductoV <= 0 || l.Cantidad <= 0)
                throw new ArgumentException("Seleccione una variante y cantidad positiva.");
            if (precio)
            {
                Importe(l.PrecioUnitario);
                Importe(l.Subtotal);
            }
        }
        if (precio)
            Importe(d.Sum(x => x.Subtotal));
    }
    public int GuardarCotizacion(Cotizacion c)
    {
        PermisosNegocio.Exigir(sesion, "Digitador");
        if (c.ID_Cliente <= 0)
            throw new ArgumentException("Seleccione un cliente.");
        Detalle(c.Detalle, true);
        return datos.GuardarCotizacion(c, sesion.ID_Usuario);
    }
    public void EstadoCotizacion(int id, string estado)
    {
        PermisosNegocio.Exigir(sesion, "Digitador");
        if (id <= 0 || !(estado == "Confirmada" || estado == "Cancelada"))
            throw new ArgumentException("Seleccione una cotización.");
        datos.EstadoCotizacion(id, estado, sesion.ID_Usuario);
    }
    public int GuardarOrden(OrdenCompra o)
    {
        PermisosNegocio.Exigir(sesion, "Digitador");
        if (o.ID_Proveedor <= 0)
            throw new ArgumentException("Seleccione un proveedor.");
        Detalle(o.Detalle, false);
        return datos.GuardarOrden(o, sesion.ID_Usuario);
    }
    public int Ingresar(IngresoBodega i)
    {
        PermisosNegocio.Exigir(sesion, "Digitador");
        if (i.ID_OrdenC <= 0 || i.ID_UbicacionA <= 0 || i.Detalle.Count == 0 || i.FechaIngreso > DateTime.Now)
            throw new ArgumentException("Seleccione orden, ubicación, fecha válida y al menos una cantidad a recibir.");
        if (i.Detalle.Select(x => x.ID_DetalleOC).Distinct().Count() != i.Detalle.Count)
            throw new ArgumentException("No duplique detalles de compra.");
        foreach (var l in i.Detalle)
        {
            if (l.ID_DetalleOC <= 0 || l.CantidadRecibida <= 0 || l.NumeroLote?.Length > 50)
                throw new ArgumentException("Cantidad o lote inválido.");
            Importe(l.CostoUnitario);
        }
        return datos.Ingresar(i, sesion.ID_Usuario);
    }
    public int Facturar(Factura f)
    {
        PermisosNegocio.Exigir(sesion, "Digitador", "Cajero");
        if (f.ID_Cliente <= 0 || f.ID_Cotizacion <= 0)
            throw new ArgumentException("Seleccione cliente y cotización válida, si corresponde.");
        if (f.ID_Cotizacion == null)
            Detalle(f.Detalle, true);
        return datos.Facturar(f, sesion.ID_Usuario);
    }
    public void Pagar(int factura, List<PagoFactura> pagos)
    {
        PermisosNegocio.Exigir(sesion, "Digitador", "Cajero");
        if (factura <= 0 || pagos.Count == 0)
            throw new ArgumentException("Seleccione una factura y agregue pagos.");
        foreach (var p in pagos)
        {
            if (p.ID_MetodoP <= 0 || p.Referencia.Length > 100)
                throw new ArgumentException("Método o referencia inválidos.");
            Importe(p.Monto);
        }
        Importe(pagos.Sum(x => x.Monto));
        datos.Pagar(factura, pagos, sesion.ID_Usuario);
    }
    public DataTable Cotizaciones()
    {
        PermisosNegocio.Exigir(sesion, "Digitador", "Cajero");
        return datos.Cotizaciones();
    }
    public List<LineaDocumento> DetalleCotizacion(int id)
    {
        PermisosNegocio.Exigir(sesion, "Digitador", "Cajero");
        return datos.DetalleCotizacion(id).AsEnumerable().Select(r => new LineaDocumento { ID_ProductoV = r.Field<int>("ID_ProductoV"), Producto = r.Field<string>("Producto")!, Cantidad = r.Field<int>("Cantidad"), PrecioUnitario = r.Field<decimal>("PrecioUnitario") }).ToList();
    }
    public DataTable Ordenes()
    {
        PermisosNegocio.Exigir(sesion, "Digitador");
        return datos.Ordenes();
    }
    public DataTable PendienteOrden(int id)
    {
        PermisosNegocio.Exigir(sesion, "Digitador");
        return datos.PendienteOrden(id);
    }
    public DataTable Facturas()
    {
        PermisosNegocio.Exigir(sesion, "Digitador", "Cajero", "Gerente");
        return datos.Facturas();
    }
    public DataTable DetalleFactura(int id)
    {
        PermisosNegocio.Exigir(sesion, "Digitador", "Cajero", "Gerente");
        return datos.DetalleFactura(id);
    }
    public DataTable Pagos(int id)
    {
        PermisosNegocio.Exigir(sesion, "Digitador", "Cajero", "Gerente");
        return datos.Pagos(id);
    }
    public DataTable Inventario()
    {
        PermisosNegocio.Exigir(sesion, "Gerente", "Digitador");
        return datos.Inventario();
    }
    public DataTable Ingresos()
    {
        PermisosNegocio.Exigir(sesion, "Gerente", "Digitador");
        return datos.Ingresos();
    }
}
