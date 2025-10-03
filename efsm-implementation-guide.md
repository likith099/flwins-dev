# FLWINS -> EFSM Automatic User Provisioning - Implementation Guide

## Problem
Users coming from FLWINS to EFSM are still being asked to login or create accounts manually, instead of being automatically provisioned and logged in.

## Solution Overview
1. **FLWINS** sends user information to EFSM via a secure token
2. **EFSM** receives the token, creates the user account automatically, and logs them in
3. **User** lands directly on the EFSM Family Portal without manual intervention

## Implementation Steps

### Step 1: Deploy Updated FLWINS Code ✅ (Already Done)
The FLWINS `AccessEfsmPortal` method has been updated to:
- Try the EFSM Provision API first (if available)
- Fall back to the AutoProvision URL with encrypted user token

### Step 2: Implement EFSM Controllers (Required)

Add these two controllers to your EFSM application:

#### A. AutoProvisionController.cs
```csharp
// Copy the code from efsm-autoprovision-controllers.cs
// This handles the fallback method with user tokens
```

#### B. ProvisionController.cs (API endpoint)
```csharp
// Also in efsm-autoprovision-controllers.cs
// This handles the proper API method
```

### Step 3: Configure EFSM App Service Settings

Ensure these are set in your EFSM App Service configuration:

```
AUTO_CREATE_ACCOUNTS = true
AZURE_CLIENT_ID = 7facd66f-0a8b-4757-823a-61e23d4909e2
AZURE_CLIENT_SECRET = <your-client-secret>
AZURE_TENANT_ID = <your-tenant-id>
FLWINS_SHARED_SECRET = <generate-a-secure-secret>
FLWINS_ALLOWED_DOMAINS = flwins-dev-dshjczeyf7dxeqdz.canadacentral-01.azurewebsites.net
ENVIRONMENT = dev
```

### Step 4: Update FLWINS Web.config

Update your FLWINS Web.config with the same shared secret:

```xml
<add key="FlwinsSharedSecret" value="<same-secret-as-efsm>" />
```

### Step 5: Configure Azure AD App Registration

For the EFSM app registration (7facd66f-0a8b-4757-823a-61e23d4909e2):

1. **API Permissions**: Add Microsoft Graph permissions:
   - `User.Invite.All` (Application permission)
   - `User.ReadWrite.All` (Application permission)
   - `Directory.ReadWrite.All` (Application permission)

2. **Grant Admin Consent** for these permissions

3. **Create Client Secret** and add to EFSM App Service settings

## How It Works

### Flow 1: API Method (Preferred)
```
FLWINS User clicks "Create EFSM Account"
    ↓
FLWINS calls EFSM /Provision/Create API
    ↓
EFSM creates user account via Microsoft Graph
    ↓
EFSM returns SSO URL to FLWINS
    ↓
FLWINS redirects user to EFSM SSO URL
    ↓
User lands on EFSM Family Portal (logged in)
```

### Flow 2: Token Method (Fallback)
```
FLWINS User clicks "Create EFSM Account"
    ↓
FLWINS creates encrypted user token
    ↓
FLWINS redirects to EFSM /AutoProvision?token=...
    ↓
EFSM decodes token and creates user account
    ↓
EFSM redirects to Azure AD login
    ↓
After login, EFSM redirects to Family Portal
```

## Testing

1. **Deploy FLWINS changes** (already done)
2. **Implement EFSM controllers** (required)
3. **Configure App Service settings** (required)
4. **Test the flow**:
   - Login to FLWINS
   - Go to Services page
   - Click "Create EFSM Account"
   - Should automatically create account and login to EFSM

## Troubleshooting

### Check EFSM Logs
Look for these log entries in EFSM Application Insights:
- `EFSM AutoProvision: User {email} created/invited successfully`
- `EFSM PostLogin: User {email} logged in successfully`

### Common Issues
1. **404 on /AutoProvision**: Controller not implemented
2. **Unauthorized**: FLWINS_SHARED_SECRET mismatch
3. **User creation fails**: Missing Azure AD permissions
4. **Token expired**: User took too long (5 minute limit)

## Security Notes
- User tokens expire after 5 minutes
- Tokens are Base64 encoded (consider encryption for production)
- Shared secret validates requests between FLWINS and EFSM
- Only users from allowed domains can be provisioned