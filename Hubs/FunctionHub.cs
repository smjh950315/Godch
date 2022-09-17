using Microsoft.AspNetCore.SignalR;
using Godch.Models;
using MyLib;
using String = MyLib.String;
using Godch.Hubs.Models;
using Group = Godch.Hubs.Models.Group;
using Godch.Event;
using Godch.GlobalStorage;
using Microsoft.AspNet.SignalR.Messaging;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Godch.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Godch.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Godch.Hubs
{
    //連接HUB
    //1.加入連線 <HubConnection>
    //2.加入通知 <HubSlot>
    //3.(選擇)連接聊天群 <GroupList>
    //Get Caller Connection Data
    public partial class FunctionHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private int LeastMessageSend = 5;
        private int OldNotifyCountPerTimes = 5;
        private GODCHContext db = new();
        private dbAction act = new();
        private dbQuery q = new();
        protected bool IsLogin()
        {            
            return GetCallerUserId() != null;
        }
        protected HubSlot CreateSlotForUser(String userId)
        {
            HubSlot slot = new HubSlot(userId);
            slot.NotifyRead = q.User(userId)?.NotifyRead;
            return slot;
        }
        protected HubConnection? GetCallerConnection()
        {
            return HubStorage.Connections.Where(c => c.ConnectionID == Context.ConnectionId).First();
        }
        public HubConnection? ConnectionOfUser(String userId)
        {
            var user = HubStorage.Connections.FirstOrDefault(c => c.ClientId == userId);
            if (user == null)
            {
                return null;
            }
            else
            {
                return user;
            }
        }
        protected LinkData? GetCallerLinkDataOfGroup(string? groupId)
        {
            if (groupId == null) { return null; }
            var hubConnection = HubStorage.Connections.Find(c => c.ConnectionID == Context.ConnectionId);
            if (hubConnection == null) { return null; }
            var linkData = hubConnection.LinkList.FirstOrDefault(l => l.GroupId == groupId);
            return linkData;
        }
        protected User? GetCallerUserData()
        {
            return q.User(GetCallerClientId());
        }
        protected string? GetCallerUserId()
        {
            try
            {
                return Context?.User?.Identity?.Name?.ToString();
            }
            catch
            {
                return null;
            }
        }
        protected String GetCallerClientId()
        {
            var connection = HubStorage.Connections.Where(c => c.ConnectionID == Context.ConnectionId).First();
            return connection?.ClientId ?? new String();
        }
        protected int? GetCallerConnectionIndex()
        {
            var conn = HubStorage.Connections.Where(c => c.ConnectionID == Context.ConnectionId).First();
            if (conn == null)
            {
                return null;
            }
            return HubStorage.Connections.IndexOf(conn);
        }
        protected int? GetCallerIndexInGroup(string groupId)
        {
            return GetConnectedMemberIndexInGroup(groupId, GetCallerUserId());
        }
        protected int? GetCallerSlotIndex()
        {
            return GetSlotIndexOfUser(null);
        }
    }
    //Get Data by Args
    public partial class FunctionHub
    {
        protected string? GetConnectionIdOfUser(String userId)
        {
            return HubStorage.Connections.FirstOrDefault(u => u.ClientId == userId)?.ConnectionID;
        }
        protected int? GetConnectionIndexOfUser(String userId)
        {
            var conn = HubStorage.Connections.FirstOrDefault(u => u.ClientId == userId);
            if (conn == null) { return null; }
            return HubStorage.Connections.IndexOf(conn);
        }

        protected Group? GetConnectedGroupById(string groupId)
        {
            return HubStorage.GroupList.FirstOrDefault(g => g.Id == groupId);
        }
        protected int? GetConnectedGroupIndexByGroupId(string groupId)
        {
            var group = GetConnectedGroupById(groupId);
            if (group == null)
            {
                return null;
            }
            int index = HubStorage.GroupList.IndexOf(group);
            return index;
        }

        protected Member? GetConnectedMemberInGroup(String groupId)
        {
            return GetConnectedMemberInGroup(groupId, GetCallerUserId());
        }
        protected Member? GetConnectedMemberInGroup(String groupId, String userId)
        {
            var group = HubStorage.GroupList.Where(g => g.Id == groupId).FirstOrDefault();
            return group?.Members.Where(m => m.Id == userId).FirstOrDefault();
        }
        protected int? GetConnectedMemberIndexInGroup(string groupId, string? userId)
        {
            var group = GetConnectedGroupById(groupId);
            var member = GetConnectedMemberInGroup(groupId, userId);
            if (member == null || group == null) { return null; }
            return group.Members.IndexOf(member);
        }

        protected HubSlot? GetSlotOfUser(String? userId)
        {
            if (userId == null)
            {
                var callerId = GetCallerUserId();
                if (callerId != null)
                {
                    return HubStorage.NotifySlot.FirstOrDefault(c => c.IsUsing == true && c.SlotId == $"notify{callerId}");
                }
                return null;
            }
            else
            {
                return HubStorage.NotifySlot.FirstOrDefault(s => s.IsUsing == true && s.SlotId == $"notify{userId}");
            }
        }
        protected int? GetSlotIndexOfUser(String? userId)
        {
            var slot = GetSlotOfUser(userId);
            if (slot == null) { return null; }
            int index = HubStorage.NotifySlot.IndexOf(slot);
            if (index == -1) { return null; }
            return index;
        }
    }
    //Basic Hub Operation
    public partial class FunctionHub
    {
        public override Task OnDisconnectedAsync(Exception? ex)
        {
            var hubConnection = HubStorage.Connections.Find(c => c.ConnectionID == Context.ConnectionId);
            if (hubConnection != null)
            {
                //回寫DB並清除記憶體內Group的Member資料
                var linkData = hubConnection.LinkList;
                foreach (var data in linkData)
                {
                    var memberConnection = HubStorage.GroupList[data.GroupIndex].Members[data.MemberIndex];
                    if (memberConnection != null)
                    {
                        if (memberConnection.ReadMessage.IsChanged)
                        {
                            long relationId = HubStorage.GroupList[data.GroupIndex].Members[data.MemberIndex].Rid;
                            var relation = db.ChatGroupRelations.FirstOrDefault(cr => cr.Rid == relationId);
                            if (relation != null)
                            {
                                relation.LastConnected = memberConnection.ReadMessage;
                                act.Update(relation);
                            }
                        }
                        HubStorage.GroupList[data.GroupIndex].Members[data.MemberIndex].Clear();
                    }
                }
                int hConnectionIndex = HubStorage.Connections.IndexOf(hubConnection);
                int nIndex = hubConnection.NotifySlotIndex;
                //將記憶體內ㄉ時間資訊寫回DB
                long? nRead = HubStorage.NotifySlot[nIndex].NotifyRead;
                var user = q.User(GetCallerClientId());
                if (user != null)
                {
                    user.NotifyRead = nRead;
                    act.Update(user);
                }
                HubStorage.NotifySlot[nIndex].Clear();
                //清除記憶體內的連線資料
                HubStorage.Connections[hConnectionIndex].Clear();
            }
            return base.OnDisconnectedAsync(ex);
        }
        public void ConnectHub()
        {
            var userId = GetCallerUserId();
            if (userId == null) 
            {
                OnDisconnectedAsync(null);
            }
            int hubIndex;
            HubConnection hubConnection = new HubConnection(Context.ConnectionId, userId);
            var unusedHub = HubStorage.Connections.FirstOrDefault(c => c.IsUsing == false);
            if (unusedHub != null)
            {
                hubIndex = HubStorage.Connections.IndexOf(unusedHub);
                HubStorage.Connections[hubIndex] = hubConnection;
            }
            else
            {
                HubStorage.Connections.Add(hubConnection);
                hubIndex = HubStorage.Connections.IndexOf(hubConnection);
            }
            //Add Notify Slot
            ConnectNotifySlot(userId, hubIndex);
        }
        public void GetGroupList()
        {
            int? i = GetCallerConnectionIndex();
            if (i == null) { return; }
            HubStorage.Connections[i.Value].GroupList = HubHelper.GetGroups(HubStorage.Connections[i.Value].ClientId);
            if (HubStorage.Connections[i.Value].GroupList == null) { return; }

            foreach (var g in HubStorage.Connections[i.Value].GroupList)
            {
                if (LazyConvert.TorF(g.IsPrivate))
                {
                    if (g.GroupTitle == "_default_")
                    {
                        int uid = HubStorage.Connections[i.Value].ClientId;
                        var gRelation = db.ChatGroupRelations.Where(gr => gr.ChatId == g.GroupId && gr.UserId != uid).ToList();
                        if (gRelation.Count() > 0)
                        {
                            string groupNameFromUserName = q.UserName(gRelation.First().UserId);
                            Clients.Caller.SendAsync("ReceiveChatList", g.GroupId, groupNameFromUserName);
                        }
                        else
                        {
                            Clients.Caller.SendAsync("ReceiveChatList", g.GroupId, "unknow user");
                        }
                    }
                    else
                    {
                        Clients.Caller.SendAsync("ReceiveChatList", g.GroupId, g.GroupTitle);
                    }
                }
                else
                {
                    Clients.Caller.SendAsync("ReceiveChatList", g.GroupId, g.GroupTitle);
                }
            }
        }
        public void ReloadGroupList(String userId)
        {
            string? connId = GetConnectionIdOfUser(userId);
            if (connId == null) { return; }
            Clients.Client(connId).SendAsync("CheckChatList");
        }
    }
    //Chat
    public partial class FunctionHub
    {
        public void SendMessageToCaller(string method, int groupIndex, int messageIndex)
        {
            string groupId = HubStorage.GroupList[groupIndex].Id;
            string UserName = q.UserName(Convert.ToInt32(HubStorage.GroupList[groupIndex].Messages[messageIndex].User));
            string Message = HubStorage.GroupList[groupIndex].Messages[messageIndex].Message;
            string Time = HubStorage.GroupList[groupIndex].Messages[messageIndex].Time;
            Clients.Caller.SendAsync(method, groupId, HubStorage.GroupList[groupIndex].Messages[messageIndex].User, UserName, Message, Time);
        }
        public void ConnectChatRoom(object roomId)
        {
            String RoomId = roomId.ToString();
            if (RoomId.IsNull) { return; }
            String userId = GetCallerConnection()?.ClientId ?? "";


            //驗證是否為群組成員
            var relation = HubHelper.GroupRelation(RoomId, userId);
            if (relation == null) { return; }
            //實體化成員
            var memberConnection = new Member(relation);

            //連接群組
            int memberIndex;
            int groupIndex = 0;
            var group = HubStorage.GroupList.Where(g => g.Id == RoomId).FirstOrDefault();
            if (group == null)
            {
                var newGroup = new Group(RoomId);
                HubStorage.GroupList.Add(newGroup);
                groupIndex = HubStorage.GroupList.IndexOf(newGroup);
                HubStorage.GroupList[groupIndex].Members.Add(memberConnection);
                memberIndex = HubStorage.GroupList[groupIndex].Members.IndexOf(memberConnection);
            }
            else
            {
                groupIndex = HubStorage.GroupList.IndexOf(group);
                var unusedMemberSlot = HubStorage.GroupList[groupIndex].Members.FirstOrDefault(m => m.IsUsing == false);
                if (unusedMemberSlot != null)
                {
                    memberIndex = HubStorage.GroupList[groupIndex].Members.IndexOf(unusedMemberSlot);
                    HubStorage.GroupList[groupIndex].Members[memberIndex] = memberConnection;
                }
                else
                {
                    HubStorage.GroupList[groupIndex].Members.Add(memberConnection);
                    memberIndex = HubStorage.GroupList[groupIndex].Members.IndexOf(memberConnection);
                }
            }
            Groups.AddToGroupAsync(Context.ConnectionId, RoomId);
            //取得Connection索引
            int? cIndex = GetCallerConnectionIndex();
            if (cIndex == null) { return; }
            int connIndex = cIndex.Value;
            //建立索引資料
            var linkData = new LinkData(RoomId, groupIndex, memberIndex);
            //寫入Connection
            HubStorage.Connections[connIndex].LinkList.Add(linkData);
            SendUnreadMsg(RoomId, relation.LastConnected);
        }
        public void SendChatMessage(string groupId, string message)
        {
            //有link表示已連入群組
            var linkData = GetCallerLinkDataOfGroup(groupId);
            if (linkData == null) { return; }

            var group = HubStorage.GroupList[linkData.GroupIndex];
            long l = group.Id;
            var chatGroup = db.ChatGroups.Find(l);
            if (chatGroup == null) { return; }
            chatGroup.LastActivity = Time.TimeNowInt();
            act.Update(chatGroup);
            var member = HubStorage.GroupList[linkData.GroupIndex].Members[linkData.MemberIndex];

            ChatMsg chatMsg = HubHelper.ToChatMsg(groupId, GetCallerUserId(), message);
            HubStorage.GroupList[linkData.GroupIndex].Messages.Add(chatMsg);
            HubHelper.WriteChatData(groupId, chatMsg);

            Clients.Group(groupId).SendAsync("ReceiveMessage", groupId, member.Id.ToString(), "userName", message, Time.ToString(chatGroup.LastActivity));
        }
        public void SendUnreadMsg(string groupId, long? fromTime)
        {
            if (fromTime == null) { return; }
            //有link表示已連入群組
            var linkData = GetCallerLinkDataOfGroup(groupId);
            if (linkData == null) { return; }
            int groupIndex = linkData.GroupIndex;
            int memberIndex = linkData.MemberIndex;
            int msgCount = HubStorage.GroupList[groupIndex].Messages.Count;
            if (msgCount == 0) { return; }

            int iNewMsgStart = 0;
            foreach (var msg in HubStorage.GroupList[groupIndex].Messages)
            {
                if (msg.TimeLong > fromTime) { break; }
                iNewMsgStart++;
            }
            HubStorage.GroupList[groupIndex].Members[memberIndex].StartMsg_Index = iNewMsgStart;
            for (int i = iNewMsgStart; i < msgCount; i++)
            {
                SendMessageToCaller("ReceiveMessage", groupIndex, i);
            }
            int countOfOldMsgSending = LeastMessageSend - iNewMsgStart + msgCount;
            countOfOldMsgSending = countOfOldMsgSending < 0 ? 0 : countOfOldMsgSending;
            SendIsReadMessage(groupId, countOfOldMsgSending);
        }
        public void SendIsReadMessage(object _groupId, int count)
        {
            string? groupId = _groupId.ToString();
            var linkData = GetCallerLinkDataOfGroup(groupId);
            if (linkData == null) { return; }
            int groupIndex = linkData.GroupIndex;
            int memberIndex = linkData.MemberIndex;
            int get_last_msg_index = HubStorage.GroupList[groupIndex].Members[memberIndex].StartMsg_Index;

            if (get_last_msg_index >= 0)
            {
                int startMsgIndex = get_last_msg_index <= count ? 0 : get_last_msg_index - count;
                HubStorage.GroupList[groupIndex].Members[memberIndex].StartMsg_Index = startMsgIndex;
                for (int i = get_last_msg_index - 1; i >= startMsgIndex; i--)
                {
                    SendMessageToCaller("ReceiveOldMessage", groupIndex, i);
                }
            }
        }
        public void OldMessage(object _groupId)
        {
            string? groupId = _groupId.ToString();
            var linkData = GetCallerLinkDataOfGroup(groupId);
            if (linkData == null) { return; }
            int groupIndex = linkData.GroupIndex;
            int memberIndex = linkData.MemberIndex;
            int last_msg_index = HubStorage.GroupList[groupIndex].Members[memberIndex].StartMsg_Index;

            if (last_msg_index >= 0)
            {
                int startMsgIndex = last_msg_index <= LeastMessageSend ? 0 : last_msg_index - LeastMessageSend;
                HubStorage.GroupList[groupIndex].Members[memberIndex].StartMsg_Index = startMsgIndex;
                for (int i = last_msg_index - 1; i >= startMsgIndex; i--)
                {
                    SendMessageToCaller("ReceiveOldMessage", groupIndex, i);
                }
            }
        }
        public void ReadMessage(object chatId)
        {
            string? groupId = chatId?.ToString();
            var linkData = GetCallerLinkDataOfGroup(groupId);
            if (linkData == null) { return; }
            var member = HubStorage.GroupList[linkData.GroupIndex].Members[linkData.MemberIndex];
            HubStorage.GroupList[linkData.GroupIndex].Members[linkData.MemberIndex].ReadMessage = Time.TimeNowInt();
            var cr = db.ChatGroupRelations.Find(member.Rid);
            if (cr != null)
            {
                cr.LastConnected = Time.TimeNowInt();
                act.Update(cr);
            }
        }
        public void NewPrivateGroup(object userId)
        {
            String uid2 = new String(userId);
            String uid1 = GetCallerUserId();
            if (uid1 == uid2) { return; }
            var groupId = q.GetPrivateGroup(uid1, uid2);
            if (groupId == null) { return; }
             
            ConnectChatRoom(groupId);
            ReloadGroupList(uid2);

            Clients.Caller.SendAsync("TriggerCalledGroup", groupId.ToString());
        }
    }
    //Notify
    public partial class FunctionHub
    {
        public void ConnectNotifySlot(String userId, int hubIndex)
        {
            HubSlot slot = CreateSlotForUser(userId);
            slot.EventList = HubHelper.ReadNotifyList(userId);
            slot.NotifyRead = q.User(userId)?.NotifyRead;
            int slotIndex;
            var unusedSlot = HubStorage.NotifySlot.Where(ns => ns.IsUsing == false).FirstOrDefault();
            if(unusedSlot != null)
            {
                slotIndex = HubStorage.NotifySlot.IndexOf(unusedSlot);
                HubStorage.NotifySlot[slotIndex] = slot;
            }
            else
            {
                HubStorage.NotifySlot.Add(slot);
                slotIndex = HubStorage.NotifySlot.IndexOf(slot);
            }            
            HubStorage.Connections[hubIndex].NotifySlotIndex = slotIndex;

            Groups.AddToGroupAsync(Context.ConnectionId, slot.SlotId);
            SendUnreadNotify(slot.NotifyRead);
        }
        public void SendNotify(EventData data)
        {
            String targetClientId = data.UserId2;
            //Save
            var evtXml = data.ToXmlData();
            GodchDirectories.NotifyData.AppendText(targetClientId + ".txt", evtXml);
            //Send Notify
            Clients.Group("notify"+targetClientId).SendAsync("ReceiveNotify", data.Description(), evtXml.Data, data.TimeString.ToString());
        }
        public void SendUnreadNotify(long? fromTime)
        {
            long FromTime = 0;
            if (fromTime != null) { FromTime=fromTime.Value; }
            int? slotIndex = GetCallerSlotIndex();
            if (slotIndex == null) { return; }
            int iStart = 0;
            foreach (var evt in HubStorage.NotifySlot[slotIndex.Value].EventList)
            {
                if (evt.TimeLong > FromTime) { break; }
                iStart++;
            }
            HubStorage.NotifySlot[slotIndex.Value].StartNotify_Index = iStart;
            for (int i = iStart; i < HubStorage.NotifySlot[slotIndex.Value].EventList.Count; i++)
            {
                var data = HubStorage.NotifySlot[slotIndex.Value].EventList[i];
                Clients.Caller.SendAsync("ReceiveNotify", data.Description(), data.Url, data.TimeString.ToString());
            }
        }
        public void OldNotify()
        {
            int? index = GetCallerSlotIndex();
            if (index == null) { return; }
            int nIndex = index.Value;
            int last_notify_index = HubStorage.NotifySlot[nIndex].StartNotify_Index;
            int startNotifyINdex;
            if (last_notify_index >= 0)
            {              
                if(last_notify_index < OldNotifyCountPerTimes)
                {
                    startNotifyINdex = 0;
                }
                else
                {
                    startNotifyINdex = last_notify_index - OldNotifyCountPerTimes;
                }

                for(int i = last_notify_index-1; i >= startNotifyINdex; i--)
                {
                    var data = HubStorage.NotifySlot[nIndex].EventList[i];
                    Clients.Caller.SendAsync("ReceiveOldNotify", data.Description(), data.Url, data.TimeString.ToString());
                }
                HubStorage.NotifySlot[nIndex].StartNotify_Index = startNotifyINdex;
            }
        }
        public void ReadNotify()
        {
            int? index = GetCallerSlotIndex();
            if (index == null) { return; }
            int nIndex = index.Value;
            HubStorage.NotifySlot[nIndex].NotifyRead = Time.TimeNowInt();
        }
    }
    //Data convert
    public partial class FunctionHub
    {
        public string? TryParseString(object? @object)
        {
            String str = new String(@object);
            return str;
        }
        public string NotValidInput(string? item)
        {
            return $"Not a valid {item} !";
        }
        public bool ValidCaller()
        {
            var user = q.User(GetCallerClientId());
            if (user == null)
            {
                SendResult(NotValidInput("password"));
                return false;
            }
            return true;
        }
        public bool ValidInput(object? something)
        {
            var val = TryParseString(something);
            if (val == null)
            {
                SendResult("Invalid value!");
                return false;
            }
            return true;
        }
    }
    //FrontEnd Interactivity
    public partial class FunctionHub
    {
        public void SendResult(string result)
        {
            Clients.Caller.SendAsync("GetResponse", result);
        }
        public void ChangeText(string? targetName, String? text)
        {
            if (targetName == null) { return; }
            string? Text = text;
            Clients.Caller.SendAsync("ChangeText", targetName, Text);
        }
        public void AddFrontEndElement(object? targetId, object? element)
        {
            string? TargetId =new String(targetId);
            string? Element = new String(element);
            Clients.Caller.SendAsync("AddElement", TargetId, Element);
        }
        public void SetFrontEndElement(object? targetId, object? element)
        {
            string? TargetId = new String(targetId);
            string? Element = new String(element);
            Clients.Caller.SendAsync("AddElement", TargetId, Element);
        }
    }
    //Account management
    public partial class FunctionHub
    {
        public void ChangeNickName(object? value)
        {
            if (!ValidInput(value) || !ValidCaller())
            {
                return;
            }
            var user = GetCallerUserData();
            user.UserName = TryParseString(value);
            act.Update(user);
            ChangeText("show-nick-name", user.UserName);
            ChangeText("welcome-title", $"Welcome! {user.UserName}");
            ChangeText("show-acc-email", $"account : {user.Account} ( {user.EmailAddress} )");
            SendResult("Success!");
        }
        public void ChangePassword(object? value)
        {
            if (!ValidInput(value) || !ValidCaller())
            {
                return;
            }
            var user = GetCallerUserData();
            user.Password = TryParseString(value);
            act.Update(user);
            ChangeText("show-password", user.Password);
            SendResult("Success!");
        }
        public void ChangeEmail(object? value)
        {
            if (!ValidInput(value) || !ValidCaller())
            {
                return;
            }
            var user = GetCallerUserData();
            user.EmailAddress = TryParseString(value);
            act.Update(user);
            ChangeText("show-email", user.EmailAddress);
            ChangeText("welcome-title", $"Welcome! {user.UserName}");
            ChangeText("show-acc-email", $"account : {user.Account} ( {user.EmailAddress} )");
            SendResult("Success!");
        }
        public void ChangeIntroduction(object? value)
        {
            if (!ValidInput(value) || !ValidCaller())
            {
                return;
            }
            var user = GetCallerUserData();
            user.Description = TryParseString(value);
            act.Update(user);
            ChangeText("show-introduction", user.Description);
            SendResult("Success!");
        }
        public void GetFriendList()
        {
            if (!IsLogin()) { return; }
            ViewList vlist = new ViewList();
            List<dynamic> friendList = vlist.CastToList(q.FriendsOfId(GetCallerClientId()));
            ChangeText("friend-verified-tab", $"Friend verified ({friendList.Count})");
            foreach (dynamic friend in friendList)
            {
                string photo = $"{friend.Photo}";
                Clients.Caller.SendAsync("AddFriendToList", $"{friend.Name}", $"/User/Index?uid={friend.Id}", photo);
            }
            List<dynamic> friendWaitingList = vlist.CastToList(q.FriendsWaiting(GetCallerClientId()));
            ChangeText("friend-waiting-tab", $"Friend waiting ({friendWaitingList.Count})");
            foreach (dynamic wfriend in friendWaitingList)
            {
                string photo = $"{wfriend.Photo}";
                Clients.Caller.SendAsync("AddWFriendToList", $"{wfriend.Name}", $"/User/Index?uid={wfriend.Id}", photo);
            }
            List<dynamic> friendUnconfirmList = vlist.CastToList(q.FriendsUnrequest(GetCallerClientId()));
            ChangeText("friend-unconfirm-tab", $"Friend unconfirm ({friendUnconfirmList.Count})");
            foreach (dynamic ufriend in friendUnconfirmList)
            {
                string photo = $"{ufriend.Photo}";
                Clients.Caller.SendAsync("AddUFriendToList", $"{ufriend.Name}", $"/User/Index?uid={ufriend.Id}", photo);
            }
        }
        public void GetFollowerList()
        {
            if (!IsLogin()) { return; }
            ViewList vlist = new ViewList();
            List<dynamic> followerList = vlist.CastToList(q.FollowersOfId(GetCallerClientId()));
            ChangeText("follower-tab", $"Follower ({followerList.Count})");
            foreach (dynamic follewer in followerList)
            {
                string photo = $"{follewer.Photo}";
                Clients.Caller.SendAsync("AddFollowerToList", $"{follewer.Name}", $"/User/Index?uid={follewer.Id}", photo);
            }
        }
        public void GetFollowingList()
        {
            if (!IsLogin()) { return; }
            ViewList vlist = new ViewList();
            List<dynamic> followingList = vlist.CastToList(q.FollowingOfId(GetCallerClientId()));
            ChangeText("following-tab", $"Following ({followingList.Count})");
            foreach (dynamic follewing in followingList)
            {
                string photo = $"{follewing.Photo}";
                Clients.Caller.SendAsync("AddFollowingToList", $"{follewing.Name}", $"/User/Index?uid={follewing.Id}", photo);
            }
        }
    }
    //Post management
    public partial class FunctionHub
    {
        public void NewPost(object? forumId,object? postTitle, object? textContent)
        {
            if (!ValidInput(forumId) || !ValidInput(postTitle) || !ValidCaller())
            {
                return;
            }
            String userId = GetCallerUserId();
            String ForumId = new String(forumId);
            if (q.Forum(ForumId) == null)
            {
                SendResult("Error!");
                return;
            }
            String PostTitle = new String(postTitle);
            String Content = new String(textContent);
            Post post = new Post();
            post.ForumId = ForumId;
            post.Title = (string?)PostTitle ?? "No title";
            post.AuthorId = userId;
            post.HeadPostId = null;
            post.LastReply = Time.TimeNowInt();
            post.Create = Time.TimeNowInt();
            post.Floor = 0;
            post.Data = null;
            act.Create(post);
            PostFileIO.WritePostContent(post.PostId, Content);
            SendEventToFollowersWithModelId("Post", "NewPost",post.PostId);
            SendResult("Success!");
        }
        public void EditPost(object? postId, object? textContent)
        {
            if (!ValidInput(postId) || !ValidInput(textContent) || !ValidCaller())
            {
                return;
            }
            String userId=GetCallerUserId();
            String PostId = new String(postId);
            String Content = new String(textContent);
            var post = q.Post(PostId);
            var hp = q.HeadPost(PostId);
            if(post == null) { return; }
            if(post.AuthorId != userId) { return; }
            PostFileIO.WritePostContent(PostId, Content);
            ChangeText($"post_text_content_{PostId}", Content);
            SendResult("Success!");
            return;
        }
        public void ReplyPost(object? postId, object? textContent)
        {
            if (!ValidInput(postId) || !ValidInput(textContent) || !ValidCaller())
            {
                return;
            }
            String userId = GetCallerUserId();
            String PostId = new String(postId);
            String Content = new String(textContent);
            var headPost = q.HeadPost(PostId);
            if (headPost == null) { return; }
            int? floor = db.Posts.Where(p => p.HeadPostId == headPost.PostId).Max(p => p.Floor);
            int refloor = (floor == null) ? 1 : (int)floor + 1;
            Post np = new Post();
            np.Title = "RE : ";
            np.AuthorId = userId;
            np.ForumId = headPost.ForumId;
            np.HeadPostId=headPost.PostId;
            np.Floor = refloor;
            np.Data = null;
            np.Create = Time.TimeNowInt();
            act.Create(np);
            headPost.LastReply = np.Create;
            PostFileIO.WritePostContent(np.PostId, Content);
            var plist = qr.RelatedPost(headPost.PostId);
            SendEvent("Post","Reply",plist);
            SendResult("Success!");
        }
    }
    //Work
    public partial class FunctionHub
    {
        public void CreateNewTag(object? tagName)
        {
            if (!IsLogin()) { return; }
            string? TagName=new String (tagName);
            if(TagName == null)
            {
                SendResult("Error");
                return;
            }
            var tagList = db.Tags.ToList();
            var tagFound=tagList.Where(t=>t.TagName == TagName);
            if (tagFound.Count() > 0)
            {
                SendResult("this tag has already exist!");
                return;
            }
            Tag newTag = new Tag
            {
                TagName = TagName,
            };
            act.Create(newTag);
            //SendResult($"'{TagName}' is Created!");
            string newTagSelection = $"<div><input id=\"{newTag.TagId}\" class=\"tag-options\" value=\"{TagName}\" type=\"checkbox\"/> {TagName}</div>";
            AddFrontEndElement("all-tag-list", newTagSelection);
        }
    }
    public partial class FunctionHub
    {
        protected dbItemRelatedQuery qr=new dbItemRelatedQuery();
        public void SendEventToFollowers(string controllerName, string actionName)
        {
            String userId = GetCallerUserId();
            var followers = q.FollowersOfId(userId);
            if(followers.Count() > 0)
            {
                SendEvent(controllerName, actionName, followers);
            }
        }
        public List<EventData>? CreateEvent(string routePath, object? model)
        {
            var evtData = EventConfig.GetEventData(routePath);
            if (!evtData.IsValid) { return null; }
            String userId = GetCallerUserId();
            EventHelper eventData = new EventHelper(evtData, userId, routePath, model);
            return eventData.EventDataList();
        }

        public void SendEvent(string controllerName, string actionName, object? model)
        {
            var evt = CreateEvent($"/{controllerName}/{actionName}", model);
            if (evt == null) { return; }
            foreach (var data in evt)
            {
                SendNotify(data);
            }
        }
    }
    public partial class FunctionHub
    {
        public void SendEventToFollowersWithModelId(string controllerName, string actionName, String modelId)
        {
            String userId = GetCallerUserId();
            var followers = q.FollowersOfId(userId);
            if (followers.Count() > 0)
            {
                SendEventWithModelId(controllerName, actionName, followers, modelId);
            }
        }
        public List<EventData>? CreateEventWithModelId(string routePath, object? model, String modelId)
        {
            var evtData = EventConfig.GetEventData(routePath);
            if (!evtData.IsValid) { return null; }
            String userId = GetCallerUserId();
            EventHelper eventData = new EventHelper(evtData, userId, routePath, model);
            return eventData.EventDataList(modelId);
        }

        public void SendEventWithModelId(string controllerName, string actionName, object? model, String modelId)
        {
            var evt = CreateEventWithModelId($"/{controllerName}/{actionName}", model, modelId);
            if (evt == null) { return; }
            foreach (var data in evt)
            {
                SendNotify(data);
            }
        }
    }
}
