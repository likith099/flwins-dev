import { Configuration, RedirectRequest } from "@azure/msal-browser";

// MSAL configuration for Azure AD B2C with External ID (Custom Domain)
const msalConfig: Configuration = {
  auth: {
    clientId: process.env.NEXT_PUBLIC_AZURE_AD_CLIENT_ID || "0a37f565-9bea-4bdd-aacf-f0d8f909c096", // Your client ID from the URL
    authority: process.env.NEXT_PUBLIC_AZURE_AD_AUTHORITY || "https://flwins.ciamlogin.com/4cc02933-c81d-4fe9-9f71-850984769f51/v2.0", // Your B2C authority
    redirectUri: process.env.NEXT_PUBLIC_REDIRECT_URI || 
      (typeof window !== 'undefined' ? window.location.origin : "http://localhost:3001"), // Redirect to home, then navigate to profile
    knownAuthorities: [
      "flwins.ciamlogin.com" // Your B2C custom domain
    ],
    postLogoutRedirectUri: typeof window !== 'undefined' ? window.location.origin : "http://localhost:3001",
  },
  cache: {
    cacheLocation: "sessionStorage", // Cache location
    storeAuthStateInCookie: false, // Set to true for IE11/Edge support
  },
  system: {
    loggerOptions: {
      loggerCallback: (level: number, message: string, containsPii: boolean) => {
        if (containsPii) {
          return;
        }
        switch (level) {
          case 0: // LogLevel.Error
            console.error(message);
            return;
          case 1: // LogLevel.Warning
            console.warn(message);
            return;
          case 2: // LogLevel.Info
            console.info(message);
            return;
          case 3: // LogLevel.Verbose
            console.debug(message);
            return;
        }
      }
    }
  }
};

// Login request configuration for Azure AD B2C External ID
export const loginRequest = {
  scopes: ["openid", "profile", "email"], // Request user profile information
  prompt: "select_account", // Allow user to select account
  redirectUri: process.env.NEXT_PUBLIC_REDIRECT_URI || 
    (typeof window !== 'undefined' ? window.location.origin : "http://localhost:3001"), // Redirect to home, then navigate to profile
  extraQueryParameters: {
    response_mode: "query" // Use query mode for B2C compatibility
  }
};

// Configuration for Microsoft Graph API (if needed)
export const graphConfig = {
  graphMeEndpoint: "https://graph.microsoft.com/v1.0/me",
};

// B2C External ID configuration
export const b2cPolicies = {
  names: {
    signUpSignIn: process.env.NEXT_PUBLIC_SIGN_UP_SIGN_IN_POLICY || "B2C_1A_SIGNUP_SIGNIN", // Your user flow name
    editProfile: process.env.NEXT_PUBLIC_EDIT_PROFILE_POLICY || "",
  },
  authorities: {
    signUpSignIn: {
      authority: process.env.NEXT_PUBLIC_AZURE_AD_AUTHORITY || "https://flwins.ciamlogin.com/4cc02933-c81d-4fe9-9f71-850984769f51/v2.0",
    },
  },
  authorityDomain: process.env.NEXT_PUBLIC_AUTHORITY_DOMAIN || "flwins.ciamlogin.com",
  tenantName: "FLWINS",
  tenantId: "4cc02933-c81d-4fe9-9f71-850984769f51"
};

export default msalConfig;