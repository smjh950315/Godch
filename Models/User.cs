using System;
using System.Collections.Generic;

namespace Godch.Models
{
    public partial class User
    {
        public User()
        {
            ChatGroupRelations = new HashSet<ChatGroupRelation>();
            ForumMembers = new HashSet<ForumMember>();
            Posts = new HashSet<Post>();
            UserRelationShipUserId2Navigations = new HashSet<UserRelationShip>();
            UserRelationShipUsers = new HashSet<UserRelationShip>();
            Works = new HashSet<Work>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string? FullName { get; set; }
        public string Account { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string? UserSetting { get; set; }
        public bool? OnWorking { get; set; }
        public string? Description { get; set; }
        public byte? Role { get; set; }
        public long? Register { get; set; }
        public long? LastActive { get; set; }
        public long? NotifyRead { get; set; }

        public virtual ICollection<ChatGroupRelation> ChatGroupRelations { get; set; }
        public virtual ICollection<ForumMember> ForumMembers { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<UserRelationShip> UserRelationShipUserId2Navigations { get; set; }
        public virtual ICollection<UserRelationShip> UserRelationShipUsers { get; set; }
        public virtual ICollection<Work> Works { get; set; }
    }
}
