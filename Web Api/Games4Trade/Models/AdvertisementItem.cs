using System;

namespace Games4TradeAPI.Models
{
    public class AdvertisementItem : ModelBase
    {
        public DateTime? DateReleased { get; set; }
        public string Description { get; set; } = null!;
        public int AdvertisementId { get; set; }
        public Advertisement Advertisement { get; set; } = null!;
        public int SystemId { get; set; }
        public virtual System System { get; set; } = null!;
        public int StateId { get; set; }
        public virtual State State { get; set; } = null!;
    }
}
