using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/author")]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Author>>> Get()
        {
            return await _context.Author.Include(a => a.Books).ToListAsync();
        }

        [HttpGet("first")]
        public async Task<ActionResult<Author>> FirstAuthor()
        {
            return await _context.Author.Include(a => a.Books).FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Author author)
        {
            _context.Add(author);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")] // api/author/{id}
        public async Task<ActionResult> Put(Author author, int id)
        {
            if(author.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");
            }

            bool exists = await _context.Author.AnyAsync(x => x.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            _context.Update(author);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")] // api/author/{id}
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Author.AnyAsync(y => y.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            _context.Remove(new Author() { Id = id });
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
