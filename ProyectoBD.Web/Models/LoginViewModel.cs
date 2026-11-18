using System.ComponentModel.DataAnnotations;

namespace ProyectoBD.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingrese el usuario.")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la contraseña.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
