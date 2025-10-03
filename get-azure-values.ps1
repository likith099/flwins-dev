# PowerShell commands to get Azure values

# Get your Azure AD Tenant ID
Connect-AzAccount
$context = Get-AzContext
$tenantId = $context.Tenant.Id
Write-Host "AZURE_TENANT_ID = $tenantId"

# Generate a secure shared secret for FLWINS integration
$sharedSecret = [System.Web.Security.Membership]::GeneratePassword(32, 8)
Write-Host "FLWINS_SHARED_SECRET = $sharedSecret"

# Get your subscription and tenant info
Get-AzTenant | Select-Object Id, Name