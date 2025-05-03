import { useState } from "react";
import styles from '../styles/addCategoryDialog.module.css';

export default function AddCategoryDialog({ isOpen, onAdd, onClose }) {

    const [newCategory, setNewCategory] = useState({ name: ''});

    const handleSubmit = (e) => {
        e.preventDefault();

        onAdd({
            ...newCategory,
            savedAt: new Date().toISOString(),
            isRead: false,
            isFavorite: false,
        })

        setNewCategory({ name: ''});
    }

    if (!isOpen) return null;

    return(
        <div className={styles.dialogOverlay}>
            <div className={styles.dialogContent}>
                <h2>Add New Category</h2>
                <form onSubmit={handleSubmit}>
                    <div className={styles.formGroup}>
                        <label htmlFor="title">Name</label>
                        <input
                            type="text"
                            id="name"
                            value={newCategory.name}
                            onChange={(e) => setNewCategory({ ...newCategory, name: e.target.value })}
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