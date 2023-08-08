namespace WebApiAuthors.Entities
{
    public class Author_Book
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        public int OrderNum { get; set; }
        public Book Book { get; set; }
        public Author Author { get; set; }
    }
}
