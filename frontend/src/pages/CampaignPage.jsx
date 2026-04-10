import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import CampaignComponent from '../components/campaign/CampaignComponent';
import { getAll, create, update, remove, sendEmails, exportCsv } from '../services/campaignService';
import { useToast } from '../hooks/useToast';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import { getErrorMessage } from '../utils';

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
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Nao foi possivel carregar as campanhas. Verifique se a API esta em execucao.'));
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (item) => {
    try {
      await create(item);
      toast.success('Campanha criada com sucesso!');
      await loadItems();
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Nao foi possivel criar a campanha.'));
    }
  };

  const handleUpdate = async (id, item) => {
    try {
      await update(id, item);
      toast.success('Campanha atualizada com sucesso!');
      await loadItems();
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Nao foi possivel atualizar a campanha.'));
    }
  };

  const handleDelete = async (id) => {
    try {
      await remove(id);
      toast.success('Campanha excluida com sucesso!');
      await loadItems();
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Nao foi possivel excluir a campanha.'));
    }
  };

  const handleViewContacts = (campaignId) => {
    navigate(`/contact?campaignId=${campaignId}`);
  };

  const handleSendEmails = async (id) => {
    try {
      const result = await sendEmails(id);
      toast.success(`E-mails disparados! ${result.emailsSent ?? 0} enviado(s).`);
      await loadItems();
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Erro ao disparar e-mails da campanha.'));
    }
  };

  const handleExportCsv = async (id, name) => {
    try {
      const response = await exportCsv(id);
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `${name || 'contatos'}.csv`);
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
      toast.success('CSV exportado com sucesso!');
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Erro ao exportar CSV.'));
    }
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
          onSendEmails={handleSendEmails}
          onExportCsv={handleExportCsv}
        />
      )}
    </div>
  );
}

export default CampaignPage;
