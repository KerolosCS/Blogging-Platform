namespace testTask.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }

        public int PostId { get; set; }
        public int UserId { get; set; }
        public string CommentContent { get; set; }

        public string Name { get; set; }

        public string CommenterEmail { get; set; }


        public DateTime Created { get; set; }
    }
}
