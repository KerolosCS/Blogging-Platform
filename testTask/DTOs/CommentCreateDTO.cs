using System.ComponentModel.DataAnnotations;

namespace testTask.DTOs
{
    public class CommentCreateDTO
    {
        public int Id { get; set; }

        public int PostId { get; set; }
        public int UserId { get; set; }
        [Required]
        public string CommentContent { get; set; }
    }
}
