import { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import ContactComponent from '../components/contact/ContactComponent';
import { getAll, getAllByCampaignId, create, update, remove } from '../services/contactService';
import { getAll as getAllCampaigns } from '../services/campaignService';
import { useToast } from '../hooks/useToast';
import LoadingSpinner from '../components/ui/LoadingSpinner';

function ContactPage() {
  const [items, setItems] = useState([]);
  const [campaigns, setCampaigns] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchParams] = useSearchParams();
  const campaignId = searchParams.get('campaignId');
  const toast = useToast();

  useEffect(() => {
    loadData();
  }, [campaignId]);

  const loadData = async () => {
    try {
      setLoading(true);
      const contactsPromise = campaignId ? getAllByCampaignId(campaignId) : getAll();
      const [contacts, campaignList] = await Promise.all([contactsPromise, getAllCampaigns()]);
      setItems(contacts);
      setCampaigns(campaignList);
    } catch {
      toast.error('Nao foi possivel carregar os contatos. Verifique se a API esta em execucao.');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (item) => {
    try {
      await create(item);
      toast.success('Contato criado com sucesso!');
      await loadData();
    } catch {
      toast.error('Nao foi possivel criar o contato. Verifique os campos e tente novamente.');
    }
  };

  const handleUpdate = async (id, item) => {
    try {
      await update(id, item);
      toast.success('Contato atualizado com sucesso!');
      await loadData();
    } catch {
      toast.error('Nao foi possivel atualizar o contato. Verifique sua conexao.');
    }
  };

  const handleDelete = async (id) => {
    try {
      await remove(id);
      toast.success('Contato excluido com sucesso!');
      await loadData();
    } catch {
      toast.error('Nao foi possivel excluir o contato.');
    }
  };

  const campaignName = campaignId
    ? campaigns.find((c) => String(c.id) === String(campaignId))?.name
    : null;

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Contatos</h1>
        <p className="text-gray-500 text-sm mt-1">
          {campaignName
            ? `Contatos da campanha: ${campaignName}`
            : 'Visualize e gerencie os contatos de cada campanha.'}
        </p>
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : (
        <ContactComponent
          items={items}
          campaigns={campaigns}
          onCreate={handleCreate}
          onUpdate={handleUpdate}
          onDelete={handleDelete}
          defaultCampaignId={campaignId || ''}
        />
      )}
    </div>
  );
}

export default ContactPage;
