import styles from '../styles/signUp.module.css';
import { useState } from 'react';
import { useRouter } from 'next/router';
import useProfiles from '@/hooks/useProfiles';

export default function SignUp() {
    const { register, authLoading, authError } = useProfiles();
    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '',
        confirmPassword: ''
    });
    const [formError, setFormError] = useState('');

    const router = useRouter();

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!validateForm()) return;

        const { success, error } = await register(formData);
        if (success) {
            router.push('/signin');
        } else {
            setFormError(error);
        }
    }

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    const validateForm = () => {
        setFormError('');

        if (!formData.username || !formData.email || !formData.password) {
            setFormError('Please fill in all required fields');
            return false;
        }

        if (formData.password !== formData.confirmPassword) {
            setFormError('Passwords do not match');
            return false;
        }

        if (formData.password.length < 6) {
            setFormError('Password must be at least 6 characters long');
            return false;
        }

        return true;
    };

    return (
        <div className={styles.signInContainer}>
            <header className={styles.containerHeader}>
                <h2>Sign Up</h2>
            </header>
        
            <form className={styles.signInForm}
                onSubmit={handleSubmit}
            >
                <div className={styles.formGroup}>
                    <label htmlFor="email">Email</label>
                    <input 
                        type="email" 
                        id="email" 
                        name="email" 
                        placeholder='Email'
                        onChange={handleChange}
                        required/>
                </div>
                <div className={styles.formGroup}>
                    <label htmlFor="username">Username</label>
                    <input 
                        type="username" 
                        id="username" 
                        name="username" 
                        placeholder='Username'
                        onChange={handleChange}
                        required/>
                </div>
                <div className={styles.formGroup}>
                    <label htmlFor="password">Password</label>
                    <input
                        type="password"
                        id="password"
                        name="password" 
                        placeholder='Password'
                        onChange={handleChange}
                        required/>
                </div><div className={styles.formGroup}>
                    <label htmlFor="password">Confirm Password</label>
                    <input
                        type="password"
                        id="confirmPassword"
                        name="confirmPassword" 
                        placeholder='Confirm Password'
                        onChange={handleChange}
                        required/>
                </div>

                {(formError || authError) && (
                    <div className={styles.errorMessage}>
                        {formError || authError}
                    </div>
                )}

                <div className={styles.formActions}>
                    <button type="submit" disabled={authLoading}>
                        {authLoading ? "Creating Account..." : "Sign Up"}
                    </button>
                </div>
                <p className={styles.signUpPrompt}>
                    Already have an account? <a href="/signin">Sign In</a>
                </p>
            </form>
        </div>
    );

}