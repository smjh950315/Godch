using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Godch.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MyLib;
using Godch.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.CSharp.RuntimeBinder;
using System.Text.Json;
using Godch.Hubs;
using String = MyLib.String;
using Microsoft.Extensions.Hosting;

namespace Godch.Event
{
    //取得相關使用者=>產生EVTDATA到相關使用者資料夾
    public class EventHelper
    {
        public string Route { get; private set; }
        public dbQuery q { get; private set; }
        public User? ActionUser { get; private set; }
        public EventProperties? EventType { get; private set; }
        public List<dynamic> RelatedUser { get; private set; }
        public List<dynamic> RelatedUserId { get; private set; }
        public List<dynamic> RelatedModel { get; private set; }
        public List<EventData> EventLogs { get; private set; }       
        public String ModelId { get; private set; }
        public Dictionary<string, string> ModelDictionary { get; private set; }
        private EventData? GetEventData(object relatedUserId, object? modelId)
        {
            if (ActionUser == null || EventType == null) return null;
            return new EventData(ActionUser.UserId, relatedUserId, EventType, modelId, Time.TimeNowInt());
        }
        private void GetEventData()
        {
            foreach (var ruid in RelatedUserId)
            {
                var log = GetEventData(ruid, ModelId);
                if (log != null) { EventLogs.Add(log); }
            }
        }
        private void LoadRelatedList(dynamic? models)
        {
            if (models == null) return;
            foreach (var model in models)
            {
                RelatedModel.Add(model);
                TryAddNoRepeatRelatedUserIdByModel(model);
                FillRelatedUserListByIdList();
            }
        }
        private void FillRelatedUserListByIdList()
        {
            foreach (dynamic uid in RelatedUserId)
            {
                var user = q.User(uid);
                if (user != null)
                {
                    RelatedUser.Add(user);
                }
            }
        }
        private void TryAddNoRepeatRelatedUserIdByModel(object? model)
        {
            if (model == null) { return; }
            string? UID = null;
            foreach (string tryKey in Config.UserIdQueryKey)
            {
                bool tried = false;
                if (!tried)
                {
                    try
                    {
                        var uid = model.GetType().GetProperties().Where(p => p.Name == tryKey).First().GetValue(model)?.ToString();
                        if (uid != null) { UID = uid; break; }
                    }
                    catch
                    {
                        tried = true;
                    }
                }
            }
            if (UID == null) { return; }
            int userId = Convert.ToInt32(UID);
            if (!RelatedUserId.Contains(userId))
            {
                RelatedUserId.Add(userId);
            }
            return;
        }
        public EventHelper()
        {
            Route = "";
            q = new dbQuery();
            ModelId = new String();
            ActionUser = new User();            
            EventType = new EventProperties();
            EventLogs = new List<EventData>();
            RelatedUser = new List<dynamic>();
            RelatedModel = new List<dynamic>();
            RelatedUserId = new List<dynamic>();
            ModelDictionary = new Dictionary<string, string>();
        }
        public EventHelper(dynamic contextContent) : this()
        {
            LoadContextContent(contextContent);
        }
        public EventHelper(EventProperties? eventType,object? actionUser,string? route,object? model):this()
        {
            CtxResult result = new CtxResult()
            {
                EventType=eventType,
                UserId=actionUser,
                ActionPath=route,
                Model=model
            };
            LoadContextContent(result);
        }
        public void LoadContextContent(dynamic contextContent)
        {
            Route = contextContent.ActionPath;
            EventType = contextContent.EventType;
            ActionUser = q.User(contextContent.UserId);
            var Model = contextContent.Model;
            //var Model = TryGetModel(contextContent);
            var firstModel = ObjectHelper.TryGetObjectInList(Model, 0);
            ModelId = ObjectHelper.TryGetMemberValue(firstModel,EventType.PrimaryKeyName);
            if (ModelId == null || ModelId.Length == 0)
            {
                ModelId = ObjectHelper.TryGetMemberValue(firstModel, EventType.SecondaryKeyName);
            }
            ModelDictionary = ObjectHelper.ClassToDictionary(firstModel);
            LoadRelatedList(Model);
            GetEventData();
        }

        public List<EventData> EventDataList()
        {
            var list = new List<EventData>();
            foreach (var log in EventLogs)
            {
                list.Add(log);
            }
            return list;
        }
        public List<EventData> EventDataList(String IdOverride)
        {
            var list = new List<EventData>();
            foreach (var log in EventLogs)
            {
                log.TargetId = IdOverride;
                list.Add(log);
            }
            return list;
        }
    }

}
