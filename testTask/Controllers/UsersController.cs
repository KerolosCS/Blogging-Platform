using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext db, ILogger<UsersController> logger,
                        IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _context = db;
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]  
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        

        public async Task<IActionResult> Login(LoginRequestDTO loginRequest)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email.ToLower() == loginRequest.Email.ToLower());
            bool isValid = user.Password == loginRequest.Password ? true : false;

            if (user == null || isValid == false)
            {
                
                return BadRequest("Email Or password is invalid");
            }

            var token = GenerateJwtToken(loginRequest.Email, user.Id);
            Log.Information($"Authorized user logged in Succesfully {user.Username} : {user.Id}");
            return Ok(new LoginResponseDTO()
            {
                Email = loginRequest.Email,
                UserName = user.Username,
                Token = token

            });


        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
                Posts = followedUserPosts.Select(p => new PostDTO
                {
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUser(int id, UserCreate userUpdate)
        {
            var user = await _context.Users
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
                    throw;
                }
            }
            Log.Information($"User Updated his credentials succsessfully {user.Username} : {user.Id}");
            return NoContent();
        }


        [HttpPost("{id}/follow")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> FollowUser(int id, int followId)
        {
            var user = await _context.Users.FindAsync(id);
            var followedUser = await _context.Users.FindAsync(followId);
            if (id == followId) {
                return BadRequest("Users can not follow themselves");
            }

            if (user == null || followedUser == null)
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
            Log.Information($" {user.Username} : {user.Id} starts follow {followedUser.Username} : {followedUser.Id}");
            return NoContent();
        }
        private string GenerateJwtToken(string username, int id)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtTokenId = $"JTI{Guid.NewGuid()}";
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: new[]
                {
                   new Claim(ClaimTypes.Name, username.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
                    new Claim(JwtRegisteredClaimNames.Sub, Convert.ToString(id)),
                },
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
