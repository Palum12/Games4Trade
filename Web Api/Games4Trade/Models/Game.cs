using System;

namespace Games4Trade.Models
{
    public class Game : AdvertisementItem
    {
        public DateTime DateDeveloped { get; set; }
        public string Developer { get; set; }
        public string Title { get; set; }
        public int GameRegionId { get; set; }
        public virtual Region GameRegion { get; set; }
        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; }
    }
}
