import api from './api';

export const getByCampaign = async (campaignId) => {
  const response = await api.get(`/emaillogs/campaign/${campaignId}`);
  return response.data;
};

export const getByContact = async (contactId) => {
  const response = await api.get(`/emaillogs/contact/${contactId}`);
  return response.data;
};
