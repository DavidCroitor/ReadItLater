import CategoryCard from "./categoryCard";
import styles from "../styles/sections.module.css";

export default function CategorySection({ 
  categories, 
  isAddCategoryDialogOpen,
  toggleAddCategoryDialog,
  handleAddCategory,
  isCreatingCategory,
  newCategoryName,
  setNewCategoryName,
  handleCategoryNameSubmit,
  handleCategoryNameCancel,
  editingCategoryId,
  editCategoryName,
  setEditCategoryName,
  handleEditCategorySubmit,
  handleEditCategoryCancel,
  handleAddInlineCategory,
  handleDeleteCategory,
  handleEditCategory,
  handleAddToCategory,
  isAuthenticated,
  user,
}) {
  return (
    <div className={styles.categoryContainer}>
      <header className={styles.containerHeader}>
        <h2>Categories</h2>
      </header>


      <div className={styles.addCategory}>
        {isAuthenticated && user ? (
          <div className={styles.addCategory}>
          <button className={styles.addCategoryButton}
            onClick={handleAddInlineCategory}>+</button>
            Add Category
          </div>
          ) : (
            <p className={styles.authPrompt}>Please sign in to manage categories</p>
        )}
      </div>

<div className={styles.categoryList}>
  {isAuthenticated && user ? (
    <>
      {categories.map((category) => (
        editingCategoryId === category.id ? (
          <div key={category.id} className={styles.newCategoryCard}>
            <form onSubmit={handleEditCategorySubmit}>
              <input
                type="text"
                value={editCategoryName}
                onChange={(e) => setEditCategoryName(e.target.value)}
                placeholder="Category name"
                autoFocus
                className={styles.newCategoryInput}
              />
              <div className={styles.newCategoryActions}>
                <button type="submit" className={styles.saveButton}>Save</button>
                <button type="button" onClick={handleEditCategoryCancel} className={styles.cancelButton}>Cancel</button>
              </div>
            </form>
          </div>
        ) : (
          <CategoryCard
            key={category.id}
            category={category}
            onAddArticle={handleAddToCategory}
            onDeleteCategory={handleDeleteCategory}
            onEditCategory={handleEditCategory}
          />
        )
      ))}
      {isCreatingCategory && (
        <div className={styles.newCategoryCard}>
          <form onSubmit={handleCategoryNameSubmit}>
            <input
              type="text"
              value={newCategoryName}
              onChange={(e) => setNewCategoryName(e.target.value)}
              placeholder="Category name"
              autoFocus
              className={styles.newCategoryInput}
            />
            <div className={styles.newCategoryActions}>
              <button type="submit" className={styles.saveButton}>Save</button>
              <button type="button" onClick={handleCategoryNameCancel} className={styles.cancelButton}>Cancel</button>
            </div>
          </form>
        </div>
      )}
    </>
    ) : (
      <p className={styles.unauthenticatedMessage}>Please sign in to view your categories</p>
    )}
  </div>
    </div>
  );
}