using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Games4Trade.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string RecoveryAddress { get; set; }
        public string Role { get; set; }
        
        public virtual ICollection<ObservedUsersRelationship> ObservedUsers { get; set; }
        public virtual ICollection<ObservedUsersRelationship> ObservingUsers { get; set; }

        public virtual ICollection<Announcement> Announcements { get; set; }
        public virtual ICollection<UserLikedGenre> LikedGenres { get; set; }
        public virtual ICollection<UserOwnedSystem> OwnedSystems { get; set; }
    }
}
