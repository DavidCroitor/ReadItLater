import styles from '../styles/forgotPassword.module.css';

export default function ForgtPassword() {

    return (
        <div className={styles.signInContainer}>
        <header className={styles.containerHeader}>
            <h2>Forgot password</h2>
        </header>
    
        <form className={styles.signInForm}>
            <div className={styles.formGroup}>
                <label htmlFor="email">Email</label>
                <input 
                    type="email" 
                    id="email" 
                    name="email" 
                    placeholder='Email'
                    required/>
            </div>
            <div className={styles.formActions}>
                <button type="submit">Send email</button>
            </div>
            <p className={styles.signUpPrompt}>Go back to <a href="/signin">Sign In</a></p>
        </form>
        </div>
    );

}