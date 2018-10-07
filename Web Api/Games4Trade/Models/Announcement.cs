
using System;

namespace Games4Trade.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public int? UserId { get; set; }
        public virtual User User { get; set; }
    }
}
