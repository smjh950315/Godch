using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Godch.Event
{
    public class EventProperties
    {
        public bool IsValid { get; set; }
        public string Route { get; set; }
        public string Target { get; set; }
        public string Event { get; set; }
        public string ControllerParameter { get; set; }
        public string PrimaryKeyName { get; set; }
        public string? SecondaryKeyName { get; set; }
        private void Set(string actionName, string @event)
        {
            Route = $"/{Route}/{actionName}";            
            Event = @event;
            IsValid = true;
        }
        public EventProperties() : this("", "", "")
        {
        }
        public EventProperties(string controllerName, string primaryKey, string controllerParameterName)
        {

            Route = controllerName;
            PrimaryKeyName = primaryKey;
            ControllerParameter = controllerParameterName;
            Target = controllerName.ToLower();
            Event = "";
        }
        public EventProperties(EventProperties baseProperties, string actionName, string @event)
            :this(baseProperties, actionName,@event,null)
        {
        }
        public EventProperties(EventProperties baseProperties, string actionName, string @event, string? secondaryKey)
            : this(baseProperties.Route, baseProperties.PrimaryKeyName, baseProperties.ControllerParameter)
        {
            Set(actionName, @event);
            SecondaryKeyName = secondaryKey;
        }
    }

}
