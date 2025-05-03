import { useState } from 'react';
import { useRouter } from 'next/router';
import Link from 'next/link';
import useProfiles from '../hooks/useProfiles';
import styles from '../styles/signIn.module.css';

export default function SignIn() {
    const router = useRouter();
    const { login, authLoading, authError } = useProfiles();
    const [formData, setFormData] = useState({
        email: '',
        password: ''
    });

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        const result = await login(formData.email, formData.password);
        if (result.success) {
            // Redirect to home page after successful login
            router.push('/');
        }
    };

    return (
        <div className={styles.signInContainer}>
            <header className={styles.containerHeader}>
                <h2>Sign In</h2>
            </header>
        
            <form className={styles.signInForm} onSubmit={handleSubmit}>
                <div className={styles.formGroup}>
                    <label htmlFor="email">Email or username</label>
                    <input 
                        type="text" 
                        id="email" 
                        name="email" 
                        value={formData.email}
                        onChange={handleChange}
                        placeholder='Email or Username'
                        required
                    />
                </div>
                <div className={styles.formGroup}>
                    <label htmlFor="password">Password</label>
                    <input
                        type="password"
                        id="password"
                        name="password" 
                        value={formData.password}
                        onChange={handleChange}
                        placeholder='Password'
                        required
                    />
                </div>
                
                {authError && (
                    <div className={styles.errorMessage}>
                        {authError}
                    </div>
                )}
                
                <div className={styles.formActions}>
                    <Link href="/forgotpassword" className={styles.forgotPassword}>
                        Forgot Password?
                    </Link>
                    <button type="submit" disabled={authLoading}>
                        {authLoading ? "Signing in..." : "Sign In"}
                    </button>
                </div>
                <p className={styles.signUpPrompt}>
                    Don't have an account? <Link href="/signup">Sign Up</Link>
                </p>
            </form>
        </div>
    );
}