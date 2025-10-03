# Quick Fix: Simple EFSM Provision Endpoint
# Add this to your EFSM app as a temporary solution

## Option 1: Simple Controller Method (Add to existing controller)

```csharp
[HttpPost]
public ActionResult Create()
{
    try
    {
        // Validate FLWINS request
        var flwinsSecret = Request.Headers["X-FLWINS-Secret"];
        var expectedSecret = System.Configuration.ConfigurationManager.AppSettings["FLWINS_SHARED_SECRET"];
        
        if (flwinsSecret != expectedSecret)
        {
            return Json(new { success = false, error = "Unauthorized" });
        }

        // Parse request
        string body = "";
        using (var reader = new System.IO.StreamReader(Request.InputStream))
        {
            body = reader.ReadToEnd();
        }
        
        // For now, just return SSO URL without creating users
        var baseUrl = $"https://{Request.Url.Host}";
        var ssoUrl = $"{baseUrl}/.auth/login/aad?post_login_redirect_url={System.Web.HttpUtility.UrlEncode($"{baseUrl}/Home/FamilyPortal")}";
        
        return Json(new { 
            success = true, 
            ssoUrl = ssoUrl 
        });
    }
    catch
    {
        return Json(new { success = false, error = "Server error" });
    }
}
```

## Option 2: Even Simpler - Just redirect to Family Portal with auth

Update your FLWINS AccessEfsmPortal method to skip the API call temporarily:

```csharp
[Authorize]
public ActionResult AccessEfsmPortal()
{
    // Temporary: Direct redirect to EFSM with Azure AD login
    var efsmBaseUrl = "https://efsmod-dev-egcyb2bahcdkamdm.canadacentral-01.azurewebsites.net";
    var familyPortalPath = "/Home/FamilyPortal";
    var redirectUrl = System.Web.HttpUtility.UrlEncode($"{efsmBaseUrl}{familyPortalPath}");
    var ssoUrl = $"{efsmBaseUrl}/.auth/login/aad?post_login_redirect_url={redirectUrl}";
    
    return Redirect(ssoUrl);
}
```

## Steps to Fix:

1. **Immediate Fix**: Update FLWINS to use Option 2 above
2. **Proper Fix**: Implement the Provision/Create endpoint in EFSM app
3. **Configure Azure AD**: Ensure both apps share the same tenant or have B2B trust
4. **Test**: User should automatically login to EFSM when coming from FLWINS

## App Service Settings Verification:

Make sure these are set correctly in EFSM App Service:

- FLWINS_SHARED_SECRET = "same-as-flwins-web-config"  
- AZURE_CLIENT_ID = "7facd66f-0a8b-4757-823a-61e23d4909e2"
- AZURE_TENANT_ID = "your-actual-tenant-id"
- AUTO_CREATE_ACCOUNTS = "true"
- FLWINS_ALLOWED_DOMAINS = "flwins-dev-dshjczeyf7dxeqdz.canadacentral-01.azurewebsites.net"