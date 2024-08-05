namespace testTask.DTOs
{
    public class PostCreateDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }


        public int AuthorId { get; set; }



        public string Content { get; set; }
        public DateTime CreationDate { get; set; }

        //public ICollection<CommentDTO>? Comments { get; set; }
    }
}
