using System;

namespace Games4TradeAPI.Models
{
    public class Announcement : ModelBase
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public int? UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
