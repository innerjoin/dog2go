using System.Web.Mvc;

namespace dog2go.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        [Authorize]
        public ActionResult Play()
        {
            return View();
        }
    }
}