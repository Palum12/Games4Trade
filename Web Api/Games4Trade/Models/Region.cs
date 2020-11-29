using System.Collections.Generic;

namespace Games4TradeAPI.Models
{
    public class Region : ModelBase
    {
        public string Value { get; set; }
        public virtual ICollection<Console> Consoles { get; set; }
        public virtual ICollection<Game> Games { get; set; }
    }
}
