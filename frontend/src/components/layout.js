import '@/styles/globals.css';

export const metadata = {
  title: "Read It Later App",
};

export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body>
        {children}
      </body>
    </html>
  );
}
