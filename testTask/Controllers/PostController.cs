using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testTask.Data;
using testTask.Models;

namespace testTask.Controllers
{
   
        [ApiController]
        [Route("api/[controller]")]
        public class PostController : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public PostController(ApplicationDbContext db)
            {
                _context = db;
            }

            [HttpGet]
            public ActionResult<IEnumerable<Post>> GetPosts()
            {
                var posts = _context.Posts.Include(p => p.Comments);
                return Ok (posts);
            }

            [HttpGet("{id}")]
            public ActionResult<Post> GetPost(int id)
            {
                var post = _context.Posts
                    .Include(p => p.Comments)
                    .SingleOrDefault(p => p.Id == id);

                if (post == null)
                {
                    return NotFound();
                }

                return Ok(post);
            }

            [HttpPost]
            public ActionResult<Post> CreatePost(Post post)
            {
                _context.Posts.Add(post);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
            }

            [HttpPut("{id}")]
            public IActionResult UpdatePost(int id, Post post)
            {
                if (id != post.Id)
                {
                    return BadRequest();
                }

                _context.Entry(post).State = EntityState.Modified;
                _context.SaveChanges();

                return NoContent();
            }

            [HttpDelete("{id}")]
            public IActionResult DeletePost(int id)
            {
                var post = _context.Posts.Find(id);
                if (post == null)
                {
                    return NotFound();
                }

                _context.Posts.Remove(post);
                _context.SaveChanges();

                return NoContent();
            }
        }

    }

