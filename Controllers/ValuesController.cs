using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using Newtonsoft;
using MyLib;
using Godch.Hubs.Models;
using System.Web;
using String = MyLib.String;
using Godch.Models;
using System.Diagnostics;
using System.Security.Cryptography;
using Godch.ViewModels;

namespace Godch.Controllers
{
    [Route("api")]
    [ApiController]
    public partial class ValuesController : Controller
    {
        [HttpGet]
        [Route("get/chatList")]
        public IEnumerable<ChatGroupInfo> GetChatList()
        {
            if (!IsLogin())
            {
                return null;
            }
            var list = HubHelper.GetGroups(UserID());
            if (list == null)
            {
                return null;
            }
            return list;
        }
        [HttpGet]
        [Route("get/tagList")]
        public dynamic GetTagList()
        {
            return db.Tags.ToList();
        }
        [HttpPost]
        [Route("set/tag")]
        public dynamic? SaveTag(dynamic json)
        {
            if (!IsLogin())
            {
                return Ok("ErrorNotLogin");
            }
            var workTagData = ProcessWorkTags(json);
            if (workTagData == null) { return null; }
            long wid = (long)workTagData.WorkId;
            var TagRelations = db.TagRelations.Where(tr => tr.WorkId == wid).ToList();
            var tagList = (List<int>)workTagData.Tags;
            List<TagRelation> tagsToRemove = new List<TagRelation>();
            foreach (var tr in TagRelations)
            {
                if (tagList.Contains(tr.TagId))
                {
                    tagList.Remove(tr.TagId);
                }
                else
                {
                    tagsToRemove.Add(tr);
                }
            }
            foreach (var trRemove in tagsToRemove)
            {
                act.Delete(trRemove);
            }
            foreach (var tId in tagList)
            {
                TagRelation r = new TagRelation
                {
                    WorkId = wid,
                    TagId = tId
                };
                act.Create(r);
            }
            return Ok(q.TagsOf(q.Work(wid)));
        }
        [HttpPost]
        [Route("get/workTags")]
        public dynamic? GetWorkTag(dynamic json)
        {
            String strType = json.ToString();
            String wid = (string?)strType.SubString("{\"wid\":\"", "\"}");
            if (!wid.IsNull)
            {
                return Ok(q.SelectedTagsOf(wid));
            }
            return null;
        }
        [HttpPost]
        [Route("search/works")]
        public dynamic? SearchWork(dynamic json)
        {
            JsonResponseData jdata = new JsonResponseData(json.GetRawText());
            if (jdata.Target != "work" || jdata.Method != "search")
            {
                return "selection is invalid";
            }
            string? searchBy = jdata.ParamName;
            string? paramValue = jdata.Id;
            ViewList vlist = new ViewList();
            if (paramValue == null || paramValue.Length == 0)
            {
                var works = db.Works.ToList();
                vlist.CastList(works);
                return vlist.Values;
            }
            if (searchBy == null)
            {
                return "selection is invalid";
            }
            if (searchBy == "not-selected") { return null; }
            else if (searchBy == "byTag")
            {
                var tid = db.Tags.FirstOrDefault(t => t.TagName.Contains(paramValue))?.TagId;
                if (tid == null) { return null; }
                List<TagRelation> r = db.TagRelations.Where(tr => tr.TagId == tid.Value).ToList();
                if (r.Count == 0) { return null; }
                List<Work> works = new List<Work>();
                foreach (TagRelation t in r)
                {
                    var w = q.Work(t.WorkId);
                    if (w != null) { works.Add(w); }
                }
                vlist.CastList(works);
                return vlist.Values;
            }
            else if (searchBy == "byTitle")
            {
                var works = db.Works.Where(w => w.WorkName.Contains(paramValue)).ToList();
                vlist.CastList(works);
                return vlist.Values;
            }
            else if (searchBy == "byAuthor")
            {
                List<Work> result = new List<Work>();
                var user = db.Users.Where(u => u.UserName.Contains(paramValue)).ToList();
                if (user.Count == 0) { return null; }
                foreach (var u in user)
                {
                    var w = db.Works.Where(w => w.AuthorId == u.UserId).ToList();
                    if (w.Count != 0)
                    {
                        foreach (var w2 in w)
                        {
                            if (w2 != null)
                            {
                                result.Add(w2);
                            }
                        }
                    }
                }
                vlist.CastList(result);
                return vlist.Values;
            }
            return "empty result";
        }

        [HttpPost]
        [Route("save/ui")]
        public dynamic? SaveUi(dynamic json)
        {
            JsonData jdata = new JsonData(json.GetRawText());
            string? bgColor = jdata.GetAttributeValue("bgColor");
            string? textColor = jdata.GetAttributeValue("textColor");
            if (bgColor == null && textColor == null)
            {
                return null;
            }
            var user = CurrentUser();
            if (user == null)
            {
                return null;
            }
            UISettings uiSettings = new UISettings(user);
            uiSettings.BgColor = bgColor;
            uiSettings.TextColor = textColor;
            uiSettings.SaveXml();
            return null;
        }

        [HttpPost]
        [Route("get/userTag")]
        public dynamic GetUserTag(dynamic json)
        {
            JsonData jsonData = new JsonData(json.GetRawText());
            String userId = jsonData.GetAttributeValue("uid");
            var tags = AccountHelper.ReadUserTag(userId);
            return tags.TagList.ToArray();
        }
        [HttpPost]
        [Route("set/userTag")]
        public dynamic? SaveUserTag(dynamic json)
        {
            JsonData jsonData = new JsonData(json.GetRawText());
            String userId = jsonData.GetAttributeValue("uid");
            string? tagString = jsonData.GetAttributeValue("tags");
            if (tagString == null)
            {
                return null;
            }
            if (tagString.StartsWith('['))
            {
                tagString = tagString.Substring(1);
            }
            if (tagString.EndsWith(']'))
            {
                tagString = tagString.Substring(0, tagString.Length - 2);
            }
            var tagIds = tagString.Split(',');
            List<string> ids2 = new List<string>();
            foreach (var id in tagIds)
            {
                string tempId = "";
                if (id.StartsWith('"'))
                {
                    tempId = id.Substring(1);
                }
                if (id.EndsWith('"'))
                {
                    tempId = tempId.Substring(0, id.Length - 1);
                }
                tempId = tempId.Trim();
                ids2.Add(tempId);
            }

            List<Tag> tagList = new List<Tag>();
            foreach (string tag in ids2)
            {
                var t = q.Tag((String)tag);
                if (t != null)
                {
                    tagList.Add(t);
                }
            }
            UserTags tags = new UserTags();
            tags.UserId = userId;
            tags.LoadTagList(tagList);
            tags.SaveXml();
            return tags.TagList.ToArray();
        }
        [HttpPost]
        [Route("search")]
        public dynamic? NavSearch(dynamic json)
        {
            JsonData data = new JsonData(json.GetRawText());
            var searchOption = data.GetAttributeValue("option");
            var searchKey = data.GetAttributeValue("keyword");
            if (searchOption == null || searchKey == null)
            {
                return null;
            }
            if(searchOption == "Work")
            {
                return db.Works.Where(w=>w.WorkName.Contains(searchKey)).ToList();
            }
            if(searchOption == "Post")
            {
                return db.Posts.Where(p => p.Title.Contains(searchKey)).ToList();
            }
            if(searchOption == "Forum")
            {
                return db.Forums.Where(f => f.ForumName.Contains(searchKey)).ToList();
            }
            return null;
        }
    }
    public partial class ValuesController
    {
        public dynamic? ProcessWorkTags(dynamic json)
        {
            dynamic? workTags = new System.Dynamic.ExpandoObject();
            String strType = json.ToString();
            string? workId = strType.SubString("\"wid\":\"", "\",");
            if (workId == null) { return null; }
            string? tagIdsRaw = strType.SubString("\"tags\":[", "]");
            string[]? tagIds;
            if (tagIdsRaw != null)
            {
                tagIds = tagIdsRaw.Split("\"");
            }
            else
            {
                return null;
            }
            List<int> tags = new List<int>();
            foreach (var s in tagIds)
            {
                if (s != "," && s != "")
                {
                    String ss = s;
                    tags.Add(ss);
                }
            }
            workTags.WorkId = new String(workId);
            workTags.Tags = tags;
            return workTags;
        }
    }
    public partial class ValuesController
    {
        [HttpPost]
        [Route("show")]
        public dynamic? GetPostedJson(dynamic? json)
        {
            return json;
        }
        [HttpPost]
        [Route("get/js")]
        public dynamic ApiPost(dynamic json)
        {
            JsonResponseData jdata = new JsonResponseData(json.GetRawText());
            return jdata;
        }
    }
}
