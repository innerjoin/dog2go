using System.Collections.Concurrent;
using System.Linq;
using System.Web.Mvc;
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
            return users.First(x => x.Value.Identifier == User.Identity.Name).Value == null;
        }
    }
}