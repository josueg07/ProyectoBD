using System.ComponentModel.DataAnnotations;

namespace ProyectoBD.Web.Models
{
    public class MarcaViewModel
    {
        public int ID_Marca { get; set; }

        [Required(ErrorMessage = "Complete nombre de la marca.")]
        [StringLength(45, ErrorMessage = "El campo admite hasta 45 caracteres.")]
        [Display(Name = "Nombre de la marca")]
        public string NombreMarca { get; set; } = string.Empty;
    }
}
