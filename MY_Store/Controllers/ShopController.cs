using System;
using System.Collections.Generic;
using System.IO;
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
        //GET: Shop/Category/name
        public ActionResult Category(string name)
        {
            List<ProductVM> productVmList;

            using (Db db = new Db())
            {
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();

                int catId = categoryDTO.Id;

                productVmList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x))
                    .ToList();
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();

                if (productCat == null)
                {
                    var catName = db.Categories.Where(x => x.Slug == name).Select(x => x.Name).FirstOrDefault();
                    ViewBag.CategoryName = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCat.CategoryName;
                }

            }
            return View(productVmList);
        }

        //GET: Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            ProductDTO dto;
            ProductVM model;

            int id = 0;

            using (Db db = new Db())
            {
                if (! db.Products.Any(x => x.Slug.Equals(name)))
                {
                    RedirectToAction("Index", "Shop");
                }

                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                id = dto.Id;

                model = new ProductVM(dto);

            }

            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));


            return View("ProductDetails", model);
        }
    }
}