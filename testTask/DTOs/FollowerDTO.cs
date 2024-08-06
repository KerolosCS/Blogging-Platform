namespace testTask.DTOs
{
    public class FollowerDTO
    {

       
        public int FollowerId { get; set; }

        public int FollowedId { get; set; }

       public ICollection<PostDTO>? Posts { get; set; }


    }
}
