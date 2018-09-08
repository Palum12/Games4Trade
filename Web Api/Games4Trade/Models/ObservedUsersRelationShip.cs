using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Games4Trade.Models
{
    public class ObservedUsersRelationShip
    {
        public int ObservingUserId { get; set; }
        public virtual User ObservingUser { get; set; }

        public int ObservedUserId { get; set; }
        public virtual User ObservedUser { get; set; }
    }
}
