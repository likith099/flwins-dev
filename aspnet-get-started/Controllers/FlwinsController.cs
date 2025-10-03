
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace aspnet_get_started.Controllers
{
    public class FlwinsController : Controller
    {
        // GET: Flwins - Main home page (public access)
        public ActionResult Index()
        {
            try
            {
                // Check if user is authenticated
                if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                {
                    var userEmail = User.Identity.Name ?? "User";
                    var displayName = GetUserDisplayName();
                    
                    ViewBag.Message = $"Welcome back, {displayName}";
                    ViewBag.UserEmail = userEmail;
                    ViewBag.DisplayName = displayName;
                    ViewBag.IsAuthenticated = true;
                }
                else
                {
                    ViewBag.Message = "Skill up for a better future.";
                    ViewBag.IsAuthenticated = false;
                }
                
                return View();
            }
            catch (Exception ex)
            {
                // Fallback for any errors
                ViewBag.Message = "Skill up for a better future.";
                ViewBag.IsAuthenticated = false;
                ViewBag.Error = ex.Message; // For debugging
                return View();
            }
        }

        // GET: Flwins/CreateEfsmAccount - Form to create EFSM account
        [Authorize]
        public ActionResult CreateEfsmAccount()
        {
            // Pre-populate form with user information from Azure AD
            var model = new EfsmAccountRequest
            {
                FirstName = GetUserClaim("given_name"),
                LastName = GetUserClaim("family_name"),
                Email = User.Identity.Name
            };
            
            return View(model);
        }

        // POST: Flwins/CreateEfsmAccount - Submit form data to EFSM
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> CreateEfsmAccount(EfsmAccountRequest model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Add authenticated user information to the request
                    model.FlwinsUserId = User.Identity.Name;
                    model.FlwinsUserName = GetUserDisplayName();
                    
                    // Send data to EFSM system
                    var efsmResponse = await SubmitToEfsm(model);
                    
                    if (efsmResponse.Success)
                    {
                        // Store the EFSM account details for redirect
                        TempData["EfsmAccountId"] = efsmResponse.AccountId;
                        TempData["EfsmRedirectUrl"] = efsmResponse.RedirectUrl;
                        TempData["UserDisplayName"] = GetUserDisplayName();
                        
                        return RedirectToAction("EfsmAccountCreated");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to create EFSM account: " + efsmResponse.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
            }

            return View(model);
        }

        // GET: Flwins/EfsmAccountCreated - Success page with redirect link
        [Authorize]
        public ActionResult EfsmAccountCreated()
        {
            var accountId = TempData["EfsmAccountId"] as string;
            var redirectUrl = TempData["EfsmRedirectUrl"] as string;
            var userDisplayName = TempData["UserDisplayName"] as string;

            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(redirectUrl))
            {
                return RedirectToAction("Index");
            }

            ViewBag.EfsmAccountId = accountId;
            ViewBag.EfsmRedirectUrl = redirectUrl;
            ViewBag.UserDisplayName = userDisplayName ?? GetUserDisplayName();
            
            return View();
        }

        // Helper method to submit data to EFSM system
        private async Task<EfsmResponse> SubmitToEfsm(EfsmAccountRequest model)
        {
            using (var client = new HttpClient())
            {
                // TODO: Replace with actual EFSM API endpoint
                var efsmApiUrl = ConfigurationManager.AppSettings["EfsmApiUrl"] ?? "https://efsm-api.example.com/api/accounts";
                
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                // TODO: Add authentication headers for EFSM API
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GetEfsmApiToken());
                
                var response = await client.PostAsync(efsmApiUrl, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<EfsmResponse>(responseContent);
                }
                else
                {
                    return new EfsmResponse 
                    { 
                        Success = false, 
                        ErrorMessage = $"API call failed with status: {response.StatusCode}" 
                    };
                }
            }
        }

        // Helper method to get EFSM API token
        private string GetEfsmApiToken()
        {
            // TODO: Implement token acquisition for EFSM API
            // This might involve calling EFSM's token endpoint with client credentials
            return "your-efsm-api-token-here";
        }

        // Helper method to get user display name from claims
        private string GetUserDisplayName()
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var claimsIdentity = User.Identity as ClaimsIdentity;
                    if (claimsIdentity != null)
                    {
                        var displayName = claimsIdentity.FindFirst("name")?.Value ?? 
                                        claimsIdentity.FindFirst("preferred_username")?.Value ??
                                        claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                        
                        if (!string.IsNullOrEmpty(displayName))
                            return displayName;
                    }
                    
                    return User.Identity.Name ?? "User";
                }
                catch
                {
                    return User.Identity.Name ?? "User";
                }
            }
            
            return "User";
        }

        // Helper method to get specific user claim
        private string GetUserClaim(string claimType)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var claimsIdentity = User.Identity as ClaimsIdentity;
                    return claimsIdentity?.FindFirst(claimType)?.Value ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
            return string.Empty;
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
        
        // FLWINS Authentication Information
        public string FlwinsUserId { get; set; }
        public string FlwinsUserName { get; set; }
    }

    // Model for EFSM API response
    public class EfsmResponse
    {
        public bool Success { get; set; }
        public string AccountId { get; set; }
        public string RedirectUrl { get; set; }
        public string ErrorMessage { get; set; }
    }
}