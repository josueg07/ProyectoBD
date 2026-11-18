using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoBD.Web.Models;

public class IngresoBodegaCrearViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una orden de compra.")]
    public int ID_OrdenC { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una ubicación.")]
    [Display(Name = "Ubicación de almacén")]
    public int ID_UbicacionA { get; set; }
    [Required(ErrorMessage = "Ingrese la fecha y hora de ingreso.")]
    [Display(Name = "Fecha y hora de ingreso")]
    public DateTime? FechaIngreso { get; set; }
    public List<LineaRecepcionViewModel> Detalle { get; set; } = new();
    [ValidateNever]
    public IEnumerable<SelectListItem> Ordenes { get; set; } = new List<SelectListItem>();
    [ValidateNever]
    public IEnumerable<SelectListItem> Ubicaciones { get; set; } = new List<SelectListItem>();
    [ValidateNever]
    public string Proveedor { get; set; } = string.Empty;
    [ValidateNever]
    public string Estado { get; set; } = string.Empty;
}

public class LineaRecepcionViewModel
{
    public int ID_DetalleOC { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad a recibir debe ser un entero no negativo.")]
    public int CantidadRecibida { get; set; }
    public decimal? CostoUnitario { get; set; }
    [StringLength(50, ErrorMessage = "El lote admite hasta 50 caracteres.")]
    public string? NumeroLote { get; set; }
    [ValidateNever]
    public string Producto { get; set; } = string.Empty;
    [ValidateNever]
    public int Solicitada { get; set; }
    [ValidateNever]
    public int RecibidaAnterior { get; set; }
    [ValidateNever]
    public int Pendiente { get; set; }
}

public class IngresoBodegaResumenViewModel
{
    public int ID_IngresoB { get; set; }
    public int ID_OrdenC { get; set; }
    public DateTime Fecha { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public long CantidadRecibida { get; set; }
    public decimal CostoTotal { get; set; }
    public List<IngresoBodegaDetalleViewModel> Detalle { get; set; } = new();
}

public class IngresoBodegaDetalleViewModel
{
    public string Producto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Subtotal => Cantidad * CostoUnitario;
}
