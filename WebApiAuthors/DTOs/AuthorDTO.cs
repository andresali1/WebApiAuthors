using WebApiAuthors.Entities;

namespace WebApiAuthors.DTOs
{
    public class AuthorDTO : Resource
    {
        public int Id { get; set; }
        public string A_Name { get; set; }       
    }
}
