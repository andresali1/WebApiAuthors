using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;

        public CommentController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Method to get a comment by the book id
        /// </summary>
        /// <param name="bookId">Id of the book</param>
        /// <returns></returns>
        [HttpGet(Name = "GetCommentsBook")]
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

        /// <summary>
        /// Method to get a comment by its Id
        /// </summary>
        /// <param name="id">Id of the comment</param>
        /// <returns></returns>
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

        /// <summary>
        /// Method to create a comment
        /// </summary>
        /// <param name="bookId">Id of the book to comment</param>
        /// <param name="commentCreationDTO">CommentCreationDTO object with the data</param>
        /// <returns></returns>
        [HttpPost(Name = "createComment")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int bookId, CommentCreationDTO commentCreationDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var user = await _userManager.FindByEmailAsync(email);
            var userId = user.Id;

            var bookExists = await _context.Book.AnyAsync(b => b.Id == bookId);

            if (!bookExists)
            {
                return NotFound();  
            }

            var comment = _mapper.Map<Comment>(commentCreationDTO);
            comment.BookId = bookId;
            comment.UserId = userId;

            _context.Add(comment);
            await _context.SaveChangesAsync();

            var commentDTO = _mapper.Map<CommentDTO>(comment);

            return CreatedAtRoute("getComment", new { id = comment.Id, bookId }, commentDTO);
        }

        /// <summary>
        /// Method to update all the data of a comment
        /// </summary>
        /// <param name="bookId">Id of the book</param>
        /// <param name="id">Id of the comment</param>
        /// <param name="commentCreationDTO">CommentCreationDTO object with the data</param>
        /// <returns></returns>
        [HttpPut("{id:int}", Name = "updateComment")]
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
