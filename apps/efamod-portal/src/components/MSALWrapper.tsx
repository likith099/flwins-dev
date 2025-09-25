"use client";

import { MsalProvider } from '@azure/msal-react';
import { PublicClientApplication } from '@azure/msal-browser';
import msalConfig from '@/lib/authConfig';
import { ReactNode } from 'react';

const instance = new PublicClientApplication(msalConfig);
instance.initialize().then(() => {
  const accounts = instance.getAllAccounts();
  if (accounts.length > 0) instance.setActiveAccount(accounts[0]);
});

export function MSALWrapper({ children }: { children: ReactNode }) {
  return <MsalProvider instance={instance}>{children}</MsalProvider>;
}
