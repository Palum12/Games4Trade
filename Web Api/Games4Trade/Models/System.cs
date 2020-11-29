using System.Collections.Generic;

namespace Games4TradeAPI.Models
{
    public class System : ModelBase
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public virtual ICollection<UserOwnedSystem> OwnedByUsers { get; set; }
        public virtual ICollection<AdvertisementItem> AdvertisementItems { get; set; }
    }
}
