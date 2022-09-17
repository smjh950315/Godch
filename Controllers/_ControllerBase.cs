using Godch.Models;
using Godch.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyLib;
using MyLib.Mvc;
using System.Dynamic;
using lz = MyLib.LazyConvert;
using String = MyLib.String;
using Storage = Godch.GlobalStorage.ControllerStorage;
using Godch.GlobalStorage;
using Godch.Hubs;
using Microsoft.AspNetCore.SignalR;
using Godch.Event;
using System;

namespace Godch.Controllers
{
    [TypeFilter(typeof(Event.EventListener))]
    public partial class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        protected PageHelper pageHelper = new PageHelper(10);
        protected IHubContext<FunctionHub> _hub;
        public RedirectToActionResult HomePage()
        {
            return RedirectToAction("Index", "Home");
        }
        public dbAction act = new();
        public GODCHContext db = new();
        public dbQuery q = new();
        public ModelCast mCast = new();
        public ViewList vl = new();
        public dynamic? ItemList = new System.Dynamic.ExpandoObject();
        public dynamic Model = new System.Dynamic.ExpandoObject();
        public string? EntityName { get; set; }
        public void AddToItemList(object? obj)
        {
            if (obj == null) { return; }
            List<object> list = new List<object>();
            if (obj.GetType() == list.GetType())
            {
                ItemList.Add(obj);
            }
            else
            {
                list.Add(obj);
                ItemList = list;
            }
        }
        public void AddDefaultPhoto(int uid)
        {
            GodchDirectories.UserData.AddFolderIfNotExist($"{uid}");
            System.IO.File.Copy($"{Config.UserData}\\default.png", $"{Config.UserData}\\{uid}\\photo.jpg");
        }
        public bool IsLogin()
        {
            if (User != null && User.Identity != null && User.Identity.Name != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public User? CurrentUser()
        {
            return IsLogin() ? q.User(Convert.ToInt32(User.Identity?.Name)) : null;
        }
        public int? UserID()
        {
            return CurrentUser()?.UserId;
        }
        public bool IsSelf(int uid) { return UserID() == uid; }
        public bool UidValid(int? uid) { return uid == null ? false : q.User(uid) != null; }
        public bool FidValid(int? fid) { return q.Forum(fid) != null; }
        public bool PidValid(long? pid) { return q.Post(pid) != null; }
        public bool WidValid(int? wid) { return q.Work(wid) != null; }
        public int GetSelfHelperIndex()
        {
            int userId;
            var uid = UserID();
            if (uid == null)
            {
                userId = 0;
            }
            else
            {
                userId = uid.Value;
            }
            var h = Storage.PageHelpers.Where(ph => ph.Id == userId).FirstOrDefault();
            if (h != null) { return Storage.PageHelpers.IndexOf(h); }
            else
            {
                PageHelper helper = new PageHelper();
                helper.Id = userId;
                Storage.PageHelpers.Add(helper);
                return Storage.PageHelpers.IndexOf(helper);
            }
        }
        public int GetSelfVarIndex()
        {
            int userId;
            var uid = UserID();
            if (uid == null)
            {
                userId = 0;
            }
            else
            {
                userId = uid.Value;
            }
            var h = Storage.UserVars.FirstOrDefault(ph => ph.Id == userId);
            if (h != null)
            {
                return Storage.UserVars.IndexOf(h);
            }
            else
            {
                UserVariable varStorage = new UserVariable(userId);
                Storage.UserVars.Add(varStorage);
                return Storage.UserVars.IndexOf(varStorage);
            }
        }
        public dynamic PageReloadAuto(dynamic? entityList, int? page)
        {
            return PageReloadAuto(EntityName, entityList, page, null, false);
        }
        public dynamic PageReloadAuto(string EntityName, dynamic? entityList, int? page, int? pageSize)
        {
            return PageReloadAuto(EntityName, entityList, page, pageSize, false);
        }
        public dynamic PageReloadAuto(string EntityName, dynamic? entityList, int? page, int? pageSize, bool update)
        {
            dynamic pageData = new System.Dynamic.ExpandoObject();
            dynamic ViewList = new System.Dynamic.ExpandoObject();
            ItemList = entityList;
            if (entityList == null)
            {
                PageHelper helper = new PageHelper();
                pageData.ViewList = helper.ToPage(page);
                pageData.Page = helper.Page;
                return pageData;
            }
            int helperIndex = GetSelfHelperIndex();
            if (page == null || Storage.PageHelpers[helperIndex].EntityNow != EntityName || update)
            {
                Storage.PageHelpers[helperIndex].ClearPageInfo();
                Storage.PageHelpers[helperIndex].SetPageSize(pageSize);
                Storage.PageHelpers[helperIndex].EntityNow = EntityName;
                Storage.PageHelpers[helperIndex].Load(entityList);
                ViewList = Storage.PageHelpers[helperIndex].ToPage(page);
            }
            else
            {
                ViewList = Storage.PageHelpers[helperIndex].ToPage(page);
            }
            var Page = Storage.PageHelpers[helperIndex].Page;
            pageData.Page = Page;
            pageData.ViewList = ViewList;
            return pageData;
        }
        public dynamic PageReloadAuto(dynamic? entityList, int? page, int? pageSize)
        {
            return PageReloadAuto(EntityName, entityList, page, pageSize, false);
        }
        public void SendNotifyToFollowers(string route, string controllerParameter, string targetName, object eventModelId, string eventName)
        {
            var uid1 = UserID();
            if (uid1 == null || uid1 == 0) { return; }
            var notifiedUserList = q.FollowersOfId(uid1.Value);
            if (notifiedUserList == null) { return; }
            List<EventData> list = new List<EventData>();
            var time = Time.TimeNowInt();
            EventProperties p = new EventProperties
            {
                Target = targetName,
                Event = eventName,
                Route = route,
                ControllerParameter = controllerParameter,
            };
            foreach (var u in notifiedUserList)
            {
                EventData evt = new EventData(uid1, u.UserId, p, eventModelId, time);
                list.Add(evt);
            }
            foreach (var data in list)
            {
                String targetClientId = data.UserId2;
                var evtXml = data.ToXmlData();
                GodchDirectories.NotifyData.AppendText($"{targetClientId}.txt", evtXml);
                _hub.Clients.Group($"notify{data.UserId2}").SendAsync("ReceiveNotify", data.Description(), evtXml.Data, data.TimeString.ToString());
            }
        }
    }
    public partial class UserController : Controller
    {
        public bool[] UserRelations(UserRelationShip? rA, UserRelationShip? rB)
        {
            return new bool[6]
            {
                    lz.TorF(rA?.Block),
                    lz.TorF(rB?.Block),
                    lz.TorF(rA?.Following),
                    lz.TorF(rB?.Following),
                    lz.TorF(rA?.FriendShip),
                    lz.TorF(rB?.FriendShip)
            };
        }
        public bool IsWaiting(int uid)
        {
            return IsWaiting(UserID(), uid);
        }
        public bool IsWaiting(int? uid1, int uid2)
        {
            var r = q.UserRelations(uid1, uid2);
            return !r[0] && r[4] && !r[5];
        }
        public bool IsWaiting(UserRelationShip? rA, int uid2)
        {
            return IsWaiting(rA, q.UserRelation(uid2, UserID()));
        }
        public bool IsWaiting(UserRelationShip? rA, UserRelationShip? rB)
        {
            var r = UserRelations(rA, rB);
            return !r[0] && r[4] && !r[5];
        }
        public bool Unrequest(int uid)
        {
            return Unrequest(UserID(), uid);
        }
        public bool Unrequest(int? uid1, int uid2)
        {
            var r = q.UserRelations(uid1, uid2);
            return r[5] && !r[4];
        }
        public bool Unrequest(UserRelationShip? rA, int uid2)
        {
            return Unrequest(rA, q.UserRelation(uid2, UserID()));
        }
        public bool Unrequest(UserRelationShip? rA, UserRelationShip? rB)
        {
            var r = UserRelations(rA, rB);
            return r[5] && !r[4];
        }
        public bool IsBlocked(int uid)
        {
            return IsBlocked(UserID(), uid);
        }
        public bool IsBlocked(int? uid1, int? uid2)
        {
            var r = q.UserRelation(uid2, uid1);
            return r != null ? r.Block : false;
        }
        public bool IsFriend(int uid)
        {
            return IsLogin() ? IsFriend(UserID(), uid) : false;
        }
        public bool IsFriend(int? uid1, int uid2)
        {
            if (IsLogin())
            {
                bool[] r = q.UserRelations(uid1, uid2);
                return !(r[0] || r[1]) && r[4] && r[5];
            }
            return false;
        }
        public bool IsFriend(UserRelationShip? rA, int uid2)
        {
            if (IsLogin())
            {
                bool[] r = UserRelations(rA, q.UserRelation(uid2, UserID()));
                return !(r[0] || r[1]) && r[4] && r[5];
            }
            return false;
        }
        public bool IsFriend(UserRelationShip? rA, UserRelationShip? rB)
        {
            if (IsLogin())
            {
                bool[] r = UserRelations(rA, rB);
                return !(r[0] || r[1]) && r[4] && r[5];
            }
            return false;
        }
        public bool IsFollowing(int uid)
        {
            return IsLogin() ? IsFollowing(UserID(), uid) : false;
        }
        public bool IsFollowing(int? uid1, int uid2)
        {
            if (IsLogin())
            {
                bool[] r = q.UserRelations(uid1, uid2);
                return !(r[0] || r[1]) && r[2];
            }
            return false;
        }
        public bool IsFollowing(UserRelationShip? rA, int uid2)
        {
            if (IsLogin())
            {
                bool[] r = UserRelations(rA, q.UserRelation(uid2, UserID()));
                return !(r[0] || r[1]) && r[2];
            }
            return false;
        }
        public bool IsFollowing(UserRelationShip? rA, UserRelationShip? rB)
        {
            if (IsLogin())
            {
                bool[] r = UserRelations(rA, rB);
                return !(r[0] || r[1]) && r[2];
            }
            return false;
        }


        public bool IsFriendAllow(int uid)
        {
            return UidValid(uid) && IsLogin() && !IsSelf(uid) && !IsFriend(uid) && !IsWaiting(uid);
        }
        public bool IsFriendAllow(UserRelationShip? rA, int uid)
        {
            return UidValid(uid) && IsLogin() && !IsSelf(uid) && !IsFriend(rA, uid) && !IsWaiting(rA, uid);
        }

        public bool IsFollowAllow(int uid)
        {
            return UidValid(uid) && IsLogin() && !IsSelf(uid) && !IsFollowing(uid);
        }
        public bool IsFollowAllow(UserRelationShip? rA, int uid)
        {
            return UidValid(uid) && IsLogin() && !IsSelf(uid) && !IsFollowing(rA, uid);
        }
    }
    public partial class ForumController : Controller
    {
        public List<User>? MembersOf(int fid)
        {
            var memberRelations = db.Forums.Find(fid) == null ?
                null : db.ForumMembers.Where(m => m.ForumId == fid).ToList();
            if (memberRelations == null) { return null; }
            List<User> members = new List<User>();
            foreach (ForumMember member in memberRelations)
            {
                var m = q.User(member.MemberId);
                if (m != null)
                {
                    members.Add(m);
                }
            }
            return members;
        }
        public List<Post>? PostBy(int uid)
        {
            return db.Posts.Where(p => p.AuthorId == uid).ToList();
        }
    }
    public partial class AccountController : Controller
    {
    }
    public partial class WorkController : Controller
    {
    }
}
