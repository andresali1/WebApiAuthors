using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;
using WebApiAuthors.Utilities;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //[controller] is changed for the prefix of the controller => route: 'api/Author'
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public AuthorController(
            ApplicationDbContext context,
            IMapper mapper,
            IAuthorizationService authorizationService
        )
        {
            _context = context;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Method to get all the authors in db
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "getAuthors")] //Get: api/author
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        public async Task<ActionResult<List<AuthorDTO>>> Get()
        {
            //For testing the Exception filter Uncomment
            //throw new NotImplementedException();

            var authors = await _context.Author.ToListAsync();
            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        [HttpGet("{id:int}", Name = "getAuthor")] //Get: api/author/{id}
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
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

            var dto = _mapper.Map<AuthorDTO_Book>(author);

            return dto;
        }        

        /// <summary>
        /// Method to get an author by its name
        /// </summary>
        /// <param name="name">Name of the author</param>
        /// <returns></returns>
        [HttpGet("{name}", Name = "getAuthorByName")] //Get: api/author/{name}
        public async Task<ActionResult<List<AuthorDTO>>> GetByName(string name)
        {
            var authors = await _context.Author.Where(a => a.A_Name.Contains(name)).ToListAsync();

            if (authors.Count < 1)
            {
                return NotFound();
            }

            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        /// <summary>
        /// Method to create a new author
        /// </summary>
        /// <param name="authorCreationDTO">AuthorCreationDTO object with data</param>
        /// <returns></returns>
        [HttpPost(Name = "createAuthor")] //Post: api/author
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

        /// <summary>
        /// MEthod to update all the data of an Author
        /// </summary>
        /// <param name="authorCreationDTO">AuthorCreationDTO object with the data</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id:int}", Name = "updateAuthor")] //Put: api/author/{id}
        public async Task<ActionResult> Put(AuthorCreationDTO authorCreationDTO, int id)
        {
            bool exists = await _context.Author.AnyAsync(x => x.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            var author = _mapper.Map<Author>(authorCreationDTO);
            author.Id = id;

            _context.Update(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Method to delete an Author
        /// </summary>
        /// <param name="id">Id of the author</param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "deleteAuthor")] //Delete: api/author/{id}
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Author.AnyAsync(y => y.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            _context.Remove(new Author() { Id = id });
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
