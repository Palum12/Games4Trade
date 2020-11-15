using System;

namespace Games4TradeAPIAPI.Models
{
    public class Announcement : ModelBase
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public int? UserId { get; set; }
        public virtual User User { get; set; }
    }
}
