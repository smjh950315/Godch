using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class Project
    {
        public Project()
        {
            ProjectParticipants = new HashSet<ProjectParticipant>();
        }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public int Publisher { get; set; }
        public byte PublishType { get; set; }
        public string? Description { get; set; }

        public virtual PublishType PublishTypeNavigation { get; set; } = null!;
        public virtual User PublisherNavigation { get; set; } = null!;
        public virtual ICollection<ProjectParticipant> ProjectParticipants { get; set; }
    }
}
