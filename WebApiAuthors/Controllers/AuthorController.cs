using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Entities;
using WebApiAuthors.Filters;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //[controller] is changed for the prefix of the controller => route: 'api/Author'
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthorController> logger;

        public AuthorController(ApplicationDbContext context, ILogger<AuthorController> logger)
        {
            _context = context;
            this.logger = logger;
        }

        //Example multiple routes for one endpoint at the same time:
        [HttpGet] //Get: api/author
        [HttpGet("list")] //Get: api/author/list
        [HttpGet("/list")] //Get: /list => ignores the controller base route
        [ResponseCache(Duration = 10)]
        public async Task<ActionResult<List<Author>>> Get()
        {
            //For testing the Exception filter Uncomment
            //throw new NotImplementedException();

            logger.LogInformation("Estamos obteniendo los autores");
            return await _context.Author.Include(a => a.Books).ToListAsync();
        }

        [HttpGet("first")] //Get: api/author/first
        [ServiceFilter(typeof(MyActionFilter))]
        public async Task<ActionResult<Author>> FirstAuthor()
        {
            return await _context.Author.Include(a => a.Books).FirstOrDefaultAsync();
        }

        [HttpGet("{id:int}")] //Get: api/author/{id}
        [ServiceFilter(typeof(MyActionFilter))]
        public async Task<ActionResult<Author>> Get(int id)
        {
            var author = await _context.Author.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);

            if(author is null)
            {
                return NotFound();
            }

            return author;
        }

        [HttpGet("{name}")] //Get: api/author/{name}
        public async Task<ActionResult<Author>> Get(string name)
        {
            var author = await _context.Author.Include(a => a.Books).FirstOrDefaultAsync(a => a.A_Name.Contains(name));

            if (author is null)
            {
                return NotFound();
            }

            return author;
        }

        [HttpPost] //Post: api/author
        public async Task<ActionResult> Post(Author author)
        {
            var sameNameExists = await _context.Author.AnyAsync(a => a.A_Name == author.A_Name);

            if (sameNameExists)
            {
                return BadRequest($"Ya existe un autor con el nombre {author.A_Name}");
            }

            _context.Add(author);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")] //Put: api/author/{id}
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

        [HttpDelete("{id:int}")] //Delete: api/author/{id}
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
