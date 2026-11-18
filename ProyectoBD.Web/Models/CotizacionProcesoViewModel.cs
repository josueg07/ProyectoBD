using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoBD.Entidades;

namespace ProyectoBD.Web.Models;

public class CotizacionCrearViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente.")]
    [Display(Name = "Cliente")]
    public int ID_Cliente { get; set; }
    [MinLength(1, ErrorMessage = "Agregue al menos una línea.")]
    public List<LineaCotizacionViewModel> Detalle { get; set; } = new();
    [ValidateNever]
    public IEnumerable<SelectListItem> Clientes { get; set; } = new List<SelectListItem>();
    [ValidateNever]
    public List<ProductoVariante> Variantes { get; set; } = new();
    public decimal Total => Detalle.Sum(l => l.Subtotal);
}

public class LineaCotizacionViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una variante.")]
    public int ID_ProductoV { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Ingrese una cantidad entera positiva.")]
    public int Cantidad { get; set; } = 1;
    [Range(typeof(decimal), "0.01", "9999999999.99", ErrorMessage = "Ingrese un precio positivo válido.")]
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Cantidad > 0 && PrecioUnitario > 0 && PrecioUnitario <= 9999999999.99m ? Cantidad * PrecioUnitario : 0;
}

public class CotizacionResumenViewModel
{
    public int ID_Cotizacion { get; set; }
    public DateTime Fecha { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<LineaDocumento> Detalle { get; set; } = new();
}
