import { useEffect, useState, useCallback, useRef } from 'react';
import { useSearchParams } from 'react-router-dom';
import ContactComponent from '../components/contact/ContactComponent';
import { getAllByCampaignId, create, update, remove } from '../services/contactService';
import { getAll as getAllCampaigns } from '../services/campaignService';
import { useToast } from '../hooks/useToast';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import { getErrorMessage } from '../utils';

function ContactPage() {
  const [items, setItems] = useState([]);
  const [campaigns, setCampaigns] = useState([]);
  const [campaignsLoading, setCampaignsLoading] = useState(true);
  const [contactsLoading, setContactsLoading] = useState(false);
  const [searchParams, setSearchParams] = useSearchParams();
  const [selectedCampaignId, setSelectedCampaignId] = useState(searchParams.get('campaignId') || '');
  const { success: toastSuccess, error: toastError } = useToast();
  const toastErrorRef = useRef(toastError);

  useEffect(() => {
    toastErrorRef.current = toastError;
  }, [toastError]);

  const loadContacts = useCallback(async (campaignId) => {
    try {
      setContactsLoading(true);
      const contacts = await getAllByCampaignId(campaignId);
      setItems(contacts);
    } catch (err) {
      toastErrorRef.current(await getErrorMessage(err, 'Nao foi possivel carregar os contatos.'));
    } finally {
      setContactsLoading(false);
    }
  }, []);

  // Carregamento inicial das campanhas. selectedCampaignId é excluído
  // intencionalmente das deps — este useEffect o modifica, incluí-lo causaria loop.
  /* eslint-disable react-hooks/exhaustive-deps */
  useEffect(() => {
    (async () => {
      try {
        setCampaignsLoading(true);
        const campaignList = await getAllCampaigns();
        setCampaigns(campaignList);
        if (!selectedCampaignId && campaignList.length > 0) {
          setSelectedCampaignId(campaignList[0].id);
        }
      } catch (err) {
        toastErrorRef.current(await getErrorMessage(err, 'Nao foi possivel carregar as campanhas. Verifique se a API esta em execucao.'));
      } finally {
        setCampaignsLoading(false);
      }
    })();
  }, []);
  /* eslint-enable react-hooks/exhaustive-deps */

  useEffect(() => {
    if (!selectedCampaignId) {
      setItems([]);
      return;
    }
    const current = new URLSearchParams(window.location.search).get('campaignId');
    if (current !== selectedCampaignId) {
      setSearchParams({ campaignId: selectedCampaignId }, { replace: true });
    }
    loadContacts(selectedCampaignId);
  }, [selectedCampaignId, loadContacts, setSearchParams]);

  const handleCreate = async (item) => {
    try {
      const novoContato = await create(item);
      toastSuccess('Contato criado com sucesso!');
      if (item.campaignId && item.campaignId !== selectedCampaignId) {
        setSelectedCampaignId(item.campaignId);
      } else {
        setItems(prev => [...prev, novoContato]);
      }
    } catch (err) {
      toastError(await getErrorMessage(err, 'Nao foi possivel criar o contato.'));
    }
  };

  const handleUpdate = async (id, item) => {
    try {
      const contatoAtualizado = await update(id, item);
      setItems(prev => prev.map(c => c.id === id ? contatoAtualizado : c));
      toastSuccess('Contato atualizado com sucesso!');
    } catch (err) {
      toastError(await getErrorMessage(err, 'Nao foi possivel atualizar o contato.'));
    }
  };

  const handleDelete = async (id) => {
    try {
      await remove(id);
      setItems(prev => prev.filter(c => c.id !== id));
      toastSuccess('Contato excluido com sucesso!');
    } catch (err) {
      toastError(await getErrorMessage(err, 'Nao foi possivel excluir o contato.'));
    }
  };

  const selectedCampaign = campaigns.find((c) => String(c.id) === String(selectedCampaignId));

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Contatos</h1>
        <p className="text-gray-500 text-sm mt-1">
          {selectedCampaign
            ? `Contatos da campanha: ${selectedCampaign.name}`
            : 'Selecione uma campanha para visualizar seus contatos.'}
        </p>
      </div>

      {campaignsLoading ? (
        <LoadingSpinner />
      ) : campaigns.length === 0 ? (
        <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4 text-sm text-yellow-800">
          Nenhuma campanha cadastrada. Crie uma campanha antes de gerenciar contatos.
        </div>
      ) : (
        <>
          <div className="mb-4 max-w-sm">
            <label className="block text-sm font-medium text-gray-700 mb-1">Campanha</label>
            <select
              value={selectedCampaignId}
              onChange={(e) => setSelectedCampaignId(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              {campaigns.map((c) => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </div>

          {contactsLoading ? (
            <LoadingSpinner />
          ) : (
            <ContactComponent
              items={items}
              campaigns={campaigns}
              onCreate={handleCreate}
              onUpdate={handleUpdate}
              onDelete={handleDelete}
              defaultCampaignId={selectedCampaignId || ''}
            />
          )}
        </>
      )}
    </div>
  );
}

export default ContactPage;
