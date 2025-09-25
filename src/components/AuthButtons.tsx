"use client";

import { useMsal } from "@azure/msal-react";
import { loginRequest } from "../lib/authConfig";
import { useState } from "react";

export const SignInButton = () => {
  const { instance } = useMsal();
  const [isLoading, setIsLoading] = useState(false);

  const handleLogin = async () => {
    setIsLoading(true);
    try {
      // Use redirect instead of popup for better user flow experience
      const response = await instance.loginPopup(loginRequest);
      console.log("Login successful:", response);
    } catch (error) {
      console.error("Login failed:", error);
      // Handle specific error cases
      if (error instanceof Error) {
        if (error.message.includes("popup_window_error")) {
          // Fallback to redirect if popup fails
          await instance.loginRedirect(loginRequest);
        }
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <button
      onClick={handleLogin}
      disabled={isLoading}
      className={`${
        isLoading 
          ? 'bg-gray-400 cursor-not-allowed' 
          : 'bg-blue-600 hover:bg-blue-700 transform hover:scale-105'
      } text-white font-semibold py-3 px-6 rounded-lg shadow-md transition duration-300 ease-in-out focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-75 flex items-center justify-center min-w-[200px]`}
    >
      {isLoading ? (
        <>
          <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          Signing in...
        </>
      ) : (
        'Sign in with Microsoft'
      )}
    </button>
  );
};

export const SignOutButton = () => {
  const { instance } = useMsal();
  const [isLoading, setIsLoading] = useState(false);

  const handleLogout = async () => {
    setIsLoading(true);
    try {
      await instance.logoutPopup({
        postLogoutRedirectUri: window.location.origin,
        mainWindowRedirectUri: window.location.origin
      });
    } catch (error) {
      console.error("Logout failed:", error);
      // Fallback to redirect logout
      await instance.logoutRedirect({
        postLogoutRedirectUri: window.location.origin,
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <button
      onClick={handleLogout}
      disabled={isLoading}
      className={`${
        isLoading 
          ? 'bg-gray-400 cursor-not-allowed' 
          : 'bg-red-600 hover:bg-red-700 transform hover:scale-105'
      } text-white font-semibold py-2 px-4 rounded-lg shadow-md transition duration-300 ease-in-out focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-opacity-75 flex items-center justify-center min-w-[120px]`}
    >
      {isLoading ? (
        <>
          <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          Signing out...
        </>
      ) : (
        'Sign Out'
      )}
    </button>
  );
};