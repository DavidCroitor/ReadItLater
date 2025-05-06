import api from "./api";

export const getArticles = () => api.get('/articles/all');
export const getArticleById = (id) => api.get(`/articles/${id}`);
export const createArticle = (articleData) => api.post('/articles', articleData);
export const updateArticle = (id, articleData) => api.put(`/articles/${id}`, articleData);
export const deleteArticle = (id) => api.delete(`/articles/${id}`);

