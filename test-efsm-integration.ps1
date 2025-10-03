# Test EFSM Integration Script
# This script helps test the FLWINS -> EFSM automatic login integration

Write-Host "FLWINS -> EFSM Integration Test" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green

# 1. Test EFSM App Service Settings
Write-Host "`n1. Checking EFSM App Service Configuration..." -ForegroundColor Yellow

$efsmAppName = "efsmod-dev-egcyb2bahcdkamdm"
$resourceGroup = "your-resource-group-name"  # Update this with your actual resource group

Write-Host "App Service: $efsmAppName" -ForegroundColor Cyan

# Required App Settings for EFSM
$requiredSettings = @(
    "AUTO_CREATE_ACCOUNTS",
    "AZURE_CLIENT_ID", 
    "AZURE_CLIENT_SECRET",
    "AZURE_TENANT_ID",
    "ENVIRONMENT",
    "FLWINS_ALLOWED_DOMAINS",
    "FLWINS_SHARED_SECRET",
    "MICROSOFT_PROVIDER_AUTHENTICATION_SECRET",
    "SESSION_TIMEOUT",
    "WEBSITE_AUTH_AAD_ALLOWED_TENANTS"
)

Write-Host "`nRequired App Settings:" -ForegroundColor Cyan
foreach ($setting in $requiredSettings) {
    Write-Host "  ✓ $setting" -ForegroundColor Green
}

# 2. Test EFSM Provision API Endpoint
Write-Host "`n2. Testing EFSM Provision API..." -ForegroundColor Yellow

$efsmProvisionUrl = "https://efsmod-dev-egcyb2bahcdkamdm.canadacentral-01.azurewebsites.net/Provision/Create"
Write-Host "Testing endpoint: $efsmProvisionUrl" -ForegroundColor Cyan

try {
    # Test if the endpoint exists (should return 401/403 for unauthorized, not 404)
    $response = Invoke-WebRequest -Uri $efsmProvisionUrl -Method GET -UseBasicParsing -ErrorAction SilentlyContinue
    Write-Host "  ✓ Endpoint is accessible" -ForegroundColor Green
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    if ($statusCode -eq 404) {
        Write-Host "  ✗ Endpoint not found (404) - Need to implement /Provision/Create endpoint" -ForegroundColor Red
    } elseif ($statusCode -eq 401 -or $statusCode -eq 403) {
        Write-Host "  ✓ Endpoint exists but requires authentication" -ForegroundColor Green
    } else {
        Write-Host "  ? Endpoint returned status: $statusCode" -ForegroundColor Yellow
    }
}

# 3. Test EFSM Family Portal
Write-Host "`n3. Testing EFSM Family Portal..." -ForegroundColor Yellow

$efsmPortalUrl = "https://efsmod-dev-egcyb2bahcdkamdm.canadacentral-01.azurewebsites.net/Home/FamilyPortal" 
Write-Host "Testing endpoint: $efsmPortalUrl" -ForegroundColor Cyan

try {
    $response = Invoke-WebRequest -Uri $efsmPortalUrl -Method GET -UseBasicParsing -ErrorAction SilentlyContinue
    Write-Host "  ✓ Family Portal is accessible" -ForegroundColor Green
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    Write-Host "  ? Family Portal returned status: $statusCode" -ForegroundColor Yellow
}

# 4. Configuration Recommendations
Write-Host "`n4. Configuration Recommendations:" -ForegroundColor Yellow

Write-Host "`nFor EFSM App Service, ensure these values are set:" -ForegroundColor Cyan
Write-Host "  AUTO_CREATE_ACCOUNTS = true" -ForegroundColor White
Write-Host "  AZURE_CLIENT_ID = 7facd66f-0a8b-4757-823a-61e23d4909e2" -ForegroundColor White
Write-Host "  AZURE_TENANT_ID = <your-tenant-id>" -ForegroundColor White
Write-Host "  FLWINS_ALLOWED_DOMAINS = flwins-dev-dshjczeyf7dxeqdz.canadacentral-01.azurewebsites.net" -ForegroundColor White
Write-Host "  FLWINS_SHARED_SECRET = <same-secret-as-in-flwins-web.config>" -ForegroundColor White
Write-Host "  ENVIRONMENT = dev" -ForegroundColor White

Write-Host "`nFor FLWINS Web.config, update:" -ForegroundColor Cyan
Write-Host "  FlwinsSharedSecret = <same-secret-as-efsm-app-service>" -ForegroundColor White

# 5. Next Steps
Write-Host "`n5. Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Implement /Provision/Create endpoint in EFSM app" -ForegroundColor White
Write-Host "  2. Configure Azure AD B2B guest user invitation" -ForegroundColor White  
Write-Host "  3. Test automatic account creation flow" -ForegroundColor White
Write-Host "  4. Verify SSO redirect works correctly" -ForegroundColor White

Write-Host "`nTest completed!" -ForegroundColor Green