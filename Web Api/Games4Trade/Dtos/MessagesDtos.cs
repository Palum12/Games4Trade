using System;

namespace Games4Trade.Dtos
{
    public class NewestMessageDto
    {
        public string Content { get; set; }
        public bool IsDelivered { get; set; }
        public int SenderId { get; set; }
        public UserSimpleDto Sender { get; set; }
        public int ReciverId { get; set; }
        public UserSimpleDto Reciver { get; set; }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsDelivered { get; set; }
        public DateTime DateCreated { get; set; }
        public int SenderId { get; set; }
        public UserSimpleDto Sender { get; set; }
        public int ReciverId { get; set; }
        public UserSimpleDto Reciver { get; set; }
    }

    public class MessagePostDto
    {
        public int SenderId { get; set; }
        public int ReciverId { get; set; }
        public string Content { get; set; }
    }
}
