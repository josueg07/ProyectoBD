using System.ComponentModel.DataAnnotations;

namespace ProyectoBD.Web.Models
{
    public class UsuarioCrearViewModel
    {
        [Required(ErrorMessage = "Ingrese los nombres.")]
        [StringLength(45)]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese los apellidos.")]
        [StringLength(45)]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el nombre de usuario.")]
        [StringLength(45)]
        [Display(Name = "Usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese una contraseña.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme la contraseña.")]
        [DataType(DataType.Password)]
        [Compare(
            nameof(Password),
            ErrorMessage = "Las contraseñas no coinciden."
        )]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarPassword { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un rol.")]
        [Display(Name = "Rol")]
        public int ID_Rol { get; set; }
    }
}
