﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers.V1
{
    [ApiController]
    [Route("api/v1/book")]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BookController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Method to get a book by its Id
        /// </summary>
        /// <param name="id">Id of the book</param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = "getBook")] //Get: api/book/{id}
        public async Task<ActionResult<BookDTO_Author>> Get(int id)
        {
            var book = await _context.Book
                .Include(bookDB => bookDB.Author_Book)
                .ThenInclude(authorBookDB => authorBookDB.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            book.Author_Book = book.Author_Book.OrderBy(ab => ab.OrderNum).ToList();

            return _mapper.Map<BookDTO_Author>(book);
        }

        /// <summary>
        /// Method to create a new book
        /// </summary>
        /// <param name="bookCreationDTO">BookCreationDTO object with data</param>
        /// <returns></returns>
        [HttpPost(Name = "createBook")] //Post: api/book
        public async Task<ActionResult> Post(BookCreationDTO bookCreationDTO)
        {
            if (bookCreationDTO.AuthorIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var authorIds = await _context.Author
                .Where(authorBD => bookCreationDTO.AuthorIds.Contains(authorBD.Id))
                .Select(x => x.Id).ToListAsync();

            if (bookCreationDTO.AuthorIds.Count != authorIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var book = _mapper.Map<Book>(bookCreationDTO);

            AsignAuthorOrder(book);

            _context.Add(book);
            await _context.SaveChangesAsync();

            var bookDTO = _mapper.Map<BookDTO>(book);

            return CreatedAtRoute("getBook", new { id = book.Id }, bookDTO);
        }

        /// <summary>
        /// Method to update all the data of a book
        /// </summary>
        /// <param name="id">Id of the book</param>
        /// <param name="bookCreationDTO">BookCreationDTO object with the data</param>
        /// <returns></returns>
        [HttpPut("{id:int}", Name = "updateBook")]
        public async Task<ActionResult> Put(int id, BookCreationDTO bookCreationDTO)
        {
            var bookDb = await _context.Book
                .Include(b => b.Author_Book)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bookDb == null)
            {
                return NotFound();
            }

            bookDb = _mapper.Map(bookCreationDTO, bookDb);

            AsignAuthorOrder(bookDb);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Method to order the authors of a book
        /// </summary>
        /// <param name="book">Given book</param>
        private void AsignAuthorOrder(Book book)
        {
            if (book.Author_Book != null)
            {
                for (int i = 0; i < book.Author_Book.Count; i++)
                {
                    book.Author_Book[i].OrderNum = i;
                }
            }
        }

        /// <summary>
        /// Method to update only needed attributes of a given book
        /// </summary>
        /// <param name="id">Id of the book</param>
        /// <param name="patchDocument">Json object with the data to update</param>
        /// <returns></returns>
        [HttpPatch("{id:int}", Name = "patchBook")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var bookDb = await _context.Book.FirstOrDefaultAsync(b => b.Id == id);

            if (bookDb == null)
            {
                return NotFound();
            }

            var bookDTO = _mapper.Map<BookPatchDTO>(bookDb);

            patchDocument.ApplyTo(bookDTO, ModelState);

            var isValid = TryValidateModel(bookDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(bookDTO, bookDb);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// MEthod to delete a book
        /// </summary>
        /// <param name="id">Id of the book</param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "deleteBook")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Book.AnyAsync(y => y.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            _context.Remove(new Book() { Id = id });
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
