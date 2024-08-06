using System.ComponentModel.DataAnnotations;

namespace testTask.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        
        public int AuthorId { get; set; }
        public User? Author { get; set; }

        public ICollection<Comment>? Comments { get; set; }
    }
}
