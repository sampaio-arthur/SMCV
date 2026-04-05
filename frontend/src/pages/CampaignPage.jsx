import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import CampaignComponent from '../components/campaign/CampaignComponent';
import { getAll, create, update, remove } from '../services/campaignService';
import { useToast } from '../hooks/useToast';
import LoadingSpinner from '../components/ui/LoadingSpinner';

function CampaignPage() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const toast = useToast();
  const navigate = useNavigate();

  useEffect(() => {
    loadItems();
  }, []);

  const loadItems = async () => {
    try {
      setLoading(true);
      const data = await getAll();
      setItems(data);
    } catch {
      toast.error('Nao foi possivel carregar as campanhas. Verifique se a API esta em execucao.');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (item) => {
    try {
      await create(item);
      toast.success('Campanha criada com sucesso!');
      await loadItems();
    } catch {
      toast.error('Nao foi possivel criar a campanha. Verifique os campos e tente novamente.');
    }
  };

  const handleUpdate = async (id, item) => {
    try {
      await update(id, item);
      toast.success('Campanha atualizada com sucesso!');
      await loadItems();
    } catch {
      toast.error('Nao foi possivel atualizar a campanha. Verifique sua conexao.');
    }
  };

  const handleDelete = async (id) => {
    try {
      await remove(id);
      toast.success('Campanha excluida com sucesso!');
      await loadItems();
    } catch {
      toast.error('Nao foi possivel excluir a campanha.');
    }
  };

  const handleViewContacts = (campaignId) => {
    navigate(`/contact?campaignId=${campaignId}`);
  };

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Campanhas</h1>
        <p className="text-gray-500 text-sm mt-1">
          Crie e gerencie campanhas de envio de curriculo.
        </p>
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : (
        <CampaignComponent
          items={items}
          onCreate={handleCreate}
          onUpdate={handleUpdate}
          onDelete={handleDelete}
          onViewContacts={handleViewContacts}
        />
      )}
    </div>
  );
}

export default CampaignPage;
