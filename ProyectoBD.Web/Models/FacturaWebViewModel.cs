using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoBD.Entidades;

namespace ProyectoBD.Web.Models;

public class FacturaCrearViewModel
{
    public int? ID_Cotizacion { get; set; }
    [Display(Name = "Cliente")]
    public int ID_Cliente { get; set; }
    public List<LineaCotizacionViewModel> Detalle { get; set; } = new();
    [ValidateNever] public IEnumerable<SelectListItem> Clientes { get; set; } = new List<SelectListItem>();
    [ValidateNever] public IEnumerable<SelectListItem> Cotizaciones { get; set; } = new List<SelectListItem>();
    [ValidateNever] public List<ProductoVariante> Variantes { get; set; } = new();
    [ValidateNever] public List<LineaDocumento> DetalleConfirmado { get; set; } = new();
    [ValidateNever] public string ClienteCotizacion { get; set; } = "";
    public decimal Total => ID_Cotizacion.HasValue ? DetalleConfirmado.Sum(l => l.Subtotal) : Detalle.Sum(l => l.Subtotal);
}

public class FacturaWebViewModel
{
    public int ID_Factura { get; set; }
    public int NumeroFactura { get; set; }
    public DateTime Fecha { get; set; }
    public string Cliente { get; set; } = "";
    public string Empleado { get; set; } = "";
    public string Estado { get; set; } = "";
    public decimal Total { get; set; }
    public decimal Pagado { get; set; }
    public decimal Saldo { get; set; }
    public List<LineaDocumento> Detalle { get; set; } = new();
    public List<PagoFactura> Pagos { get; set; } = new();
}

public class CobroViewModel
{
    public int ID_Factura { get; set; }
    [MinLength(1, ErrorMessage = "Agregue al menos un pago.")]
    public List<LineaPagoViewModel> Pagos { get; set; } = new();
    [ValidateNever] public FacturaWebViewModel Factura { get; set; } = new();
    [ValidateNever] public IEnumerable<SelectListItem> Metodos { get; set; } = new List<SelectListItem>();
    public decimal TotalPagos => Pagos.Where(p => p.Monto > 0 && p.Monto <= 9999999999.99m).Sum(p => p.Monto);
}

public class LineaPagoViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un método de pago.")]
    public int ID_MetodoP { get; set; }
    [Range(typeof(decimal), "0.01", "9999999999.99", ErrorMessage = "Ingrese un monto positivo válido.")]
    public decimal Monto { get; set; }
    [StringLength(100, ErrorMessage = "La referencia admite hasta 100 caracteres.")]
    public string? Referencia { get; set; }
}
