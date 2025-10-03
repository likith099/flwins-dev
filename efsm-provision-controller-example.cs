// EFSM Provision Controller - Add this to your EFSM app
// This handles automatic user creation and SSO from FLWINS

using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace YourEfsmApp.Controllers
{
    public class ProvisionController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> Create()
        {
            try
            {
                // 1. Validate the request comes from FLWINS
                var flwinsSecret = Request.Headers["X-FLWINS-Secret"];
                var expectedSecret = System.Configuration.ConfigurationManager.AppSettings["FLWINS_SHARED_SECRET"];
                
                if (string.IsNullOrEmpty(flwinsSecret) || flwinsSecret != expectedSecret)
                {
                    return Json(new { success = false, error = "Unauthorized" }, JsonRequestBehavior.AllowGet);
                }

                // 2. Parse the request from FLWINS
                string requestBody = "";
                using (var reader = new System.IO.StreamReader(Request.InputStream))
                {
                    requestBody = reader.ReadToEnd();
                }
                
                var request = Newtonsoft.Json.JsonConvert.DeserializeObject<FlwinsProvisionRequest>(requestBody);
                
                if (request == null || string.IsNullOrEmpty(request.email))
                {
                    return Json(new { success = false, error = "Invalid request" }, JsonRequestBehavior.AllowGet);
                }

                // 3. Check if AUTO_CREATE_ACCOUNTS is enabled
                var autoCreateAccounts = System.Configuration.ConfigurationManager.AppSettings["AUTO_CREATE_ACCOUNTS"];
                if (string.Equals(autoCreateAccounts, "true", StringComparison.OrdinalIgnoreCase))
                {
                    // 4. Create/invite user to EFSM tenant
                    await CreateOrInviteUser(request);
                }

                // 5. Generate SSO URL for automatic login
                var ssoUrl = GenerateEfsmSsoUrl(request.email, request.redirectPath ?? "/Home/FamilyPortal");
                
                // 6. Return the SSO URL to FLWINS
                return Json(new { 
                    success = true, 
                    ssoUrl = ssoUrl,
                    message = "User provisioned successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Trace.TraceError($"EFSM Provision Error: {ex.Message}");
                
                return Json(new { 
                    success = false, 
                    error = "Provisioning failed",
                    fallbackUrl = Url.Action("FamilyPortal", "Home", null, Request.Url.Scheme)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task CreateOrInviteUser(FlwinsProvisionRequest request)
        {
            try
            {
                // Microsoft Graph setup
                var clientId = System.Configuration.ConfigurationManager.AppSettings["AZURE_CLIENT_ID"];
                var clientSecret = System.Configuration.ConfigurationManager.AppSettings["AZURE_CLIENT_SECRET"];
                var tenantId = System.Configuration.ConfigurationManager.AppSettings["AZURE_TENANT_ID"];

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
                    .Filter($"mail eq '{request.email}' or userPrincipalName eq '{request.email}'")
                    .GetAsync();

                if (existingUsers.Count == 0)
                {
                    // Invite external user
                    var invitation = new Invitation
                    {
                        InvitedUserEmailAddress = request.email,
                        InvitedUserDisplayName = request.displayName ?? request.email,
                        InviteRedirectUrl = $"https://{Request.Url.Host}/Home/FamilyPortal",
                        SendInvitationMessage = false // Don't send email, we're doing SSO
                    };

                    await graphServiceClient.Invitations
                        .Request()
                        .AddAsync(invitation);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"User creation failed: {ex.Message}");
                // Continue with SSO even if user creation fails
            }
        }

        private string GenerateEfsmSsoUrl(string userEmail, string redirectPath)
        {
            // Generate URL for Azure AD authentication with automatic login
            var baseUrl = $"https://{Request.Url.Host}";
            var loginUrl = $"{baseUrl}/.auth/login/aad";
            
            // Add post-login redirect
            var encodedRedirect = System.Web.HttpUtility.UrlEncode($"{baseUrl}{redirectPath}");
            var ssoUrl = $"{loginUrl}?post_login_redirect_url={encodedRedirect}";
            
            return ssoUrl;
        }
    }

    // Request model from FLWINS
    public class FlwinsProvisionRequest
    {
        public string email { get; set; }
        public string displayName { get; set; }
        public string redirectPath { get; set; }
        public bool autoCreateAccount { get; set; }
        public bool autoLogin { get; set; }
    }
}