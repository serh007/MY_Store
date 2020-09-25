using MY_Store.Models.Data;
using MY_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;

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

        //GET: Admin/Shop/AddProduct //add product method
        [HttpGet]
        public ActionResult AddProduct()
        {
            ProductVM model = new ProductVM();

            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }

            return View(model);
        }

        //POST: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);

                }
            }

            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "That product name is taken!");
                    return View(model);
                }
            }

            int id;

            using (Db db =new Db())
            {
                ProductDTO product = new ProductDTO();
                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                id = product.Id;
            }

            TempData["SM"] = "You have added a product";


            #region Upload Image
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            var pathStringOne = Path.Combine(originalDirectory.ToString(), "Products");
            var pathStringTwo = Path.Combine(originalDirectory.ToString(), "Products\\"+ id.ToString());
            var pathStringThre = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathStringFour = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathStringFifth = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathStringOne))
            {
                Directory.CreateDirectory(pathStringOne);
            }

            if (!Directory.Exists(pathStringTwo))
            {
                Directory.CreateDirectory(pathStringTwo);
            }
            if (!Directory.Exists(pathStringThre))
            {
                Directory.CreateDirectory(pathStringThre);
            }
            if (!Directory.Exists(pathStringFour))
            {
                Directory.CreateDirectory(pathStringFour);
            }
            if (!Directory.Exists(pathStringFifth))
            {
                Directory.CreateDirectory(pathStringFifth);
            }

            if (file != null && file.ContentLength > 0)
            {
                    string ext = file.ContentType.ToLower();
                    if (ext != "image/jmg" &&
                        ext != "image/jpeg" &&
                        ext != "image/pjpeg" &&
                        ext != "image/gif" &&
                        ext != "image/x-png" &&
                        ext != "image/png" )
                    {
                        using (Db db = new Db())
                        {
                            model.Categories = new SelectList(db.Categories.ToList(),"Id","Name");
                            ModelState.AddModelError("","The image was not uploaded - wrong image extension");
                            return View(model);
                        }
                    }
            

                    string imageName = file.FileName;

                    using (Db db = new Db())
                    {
                        ProductDTO dto = db.Products.Find(id);
                        dto.ImageName = imageName;

                        db.SaveChanges();
                    }

                    var path = string.Format($"{pathStringTwo}\\{imageName}");
                    var path2 = string.Format($"{pathStringThre}\\{imageName}");

                    file.SaveAs(path);

                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200).Crop(1,1);
                    img.Save(path2);
            }
            #endregion

            return RedirectToAction("AddProduct");
        }
        //GET: Admin/Shop/Products
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            List<ProductVM> ListOfProductVM;

            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                ListOfProductVM = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new ProductVM(x))
                    .ToList();

                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                ViewBag.SelectedCat = catId.ToString();

            }

            var onePageOfProducts = ListOfProductVM.ToPagedList(pageNumber, 3);
            ViewBag.onePageOfProducts = onePageOfProducts;
            return View(ListOfProductVM);
        }

        //GET: Admin/Shop/EditProduct/Id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            ProductVM model;

            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                if (dto == null)
                {
                    return Content("Thet product not exist.");
                }

                model = new ProductVM(dto);
                model.Categories = new SelectList(db.Categories.ToList(),"Id","Name");
                model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));

            }
            return View(model);
        }

        //POST: Admin/Shop/EditProduct
        
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            int id = model.Id;

            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That product name is taken!");
                    return View(model);
                }
            }

            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;


                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }

            TempData["SM"] = "You have edited the product!";

            // uploud image

            #region Upload image

            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();

                if (ext != "image/jmg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension");
                        return View(model);
                    }
                }

                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (var file2 in di1.GetFiles())
                {
                    file2.Delete();
                }

                foreach (var file3 in di2.GetFiles())
                {
                    file3.Delete();
                }

                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }
                var path = string.Format($"{pathString1}\\{imageName}");
                var path2 = string.Format($"{pathString2}\\{imageName}");

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1,1);
                img.Save(path2);
            }

            #endregion

            return RedirectToAction("EditProduct");
        }

        //POST: Admin/Shop/DeleteProduct/id
        
        public ActionResult DeleteProduct(int id)
        {
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);
                db.SaveChanges();
            }

            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            return RedirectToAction("Products");
        }


        //POST: Admin/Shop/SaveGalleryImages/id
        
        public void SaveGalleryImages(int id)
        {
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];

                if (file != null && file.ContentLength > 0)
                {
                    var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\"+ id.ToString() +"\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    var path = string.Format($"{pathString1}\\{file.FileName}");
                    var path2 = string.Format($"{pathString2}\\{file.FileName}");

                    file.SaveAs(path);

                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200).Crop(1,1);
                    img.Save(path2);

                }
            }
        }


        //Admin/Shop/DeleteImage/id/imageName
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);

            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);

        }
    }
}