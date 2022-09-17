using Microsoft.AspNetCore.Mvc;
using MyLib;
using String = MyLib.String;
namespace Godch.Event
{
    public class EventData
    {
        public dbQuery q = new dbQuery();
        public String UserId { get; set; }
        public String UserId2 { get; set; }
        public String Target { get; set; }
        public String TargetId { get; set; }
        public string Event { get; set; }
        public String Route { get; set; }
        public string ControllerParameter { get; set; }
        public String TimeLong { get; set; }
        public String TimeString { get; set; }
        public string? Url { get; set; }
        public void SetActionUser(object userId)
        {
            UserId = userId.ToString();
        }
        public EventData()
        {
            UserId = "";
            UserId2 = "";
            Target = "";
            TargetId = "";
            Event = "";
            Route = "";
            ControllerParameter = "";
            TimeLong = "";
            TimeString = "";
        }
        public EventData(object uid1, object uid2, EventProperties eventProperties, object? targetId, long time) : this()
        {
            UserId = uid1.ToString();
            UserId2 = uid2.ToString();
            Target = eventProperties.Target;
            TargetId = targetId?.ToString();
            Event = eventProperties.Event;
            Route = eventProperties.Route;
            ControllerParameter = eventProperties.ControllerParameter;
            TimeLong = time;
            TimeString = Time.ToString(TimeLong);
        }
        public void LoadXmlData(XmlData data)
        {
            UserId = Xml.GetAttributeValue(data, "UserId");
            Target = Xml.GetAttributeValue(data, "Target");
            TargetId = Xml.GetAttributeValue(data, "TargetId");
            Event = Xml.GetAttributeValue(data, "Event");
            TimeLong = Xml.GetAttributeValue(data, "time");
            TimeString = Time.ToString(TimeLong);
            Url = Xml.GetContent(data);
        }
        public XmlData ToXmlData()
        {
            XmlData data = new XmlData();
            data.Tag = "notify";
            data.AddAttribute("UserId", UserId.ToString());
            data.AddAttribute("Target", Target.ToString());
            data.AddAttribute("TargetId", TargetId.ToString());
            data.AddAttribute("Event", Event.ToString());
            data.AddAttribute("time" , TimeLong);
            string? ControllerName = Route.SubString("/", "/");
            data.Data= $"{Config.GodchWebRoot}/{ControllerName}/Index?{ControllerParameter}={TargetId}";
            return data;
        }
        public string Description()
        {
            return $"[{Event}]{Target} by {q.UserName(UserId)}! ({TimeString})";
        }
        public string GetUrl()
        {
            string? ControllerName = Route.SubString("/", "/");
            return $"{Config.GodchWebRoot}/{ControllerName}/Index?{ControllerParameter}={TargetId}";
        }
        public void SetUrl()
        {
            string? ControllerName = Route.SubString("/", "/");
            Url= $"{Config.GodchWebRoot}/{ControllerName}/Index?{ControllerParameter}={TargetId}";
        }
    }
}
