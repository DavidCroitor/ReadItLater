import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json'
    },
    maxRedirects: 0,
});

console.log(`API configured with base URL: ${API_BASE_URL}`);

api.interceptors.request.use(
    (config) => {

        console.log(`Making ${config.method.toUpperCase()} request to: ${config.baseURL}${config.url}`);
    
        const token = localStorage.getItem('token');
        if (token) {
            config.headers['Authorization'] = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

api.interceptors.response.use(
    (response) => response,
    (error) => {
        console.error('API request failed:', {
            url: error.config?.url,
            method: error.config?.method,
            status: error.response?.status,
            data: error.response?.data,
            message: error.message
          });
      return Promise.reject(error);
    }
);

export default api;