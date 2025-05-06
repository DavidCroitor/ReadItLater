import { useRouter } from "next/router";
import styles from "../styles/sections.module.css";

export default function ProfileSection({
  isAuthenticated,
  user,
  logout,
  profileLoading,
}) {
  const router = useRouter();

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

      {isAuthenticated && user ? (
        <div className={styles.profileInfo}>
          <h3>{user.username}</h3>
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