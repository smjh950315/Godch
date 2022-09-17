using Microsoft.AspNetCore.Mvc;
using Godch.ViewModels;
using Godch.Models;
using MyLib;
using MyLib.Mvc;
using Storage = Godch.GlobalStorage.ControllerStorage;

namespace Godch.Controllers
{
    public partial class ForumController : Controller
    {
        protected static Number ForumId = new();
        public new string EntityName = "Forum";
        public IActionResult Index(int? fid, int? page)
        {
            var forum = q.Forum(fid);
            if (forum == null || fid == null)
            {
                return RedirectToAction("Forums", "Home");
            }
            var headPosts = q.HeadPosts(forum.ForumId);

            dynamic PageData = PageReloadAuto(EntityName, headPosts, page, 10);
            int varStorageIndex = GetSelfVarIndex();
            Storage.UserVars[varStorageIndex].ForumId = fid;
            Storage.UserVars[varStorageIndex].ForumName = forum.ForumName;
            ViewBag.ForumName = forum.ForumName;
            ViewBag.ForumId = forum.ForumId;
            ViewBag.Page = PageData.Page;
            return View(PageData.ViewList);
        }
        public IActionResult NewPost()
        {
            if (!ForumId.IsNull)
            {
                ViewBag.Title = "New Post @" + q.ForumName(ForumId);
                return View(new sPost());
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult NewPost(sPost p)
        {
            if(ForumId.IsNull) { return RedirectToAction("Index", "Home"); }
            var post = mCast.Cast(p);
            post.ForumId = ForumId;
            post.AuthorId = UserID();
            post.HeadPostId = null;
            post.LastReply = Time.TimeNowInt();
            post.Create = Time.TimeNowInt();
            post.Floor = 0;
            post.Data = null;
            act.Create(post);
            PostFileIO.WritePostContent(post.PostId, p.Data);
            return RedirectToAction("Index", new { fid = ForumId.Value });
        }

    }
}
