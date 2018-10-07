namespace Games4Trade.Models
{
    public class UserOwnedSystem
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int SystemId { get; set; }
        public virtual System System { get; set; }
    }
}
