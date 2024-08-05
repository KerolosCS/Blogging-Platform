namespace testTask.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }



        public ICollection<PostDTO>? Posts { get; set; }
        public ICollection<CommentDTO>? Comments { get; set; }
        public ICollection<FollowerDTO>? Followers { get; set; }
        public ICollection<FollowerDTO>? Following { get; set; }
    }
}
