using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/book")]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")] //Get: api/book/{id}
        public async Task<ActionResult<Book>> Get(int id)
        {
            return await _context.Book.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);
        }

        [HttpPost] //Post: api/book
        public async Task<ActionResult> Post(Book book)
        {
            var authorExists = await _context.Author.AnyAsync(a => a.Id == book.AuthorId);

            if(!authorExists)
            {
                return BadRequest($"No existe el autor con el Id: {book.AuthorId}");
            }

            _context.Add(book);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
