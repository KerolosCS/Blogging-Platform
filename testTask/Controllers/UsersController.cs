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

        private readonly ILogger<UsersController> _logger;

        
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext db , ILogger<UsersController> logger)
        {
            _logger = logger;
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
        public  async Task<ActionResult<UserDTO>>GetUser(int id) 
        {

            var user = await _context.Users
                            .Include(u => u.Posts)
                            .ThenInclude(p => p.Comments)
                            .Include(u => u.Comments)
                             .Include(u => u.Followers)
                            .Include(u => u.Following)
                           
                            .SingleOrDefaultAsync(u => u.Id == id);




            if (user == null)
            {
                return NotFound();
            }
            _logger.LogInformation($"My Log ---------------------------- >>> {user.Followers.ToList().Count()}");
            if (user.Followers.ToList().Count()>0)
            {
            _logger.LogInformation($"My Log ---------------------------- user.Followers.ToList()[0].FollowerId >>> {user.Followers.ToList()[0].FollowerId}");
            _logger.LogInformation($"My Log ---------------------------- user.Followers.ToList()[0].FollowerUser >>> {user.Followers.ToList()[0].FollowerUser}");
            _logger.LogInformation($"My Log ---------------------------- user.Followers.ToList()[0].FollowedId >>> {user.Followers.ToList()[0].FollowedId}");
            _logger.LogInformation($"My Log ---------------------------- user.Followers.ToList()[0].FollowedUser >>> {user.Followers.ToList()[0].FollowedUser}");

            }
            _logger.LogInformation($"My Log ---------------------------- >>> {user.Following.ToList().Count()}");
            List<CommentDTO> commentDTOs = user.Comments.Select(co => new CommentDTO
            {
                Id = co.Id,
                PostId = co.PostId,
                CommentContent = co.Text,
                UserId = co.UserId,
            }).ToList();

            List<FollowerDTO> followersDTOs = user.Followers.Select(f => new FollowerDTO
            {
                Id = f.Id,
                FollowedId = f.FollowedId,
                FollowerId = f.FollowerId,
            }).ToList();

            List<FollowerDTO> followingDTOs = user.Following.Select(f => new FollowerDTO
            {
                Id = f.Id,
                FollowedId = f.FollowedId,
                FollowerId = f.FollowerId,
            }).ToList();

            List<PostDTO> postDTOs = user.Posts.Select(post => new PostDTO
            {
                Id = post.Id,
                
                authorName = user.Username,
                Title = post.Title,
                Content = post.Content,
                CreationDate = post.CreationDate,
                Comments = post.Comments.Select(c => new CommentDTO
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    CommentContent = c.Text,
                    UserId = c.UserId,
                }).ToList(),
            }).ToList();

            UserDTO userDTO = new UserDTO()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Comments = commentDTOs,
                Following = followingDTOs.Where(f => f.FollowerId == user.Id).ToList(),
                Followers = followersDTOs.Where(f => f.FollowedId == user.Id).ToList(),
                Posts = postDTOs,
            };

            return Ok(userDTO);
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
