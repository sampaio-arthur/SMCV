import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8080';

const api = axios.create({
  baseURL: `${API_URL}/api/userprofile`,
  headers: { 'Content-Type': 'application/json' },
});

export const getAll = async () => {
  const response = await api.get('/');
  return response.data;
};

export const getById = async (id) => {
  const response = await api.get(`/${id}`);
  return response.data;
};

export const getByUserId = async (userId) => {
  const response = await api.get(`/user/${userId}`);
  return response.data;
};

export const create = async (data) => {
  const response = await api.post('/', data);
  return response.data;
};

export const update = async (id, data) => {
  const response = await api.put(`/${id}`, data);
  return response.data;
};

export const remove = async (id) => {
  const response = await api.delete(`/${id}`);
  return response.data;
};
