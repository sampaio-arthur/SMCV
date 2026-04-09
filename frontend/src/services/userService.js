import api from './api';

export const getAll = async (pageNumber = 1, pageSize = 100) => {
  const response = await api.get('/users', { params: { pageNumber, pageSize } });
  return response.data;
};

export const getById = async (id) => {
  const response = await api.get(`/users/${id}`);
  return response.data;
};

export const create = async (data) => {
  const response = await api.post('/users', data);
  return response.data;
};

export const update = async (id, data) => {
  const response = await api.put(`/users/${id}`, data);
  return response.data;
};

export const remove = async (id) => {
  const response = await api.delete(`/users/${id}`);
  return response.data;
};
