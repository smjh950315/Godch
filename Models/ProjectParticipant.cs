using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class ProjectParticipant
    {
        public long Rid { get; set; }
        public int ProjectId { get; set; }
        public int ParticipantId { get; set; }

        public virtual User Participant { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
    }
}
