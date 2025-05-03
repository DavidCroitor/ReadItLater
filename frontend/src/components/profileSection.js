import { useRouter } from "next/router";
import useProfiles from "@/hooks/useProfiles";
import styles from "@/styles/profileSection.module.css";

export default function ProfileSection() {
  const router = useRouter();
  const { user, profile, isAuthenticated, logout, profileLoading } = useProfiles();

  const handleSignIn = () => {
    router.push("/signin");
  };

  const handleLogout = () => {
    logout();
  };

  return (
    <div className={styles.profileContainer}>
      <header className={styles.containerHeader}>
        <h2>Profile</h2>
      </header>

      {isAuthenticated ? (
        <div className={styles.profileInfo}>
          <h3>{user.username}</h3>
          {profile?.bio && <p className={styles.bio}>{profile.bio}</p>}
          <p className={styles.email}>{user.email}</p>
          <button 
            className={styles.logoutButton} 
            onClick={handleLogout}
          >
            Sign Out
          </button>
        </div>
      ) : (
        <div className={styles.unauthenticatedMessage}>
          <p>Sign in to view and manage your profile</p>
          <button 
            className={styles.signInButton} 
            onClick={handleSignIn}
          >
            Sign In
          </button>
        </div>
      )}
    </div>
  );
}