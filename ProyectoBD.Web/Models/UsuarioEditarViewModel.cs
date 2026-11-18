using System.ComponentModel.DataAnnotations;

namespace ProyectoBD.Web.Models
{
    public class UsuarioEditarViewModel
    {
        public int ID_Usuario { get; set; }

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

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un rol.")]
        [Display(Name = "Rol")]
        public int ID_Rol { get; set; }
    }
}
