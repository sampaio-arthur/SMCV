import { useEffect, useState, useCallback, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import CampaignComponent from '../components/campaign/CampaignComponent';
import { getAll, create, update, remove, sendEmails, exportCsv } from '../services/campaignService';
import { useToast } from '../hooks/useToast';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import { getErrorMessage } from '../utils';

function CampaignPage() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [sendingCampaignIds, setSendingCampaignIds] = useState(() => new Set());
  const { success: toastSuccess, error: toastError } = useToast();
  const toastErrorRef = useRef(toastError);
  const navigate = useNavigate();

  useEffect(() => {
    toastErrorRef.current = toastError;
  }, [toastError]);

  const loadItems = useCallback(async () => {
    try {
      setLoading(true);
      const data = await getAll();
      setItems(data);
    } catch (err) {
      toastErrorRef.current(await getErrorMessage(err, 'Nao foi possivel carregar as campanhas. Verifique se a API esta em execucao.'));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadItems();
  }, [loadItems]);

  const handleCreate = async (item) => {
    try {
      const novaCampanha = await create(item);
      setItems(prev => [...prev, novaCampanha]);
      toastSuccess('Campanha criada com sucesso!');
    } catch (err) {
      toastError(await getErrorMessage(err, 'Nao foi possivel criar a campanha.'));
    }
  };

  const handleUpdate = async (id, item) => {
    try {
      const campanhaAtualizada = await update(id, item);
      setItems(prev => prev.map(c => c.id === id ? campanhaAtualizada : c));
      toastSuccess('Campanha atualizada com sucesso!');
    } catch (err) {
      toastError(await getErrorMessage(err, 'Nao foi possivel atualizar a campanha.'));
    }
  };

  const handleDelete = async (id) => {
    try {
      await remove(id);
      setItems(prev => prev.filter(c => c.id !== id));
      toastSuccess('Campanha excluida com sucesso!');
    } catch (err) {
      toastError(await getErrorMessage(err, 'Nao foi possivel excluir a campanha.'));
    }
  };

  const handleViewContacts = (campaignId) => {
    navigate(`/contact?campaignId=${campaignId}`);
  };

  const handleSendEmails = async (id) => {
    setSendingCampaignIds(prev => {
      const next = new Set(prev);
      next.add(id);
      return next;
    });

    try {
      const result = await sendEmails(id);
      const emailsSent = result.emailsSent ?? 0;

      if (emailsSent > 0) {
        toastSuccess(`E-mails disparados! ${emailsSent} enviado(s).`);
      } else {
        toastError('Nenhum e-mail foi enviado. A campanha foi marcada como falhou.');
      }
    } catch (err) {
      toastError(await getErrorMessage(err, 'Erro ao disparar e-mails da campanha.'));
    } finally {
      setSendingCampaignIds(prev => {
        const next = new Set(prev);
        next.delete(id);
        return next;
      });
      await loadItems();
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
      toastSuccess('CSV exportado com sucesso!');
    } catch (err) {
      toastError(await getErrorMessage(err, 'Erro ao exportar CSV.'));
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
          sendingCampaignIds={sendingCampaignIds}
        />
      )}
    </div>
  );
}

export default CampaignPage;
