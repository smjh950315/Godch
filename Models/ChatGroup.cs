using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class ChatGroup
    {
        public ChatGroup()
        {
            ChatGroupRelations = new HashSet<ChatGroupRelation>();
        }

        public long ChatId { get; set; }
        public string? ChatTitle { get; set; }
        public string? Config { get; set; }
        public long? LastActivity { get; set; }
        public bool? IsPrivate { get; set; }

        public virtual ICollection<ChatGroupRelation> ChatGroupRelations { get; set; }
    }
}
