"use client";

import { MsalProvider } from "@azure/msal-react";
import { PublicClientApplication } from "@azure/msal-browser";
import msalConfig from "../lib/authConfig";

const msalInstance = new PublicClientApplication(msalConfig);

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