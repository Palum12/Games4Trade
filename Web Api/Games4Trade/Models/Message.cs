using System;

namespace Games4TradeAPI.Models
{
    public class Message : ModelBase
    {
        public int SenderId { get; set; }
        public virtual User Sender { get; set; }
        public int ReceiverId { get; set; }
        public virtual User Receiver { get; set; }
        public string Content { get; set; }
        public bool IsDelivered { get; set; }
        public DateTime DateCreated { get; set; }
    }
}