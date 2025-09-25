# Azure Web Apps Deployment Guide

## Your Application URL: https://flwins-webapp-dev-cmbsd0b8g6ete0gb.canadacentral-01.azurewebsites.net/

## 🔧 Steps to Fix Your Deployment

### 1. Configure Environment Variables in Azure Web App

Go to your Azure Portal → App Services → flwins-webapp-dev → Configuration → Application settings

Add the following **Application Settings**:

```
NEXT_PUBLIC_AZURE_AD_CLIENT_ID = 0a37f565-9bea-4bdd-aacf-f0d8f909c096
NEXT_PUBLIC_AZURE_AD_AUTHORITY = https://flwins.ciamlogin.com/4cc02933-c81d-4fe9-9f71-850984769f51/v2.0
NEXT_PUBLIC_REDIRECT_URI = https://flwins-webapp-dev-cmbsd0b8g6ete0gb.canadacentral-01.azurewebsites.net
NEXT_PUBLIC_KNOWN_AUTHORITY = flwins.ciamlogin.com
NEXT_PUBLIC_AUTHORITY_DOMAIN = flwins.ciamlogin.com
NEXT_PUBLIC_SIGN_UP_SIGN_IN_POLICY = B2C_1A_SIGNUP_SIGNIN
```

**Important**: Click **Save** after adding all settings!

### 2. Update Azure AD B2C Redirect URI

In your Azure Portal → Azure AD B2C → App registrations → Your app (0a37f565-9bea-4bdd-aacf-f0d8f909c096):

1. Go to **Authentication**
2. Under **Single-page application** platform
3. Add this redirect URI: `https://flwins-webapp-dev-cmbsd0b8g6ete0gb.canadacentral-01.azurewebsites.net`
4. Also add for local development: `http://localhost:3001`
5. **Important**: Make sure you're adding to **Single-page application** platform, NOT Web platform
6. Click **Save**

### 3. Trigger a New Deployment

Since you've already pushed to the main branch, the deployment should have triggered automatically. If not:

```bash
# Force a new deployment by making a small change and pushing
git commit --allow-empty -m "Trigger deployment"
git push origin main
```

### 4. Check Azure Web App Logs

If the app still doesn't load:

1. Go to Azure Portal → App Services → flwins-webapp-dev
2. Click **Log stream** to see live logs
3. Check **Deployment Center** to see deployment status
4. Look for any build or runtime errors

### 5. Common Issues & Solutions

#### Issue: "Application Error" or blank page
**Solution**: Check Application Settings are saved and restart the app:
- Go to Overview → Click **Restart**

#### Issue: Authentication redirects to localhost
**Solution**: Ensure `NEXT_PUBLIC_REDIRECT_URI` is set correctly in Application Settings

#### Issue: Build fails during deployment
**Solution**: Check the GitHub Actions deployment logs in your repository

### 6. Test the Deployment

Once everything is configured:

1. Visit: https://flwins-webapp-dev-cmbsd0b8g6ete0gb.canadacentral-01.azurewebsites.net/
2. Click "Sign in with Microsoft"
3. You should be redirected to your FLWINS login page
4. After login, you should return to the home page, then automatically navigate to `/profile` page showing detailed user information

## 📋 Checklist

- [ ] Environment variables added to Azure Web App
- [ ] Production redirect URI added to B2C app registration  
- [ ] Application restarted in Azure
- [ ] Deployment completed successfully
- [ ] Authentication flow tested end-to-end

## 🚨 If Still Not Working

Check the GitHub Actions workflow status in your repository and look for any deployment errors.

The most common issue is missing environment variables in the Azure Web App configuration.