"use client";

import { useMsal } from '@azure/msal-react';
import { useEffect } from 'react';
import { useRouter } from 'next/navigation';

export default function Callback() {
  const { instance } = useMsal();
  const router = useRouter();

  useEffect(() => {
    instance
      .handleRedirectPromise()
      .then((resp) => {
        if (resp) instance.setActiveAccount(resp.account);
        router.replace('/profile');
      })
      .catch((e) => {
        console.error('MSAL redirect handling failed', e);
        router.replace('/');
      });
  }, [instance, router]);

  return <p className="p-6">Processing sign-in…</p>;
}
