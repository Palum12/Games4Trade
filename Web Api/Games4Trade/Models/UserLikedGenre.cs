namespace Games4TradeAPI.Models
{
    public class UserLikedGenre
    {
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; } = null!;
    }
}
