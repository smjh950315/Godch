using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class Tag
    {
        public Tag()
        {
            TagRelations = new HashSet<TagRelation>();
        }

        public int TagId { get; set; }
        public string TagName { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<TagRelation> TagRelations { get; set; }
    }
}
