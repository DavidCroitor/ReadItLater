import { useState, useEffect } from 'react';
import * as articleService from '../services/articleService';

export default function useArticleDetail(articleId) {
  const [article, setArticle] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    // Only fetch if we have an article ID
    if (!articleId) return;
    
    const fetchArticle = async () => {
      setLoading(true);
      setError(null);
      
      try {
        console.log(`Fetching article with ID: ${articleId}`);
        const response = await articleService.getArticleById(articleId);
        console.log('Article data received:', response.data);
        setArticle(response.data);
      } catch (err) {
        console.error('Error fetching article:', err);
        setError(err.message || 'Failed to fetch article');
      } finally {
        setLoading(false);
      }
    };

    fetchArticle();
  }, [articleId]); // Re-fetch when article ID changes

  return { article, loading, error };
}