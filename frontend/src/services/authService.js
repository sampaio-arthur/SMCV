import api from './api';

export const register = async (data) => {
  const response = await api.post('/auth/register', data);
  return response.data;
};

export const login = async (data) => {
  const response = await api.post('/auth/login', data);
  return response.data;
};

export const logout = async () => {
  await api.post('/auth/logout');
};

export const me = async () => {
  const response = await api.get('/auth/me');
  return response.data;
};
