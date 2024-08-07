using System.ComponentModel.DataAnnotations;

namespace testTask.DTOs
{
    public class LoginResponseDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email {  get; set; }
        public string Token { get; set; }
    }
}
