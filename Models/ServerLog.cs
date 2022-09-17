using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class ServerLog
    {
        public long Id { get; set; }
        public string Client { get; set; } = null!;
        public string LaunchTime { get; set; } = null!;
    }
}
