# Generate Shared Secret for FLWINS <-> EFSM Integration
# Run this script to generate a secure shared secret

Write-Host "FLWINS <-> EFSM Shared Secret Generator" -ForegroundColor Green
Write-Host "=======================================" -ForegroundColor Green

# Generate a secure random string
$bytes = New-Object Byte[] 32
$rng = [System.Security.Cryptography.RNGCryptoServiceProvider]::Create()
$rng.GetBytes($bytes)
$sharedSecret = [Convert]::ToBase64String($bytes)

Write-Host "`nGenerated Shared Secret:" -ForegroundColor Yellow
Write-Host $sharedSecret -ForegroundColor Cyan

Write-Host "`nNext Steps:" -ForegroundColor Yellow
Write-Host "1. Add this to EFSM App Service Settings:" -ForegroundColor White
Write-Host "   FLWINS_SHARED_SECRET = $sharedSecret" -ForegroundColor Gray

Write-Host "`n2. Add this to FLWINS Web.config:" -ForegroundColor White
Write-Host "   <add key=`"FlwinsSharedSecret`" value=`"$sharedSecret`" />" -ForegroundColor Gray

Write-Host "`n3. Deploy both applications with the new secret" -ForegroundColor White

Write-Host "`nIMPORTANT: Keep this secret secure and don't share it publicly!" -ForegroundColor Red