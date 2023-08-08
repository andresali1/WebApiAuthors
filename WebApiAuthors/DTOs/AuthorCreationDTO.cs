using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.DTOs
{
    public class AuthorCreationDTO
    {
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 80, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [FirstCapitalLetter]
        public string A_Name { get; set; }
    }
}
