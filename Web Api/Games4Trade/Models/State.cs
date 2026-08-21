using System.Collections.Generic;

namespace Games4TradeAPI.Models
{
    public class State : ModelBase
    {
        public string Value { get; set; } = null!;
        public virtual ICollection<AdvertisementItem> AdvertisementItems { get; set; } = [];
    }
}
