using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;

namespace dog2go.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        [Authorize]
        public ActionResult Play()
        {
            if (NotFullyAuthorized())
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        private bool NotFullyAuthorized()
        {
            ConcurrentDictionary<string, User> users = UserRepository.Instance.Get();
            if (users == null || users.Count == 0) return true;
            return users.FirstOrDefault(x => x.Value.Identifier == User.Identity.Name).Value == null;
        }

        public ActionResult ChooseGameTable()
        {
            List<GameTable> gameTableList = GameRepository.Instance.Get();
            return View(gameTableList);
        }

        [HttpGet]
        [ActionName("GetTables")]
        public ActionResult GetTables()
        {
            List<GameTable> gamesList = GameRepository.Instance.Get();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}