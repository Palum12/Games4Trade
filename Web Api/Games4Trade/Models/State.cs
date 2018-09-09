using System.Collections.Generic;

namespace Games4Trade.Models
{
    public class State
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public virtual ICollection<AdvertisementItem> AdvertisementItems { get; set; }
    }
}
