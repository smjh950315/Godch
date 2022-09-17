using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class Post
    {
        public long PostId { get; set; }
        public string Title { get; set; } = null!;
        public long? HeadPostId { get; set; }
        public int? Floor { get; set; }
        public int? AuthorId { get; set; }
        public int ForumId { get; set; }
        public long Create { get; set; }
        public long? LastReply { get; set; }
        public string? Data { get; set; }

        public virtual User? Author { get; set; }
    }
}
