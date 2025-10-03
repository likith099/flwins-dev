
using System.Web.Mvc;

namespace aspnet_get_started.Controllers
{
    public class FlwinsSimpleController : Controller
    {
        // GET: FlwinsSimple
        public ActionResult Index()
        {
            ViewBag.Message = "Skill up for a better future.";
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            ViewBag.UserName = User.Identity.IsAuthenticated ? User.Identity.Name : "Guest";
            
            return View("~/Views/Flwins/Index.cshtml");
        }
    }
}