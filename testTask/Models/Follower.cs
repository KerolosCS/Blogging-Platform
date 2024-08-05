namespace testTask.Models
{
    public class Follower
    {

        //public int Id { get; set; }



        public int FollowerId { get; set; }
        public User FollowerUser { get; set; }
        public int FollowedId { get; set; }
        public User FollowedUser { get; set; }
    }
}
