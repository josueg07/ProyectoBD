using System.ComponentModel.DataAnnotations;

namespace ProyectoBD.Web.Models
{
    public class ClienteViewModel
    {
        public int ID_Cliente { get; set; }

        [Required(ErrorMessage = "Complete tipo de cliente.")]
        [StringLength(45, ErrorMessage = "El campo admite hasta 45 caracteres.")]
        [Display(Name = "Tipo de cliente")]
        [RegularExpression("^(Particular|Empresa)$", ErrorMessage = "Seleccione Particular o Empresa.")]
        public string TipoCliente { get; set; } = "Particular";

        [Required(ErrorMessage = "Complete nombre o razón social.")]
        [StringLength(120, ErrorMessage = "El campo admite hasta 120 caracteres.")]
        [Display(Name = "Nombre o razón social")]
        public string NombreRazonSocial { get; set; } = string.Empty;
    }
}
