"use client";

import { MsalProvider } from "@azure/msal-react";
import { PublicClientApplication, EventType, EventMessage, AuthenticationResult } from "@azure/msal-browser";
import msalConfig from "../lib/authConfig";
import { useEffect } from "react";

const msalInstance = new PublicClientApplication(msalConfig);

// Initialize MSAL instance
if (!msalInstance.getActiveAccount() && msalInstance.getAllAccounts().length > 0) {
  msalInstance.setActiveAccount(msalInstance.getAllAccounts()[0]);
}

// Listen for sign-in event and set active account
msalInstance.addEventCallback((event: EventMessage) => {
  if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
    const payload = event.payload as AuthenticationResult;
    const account = payload.account;
    msalInstance.setActiveAccount(account);
    
    // Redirect to profile page after successful login
    if (typeof window !== 'undefined' && window.location.pathname === '/') {
      window.location.href = '/profile';
    }
  }
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