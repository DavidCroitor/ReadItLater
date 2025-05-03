import { useState, useRef } from "react";
import styles from "../styles/categoryCard.module.css";

export default function CategoryCard({ category, onAddArticle, onDeleteCategory, onEditCategory }) {

    const [isMenuOpen, setIsMenuOpen] = useState(false);
    const [menuPosition, setMenuPosition] = useState('bottom');

    const cardRef = useRef(null);

    const handleMenuToggle = () => {
        console.log("Menu toggled");
        if(!isMenuOpen){
            const cardRect = cardRef.current.getBoundingClientRect();
            const viewportHeight = window.innerHeight;
            
            if (cardRect.bottom > viewportHeight - 100) {
                setMenuPosition('top');
            }else {
                setMenuPosition('bottom');
            }
        }
        setIsMenuOpen(!isMenuOpen);
    };

    return (
        <div className={styles.card} ref={cardRef}>
            <div className={styles.cardHeader}>
                <h3 className={styles.categoryName}>{category.name}</h3>
                <div className={`${styles.actions} ${isMenuOpen ? styles.menuOpen : ''}`}>
                        <button
                            onClick={handleMenuToggle}
                            className={styles.menuButton}
                            title="Menu">
                            ⋮
                        </button>
                        {isMenuOpen && (
                            <div className={`${styles.menu} ${menuPosition === 'top' ? styles.menuTop : styles.menuBottom}`}>
                                <button onClick={() => onAddArticle(category.id)} className={styles.menuItem}>Add Article</button>
                                <button onClick={() => onDeleteCategory(category.id)} className={styles.menuItem}>Delete Category</button>
                                <button onClick={() => onEditCategory(category.id)} className={styles.menuItem}>Edit Category</button>
                            </div>
                        )}
                    </div>
            </div>
        </div>
    );

}