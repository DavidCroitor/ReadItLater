import { useState, useEffect } from 'react';

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
  const [profile, setProfile] = useState(null);
  const [profileLoading, setProfileLoading] = useState(false);
  const [profileError, setProfileError] = useState(null);

  // Check if there's a saved token in localStorage on initial load
  useEffect(() => {
    const savedToken = localStorage.getItem('auth_token');
    const savedUser = localStorage.getItem('auth_user');
    
    if (savedToken && savedUser) {
      setToken(savedToken);
      setUser(JSON.parse(savedUser));
    }
  }, []);

  // Fetch profile whenever user changes
  useEffect(() => {
    if (user) {
      fetchProfile();
    } else {
      setProfile(null);
    }
  }, [user]);

  // Login function
  const login = async (identifier, password) => {
    setAuthLoading(true);
    setAuthError(null);
    
    // Simulate network delay
    await new Promise(resolve => setTimeout(resolve, 800));
    
    try {
      // Find user by email
      const foundUser = Object.values(MOCK_USERS).find(
        u => u.email === identifier || u.username === identifier
      );
      
      if (!foundUser || foundUser.password !== password) {
        throw new Error('Invalid email or password');
      }
      
      // Create mock token
      const mockToken = `mock-token-${foundUser.id}-${Date.now()}`;
      
      // Store in localStorage to persist login
      localStorage.setItem('auth_token', mockToken);
      localStorage.setItem('auth_user', JSON.stringify({
        id: foundUser.id,
        email: foundUser.email,
        username: foundUser.username,
        name: foundUser.name
      }));
      
      // Update state
      setToken(mockToken);
      setUser({
        id: foundUser.id,
        email: foundUser.email,
        username: foundUser.username,
        name: foundUser.name
      });
      
      return { success: true };
    } catch (error) {
      setAuthError(error.message);
      return { success: false, message: error.message };
    } finally {
      setAuthLoading(false);
    }
  };
  
  // Logout function
  const logout = () => {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('auth_user');
    setToken(null);
    setUser(null);
    setProfile(null);
    return { success: true };
  };
  
  // Register function
  const register = async (username, email, password) => {
    setAuthLoading(true);
    setAuthError(null);
    
    // Simulate network delay
    await new Promise(resolve => setTimeout(resolve, 800));
    
    try {
      // Check if email or username already exists
      const userExists = Object.values(MOCK_USERS).some(
        u => u.email === email || u.username === username
      );
      
      if (userExists) {
        throw new Error('User with this email or username already exists');
      }
      
      // Create new user
      const newUserId = `user${Object.keys(MOCK_USERS).length + 1}`;
      const newUser = {
        id: newUserId,
        username,
        email,
        password,
        name: username,
        bio: '',
        profilePicture: '',
        savedArticles: []
      };
      
      // Add to mock database
      MOCK_USERS[newUserId] = newUser;
      
      // Auto login after registration
      return login(email, password);
    } catch (error) {
      setAuthError(error.message);
      return { success: false, message: error.message };
    } finally {
      setAuthLoading(false);
    }
  };
  
  // Fetch user profile
  const fetchProfile = async () => {
    if (!user || !token) return;
    
    setProfileLoading(true);
    setProfileError(null);
    
    // Simulate network delay
    await new Promise(resolve => setTimeout(resolve, 600));
    
    try {
      // Get user data from mock database
      const userData = MOCK_USERS[user.id];
      
      if (!userData) {
        throw new Error('User profile not found');
      }
      
      // Create profile object (excluding sensitive data like password)
      const profileData = {
        id: userData.id,
        name: userData.name,
        username: userData.username,
        email: userData.email,
        savedArticles: userData.savedArticles
      };
      
      setProfile(profileData);
    } catch (error) {
      setProfileError(error.message);
    } finally {
      setProfileLoading(false);
    }
  };
  
  // Update profile
  const updateProfile = async (profileData) => {
    if (!user || !token) {
      return { success: false, message: 'Not authenticated' };
    }
    
    setProfileLoading(true);
    
    // Simulate network delay
    await new Promise(resolve => setTimeout(resolve, 600));
    
    try {
      // Update data in mock database
      MOCK_USERS[user.id] = {
        ...MOCK_USERS[user.id],
        ...profileData
      };
      
      // Update local profile state
      setProfile(prev => ({
        ...prev,
        ...profileData
      }));
      
      return { success: true };
    } catch (error) {
      return { success: false, message: error.message };
    } finally {
      setProfileLoading(false);
    }
  };
  
  // Check if an article is saved by current user
  const isArticleSaved = (articleId) => {
    if (!profile) return false;
    return profile.savedArticles.includes(articleId);
  };

  // Save an article to user profile
  const saveArticle = async (articleId) => {
    if (!user || !token) {
      return { success: false, message: 'Not authenticated' };
    }
    
    // Simulate network delay
    await new Promise(resolve => setTimeout(resolve, 400));
    
    try {
      // Update user's saved articles
      if (!MOCK_USERS[user.id].savedArticles.includes(articleId)) {
        MOCK_USERS[user.id].savedArticles.push(articleId);
      }
      
      // Update profile state
      setProfile(prev => ({
        ...prev,
        savedArticles: [...MOCK_USERS[user.id].savedArticles]
      }));
      
      return { success: true };
    } catch (error) {
      return { success: false, message: error.message };
    }
  };
  
  // Remove a saved article
  const removeSavedArticle = async (articleId) => {
    if (!user || !token) {
      return { success: false, message: 'Not authenticated' };
    }
    
    // Simulate network delay
    await new Promise(resolve => setTimeout(resolve, 400));
    
    try {
      // Remove article from user's saved articles
      MOCK_USERS[user.id].savedArticles = MOCK_USERS[user.id].savedArticles
        .filter(id => id !== articleId);
      
      // Update profile state
      setProfile(prev => ({
        ...prev,
        savedArticles: [...MOCK_USERS[user.id].savedArticles]
      }));
      
      return { success: true };
    } catch (error) {
      return { success: false, message: error.message };
    }
  };

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
    profile,
    profileLoading,
    profileError,
    fetchProfile,
    updateProfile,
    
    // Article methods
    isArticleSaved,
    saveArticle,
    removeSavedArticle
  };
}