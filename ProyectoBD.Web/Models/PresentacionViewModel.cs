using System.ComponentModel.DataAnnotations;

namespace ProyectoBD.Web.Models
{
    public class PresentacionViewModel
    {
        public int ID_Presentacion { get; set; }

        [Required(ErrorMessage = "Complete nombre de la presentación.")]
        [StringLength(45, ErrorMessage = "El campo admite hasta 45 caracteres.")]
        [Display(Name = "Nombre de la presentación")]
        public string NombrePresentacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Complete unidad de medida.")]
        [StringLength(45, ErrorMessage = "El campo admite hasta 45 caracteres.")]
        [Display(Name = "Unidad de medida")]
        public string UnidadMedida { get; set; } = string.Empty;
    }
}
