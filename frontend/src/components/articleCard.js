import { useState, useRef } from "react";
import styles from "../styles/articleCard.module.css";

export default function ArticleCard({ 
    article, 
    onCardClick, 
    onAddToCategory, 
    onRead, 
    onFavorite, 
    onDelete 
})
{
    const [isMenuOpen, setIsMenuOpen] = useState(false);
    const [menuPosition, setMenuPosition] = useState('bottom');

    const fotmattedDate = new Date(article.savedAt).toLocaleDateString("en-UK", {
        year: "numeric",
        month: "long",
        day: "numeric",
    });

    const handleMenuToggle = (e) => {
        e.stopPropagation(); // Prevent the click event from bubbling up to the card

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

    const handleAddToCategory = (e) => {
        e.stopPropagation(); 
        setIsMenuOpen(false); 
        onAddToCategory(article.articleId);
    };

    const handleMarkAsRead = (e) => {
        e.stopPropagation(); 
        setIsMenuOpen(false);
        onRead(article.articleId);
    };

    const handleMarkAsFavorite = (e) => {
        e.stopPropagation();
        setIsMenuOpen(false);
        onFavorite(article.articleId);
    };

    const handleDelete = (e) => {
        e.stopPropagation();
        setIsMenuOpen(false);
        onDelete(article.articleId);
    };

    const cardRef = useRef(null);

    return(
        <div className={styles.card} ref={cardRef} onClick={() => onCardClick(article.articleId)}>
            <div className={styles.cardHeader}>
                <div className={styles.cardMetadata}>
                    <h3 className={styles.title} title ={article.title}>
                        {article.title || "Untitled"}
                    </h3>
                    <p className={styles.url} title={article.url}> {article.url} </p>
                    <div className={styles.indicators}>
                        {article.isRead && <span className={styles.readIndicator} title="Read">✓</span>}
                        {article.isFavourite && <span className={styles.favoriteIndicator} title="Favorite">⭐</span>}
                    </div>
                </div>
                    <p className={styles.date}>Saved on: {fotmattedDate}</p>
                    <div className={`${styles.actions} ${isMenuOpen ? styles.menuOpen : ''}`}>
                        <button
                            onClick={handleMenuToggle}
                            className={styles.menuButton}
                            title="Menu">
                            ⋮
                        </button>
                        {isMenuOpen && (
                            <div className={`${styles.menu} ${menuPosition === 'top' ? styles.menuTop : styles.menuBottom}`}>
                                <button onClick={handleAddToCategory} className={styles.menuItem}>Add to Category</button>
                                <button onClick={handleMarkAsRead} className={styles.menuItem}>Mark as Read</button>
                                <button onClick={handleMarkAsFavorite} className={styles.menuItem}>Mark as Favorite</button>
                                <button onClick={handleDelete} className={styles.menuItem}>Delete</button>
                            
                            </div>
                        )}
                    </div>
                
            </div>    
            <p className={styles.content} title={article.content}>
                {article.content 
                    ? `${article.content.substring(0, 100)}${article.content.length > 100 ? '...' : ''}`
                    : "Content not available"}
            </p>

        </div>
    );
}
