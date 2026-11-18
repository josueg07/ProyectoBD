using System.ComponentModel.DataAnnotations;

namespace ProyectoBD.Web.Models
{
    public class CambiarPasswordViewModel
    {
        public int ID_Usuario { get; set; }

        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la nueva contraseña.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NuevaPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme la nueva contraseña.")]
        [DataType(DataType.Password)]
        [Compare(
            nameof(NuevaPassword),
            ErrorMessage = "Las contraseñas no coinciden."
        )]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}
