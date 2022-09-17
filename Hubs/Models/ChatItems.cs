using Godch.Models;
using String = MyLib.String;

namespace Godch.Hubs.Models
{
    public class ChatConnection
    {
        public bool IsConnected { get; set; }
        public string? ConnectionID { get; set; }
        public String? ClientId { get; private set; }
        public String? GroupId { get; set; }

        public List<ChatMsg> Messages = new();
        public long RId { get; private set; }
        public String UserID { get; set; }
        public string? UserName { get; set; }
        public long? LastConnectedTime { get; set; }
        public void SetConnectionId(string connectionId)
        {
            ConnectionID = connectionId;
            IsConnected = true;
        }
        public ChatConnection()
        {
            RId = 0;
            UserID = 0;
            GroupId = 0;
            IsConnected = false;
        }
        public ChatConnection(ChatGroupRelation relation) : this()
        {
            RId = relation.Rid;
            GroupId = relation.ChatId;
            UserID = relation.UserId;
            LastConnectedTime = relation.LastConnected;
        }
        public ChatConnection(ChatGroupRelation relation, string userName) : this(relation)
        {
            UserName = userName;
        }
    }
    public struct ChatGroupInfo
    {
        public long GroupId { get; set; }
        public string GroupTitle { get; set; }
        public bool? IsPrivate { get; set; }
    }
    public struct ChatMsg
    {
        public string Time { get; set; }
        public string User { get; set; }
        public string Message { get; set; }
        public long TimeLong { get; set; }
    }
}
