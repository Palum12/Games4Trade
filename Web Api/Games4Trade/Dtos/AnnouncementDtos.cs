using System;

namespace Games4TradeAPI.Dtos
{

    public class AnnouncementGetDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class AnnouncementArchiveDto
    {
        public bool IsActive { get; set; }
    }

    public class AnnouncementSaveDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

}
