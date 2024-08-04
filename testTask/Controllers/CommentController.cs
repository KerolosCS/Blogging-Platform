using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testTask.Data;
using testTask.Models;

namespace testTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext db)
        {
            _context = db;
        }

        [HttpGet("GetComments")]
        public ActionResult<IEnumerable<CommentDTO>> GetComments()
        {
            var comments = _context.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                ;
            List<CommentDTO> commentDTOs = comments.Select(c => new CommentDTO { 
            Id = c.Id,
            CommentContent = c.Text,
            PostId = c.PostId,
            UserId  = c.UserId,
            
            }).ToList();



            return Ok(commentDTOs);
        }

        [HttpGet("GetComment/{id}")]
        public ActionResult<CommentDTO> GetComment(int id)
        {
            var comment = _context.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                .SingleOrDefault(c => c.Id == id);

            CommentDTO commentDTO = new CommentDTO()
            {
                UserId = id,
                CommentContent = comment.Text,
                PostId = comment.PostId,
                Id = comment.Id,
            };

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(commentDTO);
        }

        [HttpPost("Create")]
        public async Task< ActionResult<Comment>> CreateComment(CommentDTO commentDto)


        {
            Comment comment =  new Comment()
            {
                Id = commentDto.Id,
                PostId= commentDto.PostId,
                UserId= commentDto.UserId,
                Text = commentDto.CommentContent,
                CreationDate = DateTime.Now,


            };
            _context.Comments.Add(comment);
           await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateComment(int id, CommentDTO commentDto)
        {
            if (id != commentDto.Id)
            {
                return BadRequest();
            }
            Comment comment = new Comment()
            {
                Id = commentDto.Id,
                PostId = commentDto.PostId,
                UserId = commentDto.UserId,
                Text = commentDto.CommentContent,
                CreationDate = DateTime.Now,

            };


            _context.Entry(comment).State = EntityState.Modified;
           await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = _context.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
         await   _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
