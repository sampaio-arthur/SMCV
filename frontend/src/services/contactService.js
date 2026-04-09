import api from './api';

export const getById = async (id) => {
  const response = await api.get(`/contacts/${id}`);
  return response.data;
};

export const getAllByCampaignId = async (campaignId) => {
  const response = await api.get(`/contacts/campaign/${campaignId}`);
  return response.data;
};

export const create = async (data) => {
  const response = await api.post('/contacts', data);
  return response.data;
};

export const update = async (id, data) => {
  const response = await api.put(`/contacts/${id}`, data);
  return response.data;
};

export const remove = async (id) => {
  const response = await api.delete(`/contacts/${id}`);
  return response.data;
};

export const searchContacts = async (data) => {
  const response = await api.post('/contacts/search', data);
  return response.data;
};
