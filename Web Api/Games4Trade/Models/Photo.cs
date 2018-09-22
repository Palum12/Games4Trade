using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Games4Trade.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public DateTime DateCreated { get; set; }

        public int AdvertisementId { get; set; }
        public virtual Advertisement Advertisement { get; set; }

        public virtual User User { get; set; }
    }
}
