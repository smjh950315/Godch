using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class ChatGroupRelation
    {
        public long Rid { get; set; }
        public long ChatId { get; set; }
        public int UserId { get; set; }
        public long? LastConnected { get; set; }

        public virtual ChatGroup Chat { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
