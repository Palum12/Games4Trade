using System.Collections.Generic;

namespace Games4Trade.Models
{
    public class System
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public virtual ICollection<UserOwnedSystem> OwnedByUsers { get; set; }
        public virtual ICollection<AdvertisementItem> AdvertisementItems { get; set; }
    }
}
