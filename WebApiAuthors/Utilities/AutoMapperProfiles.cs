using AutoMapper;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //<from, to>
            CreateMap<AuthorCreationDTO, Author>();
            CreateMap<Author, AuthorDTO>();
            CreateMap<Author, AuthorDTO_Book>()
                .ForMember(authorDTO => authorDTO.Books, options => options.MapFrom(MapAuthorDTOBook));

            CreateMap<BookCreationDTO, Book>()
                .ForMember(book => book.Author_Book, options => options.MapFrom(MapAuthor_Book));

            CreateMap<Book, BookDTO>();

            CreateMap<Book, BookDTO_Author>()
                .ForMember(bookDTO => bookDTO.Authors, options => options.MapFrom(MapBookDTOAuthor));

            CreateMap<BookPatchDTO, Book>().ReverseMap();

            CreateMap<CommentCreationDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
        }
        
        private List<Author_Book> MapAuthor_Book(BookCreationDTO bookCreationDTO, Book book)
        {
            var result = new List<Author_Book>();

            if(bookCreationDTO.AuthorIds == null) { return result; }

            foreach (var authorId in bookCreationDTO.AuthorIds)
            {
                result.Add(new Author_Book() { AuthorId = authorId });
            }

            return result;
        }

        private List<AuthorDTO> MapBookDTOAuthor(Book book, BookDTO bookDTO)
        {
            var result = new List<AuthorDTO>();

            if(book.Author_Book == null) { return result; }

            foreach (var author_book in book.Author_Book)
            {
                result.Add(new AuthorDTO()
                {
                    Id = author_book.AuthorId,
                    A_Name = author_book.Author.A_Name
                });
            }

            return result;
        }

        private List<BookDTO> MapAuthorDTOBook(Author author, AuthorDTO authorDTO)
        {
            var result = new List<BookDTO>();

            if(author.Author_Book == null) { return result; }

            foreach (var author_Book in author.Author_Book)
            {
                result.Add(new BookDTO()
                {
                    Id = author_Book.BookId,
                    Title = author_Book.Book.Title
                });
            }

            return result;
        }
    }
}
