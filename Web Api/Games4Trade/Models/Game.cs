namespace Games4TradeAPI.Models
{
    public class Game : AdvertisementItem
    {
        public string Developer { get; set; }
        public int GameRegionId { get; set; }
        public virtual Region GameRegion { get; set; }
        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; }
    }
}
