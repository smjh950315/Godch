using Microsoft.AspNetCore.Mvc;
using Godch.ViewModels;
using Godch.Models;
using Microsoft.AspNetCore.Authorization;
using MyLib;
using System;

namespace Godch.Controllers
{
    public partial class UserController : Controller
    {
        public IActionResult Index(int? uid)
        {
            if (uid==null)
            {
                if (IsLogin())
                {
                    return RedirectToAction("Index", new { uid = UserID() });
                }
                return HomePage();
            }
            int userId = uid.Value;
            var user = q.User(uid);
            if (user == null)
            {
                return HomePage();
            }
            var duser = new dUser(user);
            duser.Followers = vl.CastToList(q.FollowersOfId(userId));
            duser.Friends = vl.CastToList(q.FriendsOfId(userId));
            ViewBag.State = "";
            ViewBag.State2 = "";
            if (IsLogin())
            {
                //var r1 = q.UserRelation(UserID(), Uid);
                //var r2 = q.UserRelation(Uid, UserID());
                ViewBag.State = IsSelf(userId) ?
                    "IsSelf" : Unrequest(userId) ?
                    "Unrequest" : IsFriendAllow(userId) ?
                    "FriendAllow" : IsFriend(userId) ?
                    "IsFriend" : IsWaiting(userId) ?
                    "Waiting" : IsBlocked(userId) ?
                    "Blocked" : "NotAllow";
                ViewBag.State2 = IsFollowing(userId)?
                    "IsFollowing":IsFollowAllow(userId)?
                    "FollowAllow":"NotAllow";
            }
            ItemList = new List<dynamic>();
            ItemList.Add(duser);
            return View(duser);
        }

        public ActionResult Follow(int uid)
        {            
            if (IsFollowAllow(uid))
            {
                var r1 = q.UserRelation(UserID(), uid);
                if (r1 == null)
                {
                    r1 = new UserRelationShip();
                    r1.UserId = UserID().Value;
                    r1.UserId2 = uid;
                    r1.Following = true;
                    act.Create(r1);
                }
                r1.Following = true;
                act.Update(r1);
                AddToItemList(q.User(uid));
            }
            return RedirectToAction("Index", new { uid = uid });
        }

        public ActionResult UnFollow(int uid)
        {
            if (!IsLogin())
            {
                return RedirectToAction("Index", new { uid = uid });
            }
            var r1 = q.UserRelation(UserID(), uid);
            if (IsFollowing(r1,uid))
            {
                r1.Following = false;
                act.Update(r1);
            }
            return RedirectToAction("Index", new { uid = uid });
        }

        public ActionResult AddFriend(int uid)
        {
            if (!IsLogin())
            {
                return RedirectToAction("Index", new { uid = uid });
            }
            var r1 = q.UserRelation(UserID(), uid);
            if (IsFriendAllow(r1,uid))
            {
                if (r1 == null)
                {
                    r1 = new UserRelationShip();
                    r1.UserId = UserID().Value;
                    r1.UserId2 = uid;
                    r1.Following = true;
                    r1.FriendShip = true;
                    act.Create(r1);
                }
                else
                {
                    r1.FriendShip = true;
                    act.Update(r1);
                }
                AddToItemList(q.User(uid));
                
            }
            return RedirectToAction("Index", new { uid = uid });
        }

        public ActionResult DeleteFriend(int uid)
        {
            if (!IsLogin())
            {
                return RedirectToAction("Index", new { uid = uid });
            }
            var r1 = q.UserRelation(UserID(), uid);
            if (IsFriend(r1,uid) || IsWaiting(r1,uid))
            {
                r1.FriendShip = false;
                act.Update(r1);
            }
            return RedirectToAction("Index", new { uid = uid });
        }

    }
}
