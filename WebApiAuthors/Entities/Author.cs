namespace WebApiAuthors.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string A_Name { get; set; }
        public List<Book> Books { get; set; }
    }
}
