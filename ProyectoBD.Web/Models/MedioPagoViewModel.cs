using System.ComponentModel.DataAnnotations;

namespace ProyectoBD.Web.Models
{
    public class MedioPagoViewModel
    {
        public int ID_MetodoP { get; set; }

        [Required(ErrorMessage = "Complete nombre del medio de pago.")]
        [StringLength(45, ErrorMessage = "El campo admite hasta 45 caracteres.")]
        [Display(Name = "Medio de pago")]
        public string NombreMetodo { get; set; } = string.Empty;
    }
}

