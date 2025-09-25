"use client";

import { useEffect } from 'react';
import { useMsal } from '@azure/msal-react';
import { useRouter } from 'next/navigation';

export default function AuthCallbackPage() {
  const { instance } = useMsal();
  const router = useRouter();

  useEffect(() => {
    const handleCallback = async () => {
      try {
        console.log("Handling auth callback...");
        
        // Handle the redirect response
        const response = await instance.handleRedirectPromise();
        
        if (response && response.account) {
          console.log("Authentication successful:", response);
          instance.setActiveAccount(response.account);
          
          // Redirect to profile page
          router.push('/profile');
        } else {
          console.log("No response or account, redirecting to home");
          router.push('/');
        }
      } catch (error) {
        console.error("Error handling auth callback:", error);
        router.push('/');
      }
    };

    handleCallback();
  }, [instance, router]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center">
      <div className="bg-white rounded-lg shadow-xl p-8 text-center">
        <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-blue-600 mx-auto mb-4"></div>
        <h2 className="text-xl font-semibold text-gray-900 mb-2">Signing you in...</h2>
        <p className="text-gray-600">Please wait while we complete your authentication.</p>
      </div>
    </div>
  );
}