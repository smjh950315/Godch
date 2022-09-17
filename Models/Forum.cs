using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class Forum
    {
        public Forum()
        {
            ForumMembers = new HashSet<ForumMember>();
        }

        public int ForumId { get; set; }
        public string ForumName { get; set; } = null!;
        public byte? PublishType { get; set; }
        public string? Description { get; set; }
        public long? LastActivity { get; set; }

        public virtual PublishType? PublishTypeNavigation { get; set; }
        public virtual ICollection<ForumMember> ForumMembers { get; set; }
    }
}
