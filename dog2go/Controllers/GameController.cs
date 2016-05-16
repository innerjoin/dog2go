using System.Collections.Concurrent;
using System.Linq;
using System.Web.Mvc;
using dog2go.Backend.Constants;
using dog2go.Backend.Model;
using dog2go.Backend.Repos;
using dog2go.Models;

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
            User user =
                UserRepository.Instance.Get().FirstOrDefault(x => x.Value.Identifier == User.Identity.Name).Value;
            DisplayNameModel model = new DisplayNameModel
            {
                DisplayUser = user, 
                ColorMeeplePath = GetUserMeeplePath()
            };
            return View(model);
        }

        private bool NotFullyAuthorized()
        {
            ConcurrentDictionary<string, User> users = UserRepository.Instance.Get();
            if (users == null || users.Count == 0) return true;
            return users.FirstOrDefault(x => x.Value.Identifier == User.Identity.Name).Value == null;
        }

        private static string GetUserMeeplePath()
        {
            ConcurrentDictionary<string, User> users = UserRepository.Instance.Get();
            switch (users.Count % GlobalDefinitions.NofParticipantsPerTable)
            {
                case 0:
                    return "meeple_yellow.png";
                case 1:
                    return "meeple_blue.png";
                case 2:
                    return "meeple_red.png";
                case 3:
                    return "meeple_green.png";
                default:
                    return "not defined";
            }
        }
    }
}