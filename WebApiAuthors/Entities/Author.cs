using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities
{
    public class Author : IValidatableObject
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 80, ErrorMessage = "El campo {0} no debe tener mas de {1} caracteres")]
        //[FirstCapitalLetter]
        public string A_Name { get; set; }

        [NotMapped] //Using EF core is required to map all properties to db, with NotMapped we can avoid this requirement
        [Range(18, 100, ErrorMessage = "El autor debe tener entre {1} y {2} años")] //The position 0 is always for display name, then is followed by the validation variables in order
        public int Age { get; set; }

        public List<Book> Books { get; set; }

        //Custom validations: to get them in the result, the user has to pass all the DataAnnotations validations before
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(A_Name))
            {
                var firstLetter = A_Name[0].ToString();

                if (firstLetter != firstLetter.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayúscula", new string[] { nameof(A_Name) });
                }
            }
        }
    }
}
