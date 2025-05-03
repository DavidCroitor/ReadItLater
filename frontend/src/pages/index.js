import { useState } from "react";
import useArticles from "@/hooks/useArticles";
import useCategories from "@/hooks/useCategories";
import CategorySection from "@/components/categorySection";
import ArticleSection from "@/components/articleSection";
import ProfileSection from "@/components/profileSection";

export default function Home() {
  const [isAddArticleDialogOpen, setIsAddArticleDialogOpen] = useState(false);
  const [isAddCategoryDialogOpen, setIsAddCategoryDialogOpen] = useState(false);
  
  const {
    articles,
    handleDeleteArticle,
    handleAddToCategory,
    handleRead,
    handleFavorite,
    handleAddArticle,
    handleCardClick
  } = useArticles();

  const {
    categories,
    isCreatingCategory,
    newCategoryName,
    setNewCategoryName,
    editingCategoryId,
    editCategoryName,
    setEditCategoryName,
    handleDeleteCategory,
    handleAddCategory,
    handleAddInlineCategory,
    handleCategoryNameSubmit,
    handleCategoryNameCancel,
    handleEditCategory,
    handleEditCategorySubmit,
    handleEditCategoryCancel
  } = useCategories();

  const toggleAddArticleDialog = () => {
    setIsAddArticleDialogOpen(!isAddArticleDialogOpen);
  };

  const toggleAddCategoryDialog = () => {
    setIsAddCategoryDialogOpen(!isAddCategoryDialogOpen);
  };

  return (
    <div>
      <div className="containerWrapper">
        <CategorySection  className="categoryContainer"
          categories={categories}
          isAddCategoryDialogOpen={isAddCategoryDialogOpen}
          toggleAddCategoryDialog={toggleAddCategoryDialog}
          handleAddCategory={handleAddCategory}
          isCreatingCategory={isCreatingCategory}
          newCategoryName={newCategoryName}
          setNewCategoryName={setNewCategoryName}
          handleCategoryNameSubmit={handleCategoryNameSubmit}
          handleCategoryNameCancel={handleCategoryNameCancel}
          editingCategoryId={editingCategoryId}
          editCategoryName={editCategoryName}
          setEditCategoryName={setEditCategoryName}
          handleEditCategorySubmit={handleEditCategorySubmit}
          handleEditCategoryCancel={handleEditCategoryCancel}
          handleAddInlineCategory={handleAddInlineCategory}
          handleDeleteCategory={handleDeleteCategory}
          handleEditCategory={handleEditCategory}
          handleAddToCategory={handleAddToCategory}
        />
        
        <ArticleSection className="articleContainer"
          articles={articles}
          isAddArticleDialogOpen={isAddArticleDialogOpen}
          toggleAddArticleDialog={toggleAddArticleDialog}
          handleAddArticle={handleAddArticle}
          handleDeleteArticle={handleDeleteArticle}
          handleAddToCategory={handleAddToCategory}
          handleRead={handleRead}
          handleFavorite={handleFavorite}
          handleCardClick={handleCardClick}
        />
        
        <ProfileSection className="profileContainer"
         />
      </div>
    </div>
  );
}