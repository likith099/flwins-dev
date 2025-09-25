"use client";

import { useMsal } from '@azure/msal-react';
import { loginRequest } from '@/lib/authConfig';

export function SignInButton() {
  const { instance } = useMsal();
  return (
    <button className="bg-blue-600 text-white px-4 py-2 rounded" onClick={() => instance.loginRedirect(loginRequest)}>
      Sign in with Microsoft
    </button>
  );
}

export function SignOutButton() {
  const { instance } = useMsal();
  return (
    <button
      className="bg-red-600 text-white px-4 py-2 rounded"
      onClick={() => instance.logoutRedirect({ postLogoutRedirectUri: window.location.origin })}
    >
      Sign Out
    </button>
  );
}
