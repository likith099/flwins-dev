import { Configuration, PopupRequest } from "@azure/msal-browser";

// MSAL configuration
const msalConfig: Configuration = {
  auth: {
    clientId: process.env.NEXT_PUBLIC_AZURE_AD_CLIENT_ID || "", // Replace with your Azure AD App Registration client ID
    authority: process.env.NEXT_PUBLIC_AZURE_AD_AUTHORITY || "", // Replace with your Azure AD tenant authority
    redirectUri: process.env.NEXT_PUBLIC_REDIRECT_URI || "/", // Must match the redirect URI configured in Azure AD
  },
  cache: {
    cacheLocation: "sessionStorage", // This configures where your cache will be stored
    storeAuthStateInCookie: false, // Set this to "true" if you're having issues on IE11 or Edge
  },
};

// Add scopes here for ID token to be used at Microsoft Graph API endpoints.
export const loginRequest: PopupRequest = {
  scopes: ["User.Read"],
};

export const graphConfig = {
  graphMeEndpoint: "https://graph.microsoft.com/v1.0/me",
};

export default msalConfig;