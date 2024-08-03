namespace testTask.Models
{
    public class CommentDTO
    {
        public int Id { get; set; }

        public int PostId { get; set; }
        public int UserId { get; set; }
        public string CommentContent { get; set; }
    }
}
