using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Serilog;
using testTask.Data;
using testTask.DTOs;
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
                Log.Information("User try to create acount with used name or email");
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

        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users
                                        .Include(u => u.Posts)
                                        .ThenInclude(p => p.Comments)
                                        .Include(u => u.Comments)
                                        .SingleOrDefaultAsync(u => u.Id == id);
           
           

            if (user == null)
            {
                return NotFound();
            }
            
            var followedUserIds = await _context.Followers
        .Where(f => f.FollowerId == id)
        .Select(f => f.FollowedId)
        .ToListAsync();

           
            var followedUserPosts = await _context.Posts
                .Where(p => followedUserIds.Contains(p.AuthorId))
                .Include(p => p.Comments)
                .Include(p => p.Author) 
                .ToListAsync();

            var allfollowers = await _context.Followers
                .Include(f => f.FollowerUser)
                .Include(f => f.FollowedUser)
                .ToListAsync();
        
            ;

            var commentDTOs = user.Comments.Select(co => new CommentDTO
            {
                Id = co.Id,
                PostId = co.PostId,
                CommentContent = co.Text,
                UserId = co.UserId,
            }).ToList();

            var allfollowersDTOs = allfollowers.Select(f => new FollowerDTO
            {
                FollowedId = f.FollowedUser.Id,
                FollowerId = f.FollowerUser.Id,
                Posts = followedUserPosts.Select(p=> new PostDTO {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                AuthorId = p.AuthorId,
                authorName = p.Author.Username,
                Comments = p.Comments.Select(c => new CommentDTO
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    CommentContent = c.Text,
                    UserId = c.UserId,
                    CommenterEmail = c.User.Username,
                    Name = c.User.Username,
                    Created = c.CreationDate,

                }).ToList(),
                
                }).ToList(),

            }).ToList();
          
            var postDTOs = user.Posts.Select(post => new PostDTO
            {
                Id = post.Id,
                authorName = user.Username,
                AuthorId = user.Id,
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

            var userDTO = new UserDTO()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Comments = commentDTOs,
                Following = allfollowersDTOs.Where(f => f.FollowerId == user.Id).ToList(),
                Followers = allfollowersDTOs.Where(f => f.FollowedId == user.Id).ToList(),


                Posts = postDTOs,
            };




            return Ok(userDTO);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserCreate userUpdate)
        {
            var user =  await _context.Users
                          .AsNoTracking()
                          .SingleOrDefaultAsync(u => u.Id == id);

            if (id != user?.Id)
            {
                return BadRequest();
            }

            User userUpdated = new User()
            {
              Username = userUpdate.Username,
              Email = userUpdate.Email,
              Id = user.Id,
              Password = userUpdate.Password,   
            };

             _context.Entry(userUpdated).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
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
            return Ok("Updated");
        }


        [HttpPost("{id}/follow")]
        public async Task<IActionResult> FollowUser(int id, int followId)
        {
            var user = await _context.Users.FindAsync(id);
            var followedUser = await _context.Users.FindAsync(followId);

            if (user == null || followedUser == null || id == user.Id)
            {
                return NotFound("One or both users not found.");
            }

            var follower = new Follower
            {
                FollowerId = id,  // Person who will follow
                FollowedId = followId  // Person who will be followed
            };

            _context.Followers.Add(follower);
            await _context.SaveChangesAsync();

            return NoContent();
        }
   
    }
}
