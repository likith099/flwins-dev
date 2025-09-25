"use client";

import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from "@azure/msal-react";
import { SignInButton, SignOutButton } from "./AuthButtons";

export const ProfileContent = () => {
  const { accounts } = useMsal();
  const account = accounts[0];

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <div className="text-center">
          <h1 className="text-4xl font-bold text-gray-900 mb-2">FlWins</h1>
          <p className="text-gray-600">Azure AD Authentication Demo</p>
        </div>

        <AuthenticatedTemplate>
          <div className="bg-white rounded-lg shadow-xl p-8 space-y-6">
            <div className="text-center">
              <div className="w-20 h-20 bg-blue-500 rounded-full mx-auto mb-4 flex items-center justify-center">
                <span className="text-white text-2xl font-bold">
                  {account?.name?.charAt(0) || "U"}
                </span>
              </div>
              <h2 className="text-2xl font-bold text-gray-900 mb-2">
                Welcome back!
              </h2>
              <p className="text-gray-600 mb-1">
                <strong>Name:</strong> {account?.name || "Unknown"}
              </p>
              <p className="text-gray-600 mb-4">
                <strong>Email:</strong> {account?.username || "Unknown"}
              </p>
            </div>
            
            <div className="border-t pt-6">
              <div className="text-center">
                <SignOutButton />
              </div>
            </div>
          </div>
        </AuthenticatedTemplate>

        <UnauthenticatedTemplate>
          <div className="bg-white rounded-lg shadow-xl p-8 space-y-6">
            <div className="text-center">
              <div className="w-20 h-20 bg-gray-300 rounded-full mx-auto mb-4 flex items-center justify-center">
                <svg
                  className="w-10 h-10 text-gray-500"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
                  />
                </svg>
              </div>
              <h2 className="text-2xl font-bold text-gray-900 mb-2">
                Welcome to FlWins
              </h2>
              <p className="text-gray-600 mb-6">
                Please sign in with your Microsoft account to continue
              </p>
            </div>
            
            <div className="text-center">
              <SignInButton />
            </div>
            
            <div className="text-center text-sm text-gray-500">
              <p>Secure authentication powered by Microsoft Azure AD</p>
            </div>
          </div>
        </UnauthenticatedTemplate>
      </div>
    </div>
  );
};