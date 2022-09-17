using Godch.Event;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using String = MyLib.String;
namespace Godch.Hubs.Models
{
    public class HubConnection
    {
        public bool IsUsing { get; private set; }
        public bool IsConnected { get; private set; }
        public string ConnectionID { get; private set; }
        public String ClientId { get; private set; }
        public String GroupId { get; private set; }
        public List<LinkData> LinkList { get; private set; }
        public List<ChatGroupInfo>? GroupList { get; set; }
        public int NotifySlotIndex { get; set; }
        public void Clear()
        {
            LinkList.Clear();
            if(GroupList != null) { GroupList.Clear(); }
            IsUsing = false;
        }
        public void SetConnectionId(string? connectionId)
        {
            ConnectionID = connectionId;
            if (ConnectionID != null)
            {
                IsConnected = true;
            }
            else
            {
                IsConnected = false;
            }
        }
        public HubConnection() : this("", null, null)
        {
        }
        public HubConnection(string connectionId, string? clientId):this(connectionId, clientId, null) 
        {
        }
        public HubConnection(string connectionId, string? clientId,string? groupId)
        {
            ConnectionID = connectionId;
            GroupId = groupId ?? "N/A";
            ClientId = clientId ?? "N/A";
            LinkList = new List<LinkData>();
            GroupList = new List<ChatGroupInfo>();
            SetConnectionId(connectionId);
            IsUsing = true;
        }
    }
    public class LinkData
    {
        public String GroupId { get; set; }
        public int GroupIndex { get; set; }
        public int MemberIndex { get; set; }        
        public LinkData()
        {
            GroupId = "";
        }
        public LinkData(String groupId, int groupIndex, int memberIndex) : this()
        {   
            GroupId = groupId;
            GroupIndex = groupIndex;
            MemberIndex = memberIndex;
        }
    }

}
