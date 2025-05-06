import { useState, useEffect, use } from 'react';
import * as authService from '../services/authService'; // Import your auth service

// Mock user data
const MOCK_USERS = {
  'user1': {
    id: 'user1',
    username: 'johndoe',
    email: 'john@example.com',
    password: 'password123', 
    savedArticles: [1, 2]
  },
  'user2': {
    id: 'user2',
    username: 'janesmith',
    email: 'jane@example.com',
    password: 'password456',
    name: 'Jane Smith',
    bio: 'Frontend developer and UX enthusiast',
    profilePicture: 'https://randomuser.me/api/portraits/women/1.jpg',
    savedArticles: [3]
  }
};

export default function useProfiles() {
  // Authentication state
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(null);
  const [authLoading, setAuthLoading] = useState(false);
  const [authError, setAuthError] = useState(null);
  
  // Profile state
  const [profileLoading, setProfileLoading] = useState(false);
  const [profileError, setProfileError] = useState(null);

  useEffect(() => {
    const checkAuth = async () => {
      const storedToken = localStorage.getItem('token');
      if(storedToken) {
        setToken(storedToken);
        await fetchProfile();
      }
    };

    checkAuth();
  }, []);

  useEffect(() => {
    // Only fetch profile if we have a token
    if (token) {
      console.log('Token available, fetching profile...');
      fetchProfile();
    }
  }, [token]);

  useEffect(() => {
    console.log('User state updated:', user);
  }, [user]);

  const fetchProfile = async () => {
    if(!token) return;

    setProfileLoading(true);
    setProfileError(null);

    try {
      const response = await authService.getUserProfile();
      setUser(response.data); // Update user state with profile data

    } catch (error) {
      setProfileError(error.response.data.message || 'Failed to fetch profile');
      
      if(error.response?.status === 401) {
        logout(); 
      }

      return { success: false, error: profileError };
    
    } finally {
      setProfileLoading(false);
    }
  };

  const login = async (loginIdentifier, password) => {
    setAuthLoading(true);
    setAuthError(null);

    try{
      const response = await authService.login({ loginIdentifier, password });
      const { accessToken, refreshToken, userDto, accessTokenExpiration } = response.data;
      
      setToken(accessToken);
      setUser(userDto);
      
      localStorage.setItem('token', accessToken);
      localStorage.setItem('refreshToken', refreshToken);
      localStorage.setItem('accessTokenExpiration', accessTokenExpiration);

      return { success: true};
    }
    catch (error) {
      setAuthError(error.response.data.message || 'Login failed');
      return { success: false, error: authError };
    } finally {
      setAuthLoading(false);
    }
  }

  const logout = () => {
    setUser(null);
    setToken(null);
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('accessTokenExpiration');
  };

  const register = async (userData) => {
    setAuthLoading(true);
    setAuthError(null);

    try{

      const response = await authService.register({
        email: userData.email,
        username: userData.username,
        password: userData.password,
        confirmPassword: userData.confirmPassword,
      });
      const { accessToken, refreshToken, userDto } = response.data;
      
      if (!response.data.accessToken) {
        // Automatically login after successful registration
        return await login(userData.email, userData.password);
      }

      setToken(accessToken);
      setUser(userDto);
      
      localStorage.setItem('token', accessToken);
      localStorage.setItem('refreshToken', refreshToken);

      return { success: true };
    }
    catch (error) {
      console.error('Registration error:', error);
      
      // Get a more specific error message
      const errorMessage = 
        error.response?.data?.message || 
        error.response?.data?.error || 
        error.message || 
        'Registration failed';
        
      setAuthError(errorMessage);
      return { success: false, error: authError };
    } finally {
      setAuthLoading(false);
    }
  }

  const updateProfile = async (userData) => {
    if(!token) return{ success: false, error: 'Not authenticated' };

    setProfileLoading(true);
    setProfileError(null);

    try {
      const response = await authService.updateUserProfile(userData);
      setUser(response.data); // Update user state with new profile data
    } catch (error) {
      setProfileError(error.response.data.message || 'Failed to update profile');
      return { success: false, error: profileError };
    } finally {
      setProfileLoading(false);
    }
  }



  return {
    // Auth state and methods
    user,
    token,
    isAuthenticated: !!token,
    authLoading,
    authError,
    login,
    logout,
    register,
    
    // Profile state and methods
    profileLoading,
    profileError,
    fetchProfile,
    updateProfile,
  };
}