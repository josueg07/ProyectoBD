using System.ComponentModel.DataAnnotations;

namespace ProyectoBD.Web.Models
{
    public class ColorViewModel
    {
        public int ID_Color { get; set; }

        [Required(ErrorMessage = "Complete nombre del color.")]
        [StringLength(45, ErrorMessage = "El campo admite hasta 45 caracteres.")]
        [Display(Name = "Nombre del color")]
        public string NombreColor { get; set; } = string.Empty;
    }
}
