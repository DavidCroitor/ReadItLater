import ArticleCard from "./articleCard";
import AddArticleDialog from "./addArticleDialog";
import styles from "../styles/sections.module.css";

export default function ArticleSection({
  articles,
  isAddArticleDialogOpen,
  toggleAddArticleDialog,
  handleAddArticle,
  handleDeleteArticle,
  handleAddToCategory,
  handleRead,
  handleFavorite,
  handleCardClick,
  isAuthenticated,
  user,
}) {

  return (
    <div className={styles.articleContainer}>
      <header className={styles.containerHeader}>
        <h2>Articles</h2>
        
      </header>

      <div className={styles.addArticle}>
        {isAuthenticated && user ? (
          <div className={styles.addArticle}>
            <button 
              onClick={toggleAddArticleDialog}
              className={styles.addArticleButton}
            >+</button>
            <p>Add Article</p>
          </div>
        ) : (
          <p className={styles.authPrompt}>Please sign in to manage articles</p>
        )}
      </div>
      
      <AddArticleDialog 
        isOpen={isAddArticleDialogOpen} 
        onClose={toggleAddArticleDialog}
        onAdd={(articleData) => {
          handleAddArticle(articleData);
          toggleAddArticleDialog();
        }}
      />

      <div className={styles.articleList}>
        {isAuthenticated && user ? (
          articles.length > 0 ? (
            articles.map((article) => (
              <ArticleCard
                key={article.articleId}
                article={article}
                onDelete={handleDeleteArticle}
                onAddToCategory={handleAddToCategory}
                onCardClick={handleCardClick}
                onRead={handleRead}
                onFavorite={handleFavorite}
              />
            ))
          ) : (
            <p className={styles.emptyState}>No articles saved yet.</p>
          )
        ) : (
          <div className={styles.unauthenticatedMessage}>
            <p>Sign in to view your saved articles</p>
          </div>
        )}
      </div>
    </div>
  );
}