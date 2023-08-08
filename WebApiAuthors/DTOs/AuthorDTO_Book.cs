namespace WebApiAuthors.DTOs
{
    public class AuthorDTO_Book : AuthorDTO
    {
        public List<BookDTO> Books { get; set; }
    }
}
