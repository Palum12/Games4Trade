namespace Games4TradeAPI.Models
{
    public class UserOwnedSystem
    {
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public int SystemId { get; set; }
        public virtual System System { get; set; } = null!;
    }
}
