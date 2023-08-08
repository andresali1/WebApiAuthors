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
            return Ok();
        }
    }
}
