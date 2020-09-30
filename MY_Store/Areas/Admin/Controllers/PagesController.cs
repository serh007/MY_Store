using MY_Store.Models.Data;
using MY_Store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Web.WebPages.Html;

namespace MY_Store.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        [HttpGet]
        public ActionResult Index()
        {
            List<PageVM> pageList;
            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {           
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                string slug;
                PagesDTO dto = new PagesDTO();

                dto.Title = model.Title.ToUpper();

                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That titlt already exsist");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That slug already exsist");
                    return View(model);
                }
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //save to db
                db.Pages.Add(dto);
                db.SaveChanges();
            }
                //massage srow temp data
                TempData["SM"] = "You have added a new page!";

                //return to INDEX
                return RedirectToAction("Index");
            

        }

        //GET: Admin/Pages/EditPage
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            PageVM model;
            using (Db db = new Db())
            {
                PagesDTO dto = db.Pages.Find(id);
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }
                model = new PageVM(dto);
            }
            return View(model);
        }

        //POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                int id = model.Id;

                string slug = "home";

                PagesDTO dto = db.Pages.Find(id);

                dto.Title = model.Title;

                if (model.Slug != "home")
                {
                    if(string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                if (db.Pages.Where(x => x.Id !=id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "Thet title already exist.");
                    return View();
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Thet slug already exist.");
                    return View();
                }
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                db.SaveChanges();

            }

            TempData["SM"] = "You have edited the page.";

            return RedirectToAction("EditPage");
        }

        //GET: Admin/Pages/PageDetails/id
        [HttpGet]
        public ActionResult PageDetails(int id)
        {
            PageVM model;

            using (Db db = new Db())
            {
                PagesDTO dto = db.Pages.Find(id);

                if(dto == null)
                {
                    return Content("The page dos  not exist.");
                }

                model = new PageVM(dto);
            }
            return View(model);
        }

        //GET: Admin/Pages/DeletePage/id
        [HttpGet]
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                PagesDTO dto = db.Pages.Find(id);

                db.Pages.Remove(dto);

                db.SaveChanges();
            }

            TempData["SM"] = "You have delete a page!";
            
            return RedirectToAction("Index");
        }


        //POST: Admin/Pages/ReorderPages  //add sorting
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;

                PagesDTO dto;
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }

                
            }
        }

        //GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            SidebarVM model;

            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1); //Поміняти на НЕ привязане значення!

                model = new SidebarVM(dto);
            }
            return View(model);
        }

        //POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1); //Поміняти на НЕ привязане значення!

                dto.Body = model.Body;

                db.SaveChanges();
            }
            TempData["SM"] = "You have edited the sidebar!";
            return RedirectToAction("EditSidebar");
        }

    }
}