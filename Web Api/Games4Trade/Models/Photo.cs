using System;

namespace Games4Trade.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public DateTime DateCreated { get; set; }
        public int? AdvertisementId { get; set; }
        public virtual Advertisement Advertisement { get; set; }
        public virtual User User { get; set; }
    }
}
