using Microsoft.AspNetCore.Mvc;
using Godch.Models;
using Godch.ViewModels;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using MyLib;
using Godch.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Godch.Controllers
{
    //uid_wid_count_f_[fileName]
    //uid_wid_multi.txt
    //<part>fileName</part>
    public partial class WorkController : Controller
    {
        private readonly string _folder;
        private Number? WorkId { get; set; }
        private TagSelection _tagSelection = new();
        private readonly static Dictionary<string, string> _contentTypes = new Dictionary<string, string>
        {
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"}
        };
        public WorkController(IHostingEnvironment env, IHubContext<FunctionHub> hub)
        {
            // 把上傳目錄設為：wwwroot\UploadFolder
            _folder = $@"{env.WebRootPath}\_GodchData\Works";
            _hub = hub;
        }
        public IActionResult Index(long? wid)
        {
            if (wid == null && WorkId == null) { return RedirectToAction("Index"); }
            var w = q.Work(wid);
            if (w != null)
            {
                _tagSelection = q.TagsOf(w);
                WorkId = wid;
                dWork? details = mCast.dCast(w);
                return View(details);
            }
            return Redirect("~/Home/Works");
        }
        public IActionResult Create()
        {
            return View();
        }

        [Route("Upload")]
        [HttpPost]
        [DisableFormValueModelBindingFilter]
        public async Task<IActionResult> Upload()
        {
            var author = CurrentUser();
            if (author == null) { return RedirectToAction("Index"); }
            Work w = new();
            w.AuthorId = author.UserId;
            w.WorkName = "Uploading";
            w.UploadTime = Time.TimeNowInt();
            act.Create(w);

            string fileResult = "";
            string wFileNameStart = $"{author.UserId}_{w.WorkId}";
            string workFileName = "";
            GodchDirectories.Works.AddFolderIfNotExist(w.WorkId);
            var fileCount = 0;
            var formValueProvider = await Request.StreamFile(
                (file) =>
                { 
                    fileCount++;
                    workFileName = $"{wFileNameStart}_{file.FileName}";
                    fileResult=workFileName;
                    return System.IO.File.Create($"{Config.Works}\\{w.WorkId}\\{workFileName}");
                }
                );
            if (fileResult.Length<1)
            {
                act.Delete(w);
                return Redirect("~/Home/Works");
            }
            w.WorkName = formValueProvider.GetValue("WorkName").ToString();
            w.Description = formValueProvider.GetValue("Description").ToString();
            w.FileUrl = $"{w.WorkId}/{workFileName}";
            act.Update(w);
            SendNotifyToFollowers("/Work/Upload", "wid", "Work", w.WorkId, "Upload");
            return Redirect("~/Home/Works");
        }
        public IActionResult Edit()
        {
            if (WorkId == null) { return RedirectToAction("Index", "Home"); }
            var w = q.Work(WorkId);
            if (w != null)
            {
                return View(w);
            }
            return View();
        }



    }
}
