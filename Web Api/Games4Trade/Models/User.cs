using System.Collections.Generic;


namespace Games4TradeAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string RecoveryAddress { get; set; }
        public string Role { get; set; }
        public int? PhotoId { get; set; }

        public virtual Photo Photo { get; set; }
        
        public virtual ICollection<ObservedUsersRelationship> ObservedUsers { get; set; }
        public virtual ICollection<ObservedUsersRelationship> ObservingUsers { get; set; }

        public virtual ICollection<Message> MessagesSent { get; set; }
        public virtual ICollection<Message> MessagesRecived { get; set; }

        public virtual ICollection<Advertisement> Advertisements { get; set; }
        public virtual ICollection<Announcement> Announcements { get; set; }
        public virtual ICollection<UserLikedGenre> LikedGenres { get; set; }
        public virtual ICollection<UserOwnedSystem> OwnedSystems { get; set; }
    }
}
