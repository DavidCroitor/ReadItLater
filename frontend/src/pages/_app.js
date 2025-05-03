import '@/styles/globals.css';
import { createContext } from 'react';
import useProfiles from '@/hooks/useProfiles';

// Create profile context
export const ProfileContext = createContext();

export default function App({ Component, pageProps }) {
  const profileData = useProfiles();
  
  return (
    <ProfileContext.Provider value={profileData}>
      <Component {...pageProps} />
    </ProfileContext.Provider>
  );
}