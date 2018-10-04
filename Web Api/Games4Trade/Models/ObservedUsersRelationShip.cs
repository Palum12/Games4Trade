namespace Games4Trade.Models
{
    public class ObservedUsersRelationship
    {
        public int ObservingUserId { get; set; }
        public virtual User ObservingUser { get; set; }

        public int ObservedUserId { get; set; }
        public virtual User ObservedUser { get; set; }
    }
}
