using System;

namespace Games4Trade.Dtos
{
    public class NewestMessageDto
    {
        public string Content { get; set; }
        public bool IsDelivered { get; set; }
        public int OtherUserId { get; set; }
        public UserSimpleDto OtherUser { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsDelivered { get; set; }
        public DateTime DateCreated { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
    }

    public class MessagePostDto
    {
        public int ReceiverId { get; set; }
        public string Content { get; set; }
    }
}
