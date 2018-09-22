using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Games4Trade.Dtos
{

    public class AnnouncementGetDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class AnnouncementSaveDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

}
