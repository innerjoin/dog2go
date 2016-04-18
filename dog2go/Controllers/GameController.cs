using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dog2go.Backend.Hubs;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using Microsoft.AspNet.SignalR;

namespace dog2go.Controllers
{
    public class GameController : Controller
    {

        public ActionResult Play()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                //GameHub gameHub = new GameHub(UserRepository.Instance);
                //gameHub.Login(user.Nickname, null);
                return Redirect("Game/Play");
            }
            return View(user);
        }
    }
}