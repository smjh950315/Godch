using Godch.Models;
using Godch.ViewModels;
using Xml = MyLib.Xml;
using String = MyLib.String;
using MyLib;
using System.IO;
using System.Drawing;

namespace Godch
{
    public static class AccountHelper
    {
        public static string? LogDataCheck(vLogin l)
        {
            GODCHContext db = new GODCHContext();
            var user = db.Users.Where(u=>u.Account == l.Account).FirstOrDefault();
            if (user == null) return null;
            if (l.Password != user.Password) return null;
            return user.UserId.ToString();
        }
        public static UISettings ReadUI(User? user)
        {
            if (user == null) { return new UISettings(); }
            UISettings settings = new UISettings(user);
            return settings;
        }
        public static UISettings ReadUI(int uid)
        {
            dbQuery q = new dbQuery();
            var user = q.User(uid);
            return ReadUI(user);
        }
        public static UserTags ReadUserTag(String userId)
        {
            dbQuery q = new dbQuery();
            return new UserTags(q.User(userId));
        }
        public static List<ChatGroup> GetChatGroups(int? uid)
        {
            GODCHContext db = new GODCHContext();
            List<ChatGroup> chats = new();
            var crs = db.ChatGroupRelations.Where(g => g.UserId == uid).ToList();
            foreach (var cr in crs)
            {
                var chat = db.ChatGroups.Find(cr.ChatId);
                if (chat != null)
                {
                    chats.Add(chat);
                }
            }
            return chats;
        }
    }
    public class UISettings
    {        
        public string? BgColor = "#FFFFFF";
        public string? TextColor = "#000000";
        public string? Font = "Consolas";
        public String UserId = new String();
        private XmlData ToXml()
        {
            XmlData xml = new XmlData("ui");
            xml.AddAttribute("BgColor", BgColor);
            xml.AddAttribute("TextColor", TextColor);
            xml.AddAttribute("Font", Font);
            return xml;
        }
        private void SetDefault()
        {
            BgColor = "#FFFFFF";
            TextColor = "#000000";
            Font = "Consolas";
        }
        private void ReadXmlData(XmlData? xml)
        {            
            if (xml == null)
            {
                SetDefault();
            }
            else
            {
                BgColor = xml.GetAttribute("BgColor")?.Value ?? BgColor;
                TextColor = xml.GetAttribute("TextColor")?.Value ?? TextColor;
                Font = xml.GetAttribute("Font")?.Value ?? Font;
            }
        }
        public void ReadXml()
        {
            if (UserId.Length == 0)
            {
                return;
            }
            var path = $"{Config.UserData}\\{UserId}\\ui.txt";
            if (File.Exists(path))
            {
                var xml = Xml.ReadXml(path, "ui");
                if (xml.Count == 1)
                {
                    ReadXmlData(xml[0]);
                }
            }
            else
            {
                SetDefault();
                var xmlString = Xml.GetString(ToXml());
                File.AppendAllText(path, xmlString);
            }
        }
        public void SaveXml()
        {
            if (UserId.Length == 0)
            {
                return;
            }
            XmlData xml = new XmlData("ui");
            xml.AddAttribute("BgColor", BgColor);
            xml.AddAttribute("TextColor", TextColor);
            xml.AddAttribute("Font", Font);
            var xmlString = Xml.GetString(xml);
            File.WriteAllText($"{Config.UserData}\\{UserId}\\ui.txt", xmlString);
        }
        public UISettings()
        {
        }
        public UISettings(User? user)
        {
            if (user != null)
            {
                UserId = user.UserId;
                ReadXml();
            }
        }
    }
    public class UserTags
    {
        private dbQuery q = new dbQuery();
        public List<Tag> TagList = new List<Tag>();
        public String UserId = new String();
        public String StringType = new String();
        private void ListToStringType()
        {
            if (TagList.Count==0)
            {
                return ;
            }
            else
            {
                StringType = "";
                for(int i = 0; i < TagList.Count; i++)
                {
                    if(i == TagList.Count - 1)
                    {
                        StringType += TagList[i].TagId;
                    }
                    else
                    {
                        StringType += TagList[i].TagId;
                        StringType += ",";
                    }
                }
            }
        }
        private void ReadXmlData(XmlData? xml)
        {
            if(xml == null) { return ; }
            StringType = xml.Data;
            string[] data = StringType.Split(",");
            foreach(var tagId in data)
            {
                var tag = q.Tag((String)tagId);
                if(tag != null)
                {
                    TagList.Add(tag);
                }
            }
        }
        public void ReadXml()
        {
            if(UserId.Length == 0 || UserId == 0)
            {
                return ;
            }
            var path = $"{Config.UserData}\\{UserId}\\tag.txt";
            if (File.Exists(path))
            {
                var xml = Xml.ReadXml(path, "tag");
                if(xml.Count == 1)
                {
                    ReadXmlData(xml[0]);                    
                }
            }
        }
        public void SaveXml()
        {
            if(UserId.Length==0 || UserId == 0)
            {
                return;
            }
            XmlData xml = new XmlData("tag");
            ListToStringType();
            xml.Data = StringType;
            var xmlString = Xml.GetString(xml);
            File.WriteAllText($"{Config.UserData}\\{UserId}\\tag.txt",xmlString);
        }
        public void LoadTagList(List<Tag> tags)
        {
            TagList = tags;
        }
        public UserTags()
        {
            TagList = new List<Tag>();
        }
        public UserTags(User? user):this()
        {
            if(user != null)
            {
                UserId = user.UserId;
                ReadXml();
            }
        }
    }

}
