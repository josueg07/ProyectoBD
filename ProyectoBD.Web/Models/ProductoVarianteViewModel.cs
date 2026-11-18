using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoBD.Web.Models
{
    public class ProductoVarianteViewModel
    {
        public int ID_ProductoV { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un producto.")]
        [Display(Name = "Producto")]
        public int ID_Producto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una presentación.")]
        [Display(Name = "Presentación")]
        public int ID_Presentacion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un color válido o Sin color.")]
        [Display(Name = "Color")]
        public int? ID_Color { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999.99", ErrorMessage = "Ingrese un precio entre Q0.01 y Q9,999,999,999.99.")]
        [Display(Name = "Precio de venta")]
        public decimal PrecioVenta { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Productos { get; set; } = new List<SelectListItem>();
        [ValidateNever]
        public IEnumerable<SelectListItem> Presentaciones { get; set; } = new List<SelectListItem>();
        [ValidateNever]
        public IEnumerable<SelectListItem> Colores { get; set; } = new List<SelectListItem>();
    }

    public class VarianteListadoViewModel
    {
        public int ID_ProductoV { get; set; }
        public int ID_Producto { get; set; }
        public string Producto { get; set; } = string.Empty;
        public string Presentacion { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }
    }
}
