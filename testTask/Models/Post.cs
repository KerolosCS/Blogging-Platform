namespace testTask.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        public int AuthorId { get; set; }
        public User? Author { get; set; }

        public ICollection<Comment>? Comments { get; set; }
    }
}
