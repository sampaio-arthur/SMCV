import api from './api';

export const getAll = async () => {
  const response = await api.get('/campaigns');
  return response.data;
};

export const getById = async (id) => {
  const response = await api.get(`/campaigns/${id}`);
  return response.data;
};

export const create = async (data) => {
  const response = await api.post('/campaigns', data);
  return response.data;
};

export const update = async (id, data) => {
  const response = await api.put(`/campaigns/${id}`, data);
  return response.data;
};

export const remove = async (id) => {
  const response = await api.delete(`/campaigns/${id}`);
  return response.data;
};

export const sendEmails = async (id) => {
  const response = await api.post(`/campaigns/${id}/send`);
  return response.data;
};

export const exportCsv = async (id) => {
  const response = await api.get(`/campaigns/${id}/export-csv`, {
    responseType: 'blob',
  });
  return response;
};
