﻿using System.ComponentModel.DataAnnotations;

namespace testTask.Models
{
    public class Comment
    {
        public int Id { get; set; }


        
        public int PostId { get; set; }
        public Post? Post { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
        [Required] 
        public string Text { get; set; }



        public DateTime CreationDate { get; set; }
    }
}
