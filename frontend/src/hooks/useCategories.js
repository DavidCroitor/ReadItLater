import { useState, useEffect } from "react";
import useProfiles from "./useProfiles";

const MOCK_CATEGORIES = [
    { id: 1, userId:"user1", name: "Frontend" },
    { id: 2, userId:"user1", name: "Backend" },
    { id: 3, userId:"user2", name: "Design" },
];

export default function useCategories() {
    const [isCreatingCategory, setIsCreatingCategory] = useState(false);
    const [newCategoryName, setNewCategoryName] = useState("");
    const [editingCategoryId, setEditingCategoryId] = useState(null);
    const [editCategoryName, setEditCategoryName] = useState("");

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const [categories, setCategories] = useState([]);

    const { user, isAuthenticated } = useProfiles();

    useEffect(() => {
        fetchCategories();
    }, [user])

    const fetchCategories = async () => {
        setLoading(true);
        setError(null);

        try {
          // Filter articles by user ID if authenticated
          let userCategories = [];
          
          if (isAuthenticated && user?.id) {
            console.log("Fetching categories for user:", user.id);
            userCategories = MOCK_CATEGORIES.filter(article => article.userId === user.id);
          } else {
            // If not authenticated, return empty array
            userCategories = [];
          }
          
          setCategories(userCategories);
        } catch (err) {
          console.error("Error fetching articles:", err);
          setError("Failed to load articles. Please try again later.");
        } finally {
          setLoading(false);
        }
    };

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
        setIsCreatingCategory(!isCreatingCategory);
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
