using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoBD.Entidades;

namespace ProyectoBD.Web.Models;

public class OrdenCompraCrearViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un proveedor.")]
    [Display(Name = "Proveedor")]
    public int ID_Proveedor { get; set; }
    [MinLength(1, ErrorMessage = "Agregue al menos una línea.")]
    public List<LineaOrdenCompraViewModel> Detalle { get; set; } = new();
    [ValidateNever]
    public IEnumerable<SelectListItem> Proveedores { get; set; } = new List<SelectListItem>();
    [ValidateNever]
    public List<ProductoVariante> Variantes { get; set; } = new();
}

public class LineaOrdenCompraViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una variante.")]
    public int ID_ProductoV { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Ingrese una cantidad entera positiva.")]
    public int Cantidad { get; set; } = 1;
}

public class OrdenCompraResumenViewModel
{
    public int ID_OrdenC { get; set; }
    public DateTime Fecha { get; set; }
    public string Proveedor { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public List<LineaPendienteViewModel> Detalle { get; set; } = new();
}

public class LineaPendienteViewModel
{
    public int ID_DetalleOC { get; set; }
    public string Producto { get; set; } = string.Empty;
    public int Solicitada { get; set; }
    public int Recibida { get; set; }
    public int Pendiente { get; set; }
}
