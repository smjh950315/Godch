using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class PublishType
    {
        public PublishType()
        {
            Forums = new HashSet<Forum>();
            Works = new HashSet<Work>();
        }

        public byte TypeId { get; set; }
        public string Description { get; set; } = null!;
        public string? Detail { get; set; }

        public virtual ICollection<Forum> Forums { get; set; }
        public virtual ICollection<Work> Works { get; set; }
    }
}
