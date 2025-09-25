import type { NextConfig } from 'next';

const nextConfig: NextConfig = {
  reactStrictMode: true,
  // Produce a self-contained server bundle at .next/standalone
  output: 'standalone',
};

export default nextConfig;
