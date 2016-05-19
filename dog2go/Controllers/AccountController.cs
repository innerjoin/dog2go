using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using dog2go.Backend.Constants;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using dog2go.Controllers.Helpers;
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
            string userName = loginModel.UserName;
            if (!ModelState.IsValid || UserNameTaken(userName))
            {
                return View(loginModel);
            }
                

            FormsAuthentication.SetAuthCookie(userName, true);
            User user = new User()
            {
                Identifier = userName,
                Nickname = userName,
                ConnectionIds = new HashSet<string>()
            };
            UserRepository.Instance.Get().GetOrAdd(userName, user);
            return RedirectToAction($"ChooseGameTable", $"Game");
        }

        private bool UserNameTaken(string userName)
        {
            ModelState.AddModelError(string.Empty, "Username already taken!");
            return UserRepository.Instance.Get().ContainsKey(userName);
        }

        [HttpGet]
        [FullyAuthorized]
        [ActionName("SignOut")]
        public ActionResult PostSignOut()
        {
            FormsAuthentication.SignOut();
            User user;
            UserRepository.Instance.Get().TryRemove(User.Identity.Name, out user);
            return RedirectToAction($"Login", $"Account");
        }
    }
}