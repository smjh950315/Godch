using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Godch.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MyLib;
using Godch.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.CSharp.RuntimeBinder;
using System.Text.Json;
using MyLib.Mvc;
using Godch.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Godch.Event
{
    public class EventListener : Controllers.Controller, IActionFilter
    {
        private readonly IHubContext<FunctionHub> _hub;
        public EventListener(IHubContext<FunctionHub> hub)
        {
            _hub = hub;
        }
        public dynamic? GetCurrentPageResultModel(dynamic? context)
        {
            try
            {
                return context?.Result?.Model ?? null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public dynamic? GetAllPageResultModel(dynamic? context)
        {
            try
            {
                return context.Controller.ItemList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<EventData>? GetEventList(dynamic context)
        {
            var evtData = EventConfig.GetEventData(context.HttpContext.Request.Path.Value);
            if (!evtData.IsValid) { return null; }
            if (context.HttpContext.User.Identity == null) { return null; }
            if (context.HttpContext.User.Identity.Name == null) { return null; }
            CtxResult contextResult = new CtxResult();
            contextResult.EventType = evtData;
            contextResult.UserId = Convert.ToInt32(context.HttpContext.User.Identity.Name);
            contextResult.ActionPath = context.HttpContext.Request.Path.Value;
            contextResult.Model = GetAllPageResultModel(context);
            if (contextResult == null) { return new List<EventData>(); }
            EventHelper helper = new(contextResult);
            return helper.EventDataList();
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            return;
        }
        public override async void OnActionExecuted(ActionExecutedContext context)
        {
            var EvtDataList = GetEventList(context);
            if(EvtDataList == null) { return; }
            foreach(var data in EvtDataList)
            {
                if(data.UserId2 != null)
                {                    
                    string targetUser = data.UserId2;
                    var evtXml = data.ToXmlData();
                    GodchDirectories.NotifyData.AppendText(targetUser + ".txt", evtXml);                    
                    await _hub.Clients.Group("notify"+targetUser).SendAsync("ReceiveNotify", data.Description(), data.Url, data.TimeString.ToString());
                }                
            }
            return;
        }
    }
    public class CtxResult
    {
        public object? EventType { get; set; }
        public object? UserId { get; set; }
        public object? ActionPath { get; set; }
        public object? Model { get; set; }

    }
}
