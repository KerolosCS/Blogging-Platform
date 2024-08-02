using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using testTask.Data;
using testTask.Models;
namespace testTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext db)
        {
            _context = db;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(UserCreate userCreate)
        {
            if (_context.Users.FirstOrDefault(u => u.Email == userCreate.Email) != null || _context.Users.FirstOrDefault(u => u.Username == userCreate.Username) != null)
            {
                return BadRequest("Email or Username already exists.");
            }

            User user = new User()
            {
                Username = userCreate.Username,
                Email = userCreate.Email,   
                Password = userCreate.Password,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
             return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpGet("GetUser/{id}")] 
        public   ActionResult<UserDTO>GetUser(int id) 
        {

            User? user =  _context.Users.Include(u => u.Posts)
                                         .Include(p => p.Comments)
                                         .Include(u => u.Following)
                                         .Include(u => u.Followers)
                                         
                                         .SingleOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

           
            // List<CommentDTO> cemmentDTOs = user.Comments.Select(co => new CommentDTO
            //{
            //     Id = co.Id,
            //     PostId = co.Id,
            //     Text = co.Text,
            //     UserId = co.UserId,

            //}).ToList();
            //List<PostDTO> postDTOs = user.Posts.Select(post => new PostDTO
            //{
            //    Id = post.Id,
            //    Title = post.Title,
            //    Content = post.Content,
            //    CreationDate = post.CreationDate,
            //    Comments = cemmentDTOs,  
            //}).ToList();

            UserDTO userDTO = new UserDTO()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Comments = user.Comments,
                Followers = user.Followers,
                //Following = user.Following,

                Posts = user.Posts,
            };
            return  Ok(userDTO);
        }

        [HttpPut("Update/{id}")]
        public  IActionResult UpdateUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;
            try
            {
                 _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(u => u.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw ;
                }
            }
            return NoContent();
        }

        //[HttpDelete("{id}")]
        //public IActionResult DeleteUser(int id)
        //{
        //    var user =  _context.Users.SingleOrDefault(u=> u.Id==id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Users.Remove(user);
        //     _context.SaveChangesAsync();
        //    return NoContent();
        //}

        [HttpPost("{id}/follow")]
        public async Task<IActionResult> FollowUser(int id, int followId)
        {
            if (id == followId)
            {
                return BadRequest("Users cannot follow themselves.");
            }

            var follower = new Follower
            {
                FollowerId = id,  // Person Who will follow
                FollowedId = followId  // person who will be followeddddd
            };

            _context.Followers.Add(follower);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
