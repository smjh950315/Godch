using Godch.Models;
using Godch.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using act = Godch.dbAction;
using vList = Godch.ViewModels.ViewList;

namespace Godch.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            int maxEach = 8;
            var workList = q.WorksOfUserTag(UserID());
            var workListByFollowing = q.WorksFromFollowingUser(UserID());
            int cWL = workList.Count();
            int cFWL=workListByFollowing.Count();

            int countNeededWL = maxEach - cWL < 0 ? 0 : maxEach - cWL;
            int countNeededFWL = maxEach - cFWL < 0? 0: maxEach - cFWL;

            if (countNeededWL > 0|| countNeededFWL > 0)
            {
                var works = db.Works.OrderByDescending(w => w.UploadTime).Take(countNeededWL+ countNeededFWL);
                int i = 0,j = 0;
                foreach (var w in works)
                {
                    if (countNeededWL > i)
                    {
                        workList.Add(w);
                        i++;
                    }
                    else
                    {
                        workListByFollowing.Add(w);
                        j++;
                    }
                }
            }
            vList wvL = new vList(workList);
            vList fwvL = new vList(workListByFollowing);
            ViewBag.wList = wvL.Values;
            ViewBag.fwList = fwvL.Values;
            return View();
        }
        public IActionResult Forums()
        {
            var fList = new vList(q.Forums());
            return View(fList);
        }
        public IActionResult NewForum()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewForum(Forum forum)
        {
            act.Create(forum);
            return RedirectToAction("Index", "Forum", new { fid = forum.ForumId });
        }

        public IActionResult Works()
        {
            var works = q.Works();
            if (works == null)
            {
                var Works = new List<Work>();
                ViewBag.Works = new vList(Works);
            }
            else
            {
                ViewBag.Works = new vList(works);
            }
            return View(ViewBag.Works);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}