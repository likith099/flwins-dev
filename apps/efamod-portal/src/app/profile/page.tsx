"use client";

import { useMsal } from '@azure/msal-react';
import { postForm } from '@/lib/postForm';

export default function Profile() {
  const { accounts, instance } = useMsal();
  const account = accounts[0];

  const handleSr = async () => {
    if (!account) return;
    try {
      const result = await instance.acquireTokenSilent({
        scopes: ['openid', 'profile', 'email'],
        account,
      });
      const idToken = result.idToken;
      const baseUrl = process.env.NEXT_PUBLIC_EFSMOD_URL || 'http://localhost:4010';
      postForm(`${baseUrl}/sso/entra`, {
        id_token: idToken,
        return: '/profile',
      });
    } catch (e) {
      console.error('Failed to get id_token:', e);
    }
  };

  if (!account) return <p className="p-6">Please sign in first.</p>;
  const claims = account.idTokenClaims as any;
  return (
    <main className="p-6 space-y-4">
      <h1 className="text-2xl font-bold">Profile</h1>
      <div className="space-y-1">
        <div>Name: {claims?.name || account.name}</div>
        <div>Email: {claims?.email || claims?.preferred_username || account.username}</div>
        <div>User ID: {claims?.oid || claims?.sub}</div>
      </div>
      <button className="bg-green-600 text-white px-4 py-2 rounded" onClick={handleSr}>
        SR Application Form
      </button>
    </main>
  );
}
