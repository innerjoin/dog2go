using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using dog2go.Models;

namespace dog2go.Controllers
{
    public class AccountController : Controller
    {

        public ViewResult Login()
        {

            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public ActionResult PostLogin(LoginViewModel loginModel)
        {

            if (ModelState.IsValid)
            {
                FormsAuthentication.SetAuthCookie(loginModel.UserName, true);
                return RedirectToAction("Play", "Game");
            }

            return View(loginModel);
        }

        [HttpGet]
        [ActionName("SignOut")]
        public ActionResult PostSignOut()
        {

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}