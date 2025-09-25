import './globals.css';
import { MSALWrapper } from '@/components/MSALWrapper';

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body>
        <MSALWrapper>{children}</MSALWrapper>
      </body>
    </html>
  );
}
