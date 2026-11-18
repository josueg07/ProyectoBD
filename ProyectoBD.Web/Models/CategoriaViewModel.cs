using System.ComponentModel.DataAnnotations;

namespace ProyectoBD.Web.Models
{
    public class CategoriaViewModel
    {
        public int ID_Categoria { get; set; }

        [Required(ErrorMessage = "Complete nombre de la categoría.")]
        [StringLength(45, ErrorMessage = "El campo admite hasta 45 caracteres.")]
        [Display(Name = "Nombre de la categoría")]
        public string NombreCategoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "Complete descripción.")]
        [StringLength(60, ErrorMessage = "El campo admite hasta 60 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;
    }
}
