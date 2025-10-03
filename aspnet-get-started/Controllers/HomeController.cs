using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace aspnet_get_started.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            // Set up ViewBag for FLWINS page
            ViewBag.Message = "Skill up for a better future.";
            ViewBag.IsAuthenticated = User?.Identity?.IsAuthenticated ?? false;
            ViewBag.DisplayName = (User?.Identity?.IsAuthenticated == true) ? (User.Identity.Name ?? "User") : "Guest";
            ViewBag.UserEmail = (User?.Identity?.IsAuthenticated == true) ? User.Identity.Name : "";
            
            // Return the regular Home Index view (now with FLWINS content)
            return View();
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
            
            ViewBag.UserEmail = User.Identity.IsAuthenticated ? User.Identity.Name : "";
            ViewBag.UserName = GetUserDisplayName();
            
            return View("~/Views/Flwins/CreateEfsmAccount.cshtml", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEfsmAccount(EfsmAccountRequest model)
        {
            if (ModelState.IsValid)
            {
                // Call EFSM Provision API to get an SSO URL
                try
                {
                    var efsmProvisionUrl = System.Configuration.ConfigurationManager.AppSettings["EfsmProvisionUrl"];
                    var redirectPath = System.Configuration.ConfigurationManager.AppSettings["EfsmDefaultRedirectPath"] ?? "/SR/Start";

                    if (string.IsNullOrWhiteSpace(efsmProvisionUrl))
                    {
                        ModelState.AddModelError("", "EFSM configuration is missing. Please set EfsmProvisionUrl in Web.config.");
                        ViewBag.UserEmail = User.Identity.IsAuthenticated ? User.Identity.Name : "";
                        ViewBag.UserName = GetUserDisplayName();
                        return View(model);
                    }

                    var payload = new
                    {
                        email = model.Email,
                        displayName = string.Format("{0} {1}", model.FirstName, model.LastName).Trim(),
                        redirectPath = redirectPath
                    };

                    using (var http = new HttpClient())
                    {
                        var json = JsonConvert.SerializeObject(payload);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = http.PostAsync(efsmProvisionUrl, content).Result;
                        var body = response.Content.ReadAsStringAsync().Result;

                        if (!response.IsSuccessStatusCode)
                        {
                            ModelState.AddModelError("", "EFSM Provisioning failed: " + response.StatusCode + ". " + body);
                            ViewBag.UserEmail = User.Identity.IsAuthenticated ? User.Identity.Name : "";
                            ViewBag.UserName = GetUserDisplayName();
                            return View(model);
                        }

                        var efsmResult = JsonConvert.DeserializeObject<EfsmProvisionResponse>(body);
                        if (efsmResult == null || string.IsNullOrWhiteSpace(efsmResult.ssoUrl))
                        {
                            ModelState.AddModelError("", "EFSM Provisioning returned an invalid response.");
                            ViewBag.UserEmail = User.Identity.IsAuthenticated ? User.Identity.Name : "";
                            ViewBag.UserName = GetUserDisplayName();
                            return View(model);
                        }

                        // Success: store details for confirmation view
                        TempData["EfsmAccountId"] = "EFSM-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                        TempData["EfsmRedirectUrl"] = efsmResult.ssoUrl;
                        TempData["UserDisplayName"] = GetUserDisplayName();
                        TempData["EfsmProvisionMessage"] = efsmResult.message;

                        return RedirectToAction("EfsmAccountCreated");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unexpected error during EFSM provisioning: " + ex.Message);
                    ViewBag.UserEmail = User.Identity.IsAuthenticated ? User.Identity.Name : "";
                    ViewBag.UserName = GetUserDisplayName();
                    return View(model);
                }
            }

            ViewBag.UserEmail = User.Identity.IsAuthenticated ? User.Identity.Name : "";
            ViewBag.UserName = GetUserDisplayName();
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
            ViewBag.EfsmProvisionMessage = TempData["EfsmProvisionMessage"] as string;
            
            return View("~/Views/Flwins/EfsmAccountCreated.cshtml");
        }

        [AllowAnonymous]
        public ActionResult Services()
        {
            ViewBag.IsAuthenticated = User?.Identity?.IsAuthenticated ?? false;
            ViewBag.UserName = GetUserDisplayName();
            
            return View();
        }

        [Authorize]
        public ActionResult AccessEfsmPortal()
        {
            try
            {
                // TEMPORARY FIX: Direct SSO redirect to EFSM with Azure AD login
                // This bypasses the Provision API until it's implemented on EFSM side
                
                var efsmBaseUrl = "https://efsmod-dev-egcyb2bahcdkamdm.canadacentral-01.azurewebsites.net";
                var familyPortalPath = "/Home/FamilyPortal";
                var redirectUrl = System.Web.HttpUtility.UrlEncode($"{efsmBaseUrl}{familyPortalPath}");
                var ssoUrl = $"{efsmBaseUrl}/.auth/login/aad?post_login_redirect_url={redirectUrl}";
                
                // Log user access for debugging
                System.Diagnostics.Trace.TraceInformation($"FLWINS->EFSM SSO: User {User.Identity.Name} accessing EFSM via {ssoUrl}");
                
                return Redirect(ssoUrl);
            }
            catch (Exception ex)
            {
                // Log error and fallback
                System.Diagnostics.Trace.TraceError($"FLWINS->EFSM SSO Error: {ex.Message}");
                
                // Fallback: direct redirect to EFSM Family Portal
                return Redirect("https://efsmod-dev-egcyb2bahcdkamdm.canadacentral-01.azurewebsites.net/Home/FamilyPortal");
            }
        }

        [Authorize]
        public ActionResult MyAccount()
        {
            ViewBag.UserName = GetUserDisplayName();
            ViewBag.UserEmail = User.Identity.Name;
            ViewBag.LoginTime = DateTime.Now.ToString("MMM dd, yyyy HH:mm");
            
            return View();
        }

        private string GetUserDisplayName()
        {
            return User.Identity.IsAuthenticated ? (User.Identity.Name ?? "User") : "User";
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        [AllowAnonymous]
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

    // EFSM Provision API response contract
    public class EfsmProvisionResponse
    {
        public string status { get; set; }
        public string ssoUrl { get; set; }
        public string message { get; set; }
        public object graphResult { get; set; }
    }
}