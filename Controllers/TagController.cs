using Godch.Models;
using Godch.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Godch.Controllers
{
    public class TagController : Controller
    {
        private int? TagId { get; set; }
        public IActionResult Index(int? page)
        {
            var vlist = db.Tags.ToList();
            pageHelper.Load(vlist);            
            return View(pageHelper.ToPage(page));
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Tag tag)
        {
            var checkTag = q.Tag(tag.TagId);
            if (checkTag != null) { return View(checkTag); }
            db.Tags.Add(tag);
            return View();
        }
        public IActionResult Detail(int? tid)
        {
            if (tid == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Tag? tag = db.Tags.Find(tid);
            TagId = tid.Value;
            return View(tag);
        }
        public IActionResult Edit()
        {
            var tag = db.Tags.Find(TagId);
            if(tag == null) 
            { 
                return RedirectToAction("Index", "Home"); 
            }
            return View(tag);
        }
        [HttpPost]
        public IActionResult Edit(Tag tag)
        {
            if(TagId == null)
            {
                return RedirectToAction("Index");
            }
            tag.TagId = TagId.Value;
            act.Update(tag);
            return RedirectToAction("Detail", new {tid = tag.TagId});
        }

    }
}
