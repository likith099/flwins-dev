# EFAMod Portal (Next.js + Azure AD B2C)

This app mirrors the FLWINS app using MSAL (redirect flow) and includes an SR Application Form button that securely POSTs the id_token to EFAMod for SSO and JIT provisioning.

## Azure details needed from you
Please provide these so I can wire everything end-to-end:
- B2C/External ID tenant:
  - Tenant ID (GUID)
  - Custom domain (e.g., yourtenant.ciamlogin.com)
- App Registration (SPA) for this app:
  - Client ID (Application ID)
  - Authority URL: https://<custom-domain>/<tenant-id-or-name>/v2.0
  - Redirect URIs to add under SPA platform:
    - http://localhost:3005/auth/callback (local)
    - https://<your-app-service>.azurewebsites.net/auth/callback (prod)
  - If a specific policy/user flow is required, its name (tfp/acr), e.g. B2C_1A_SIGNUP_SIGNIN
- Azure App Service for EFAMod Portal:
  - App name (e.g., efamod-portal-dev)
  - Resource group name
  - Subscription ID
  - The public URL (e.g., https://efamod-portal-dev.azurewebsites.net)
- GitHub Actions OIDC/Azure secrets for deployment (or I can guide you to set them up):
  - Client ID, Tenant ID, Subscription ID for the federated credential
- EFAMod SSO base URL (where the app will POST the id_token), e.g., https://efamod.yourdomain.com

## Local setup
1. Copy `.env.local.example` to `.env.local` and fill values:
   - NEXT_PUBLIC_AZURE_AD_CLIENT_ID
   - NEXT_PUBLIC_AZURE_AD_AUTHORITY
   - NEXT_PUBLIC_AUTHORITY_DOMAIN
   - NEXT_PUBLIC_REDIRECT_URI (keep default for local)
   - NEXT_PUBLIC_EFSMOD_URL (EFAMod SSO endpoint base)
2. Install and run:
   - cd apps/efamod-portal
   - npm install
   - npm run dev
3. Open http://localhost:3005

## Deploy to Azure App Service
- This app ships with `server.js` and `web.config` for App Service (Node 22 LTS).
- A GitHub Actions workflow `.github/workflows/main_efamod-portal.yml` is included with placeholders. Provide the Azure values above and I’ll finalize it.
