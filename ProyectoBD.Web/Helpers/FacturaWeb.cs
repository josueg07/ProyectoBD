using System.Data;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Helpers;

public static class FacturaWeb
{
    public static List<FacturaWebViewModel> Listar(ProcesoNegocio negocio) => negocio.Facturas().AsEnumerable().Select(r => new FacturaWebViewModel
    {
        ID_Factura = r.Field<int>("ID_Factura"), NumeroFactura = r.Field<int>("NumeroFactura"),
        Fecha = r.Field<DateTime>("FechaFactura"), Cliente = r.Field<string>("Cliente")!,
        Empleado = r.Field<string>("Usuario")!, Estado = r.Field<string>("Estado")!,
        Total = r.Field<decimal>("Total"), Pagado = r.Field<decimal>("Pagado"), Saldo = r.Field<decimal>("Saldo")
    }).ToList();

    public static FacturaWebViewModel? Detalle(ProcesoNegocio negocio, int id)
    {
        var factura = Listar(negocio).FirstOrDefault(f => f.ID_Factura == id);
        if (factura == null) return null;
        factura.Detalle = negocio.DetalleFactura(id).AsEnumerable().Select(r => new LineaDocumento
        {
            Producto = r.Field<string>("Producto")!, Cantidad = r.Field<int>("Cantidad"), PrecioUnitario = r.Field<decimal>("PrecioUnitario")
        }).ToList();
        factura.Pagos = negocio.Pagos(id).AsEnumerable().Select(r => new PagoFactura
        { Metodo = r.Field<string>("NombreMetodo")!, Monto = r.Field<decimal>("Monto"), Referencia = r.Field<string?>("Referencia") ?? "" }).ToList();
        return factura;
    }
}
