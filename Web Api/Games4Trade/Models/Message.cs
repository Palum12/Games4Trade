using System;

namespace Games4Trade.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public virtual User Sender { get; set; }
        public int ReciverId { get; set; }
        public virtual User Reciver { get; set; }
        public string Content { get; set; }
        public bool IsDelivered { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
