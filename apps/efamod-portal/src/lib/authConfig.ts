import { Configuration } from '@azure/msal-browser';

const msalConfig: Configuration = {
  auth: {
    clientId: process.env.NEXT_PUBLIC_AZURE_AD_CLIENT_ID || '',
    authority: process.env.NEXT_PUBLIC_AZURE_AD_AUTHORITY || '',
    // If NEXT_PUBLIC_REDIRECT_URI is provided, we use it as-is.
    // Recommended: set it to <origin>/auth/callback and add the same in Azure.
    redirectUri:
      process.env.NEXT_PUBLIC_REDIRECT_URI ||
      (typeof window !== 'undefined'
        ? `${window.location.origin}/auth/callback`
        : 'http://localhost:3005/auth/callback'),
    knownAuthorities: [
      process.env.NEXT_PUBLIC_AUTHORITY_DOMAIN ||
        process.env.NEXT_PUBLIC_KNOWN_AUTHORITY ||
        '',
    ],
    postLogoutRedirectUri:
      process.env.NEXT_PUBLIC_POST_LOGOUT_REDIRECT_URI ||
      (typeof window !== 'undefined'
        ? window.location.origin
        : 'http://localhost:3005'),
  },
  cache: {
    cacheLocation: 'sessionStorage',
    storeAuthStateInCookie: false,
  },
};

export const loginRequest = {
  scopes: ['openid', 'profile', 'email'],
  prompt: 'select_account',
  redirectUri:
    process.env.NEXT_PUBLIC_REDIRECT_URI ||
    (typeof window !== 'undefined'
      ? `${window.location.origin}/auth/callback`
      : 'http://localhost:3005/auth/callback'),
  extraQueryParameters: { response_mode: 'fragment' },
};

export default msalConfig;
