import { useState, useCallback } from 'react';
import { Plus, Pencil, Trash2, Eye, Send, Download, Loader2, RefreshCw } from 'lucide-react';
import { formatDate } from '../../utils';
import StatusBadge from '../ui/StatusBadge';
import EmptyState from '../ui/EmptyState';
import ConfirmDialog from '../ui/ConfirmDialog';
import { useConfirm } from '../../hooks/useConfirm';

const campaignStatusConfig = {
  Draft: { label: 'Rascunho', bg: 'bg-gray-100', text: 'text-gray-600' },
  Running: { label: 'Enviando', bg: 'bg-yellow-100', text: 'text-yellow-700', extra: 'animate-pulse' },
  Completed: { label: 'Concluido', bg: 'bg-green-100', text: 'text-green-700' },
  Cancelled: { label: 'Cancelado', bg: 'bg-gray-100', text: 'text-gray-500' },
  Failed: { label: 'Falhou', bg: 'bg-red-100', text: 'text-red-700' },
  PartialSuccess: { label: 'Parcial', bg: 'bg-orange-100', text: 'text-orange-700' },
};

const statusFilters = [
  { value: '', label: 'Todas' },
  { value: 'Draft', label: 'Rascunho' },
  { value: 'Completed', label: 'Concluido' },
  { value: 'Cancelled', label: 'Cancelado' },
  { value: 'Failed', label: 'Falhou' },
  { value: 'PartialSuccess', label: 'Parcial' },
];

const emptyForm = { name: '', niche: '', region: '', emailSubject: '', emailBody: '' };

function CampaignComponent({
  items = [],
  onCreate,
  onUpdate,
  onDelete,
  onViewContacts,
  onSendEmails,
  onExportCsv,
  sendingCampaignIds = new Set(),
}) {
  const [showForm, setShowForm] = useState(false);
  const [editing, setEditing] = useState(null);
  const [formData, setFormData] = useState(emptyForm);
  const [submitting, setSubmitting] = useState(false);
  const [statusFilter, setStatusFilter] = useState('');
  const { confirm, dialogProps } = useConfirm();

  const filteredItems = statusFilter
    ? items.filter((i) => i.status === statusFilter)
    : items;

  const isLocked = useCallback((status) => status === 'Running' || status === 'Completed', []);
  const canSendEmails = useCallback(
    (status) => status === 'Draft' || status === 'Failed' || status === 'PartialSuccess',
    []
  );
  const isRetryStatus = useCallback((status) => status === 'Failed' || status === 'PartialSuccess', []);
  const isCampaignSending = useCallback(
    (item) => sendingCampaignIds.has(item.id) || item.status === 'Running',
    [sendingCampaignIds]
  );

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      if (editing) {
        await onUpdate(editing.id, {
          name: formData.name,
          emailSubject: formData.emailSubject,
          emailBody: formData.emailBody,
        });
        setEditing(null);
      } else {
        await onCreate(formData);
      }
      setFormData(emptyForm);
      setShowForm(false);
    } finally {
      setSubmitting(false);
    }
  };

  const handleEdit = (item) => {
    setEditing(item);
    setFormData({
      name: item.name ?? '',
      niche: item.niche ?? '',
      region: item.region ?? '',
      emailSubject: item.emailSubject ?? '',
      emailBody: item.emailBody ?? '',
    });
    setShowForm(true);
  };

  const handleCancel = () => {
    setEditing(null);
    setFormData(emptyForm);
    setShowForm(false);
  };

  const renderSendAction = (item) => {
    const isSending = isCampaignSending(item);

    if (isSending) {
      return (
        <button
          type="button"
          disabled
          className="p-1.5 text-yellow-600 bg-yellow-50 rounded transition-colors mr-1 cursor-wait"
          title="Enviando e-mails"
        >
          <Loader2 className="w-4 h-4 animate-spin" />
        </button>
      );
    }

    if (!canSendEmails(item.status)) return null;

    const SendIcon = isRetryStatus(item.status) ? RefreshCw : Send;

    return (
      <button
        onClick={() => onSendEmails?.(item.id)}
        className="p-1.5 text-gray-400 hover:text-green-600 hover:bg-green-50 rounded transition-colors mr-1"
        title={isRetryStatus(item.status) ? 'Tentar reenviar e-mails' : 'Enviar e-mails'}
      >
        <SendIcon className="w-4 h-4" />
      </button>
    );
  };

  return (
    <div className="space-y-4">
      {/* Toolbar */}
      <div className="flex justify-between items-center">
        <p className="text-sm text-gray-600">{filteredItems.length} campanha(s)</p>
        <button
          onClick={() => { setEditing(null); setFormData(emptyForm); setShowForm(true); }}
          className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition-colors"
        >
          <Plus className="w-4 h-4" />
          Nova Campanha
        </button>
      </div>

      {/* Status filter tabs */}
      <div className="flex gap-2 flex-wrap">
        {statusFilters.map((f) => (
          <button
            key={f.value}
            onClick={() => setStatusFilter(f.value)}
            className={`px-3 py-1.5 text-sm rounded-lg transition-colors ${
              statusFilter === f.value
                ? 'bg-blue-600 text-white'
                : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
            }`}
          >
            {f.label}
          </button>
        ))}
      </div>

      {/* Form */}
      {showForm && (
        <form onSubmit={handleSubmit} className="bg-white p-4 rounded-lg border border-gray-200 space-y-3">
          <h3 className="font-semibold text-gray-800">{editing ? 'Editar Campanha' : 'Nova Campanha'}</h3>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Nome da campanha *</label>
            <input
              type="text"
              required
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Ex: Devs Floripa - Marco 2026"
            />
          </div>

          {!editing && (
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Nicho *</label>
                <input
                  type="text"
                  required
                  value={formData.niche}
                  onChange={(e) => setFormData({ ...formData, niche: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Ex: Tecnologia"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Regiao *</label>
                <input
                  type="text"
                  required
                  value={formData.region}
                  onChange={(e) => setFormData({ ...formData, region: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Ex: Florianopolis - SC"
                />
              </div>
            </div>
          )}

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Assunto do e-mail *</label>
            <input
              type="text"
              required
              value={formData.emailSubject}
              onChange={(e) => setFormData({ ...formData, emailSubject: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Ex: Curriculo - Desenvolvedor Full Stack"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Corpo do e-mail *</label>
            <textarea
              required
              rows={4}
              value={formData.emailBody}
              onChange={(e) => setFormData({ ...formData, emailBody: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-y"
              placeholder="Escreva o corpo do e-mail..."
            />
          </div>

          <div className="flex gap-2">
            <button
              type="submit"
              disabled={submitting}
              className="px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {submitting ? 'Salvando...' : editing ? 'Atualizar' : 'Criar'}
            </button>
            <button
              type="button"
              onClick={handleCancel}
              className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-lg transition-colors"
            >
              Cancelar
            </button>
          </div>
        </form>
      )}

      {/* List */}
      {filteredItems.length === 0 ? (
        <EmptyState
          title="Nenhuma campanha encontrada"
          description={statusFilter ? 'Nenhuma campanha com este status.' : 'Crie a primeira campanha para comecar.'}
          actionLabel={statusFilter ? undefined : 'Nova Campanha'}
          onAction={statusFilter ? undefined : () => { setEditing(null); setFormData(emptyForm); setShowForm(true); }}
        />
      ) : (
        <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Nome</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Nicho</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Regiao</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Status</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Contatos</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Criado em</th>
                <th className="px-4 py-3 text-right font-semibold text-gray-600">Acoes</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {filteredItems.map((item) => (
                <tr key={item.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-4 py-3 font-medium text-gray-800">{item.name}</td>
                  <td className="px-4 py-3 text-gray-500">{item.niche ?? '—'}</td>
                  <td className="px-4 py-3 text-gray-500">{item.region ?? '—'}</td>
                  <td className="px-4 py-3">
                    <StatusBadge status={item.status} config={campaignStatusConfig} />
                  </td>
                  <td className="px-4 py-3 text-gray-500">
                    {item.totalContacts ?? 0} contato(s)
                  </td>
                  <td className="px-4 py-3 text-gray-500">{item.createdAt ? formatDate(item.createdAt) : '—'}</td>
                  <td className="px-4 py-3 text-right">
                    <button
                      onClick={() => onViewContacts?.(item.id)}
                      className="p-1.5 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded transition-colors mr-1"
                      title="Ver contatos"
                    >
                      <Eye className="w-4 h-4" />
                    </button>
                    {renderSendAction(item)}
                    <button
                      onClick={() => onExportCsv?.(item.id, item.name)}
                      className="p-1.5 text-gray-400 hover:text-purple-600 hover:bg-purple-50 rounded transition-colors mr-1"
                      title="Exportar CSV"
                    >
                      <Download className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => handleEdit(item)}
                      disabled={isLocked(item.status) || isCampaignSending(item)}
                      className="p-1.5 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded transition-colors mr-1 disabled:opacity-30 disabled:cursor-not-allowed disabled:hover:text-gray-400 disabled:hover:bg-transparent"
                      title={isLocked(item.status) || isCampaignSending(item) ? 'Edicao bloqueada' : 'Editar'}
                    >
                      <Pencil className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => confirm(() => onDelete(item.id))}
                      disabled={isCampaignSending(item)}
                      className="p-1.5 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded transition-colors disabled:opacity-30 disabled:cursor-not-allowed disabled:hover:text-gray-400 disabled:hover:bg-transparent"
                      title={isCampaignSending(item) ? 'Exclusao bloqueada durante envio' : 'Excluir'}
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <ConfirmDialog
        {...dialogProps}
        title="Excluir campanha"
        message="Tem certeza que deseja excluir esta campanha? Esta acao nao pode ser desfeita."
      />
    </div>
  );
}

export default CampaignComponent;
