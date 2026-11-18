using System.ComponentModel.DataAnnotations;

namespace ProyectoBD.Web.Models
{
    public class RolViewModel
    {
        public int ID_Rol { get; set; }

        [Required(ErrorMessage = "Ingrese el nombre del rol.")]
        [StringLength(
            45,
            ErrorMessage = "El nombre del rol no puede superar 45 caracteres."
        )]
        [Display(Name = "Nombre del rol")]
        public string NombreRol { get; set; } = string.Empty;
    }
}
