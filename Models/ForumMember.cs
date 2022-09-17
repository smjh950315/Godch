using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class ForumMember
    {
        public long Rid { get; set; }
        public int ForumId { get; set; }
        public int MemberId { get; set; }
        public byte Role { get; set; }

        public virtual Forum Forum { get; set; } = null!;
        public virtual User Member { get; set; } = null!;
    }
}
