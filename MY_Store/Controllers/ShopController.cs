using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MY_Store.Models.Data;
using MY_Store.Models.ViewModels.Shop;

namespace MY_Store.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            List<CategoryVM> categoryVmList;

            using (Db db = new Db())
            {
                categoryVmList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x))
                    .ToList();
            }

            return PartialView("_CategoryMenuPartial", categoryVmList);
        }
    }
}