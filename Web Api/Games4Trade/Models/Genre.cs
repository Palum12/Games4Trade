using System.Collections.Generic;


namespace Games4TradeAPI.Models
{
    public class Genre : ModelBase
    {
        public string Value { get; set; }
        public virtual ICollection<UserLikedGenre> LikedByUsers { get; set; }
        public virtual ICollection<Game> Games { get; set; }
    }
}
