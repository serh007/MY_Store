using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MY_Store.Models.Data;
using MY_Store.Models.ViewModels.Account;

namespace MY_Store.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {

            return RedirectToAction("Login");
        }

        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password do not match!");
                return View("CreateAccount", model);
            }

            using (Db db = new Db())
            {
                if (db.Users.Any(x=>x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", $"Username {model.Username} is taken.");
                    model.Username = "";
                    return View("CreateAccount", model);
                }

                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    Username = model.Username,
                    Password = model.Password
                };

                db.Users.Add(userDTO);

                db.SaveChanges();

                int id = userDTO.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2  //user id 2
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            TempData["SM"] = "You are now registered and can login.";

            return RedirectToAction("Login");
        }
        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            string userName = User.Identity.Name;

            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("user-profile");
            }

            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x=>x.Username.Equals(model.Username)&& x.Password.Equals(model.Password)))
                {
                    isValid = true;
                }

                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }
        }

        // GET: /account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        //partial view user

        public ActionResult UserNavPartial()
        {
            string userName = User.Identity.Name;

            UserNavPartialVM model;
            using (Db db = new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }
            return PartialView(model);
        }

        //GET: /account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            string userName = User.Identity.Name;

            UserProfileVM model;

            using (Db db = new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                model = new UserProfileVM(dto);
            }
            return View("UserProfile", model);
        }

        //POST: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileVM model)
        {
            bool userNameIsChanged = false;
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View("UserProfile", model);
                }
            }

            using (Db db = new Db())
            {
                string userName = User.Identity.Name;
                if (userName != model.Username)
                {
                    userName = model.Username;
                    userNameIsChanged = true;
                }

                if (db.Users.Where(x=> x.Id != model.Id).Any(x=> x.Username == userName))
                {
                    ModelState.AddModelError("", $"Username {model.Username} already exist.");
                    model.Username = "";
                    return View("UserProfile", model);
                }

                UserDTO dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAdress = model.EmailAdress;
                dto.Username = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                db.SaveChanges();
            }

            TempData["SM"] = "You have edited your profile!";

            if (!userNameIsChanged)
            {
                return View("UserProfile", model);
            }
            else
            {
                return RedirectToAction("Logout");
            }
            
        }
    }
}