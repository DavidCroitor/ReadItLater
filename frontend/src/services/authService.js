import api from "./api";

export const login = (credentials) => api.post('/account/login', credentials);
export const register = (userData) => api.post('/account/register', userData);
export const getUserProfile = () => api.get('/profile/me');
export const updateUserProfile = (userData) => api.put('/profile/update', userData);

