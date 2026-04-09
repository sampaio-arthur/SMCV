import api from './api';

export const getAll = async (pageNumber = 1, pageSize = 100) => {
  const response = await api.get('/userprofiles', { params: { pageNumber, pageSize } });
  return response.data;
};

export const getById = async (id) => {
  const response = await api.get(`/userprofiles/${id}`);
  return response.data;
};

export const getByUserId = async (userId) => {
  const response = await api.get(`/userprofiles/user/${userId}`);
  return response.data;
};

export const create = async () => {
  const response = await api.post('/userprofiles');
  return response.data;
};

export const uploadResume = async (id, file) => {
  const formData = new FormData();
  formData.append('resumeFile', file);
  const response = await api.post(`/userprofiles/${id}/upload-resume`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return response.data;
};

export const update = async (id, data) => {
  const response = await api.put(`/userprofiles/${id}`, data);
  return response.data;
};

export const remove = async (id) => {
  const response = await api.delete(`/userprofiles/${id}`);
  return response.data;
};
