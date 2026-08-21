using System;

namespace Games4TradeAPI.Models
{
    public class Message : ModelBase
    {
        public int SenderId { get; set; }
        public virtual User Sender { get; set; } = null!;
        public int ReceiverId { get; set; }
        public virtual User Receiver { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool IsDelivered { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
