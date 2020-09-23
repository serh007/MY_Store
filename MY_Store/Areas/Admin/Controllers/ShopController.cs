using MY_Store.Models.Data;
using MY_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MY_Store.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        [HttpGet]
        public ActionResult Categories()
        {
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
            return View(categoryVMList);
        }

        // POST Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;

            using (Db db= new Db())
            {
                if (db.Categories.Any(x => x.Name == catName))
                {
                    return "titletaken";
                }
                CategoryDTO dto = new CategoryDTO();

                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                db.Categories.Add(dto);
                db.SaveChanges();
                id = dto.Id.ToString();
            }
            return id;
        }
        //POST: Admin/Shop/ReorderCategories  //add sorting
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;

                CategoryDTO dto;
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }


            }
        }

        //GET: Admin/Shop/DeleteCategory/id
        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                CategoryDTO dto = db.Categories.Find(id);

                db.Categories.Remove(dto);

                db.SaveChanges();
            }

            TempData["SM"] = "You have delete a catecory!";

            return RedirectToAction("Categories");
        }


        //POST: Admin/Shop/RenameCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";
                CategoryDTO dto = db.Categories.Find(id);
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();
                db.SaveChanges();
            }
            return "ok";
        }
    }
}