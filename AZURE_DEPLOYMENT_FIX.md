# Azure Web App Deployment Fix Guide

## Issues Identified

1. **Missing Environment Variables**: The Azure Web App doesn't have the required environment variables configured
2. **No Build Process**: The GitHub Actions workflow wasn't building the Next.js app properly
3. **Missing Server Configuration**: Azure needs a server.js file to run Next.js applications

## Fixes Applied

### 1. Updated GitHub Actions Workflow
- Fixed build process to properly build Next.js app
- Added environment variables to build step
- Added Azure Web App settings configuration
- Added hidden files inclusion for deployment

### 2. Created Server Files
- `server.js`: Custom server for Azure Web App to run Next.js
- `web.config`: IIS configuration for Azure Web App

### 3. Updated Package.json
- Added `server` script for Azure Web App startup

## Required Actions

### Step 1: Add GitHub Secrets

Go to your GitHub repository → Settings → Secrets and variables → Actions → New repository secret

Add these secrets with the following values:

```
NEXT_PUBLIC_AZURE_AD_CLIENT_ID = 0a37f565-9bea-4bdd-aacf-f0d8f909c096
NEXT_PUBLIC_AZURE_AD_AUTHORITY = https://flwins.ciamlogin.com/4cc02933-c81d-4fe9-9f71-850984769f51/v2.0
NEXT_PUBLIC_REDIRECT_URI = https://flwins-webapp-dev-cmbsd0b8g6ete0gb.canadacentral-01.azurewebsites.net
NEXT_PUBLIC_KNOWN_AUTHORITY = flwins.ciamlogin.com
NEXT_PUBLIC_AUTHORITY_DOMAIN = flwins.ciamlogin.com
NEXT_PUBLIC_SIGN_UP_SIGN_IN_POLICY = B2C_1A_SIGNUP_SIGNIN
```

### Step 2: Update Azure AD B2C Redirect URI

1. Go to Azure Portal
2. Navigate to your B2C tenant (FLWINS)
3. Go to App registrations → Your app (0a37f565-9bea-4bdd-aacf-f0d8f909c096)
4. Click Authentication
5. Add these redirect URIs:
   - `https://flwins-webapp-dev-cmbsd0b8g6ete0gb.canadacentral-01.azurewebsites.net`
   - Remove any `/profile` endpoints if they exist
6. Enable implicit grant tokens (ID tokens and Access tokens)
7. Save changes

### Step 3: Configure Azure Web App Startup

1. Go to Azure Portal
2. Navigate to your Web App (flwins-webapp-dev)
3. Go to Configuration → General settings
4. Set **Startup Command**: `node server.js`
5. Set **Node.js version**: `~22`
6. Save changes

### Step 4: Push Changes and Deploy

1. Commit and push all changes:
```bash
git add .
git commit -m "Fix Azure Web App deployment configuration"
git push origin main
```

2. This will trigger the GitHub Actions workflow
3. Monitor the workflow in GitHub Actions tab
4. Wait for deployment to complete (5-10 minutes)

### Step 5: Test the Application

1. Go to: https://flwins-webapp-dev-cmbsd0b8g6ete0gb.canadacentral-01.azurewebsites.net
2. Click "Sign In with Microsoft"
3. Complete authentication
4. Verify the profile button works

## Troubleshooting

If the app still doesn't work after following these steps:

### Check Application Logs
1. Go to Azure Portal → Your Web App → Monitoring → Log stream
2. Look for error messages

### Check Environment Variables
1. Go to Azure Portal → Your Web App → Configuration → Application settings
2. Verify all environment variables are set correctly

### Restart the Web App
1. Go to Azure Portal → Your Web App → Overview
2. Click "Restart"
3. Wait 2-3 minutes for restart to complete

## Expected Behavior After Fix

1. **GitHub Actions**: Should build successfully with no errors
2. **Azure Web App**: Should start and serve the Next.js application
3. **Authentication**: Should work with Azure AD B2C
4. **Navigation**: Profile button should work correctly

## Files Created/Modified

- `.github/workflows/main_flwins-webapp-dev.yml` - Updated build and deployment process
- `server.js` - Custom server for Azure Web App
- `web.config` - IIS configuration for Azure Web App  
- `package.json` - Added server script

The main issue was that Azure Web App didn't know how to start your Next.js application and was missing the required environment variables. These fixes should resolve the deployment issue.