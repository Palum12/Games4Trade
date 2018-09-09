using System.Collections.Generic;

namespace Games4Trade.Models
{
    public class Region
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public virtual ICollection<Console> Consoles { get; set; }
        public virtual ICollection<Game> Games { get; set; }

    }
}
