using Godch.Models;
using MyLib;
using System.Security.Cryptography;
using String = MyLib.String;

namespace Godch.Hubs.Models
{
    public class Group
    {
        private dbQuery q = new();
        public List<ChatMsg> Messages = new();
        public List<Member> Members = new();
        public bool IsUsing { get; private set; }
        public bool IsInitialized { get; private set; }
        public String Id { get; private set; }        
        public String Title { get; private set; } 
        public void LoadMessage()
        {
            Messages = HubHelper.ReadChatData(Id);
        }
        private void Initialization(dynamic groupId)
        {
            if (groupId != null)
            {
                var group = q.Group(groupId);
                if (group != null)
                {
                    Id = groupId.ToString();
                    Title = group.ChatTitle ?? "N/A";
                    IsInitialized = true;
                }
                else
                {
                    Id = 0;
                    Title = "N/A";
                }
            }
            LoadMessage();
        }
        public void Clear()
        {
            Messages.Clear();
            Members.Clear();
            Id = -1;
            IsUsing=false;
        }
        public Group()
        {
            Id = 0;
            Title = "N/A";
            IsInitialized = false;
            IsUsing = true;
        }
        public Group(dynamic groupId) : this()
        {
            Initialization(groupId);
        }
        public Group(ChatGroup? group) : this(group?.ChatId)
        {
        }
        public void AddConnection(Member member)
        {
            Members.Add(member);
        }
    }
    public class Member
    {
        private dbQuery q = new();
        public bool IsUsing { get; private set; }
        public String Id { get; private set; }
        public Number ReadMessage { get; set; }
        public bool IsValid { get; private set; }
        public string Name { get; private set; }
        public long Rid { get; set; }
        public int StartMsg_Index { get; set; }
        public void SetValue(String? userId)
        {
            if (userId?.ToString() != null)
            {
                var user = q.User(userId);
                if (user != null)
                {
                    Id = userId;
                    Name = user.UserName;
                    IsValid = true;
                }
                else
                {
                    IsValid = false;
                }
            }
        }
        public void Clear()
        {
            IsUsing = false;
            Id = -1;
        }
        public Member()
        {
            Id = new();
            Name = "N/A";
            ReadMessage = new Number();
            IsUsing = true;
        }
        public Member(int? userId) : this()
        {
            SetValue(userId);
        }
        public Member(String? userId) : this()
        {
            SetValue(userId);
        }
        public Member(User? user) : this()
        {
            SetValue(user?.UserId);
        }
        public Member(ChatGroupRelation relation) : this()
        {
            SetValue(relation.UserId);
            Rid = relation.Rid;
            ReadMessage = relation.LastConnected;
            ReadMessage.IsChanged = false;
        }
    }
}
