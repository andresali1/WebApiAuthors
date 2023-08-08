using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //[controller] is changed for the prefix of the controller => route: 'api/Author'
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuthorController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet] //Get: api/author
        public async Task<ActionResult<List<AuthorDTO>>> Get()
        {
            //For testing the Exception filter Uncomment
            //throw new NotImplementedException();

            var authors = await _context.Author.ToListAsync();
            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        [HttpGet("{id:int}", Name = "getAuthor")] //Get: api/author/{id}
        public async Task<ActionResult<AuthorDTO_Book>> Get(int id)
        {
            var author = await _context.Author
            .Include(authorDB => authorDB.Author_Book)
            .ThenInclude(authorBookDB => authorBookDB.Book)
            .FirstOrDefaultAsync(a => a.Id == id);

            if (author is null)
            {
                return NotFound();
            }

            return _mapper.Map<AuthorDTO_Book>(author);
        }

        [HttpGet("{name}")] //Get: api/author/{name}
        public async Task<ActionResult<List<AuthorDTO>>> Get(string name)
        {
            var authors = await _context.Author.Where(a => a.A_Name.Contains(name)).ToListAsync();

            if (authors.Count < 1)
            {
                return NotFound();
            }

            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        [HttpPost] //Post: api/author
        public async Task<ActionResult> Post([FromBody] AuthorCreationDTO authorCreationDTO)
        {
            var sameNameExists = await _context.Author.AnyAsync(a => a.A_Name == authorCreationDTO.A_Name);

            if (sameNameExists)
            {
                return BadRequest($"Ya existe un autor con el nombre {authorCreationDTO.A_Name}");
            }

            var author = _mapper.Map<Author>(authorCreationDTO);

            _context.Add(author);
            await _context.SaveChangesAsync();

            var authorDTO = _mapper.Map<AuthorDTO>(author);

            return CreatedAtRoute("getAuthor", new { id = author.Id }, authorDTO);
        }

        [HttpPut("{id:int}")] //Put: api/author/{id}
        public async Task<ActionResult> Put(Author author, int id)
        {
            if (author.Id != id)
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
