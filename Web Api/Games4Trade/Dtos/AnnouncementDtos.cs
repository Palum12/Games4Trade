using System;

namespace Games4TradeAPI.Dtos
{

    public class AnnouncementGetDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class AnnouncementArchiveDto
    {
        public bool IsActive { get; set; }
    }

    public class AnnouncementSaveDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

}
