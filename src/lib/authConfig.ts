import { Configuration, PopupRequest } from "@azure/msal-browser";

// MSAL configuration for Azure AD with User Flows
const msalConfig: Configuration = {
  auth: {
    clientId: process.env.NEXT_PUBLIC_AZURE_AD_CLIENT_ID || "", // Your App Registration client ID
    authority: process.env.NEXT_PUBLIC_AZURE_AD_AUTHORITY || "", // Your authority URL with user flow
    redirectUri: process.env.NEXT_PUBLIC_REDIRECT_URI || "http://localhost:3000", // Redirect URI
    knownAuthorities: [
      // Add your B2C domain or tenant domain here
      process.env.NEXT_PUBLIC_KNOWN_AUTHORITY || ""
    ],
  },
  cache: {
    cacheLocation: "sessionStorage", // Cache location
    storeAuthStateInCookie: false, // Set to true for IE11/Edge support
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
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

// Login request configuration for user flows
export const loginRequest: PopupRequest = {
  scopes: ["openid", "profile", "email"], // Request user profile information
  prompt: "select_account", // Allow user to select account
};

// Configuration for Microsoft Graph API (if needed)
export const graphConfig = {
  graphMeEndpoint: "https://graph.microsoft.com/v1.0/me",
};

// User flow configuration
export const b2cPolicies = {
  names: {
    signUpSignIn: process.env.NEXT_PUBLIC_SIGN_UP_SIGN_IN_POLICY || "",
    editProfile: process.env.NEXT_PUBLIC_EDIT_PROFILE_POLICY || "",
  },
  authorities: {
    signUpSignIn: {
      authority: process.env.NEXT_PUBLIC_AZURE_AD_AUTHORITY || "",
    },
  },
  authorityDomain: process.env.NEXT_PUBLIC_AUTHORITY_DOMAIN || "",
};

export default msalConfig;