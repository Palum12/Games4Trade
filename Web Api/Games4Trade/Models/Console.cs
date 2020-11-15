namespace Games4TradeAPIAPI.Models
{
    public class Console : AdvertisementItem
    {
        public int ConsoleRegionId { get; set; }
        public virtual Region ConsoleRegion { get; set; }
    }
}
