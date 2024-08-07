using System.ComponentModel.DataAnnotations;

namespace testTask.DTOs
{
    public class UserCreate
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
