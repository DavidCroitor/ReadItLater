import { useState } from "react";
import styles from '../styles/addArticleDialog.module.css';

export default function AddArticleDialog({ isOpen, onAdd, onClose }) {

    const [newArticle, setNewArticle] = useState({ url: '' });

    const handleSubmit = (e) => {
        e.preventDefault();

        onAdd({
            ...newArticle,
            savedAt: new Date().toISOString(),
            isRead: false,
            isFavorite: false,
        })

        setNewArticle({ url: '' });
    }

    if (!isOpen) return null;

    return(
        <div className={styles.dialogOverlay}>
            <div className={styles.dialogContent}>
                <h2>Add New Article</h2>
                <form onSubmit={handleSubmit}>
                    <div className={styles.formGroup}>
                        <label htmlFor="url">URL</label>
                        <input
                            type="url"
                            id="url"
                            value={newArticle.url}
                            onChange={(e) => setNewArticle({ ...newArticle, url: e.target.value })}
                            required
                        />
                    </div>
                    <div className={styles.dialogActions}>
                        <button type="submit">Add Article</button>
                        <button type="button" onClick={onClose}>Cancel</button>
                    </div>
                </form>
            </div>

        </div>
    )

}