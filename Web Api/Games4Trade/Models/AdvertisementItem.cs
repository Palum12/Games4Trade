using System;

namespace Games4TradeAPIAPI.Models
{
    public class AdvertisementItem : ModelBase
    {
        public DateTime? DateReleased { get; set; }
        public string Description { get; set; }
        public int AdvertisementId { get; set; }
        public Advertisement Advertisement { get; set; }
        public int SystemId { get; set; }
        public virtual System System { get; set; }
        public int StateId { get; set; }
        public virtual State State { get; set; }
    }
}
