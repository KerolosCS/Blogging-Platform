using System.ComponentModel.DataAnnotations;

namespace testTask.DTOs
{
    public class PostCreateDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }


        public int AuthorId { get; set; }


        [Required]
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }

        //public ICollection<CommentDTO>? Comments { get; set; }
    }
}
