// AutoProvision Controller for EFSM App
// This handles automatic user creation when users come from FLWINS
// Add this to your EFSM application

using System;
using System.Web.Mvc;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace YourEfsmApp.Controllers
{
    public class AutoProvisionController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> Index(string token, string redirect = "/Home/FamilyPortal")
        {
            try
            {
                // 1. Validate and decode the token from FLWINS
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                var userInfoJson = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var userInfo = JsonConvert.DeserializeObject<FlwinsUserInfo>(userInfoJson);

                if (userInfo == null || string.IsNullOrEmpty(userInfo.email) || userInfo.source != "flwins")
                {
                    return RedirectToAction("Login", "Account");
                }

                // 2. Check if token is not too old (optional security check)
                var tokenAge = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - userInfo.timestamp;
                if (tokenAge > 300) // 5 minutes
                {
                    ViewBag.Message = "Session expired. Please try again from FLWINS.";
                    return RedirectToAction("Login", "Account");
                }

                // 3. Check if AUTO_CREATE_ACCOUNTS is enabled
                var autoCreateAccounts = System.Configuration.ConfigurationManager.AppSettings["AUTO_CREATE_ACCOUNTS"];
                if (string.Equals(autoCreateAccounts, "true", StringComparison.OrdinalIgnoreCase))
                {
                    // 4. Create/invite user to EFSM tenant
                    var userCreated = await CreateOrInviteUser(userInfo);
                    
                    if (userCreated)
                    {
                        // Log successful creation
                        System.Diagnostics.Trace.TraceInformation($"EFSM AutoProvision: User {userInfo.email} created/invited successfully");
                    }
                }

                // 5. Store user info in session for post-login processing
                Session["FlwinsUserInfo"] = userInfo;
                Session["FlwinsRedirect"] = redirect;

                // 6. Redirect to Azure AD login with post-login redirect
                var baseUrl = $"https://{Request.Url.Host}";
                var postLoginUrl = System.Web.HttpUtility.UrlEncode($"{baseUrl}/AutoProvision/PostLogin");
                var loginUrl = $"{baseUrl}/.auth/login/aad?post_login_redirect_url={postLoginUrl}";

                return Redirect(loginUrl);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"EFSM AutoProvision Error: {ex.Message}");
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public ActionResult PostLogin()
        {
            try
            {
                // User has completed Azure AD login, now redirect to final destination
                var flwinsRedirect = Session["FlwinsRedirect"] as string ?? "/Home/FamilyPortal";
                var userInfo = Session["FlwinsUserInfo"] as FlwinsUserInfo;

                if (userInfo != null)
                {
                    // Log successful login
                    System.Diagnostics.Trace.TraceInformation($"EFSM PostLogin: User {userInfo.email} logged in successfully, redirecting to {flwinsRedirect}");
                    
                    // Clear session data
                    Session.Remove("FlwinsUserInfo");
                    Session.Remove("FlwinsRedirect");
                }

                return Redirect(flwinsRedirect);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"EFSM PostLogin Error: {ex.Message}");
                return Redirect("/Home/FamilyPortal");
            }
        }

        private async Task<bool> CreateOrInviteUser(FlwinsUserInfo userInfo)
        {
            try
            {
                // Microsoft Graph setup
                var clientId = System.Configuration.ConfigurationManager.AppSettings["AZURE_CLIENT_ID"];
                var clientSecret = System.Configuration.ConfigurationManager.AppSettings["AZURE_CLIENT_SECRET"];
                var tenantId = System.Configuration.ConfigurationManager.AppSettings["AZURE_TENANT_ID"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(tenantId))
                {
                    System.Diagnostics.Trace.TraceWarning("EFSM: Azure AD configuration missing, skipping user creation");
                    return false;
                }

                var app = ConfidentialClientApplicationBuilder
                    .Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                    .Build();

                var authProvider = new ClientCredentialProvider(app);
                var graphServiceClient = new GraphServiceClient(authProvider);

                // Check if user already exists
                var existingUsers = await graphServiceClient.Users
                    .Request()
                    .Filter($"mail eq '{userInfo.email}' or userPrincipalName eq '{userInfo.email}'")
                    .GetAsync();

                if (existingUsers.Count == 0)
                {
                    // Invite external user
                    var invitation = new Invitation
                    {
                        InvitedUserEmailAddress = userInfo.email,
                        InvitedUserDisplayName = userInfo.displayName ?? userInfo.email,
                        InviteRedirectUrl = $"https://{Request.Url.Host}/Home/FamilyPortal",
                        SendInvitationMessage = false // Don't send email, we're doing SSO
                    };

                    var result = await graphServiceClient.Invitations
                        .Request()
                        .AddAsync(invitation);

                    System.Diagnostics.Trace.TraceInformation($"EFSM: User {userInfo.email} invited successfully");
                    return true;
                }
                else
                {
                    System.Diagnostics.Trace.TraceInformation($"EFSM: User {userInfo.email} already exists");
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"EFSM User creation failed: {ex.Message}");
                return false; // Continue with login even if user creation fails
            }
        }
    }

    // Also add the Provision controller for the API endpoint
    public class ProvisionController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> Create()
        {
            try
            {
                // Validate the request comes from FLWINS
                var flwinsSecret = Request.Headers["X-FLWINS-Secret"];
                var expectedSecret = System.Configuration.ConfigurationManager.AppSettings["FLWINS_SHARED_SECRET"];
                
                if (string.IsNullOrEmpty(flwinsSecret) || flwinsSecret != expectedSecret)
                {
                    return Json(new { success = false, error = "Unauthorized" }, JsonRequestBehavior.AllowGet);
                }

                // Parse the request from FLWINS
                string requestBody = "";
                using (var reader = new System.IO.StreamReader(Request.InputStream))
                {
                    requestBody = reader.ReadToEnd();
                }
                
                var request = JsonConvert.DeserializeObject<FlwinsProvisionRequest>(requestBody);
                
                if (request == null || string.IsNullOrEmpty(request.email))
                {
                    return Json(new { success = false, error = "Invalid request" }, JsonRequestBehavior.AllowGet);
                }

                // Create user info token like the AutoProvision controller expects
                var userInfo = new FlwinsUserInfo
                {
                    email = request.email,
                    displayName = request.displayName,
                    source = "flwins",
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                var userInfoJson = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userInfo)));
                var baseUrl = $"https://{Request.Url.Host}";
                var ssoUrl = $"{baseUrl}/AutoProvision?token={userInfoJson}&redirect={request.redirectPath ?? "/Home/FamilyPortal"}";
                
                return Json(new { 
                    success = true, 
                    ssoUrl = ssoUrl,
                    message = "User provisioning initiated"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"EFSM Provision Error: {ex.Message}");
                
                return Json(new { 
                    success = false, 
                    error = "Provisioning failed"
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }

    // Models
    public class FlwinsUserInfo
    {
        public string email { get; set; }
        public string displayName { get; set; }
        public string source { get; set; }
        public long timestamp { get; set; }
    }

    public class FlwinsProvisionRequest
    {
        public string email { get; set; }
        public string displayName { get; set; }
        public string redirectPath { get; set; }
        public bool autoCreateAccount { get; set; }
        public bool autoLogin { get; set; }
        public string source { get; set; }
    }
}