using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class UserRelationShip
    {
        public long Rid { get; set; }
        public int UserId { get; set; }
        public int UserId2 { get; set; }
        public bool Block { get; set; }
        public bool? Following { get; set; }
        public bool? FriendShip { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual User UserId2Navigation { get; set; } = null!;
    }
}
