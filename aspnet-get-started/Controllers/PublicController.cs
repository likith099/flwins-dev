using System;
using System.Web;
using System.Web.Mvc;

namespace aspnet_get_started
{
    public class PublicController : Controller
    {
        // Completely public controller for anonymous access
        
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Message = "Skill up for a better future.";
            ViewBag.IsAuthenticated = false; // Always false for public access
            ViewBag.DisplayName = "Guest";
            ViewBag.UserEmail = "";
            
            return View("~/Views/Flwins/Index.cshtml");
        }
        
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Redirect to Azure AD login
            return Redirect("/.auth/login/aad");
        }
    }
}