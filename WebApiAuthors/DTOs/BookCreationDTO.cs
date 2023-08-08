using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.DTOs
{
    public class BookCreationDTO
    {
        [Display(Name = "Título")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [FirstCapitalLetter]
        [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        public string Title { get; set; }

        public List<int> AuthorIds { get; set; }
    }
}
