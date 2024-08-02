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

        [HttpGet]
        public ActionResult<IEnumerable<Comment>> GetComments()
        {
            var comments = _context.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                ;
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public ActionResult<Comment> GetComment(int id)
        {
            var comment = _context.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                .SingleOrDefault(c => c.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        [HttpPost]
        public ActionResult<Comment> CreateComment(Comment comment)
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateComment(int id, Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteComment(int id)
        {
            var comment = _context.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
