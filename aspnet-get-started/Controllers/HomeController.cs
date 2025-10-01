using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace aspnet_get_started.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Set up ViewBag for FLWINS page
            ViewBag.Message = "Skill up for a better future.";
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            ViewBag.DisplayName = User.Identity.IsAuthenticated ? (User.Identity.Name ?? "User") : "Guest";
            ViewBag.UserEmail = User.Identity.IsAuthenticated ? User.Identity.Name : "";
            
            // Return the FLWINS view directly from Home controller
            return View("~/Views/Flwins/Index.cshtml");
        }

        [Authorize]
        public ActionResult CreateEfsmAccount()
        {
            // Simple EFSM account creation form
            var model = new EfsmAccountRequest
            {
                FirstName = User.Identity.IsAuthenticated ? GetUserDisplayName().Split(' ').FirstOrDefault() : "",
                LastName = User.Identity.IsAuthenticated ? GetUserDisplayName().Split(' ').LastOrDefault() : "",
                Email = User.Identity.IsAuthenticated ? User.Identity.Name : ""
            };
            
            return View("~/Views/Flwins/CreateEfsmAccount.cshtml", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEfsmAccount(EfsmAccountRequest model)
        {
            if (ModelState.IsValid)
            {
                // Simulate successful account creation
                TempData["EfsmAccountId"] = "EFSM-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                TempData["EfsmRedirectUrl"] = "https://efsm-portal.example.com/sr/start?token=demo";
                TempData["UserDisplayName"] = GetUserDisplayName();
                
                return RedirectToAction("EfsmAccountCreated");
            }

            return View("~/Views/Flwins/CreateEfsmAccount.cshtml", model);
        }

        [Authorize]
        public ActionResult EfsmAccountCreated()
        {
            var accountId = TempData["EfsmAccountId"] as string;
            var redirectUrl = TempData["EfsmRedirectUrl"] as string;
            var userDisplayName = TempData["UserDisplayName"] as string;

            if (string.IsNullOrEmpty(accountId))
            {
                return RedirectToAction("Index");
            }

            ViewBag.EfsmAccountId = accountId;
            ViewBag.EfsmRedirectUrl = redirectUrl;
            ViewBag.UserDisplayName = userDisplayName ?? GetUserDisplayName();
            
            return View("~/Views/Flwins/EfsmAccountCreated.cshtml");
        }

        private string GetUserDisplayName()
        {
            return User.Identity.IsAuthenticated ? (User.Identity.Name ?? "User") : "User";
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }

    // Model for EFSM account creation request
    public class EfsmAccountRequest
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Organization")]
        public string Organization { get; set; }

        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Required]
        [Display(Name = "Request Type")]
        public string RequestType { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }
    }
}