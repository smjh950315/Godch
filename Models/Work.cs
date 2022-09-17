using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class Work
    {
        public Work()
        {
            TagRelations = new HashSet<TagRelation>();
        }

        public long WorkId { get; set; }
        public string WorkName { get; set; } = null!;
        public int AuthorId { get; set; }
        public long UploadTime { get; set; }
        public byte? PublishType { get; set; }
        public string? Description { get; set; }
        public string? FileUrl { get; set; }

        public virtual User Author { get; set; } = null!;
        public virtual PublishType? PublishTypeNavigation { get; set; }
        public virtual ICollection<TagRelation> TagRelations { get; set; }
    }
}
