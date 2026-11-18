using System.ComponentModel.DataAnnotations;

namespace ProyectoBD.Web.Models
{
    public class ProveedorViewModel
    {
        public int ID_Proveedor { get; set; }

        [Required(ErrorMessage = "Complete nombre del proveedor.")]
        [StringLength(45, ErrorMessage = "El campo admite hasta 45 caracteres.")]
        [Display(Name = "Nombre del proveedor")]
        public string Nombre_Proveedor { get; set; } = string.Empty;
    }
}
