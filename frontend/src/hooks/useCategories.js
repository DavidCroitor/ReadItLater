import { useState, useEffect } from "react";

export default function useCategories() {
    // Mock category data
    const [categories, setCategories] = useState([
    { id: 1, name: "Frontend" },
    { id: 2, name: "Backend" },
    { id: 3, name: "Design" },
    ]);

    const [isCreatingCategory, setIsCreatingCategory] = useState(false);
    const [newCategoryName, setNewCategoryName] = useState("");
    const [editingCategoryId, setEditingCategoryId] = useState(null);
    const [editCategoryName, setEditCategoryName] = useState("");

    const handleAddCategory = (newCategory) => {
        console.log(`Adding new category: ${JSON.stringify(newCategory)}`);
        // Update state to add new category
        setCategories([...categories, { ...newCategory, id: categories.length + 1 }]);
        setIsAddCategoryDialogOpen(false);
      }
    
      const handleDeleteCategory = (id) => {
        console.log(`Deleting category with id: ${id}`);
        // Update state to remove category
        setCategories(categories.filter(category => category.id !== id));
      }
    
      const handleAddInlineCategory = () => {
        setIsCreatingCategory(true);
      };
      
      const handleCategoryNameSubmit = (e) => {
        e.preventDefault();
        if (newCategoryName.trim()) {
          const newCategory = {
            id: categories.length + 1,
            name: newCategoryName.trim()
          };
          setCategories([...categories, newCategory]);
          setNewCategoryName("");
          setIsCreatingCategory(false);
        }
      };
      
      const handleCategoryNameCancel = () => {
        setNewCategoryName("");
        setIsCreatingCategory(false);
      };
    
      const handleEditCategory = (id) => {
        const categoryToEdit = categories.find(category => category.id === id);
        if (categoryToEdit) {
          setEditingCategoryId(id);
          setEditCategoryName(categoryToEdit.name);
        }
      }
      const handleEditCategorySubmit = (e) => {
        e.preventDefault();
        if (editCategoryName.trim()) {
          setCategories(categories.map(category => 
            category.id === editingCategoryId 
              ? { ...category, name: editCategoryName.trim() } 
              : category
          ));
          setEditingCategoryId(null);
          setEditCategoryName("");
        }
      };
      
      const handleEditCategoryCancel = () => {
        setEditingCategoryId(null);
        setEditCategoryName("");
      };

    return {
        categories,
        isCreatingCategory,
        newCategoryName,
        setNewCategoryName,
        editingCategoryId,
        editCategoryName,
        setEditCategoryName,
        handleAddCategory,
        handleDeleteCategory,
        handleAddInlineCategory,
        handleCategoryNameSubmit,
        handleCategoryNameCancel,
        handleEditCategory,
        handleEditCategorySubmit,
        handleEditCategoryCancel
    };

}
