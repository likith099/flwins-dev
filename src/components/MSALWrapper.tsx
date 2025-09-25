"use client";

import { MsalProvider } from "@azure/msal-react";
import { PublicClientApplication } from "@azure/msal-browser";
import msalConfig from "../lib/authConfig";
import { useEffect } from "react";

const msalInstance = new PublicClientApplication(msalConfig);

// Handle redirect promises on app initialization
msalInstance.initialize().then(() => {
  // Handle redirect promise which returns the AuthenticationResult object
  return msalInstance.handleRedirectPromise();
}).then((response) => {
  if (response !== null) {
    // Handle successful authentication response
    console.log("Authentication successful:", response);
  }
}).catch((error) => {
  console.error("Authentication error:", error);
});

interface MSALWrapperProps {
  children: React.ReactNode;
}

export const MSALWrapper: React.FC<MSALWrapperProps> = ({ children }) => {
  return (
    <MsalProvider instance={msalInstance}>
      {children}
    </MsalProvider>
  );
};