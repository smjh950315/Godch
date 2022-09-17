using Godch.GlobalStorage;
using Godch.Models;
using Godch.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MyLib;
using MyLib.Mvc;
using System.Security.Cryptography;
using Storage = Godch.GlobalStorage.ControllerStorage;

namespace Godch.Controllers
{
    public class PostController : Controller
    {
        public new string EntityName = "Post";
        private string? ForumName { get; set; }
        //[Authorize]
        public IActionResult Index(long? pid, int? page)
        {
            var headPost = q.HeadPost(pid);
            if(headPost == null || pid == null)
            {
                return RedirectToAction("Forums", "Home");
            }
            var postList = q.PostFamily(pid);

            dynamic pageData = PageReloadAuto(EntityName, postList, page, 10, true);
            int varStorageIndex = GetSelfVarIndex();

            Storage.UserVars[varStorageIndex].PostId = pid;

            ViewBag.CurrentUserID = UserID();
            ViewBag.ForumId = Storage.UserVars[varStorageIndex].ForumId;
            ViewBag.PostId = Storage.UserVars[varStorageIndex].PostId;
            ViewBag.Title = headPost.Title + " @" + Storage.UserVars[varStorageIndex].ForumName;
            ViewBag.Page = pageData.Page; 
            return View(pageData.ViewList);
        }
        public IActionResult Reply(long HPId)
        {
            var v = db.Posts.Where(p => p.HeadPostId == HPId);
            return View(v);
        }
        public IActionResult Return()
        {
            int varStorageIndex = GetSelfVarIndex();
            int? fid = Storage.UserVars[varStorageIndex].ForumId;
            if (fid == null || fid == 0)
            {
                int pid = Storage.UserVars[varStorageIndex].PostId;
                var post = q.Post(pid);
                var forum = q.Forum(post?.ForumId);
                if (forum != null)
                {
                    return RedirectToAction("Index", "Forum", new { fid = forum.ForumId});
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Forum", new { fid = fid.Value }); 
        }
    }
}
