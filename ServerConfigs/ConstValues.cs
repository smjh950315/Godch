using MyLib;
using Godch.Event;
using String = MyLib.String;
using Godch.Models;

namespace Godch
{
    //Notify: root\\NotifyData\\userId.txt
    //Chat: root\\ChatData\\ChatId.txt
    //Post: root\\PostData\\PostId.txt
    //Work: root\\Works\\workId.XXX
    //User???: root\\UserData\\UserId\\???.txt
    // ???: ui
    public static class Config
    {
        public static readonly string Root = "Z:/Data/_GodchData";
        public static readonly string Posts = $"{Root}/Posts";
        public static readonly string Works = $"{Root}/Works";
        public static readonly string UserData = $"{Root}/UserData";
        public static readonly string ChatData = $"{Root}/ChatData";
        public static readonly string NotifyData = $"{Root}/NotifyData";
        public static readonly int PostBackupVerMax = 100;
        public static readonly string ServerConfigXml = "web.config";
        public static readonly string GodchWebRoot = "http://localhost:11920";
        public static readonly string GodchApi = "http://localhost:11920/api/";
        public static string[] UserIdQueryKey = new string[] { "AuthorId", "UserId" };
    }
    public static class EventConfig
    {
        public static class Base
        {
            public static EventProperties Post = new EventProperties("Post", "PostId", "pid");
            public static EventProperties Forum = new EventProperties("Forum", "ForumId", "fid");
            public static EventProperties User = new EventProperties("User", "UserId", "uid");
            public static EventProperties Work = new EventProperties("Work", "WorkId", "wid");
        }
        public static EventProperties[] EventList = new EventProperties[]
        {
            new EventProperties(Base.Post,"NewPost","post","HeadPostId"),
            new EventProperties(Base.Post,"Reply","reply","HeadPostId"),
            new EventProperties(Base.Post,"Edit","edit","HeadPostId"),
            new EventProperties(Base.Post,"Mention","mentioned"),
            new EventProperties(Base.User,"Follow","followed"),
            new EventProperties(Base.User,"AddFriend","invited"),
            new EventProperties(Base.User,"Index","view"),
            new EventProperties(Base.Work,"Upload","view")
        };
        public static EventProperties GetEventData(string? contextRoute)
        {
            if (contextRoute == null)
            {
                return new EventProperties();
            }
            foreach (EventProperties e in EventList)
            {
                if (e.Route == contextRoute)
                {
                    return e;
                }
            }
            return new EventProperties();
        }
        public static EventProperties GetEventData(string? controllerName, string? actionName)
        {
            if (controllerName == null)
            {
                return new EventProperties();
            }
            foreach (EventProperties e in EventList)
            {
                if (e.Route == $"/{controllerName}/{actionName}")
                {
                    return e;
                }
            }
            return new EventProperties();
        }
    }
    public static class ServerMessage
    {
        public static readonly string InvalidAccount = "Invalid account!";
        public static readonly string InvalidEmail = "Invalid Email!";
        public static readonly string LoginError = "Invalid account or password!";
        public static readonly string LoginSuccessed = "Welcome ";
    }
    public static class GodchDirectories
    {
        public static SetDirectory Root = new(Config.Root);
        public static SetDirectory Posts = new(Config.Posts);
        public static SetDirectory Works = new(Config.Works);
        public static SetDirectory UserData = new(Config.UserData);
        public static SetDirectory ChatData = new(Config.ChatData);
        public static SetDirectory NotifyData = new(Config.NotifyData);
    }

}
