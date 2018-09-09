using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Games4Trade.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public virtual ICollection<UserLikedGenre> LikedByUsers { get; set; }
        public virtual ICollection<Game> Games { get; set; }
    }
}
