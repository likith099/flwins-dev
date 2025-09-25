# Azure AD B2C Configuration Steps

## Your React App is Ready! 🎉

Your application is now configured to work with your FLWINS B2C tenant and will open the login in the same browser tab (redirect mode).

## Next Steps - Configure Your B2C App Registration

You need to add the local development URL as a redirect URI in your Azure AD B2C app registration.

### 1. Go to Azure Portal
- Navigate to: https://portal.azure.com/
- Go to your FLWINS B2C tenant

### 2. Find Your App Registration
- Go to "App registrations"
- Find the app with Client ID: `0a37f565-9bea-4bdd-aacf-f0d8f909c096`

### 3. Update Redirect URIs
- Click on "Authentication" in the left menu
- Under "Single-page application" platform, add:
  - `http://localhost:3001` (for local development)
  - Keep your existing production redirect URIs

### 4. Test Your Application
- Your app is running at: **http://localhost:3001**
- Click "Sign in with Microsoft" button
- It will now redirect to your custom FLWINS login screen in the same tab
- After successful login, you'll be redirected back with user information displayed

## What's Configured

✅ **Client ID**: 0a37f565-9bea-4bdd-aacf-f0d8f909c096
✅ **Authority**: https://flwins.ciamlogin.com/4cc02933-c81d-4fe9-9f71-850984769f51/v2.0
✅ **Redirect Mode**: Same tab (no popup)
✅ **User Info Display**: Email, name, and profile details
✅ **Development Server**: Running on http://localhost:3001

## User Flow Features
- Custom FLWINS branding and login experience
- User registration and sign-in in the same flow
- Profile information extraction and display
- Secure token management
- Same-tab authentication (no popups)

Once you add the redirect URI, your authentication flow will work seamlessly with your custom user flow!