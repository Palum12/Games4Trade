using System;

namespace Games4TradeAPI.Models
{
    public class Photo : ModelBase
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public DateTime DateCreated { get; set; }
        public int? AdvertisementId { get; set; }
        public virtual Advertisement Advertisement { get; set; }
        public virtual User User { get; set; }
    }
}
