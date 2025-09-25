# URGENT FIX: AADSTS900561 Error Resolution

## 🚨 Error: "The endpoint only accepts POST requests. Received a GET request."

This error occurs when the Azure AD B2C configuration doesn't match the authentication flow. Here's how to fix it:

## 🔧 **IMMEDIATE ACTION REQUIRED:**

### 1. Update Your Azure AD B2C App Registration

Go to [Azure Portal](https://portal.azure.com/) → Your FLWINS B2C tenant → App registrations → Your app (0a37f565-9bea-4bdd-aacf-f0d8f909c096):

#### A. Authentication Configuration
1. Click **Authentication** in the left menu
2. Under **Platform configurations**, find **Single-page application**
3. **REMOVE** any `/profile` endpoints if they exist
4. **ADD** these exact redirect URIs:
   - `http://localhost:3001` (for local development)
   - `https://flwins-webapp-dev-cmbsd0b8g6ete0gb.canadacentral-01.azurewebsites.net` (for production)

#### B. Implicit Grant Settings
1. In the same Authentication section
2. Under **Implicit grant and hybrid flows**
3. **ENABLE** both:
   - ✅ **Access tokens (used for implicit flows)**
   - ✅ **ID tokens (used for implicit and hybrid flows)**

#### C. Advanced Settings
1. Scroll down to **Advanced settings**
2. Set **Treat application as a public client**: **No**

### 2. User Flow Configuration Check

Go to **User flows** in your B2C tenant:

1. Find your sign-up/sign-in user flow
2. Click on it → **Properties**
3. Ensure **Token compatibility settings** → **Issuer claim**: **issuer URL with "tfp" claim**

### 3. Test the Fixed Configuration

After making these changes:

1. **Wait 5-10 minutes** for Azure AD to propagate changes
2. Go to: **http://localhost:3001**
3. Click "Sign in with Microsoft"
4. You should now be redirected properly to your FLWINS login screen
5. After successful login, you'll return to the home page and automatically navigate to the profile page

## 🎯 **What Changed:**

- **Removed `/profile` from redirect URIs** - Azure AD B2C works better with root redirects
- **Added proper response mode** handling in the app
- **Enhanced event handling** for successful authentication
- **Automatic navigation** to profile page after successful login

## 📋 **Verification Steps:**

- [ ] Redirect URIs updated in Azure AD B2C (without `/profile`)
- [ ] Implicit grant tokens enabled
- [ ] Application set as public client = No  
- [ ] Development server restarted
- [ ] Login tested successfully

## ⚠️ **If Still Not Working:**

1. **Clear browser cache and cookies**
2. **Try incognito/private browser window**
3. **Check browser console for additional errors**
4. **Verify the exact redirect URI in the error message matches your configuration**

The authentication should now work correctly with your custom FLWINS user flow!