"use client";

import { SignInButton } from '@/components/AuthButtons';
import { useMsal } from '@azure/msal-react';
import Link from 'next/link';

export default function Home() {
  const { accounts } = useMsal();
  const account = accounts[0];
  return (
    <main className="min-h-screen flex flex-col items-center justify-center gap-6">
      <h1 className="text-2xl font-bold">EFAMod Portal</h1>
      {!account ? (
        <SignInButton />
      ) : (
        <Link className="text-blue-600 underline" href="/profile">Go to Profile</Link>
      )}
    </main>
  );
}
