using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoBD.Web.Models
{
    public class ProductoViewModel
    {
        public int ID_Producto { get; set; }

        [Required(ErrorMessage = "Ingrese el nombre del producto.")]
        [StringLength(45)]
        [Display(Name = "Nombre del producto")]
        public string NombreProducto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la descripción.")]
        [StringLength(45)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría.")]
        [Display(Name = "Categoría")]
        public int ID_CategoriaP { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una marca.")]
        [Display(Name = "Marca")]
        public int ID_Marca { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Categorias { get; set; } = new List<SelectListItem>();
        [ValidateNever]
        public IEnumerable<SelectListItem> Marcas { get; set; } = new List<SelectListItem>();
    }

    public class ProductoListadoViewModel
    {
        public int ID_Producto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
    }
}
