using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class TagRelation
    {
        public long Rid { get; set; }
        public int TagId { get; set; }
        public long WorkId { get; set; }

        public virtual Tag Tag { get; set; } = null!;
        public virtual Work Work { get; set; } = null!;
    }
}
