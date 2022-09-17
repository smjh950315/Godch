using Godch.Models;
using MyLib;
using MyLib.Mvc;
using Xml = MyLib.Xml;
using String = MyLib.String;
using Godch.Hubs.Models;
using dir = Godch.GodchDirectories;
using Godch.Event;

namespace Godch
{
    public static partial class HubHelper
    {
        public static void WriteChatData(String? groupId, String time, String userId, String msg)
        {
            if (groupId == null) { return; }
            XmlData data = new XmlData("msg", msg);
            data.AddAttribute("time", time);
            data.AddAttribute("user", userId);
            var convertedData = Xml.GetString(data);
            dir.ChatData.AppendText(groupId + ".txt", convertedData);
        }
        public static List<ChatMsg> ReadChatData(string path)
        {
            List<ChatMsg> msgs = new();
            string[] rawDatas = dir.ChatData.ReadText($"{path}.txt").Split("</msg>\n<msg");
            int i = 0;
            foreach (string data in rawDatas)
            {
                ChatMsg msg = new();
                if (i == 0)
                {
                    msg = ConvertRawToMsg($"{data}</msg>");
                }
                else
                {
                    msg = ConvertRawToMsg($"<msg {data}</msg>");
                }
                if (!string.IsNullOrEmpty(msg.Message))
                {
                    msgs.Add(msg);
                }
                i++;
            }
            return msgs;
        }
        public static ChatMsg ConvertRawToMsg(string rawData)
        {
            ChatMsg msg = new();
            if (rawData != null && rawData.StartsWith("<msg ") && rawData.EndsWith("</msg>"))
            {
                XmlData data = Xml.ReadTab(rawData, "msg");
                var time = Convert.ToInt64(Xml.GetAttributeValue(data, "time"));
                msg.Time = Time.ToString(time);
                msg.User = Xml.GetAttributeValue(data, "user");
                msg.Message = Xml.GetContent(data);
                msg.TimeLong = time;
            }
            return msg;
        }
        public static ChatMsg ToChatMsg(string groupId, string userId, string msg)
        {
            ChatMsg data = new ChatMsg();
            data.Message = msg;
            data.User = userId;
            data.TimeLong = Time.TimeNowInt();
            data.Time = data.TimeLong.ToString();
            return data;
        }
    }
    public static partial class HubHelper
    {
        public static void WriteChatData(ChatConnection conn, string msg)
        {
            WriteChatData(conn.GroupId, Time.TimeNowInt(), conn.UserID, msg);
        }
        public static void WriteChatData(string groupId, string userId, string msg)
        {
            WriteChatData(groupId, Time.TimeNowInt(), userId, msg);
        }
        public static void WriteChatData(string groupId, ChatMsg msg)
        {
            WriteChatData(groupId, msg.TimeLong, msg.User, msg.Message);
        }
        public static bool IsGroupMember(ChatConnection conn)
        {
            return IsGroupMember(conn.GroupId, conn.UserID);
        }
        public static bool IsGroupMember(long chid, int? uid)
        {
            if (uid.HasValue)
            {
                GODCHContext db = new();
                var relation = db.ChatGroupRelations
                    .Where(GR => GR.ChatId == chid && GR.UserId == uid).FirstOrDefault();
                return relation != null;
            }
            else { return false; }
        }
        public static ChatGroupRelation? GroupRelation(long chid, int? uid)
        {
            if (uid.HasValue)
            {
                GODCHContext db = new();
                var relation = db.ChatGroupRelations
                    .Where(GR => GR.ChatId == chid && GR.UserId == uid).FirstOrDefault();
                return relation;
            }
            else { return null; }
        }

    }
    public static partial class HubHelper
    {
        public static ChatConnection NewChatConnection(string connectionId, string userName, ChatGroupRelation relation)
        {
            ChatConnection conn = new(relation, userName);
            conn.SetConnectionId(connectionId);
            return conn;
        }
        public static List<ChatGroupInfo>? GetGroups(int? uid)
        {
            dbQuery q = new();
            var groups = q.ChatGroupOfUser(uid);
            if (groups == null) { return null; }
            List<ChatGroupInfo> infos = new();
            groups.Sort((x, y) => x.LastActivity.Value.CompareTo(y.LastActivity.Value));
            foreach (var group in groups)
            {
                infos.Add(ToChatGroupInfo(group));
            }
            return infos;
        }
        public static ChatGroupInfo ToChatGroupInfo(ChatGroup group)
        {
            ChatGroupInfo info = new();
            info.GroupId = group.ChatId;
            info.GroupTitle = group.ChatTitle ?? "Untitled";
            info.IsPrivate = group.IsPrivate;
            return info;
        }
    }
    public static partial class HubHelper
    {
        public static EventData LoadNotify(XmlData data)
        {
            EventData eventData = new EventData();
            eventData.LoadXmlData(data);
            return eventData;
        }
        public static List<EventData> ReadNotifyList(string userId)
        {
            if (!dir.NotifyData.Exist($"{userId}.txt"))
            {
                return new List<EventData>();
            }
            var path = $"{Config.NotifyData}\\{userId}.txt";
            List<XmlData> data = Xml.ReadXml(path, "notify");
            List<EventData> list = new List<EventData>();
            foreach(var item in data)
            {
                var n = LoadNotify(item);
                if(n != null)
                {
                    list.Add(n);
                }
            }
            return list;
        }
    } 

}
