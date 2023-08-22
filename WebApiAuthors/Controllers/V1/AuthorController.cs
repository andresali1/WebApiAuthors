﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;
using WebApiAuthors.Utilities;

namespace WebApiAuthors.Controllers.V1
{
    [ApiController]
    //[Route("api/v1/[controller]")] //[controller] is changed for the prefix of the controller => route: 'api/Author'
    [Route("api/[controller]")]
    [IsPresentHeader("x-version", "1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    [ApiConventionType(typeof(DefaultApiConventions))] //To use conventions to document the most common response types for all the methods in this class
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
        /// <param name="paginationDTO">Object to indicate the records to receive and the page from Query</param>
        /// <returns></returns>
        [HttpGet(Name = "getAuthorsv1")] //Get: api/author
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        public async Task<ActionResult<List<AuthorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            //For testing the Exception filter Uncomment
            //throw new NotImplementedException();

            var queryable = _context.Author.AsQueryable();
            await HttpContext.InsertPaginationParamsInHeaders(queryable);

            var authors = await queryable.OrderBy(author => author.A_Name).Page(paginationDTO).ToListAsync();
            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        /// <summary>
        /// Method to get an Author by its Id
        /// </summary>
        /// <param name="id">Id of the author</param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = "getAuthorv1")] //Get: api/author/{id}
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        [ProducesResponseType(200)] //To document the posible response type of this endpoint
        [ProducesResponseType(404)]
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
        [HttpGet("{name}", Name = "getAuthorByNamev1")] //Get: api/author/{name}
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
        [HttpPost(Name = "createAuthorv1")] //Post: api/author
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
        [HttpPut("{id:int}", Name = "updateAuthorv1")] //Put: api/author/{id}
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
        [HttpDelete("{id:int}", Name = "deleteAuthorv1")] //Delete: api/author/{id}
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
