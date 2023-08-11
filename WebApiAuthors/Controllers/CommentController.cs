using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/book/{bookId:int}/comment")]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CommentController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDTO>>> Get(int bookId)
        {
            var bookExists = await _context.Book.AnyAsync(b => b.Id == bookId);

            if (!bookExists)
            {
                return NotFound();
            }

            var comments = await _context.Comment.Where(c => c.BookId == bookId).ToListAsync();
            return _mapper.Map<List<CommentDTO>>(comments);
        }

        [HttpGet("{id:int}", Name = "getComment")]
        public async Task<ActionResult<CommentDTO>> GetById(int id)
        {
            var comment = await _context.Comment.FirstOrDefaultAsync(c => c.Id == id);

            if(comment == null)
            {
                return NotFound();
            }

            return _mapper.Map<CommentDTO>(comment);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int bookId, CommentCreationDTO commentCreationDTO)
        {
            var bookExists = await _context.Book.AnyAsync(b => b.Id == bookId);

            if (!bookExists)
            {
                return NotFound();
            }

            var comment = _mapper.Map<Comment>(commentCreationDTO);
            comment.BookId = bookId;

            _context.Add(comment);
            await _context.SaveChangesAsync();

            var commentDTO = _mapper.Map<CommentDTO>(comment);

            return CreatedAtRoute("getComment", new { id = comment.Id, bookId }, commentDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int bookId, int id, CommentCreationDTO commentCreationDTO)
        {
            var bookExists = await _context.Book.AnyAsync(b => b.Id == bookId);
            var commentExists = await _context.Comment.AnyAsync(c => c.Id == id);

            if (!bookExists || !commentExists)
            {
                return NotFound();
            }

            var comment = _mapper.Map<Comment>(commentCreationDTO);
            comment.Id = id;
            comment.BookId = bookId;

            _context.Update(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
