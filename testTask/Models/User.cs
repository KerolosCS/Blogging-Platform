namespace testTask.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
      
        
        
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Follower>? Following { get; set; }
        public ICollection<Follower>? Followers { get; set; }
    }
}
