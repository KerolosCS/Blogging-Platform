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
        [StringLength(100, MinimumLength = 7, ErrorMessage = "Password must be at least 7 characters long.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{7,}$", ErrorMessage = "Password must be at least 7 characters long, and contain at least one letter, one number, and one special character.")]
        public string Password { get; set; }
    }
}
