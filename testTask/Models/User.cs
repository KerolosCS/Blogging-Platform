using System.ComponentModel.DataAnnotations;

namespace testTask.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
      
        
        
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Follower>? Following { get; set; }
        public ICollection<Follower>? Followers { get; set; }
    }
}
