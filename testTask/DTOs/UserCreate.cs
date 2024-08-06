using System.ComponentModel.DataAnnotations;

namespace testTask.DTOs
{
    public class UserCreate
    {
        [Required]
        public string Username { get; set; }
        [Required]
      
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
