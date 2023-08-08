namespace WebApiAuthors.DTOs
{
    public class BookDTO_Author : BookDTO
    {
        public List<AuthorDTO> Authors { get; set; }
    }
}
