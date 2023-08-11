using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Display(Name = "Título")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [FirstCapitalLetter]
        [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        public string Title { get; set; }

        public DateTime? PublicationDate { get; set; }

        public List<Comment> Comments { get; set; }

        public List<Author_Book> Author_Book { get; set; }
    }
}
