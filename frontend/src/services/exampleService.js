import axios from 'axios';

// VITE_API_URL can be set in .env or passed at Docker build time.
// Defaults to localhost:8080 for local development.
const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8080';

const api = axios.create({
  baseURL: `${API_URL}/api/example`,
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
