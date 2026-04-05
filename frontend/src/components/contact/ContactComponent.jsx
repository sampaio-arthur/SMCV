import { useState, useCallback } from 'react';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { formatDate } from '../../utils';
import StatusBadge from '../ui/StatusBadge';
import EmptyState from '../ui/EmptyState';
import SearchInput from '../ui/SearchInput';
import ConfirmDialog from '../ui/ConfirmDialog';
import { useConfirm } from '../../hooks/useConfirm';

const emailStatusConfig = {
  Pending: { label: 'Pendente', bg: 'bg-gray-100', text: 'text-gray-600' },
  Sent: { label: 'Enviado', bg: 'bg-green-100', text: 'text-green-700' },
  Failed: { label: 'Falhou', bg: 'bg-red-100', text: 'text-red-700' },
};

const statusFilters = [
  { value: '', label: 'Todos' },
  { value: 'Pending', label: 'Pendente' },
  { value: 'Sent', label: 'Enviado' },
  { value: 'Failed', label: 'Falhou' },
];

const emptyForm = { campaignId: '', companyName: '', email: '', region: '' };

function ContactComponent({ items = [], campaigns = [], onCreate, onUpdate, onDelete, defaultCampaignId }) {
  const [showForm, setShowForm] = useState(false);
  const [editing, setEditing] = useState(null);
  const [formData, setFormData] = useState({ ...emptyForm, campaignId: defaultCampaignId || '' });
  const [submitting, setSubmitting] = useState(false);
  const [statusFilter, setStatusFilter] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const { confirm, dialogProps } = useConfirm();

  const filteredItems = items.filter((item) => {
    if (statusFilter && item.emailStatus !== statusFilter) return false;
    if (searchTerm) {
      const term = searchTerm.toLowerCase();
      return (
        (item.companyName?.toLowerCase().includes(term)) ||
        (item.email?.toLowerCase().includes(term))
      );
    }
    return true;
  });

  const statusCounts = {
    Pending: items.filter((i) => i.emailStatus === 'Pending').length,
    Sent: items.filter((i) => i.emailStatus === 'Sent').length,
    Failed: items.filter((i) => i.emailStatus === 'Failed').length,
  };

  const handleSearchChange = useCallback((value) => {
    setSearchTerm(value);
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      if (editing) {
        await onUpdate(editing.id, {
          campaignId: formData.campaignId,
          companyName: formData.companyName,
          email: formData.email,
          region: formData.region,
        });
        setEditing(null);
      } else {
        await onCreate(formData);
      }
      setFormData({ ...emptyForm, campaignId: defaultCampaignId || '' });
      setShowForm(false);
    } finally {
      setSubmitting(false);
    }
  };

  const handleEdit = (item) => {
    setEditing(item);
    setFormData({
      campaignId: item.campaignId ?? '',
      companyName: item.companyName ?? '',
      email: item.email ?? '',
      region: item.region ?? '',
    });
    setShowForm(true);
  };

  const handleCancel = () => {
    setEditing(null);
    setFormData({ ...emptyForm, campaignId: defaultCampaignId || '' });
    setShowForm(false);
  };

  const openNewForm = () => {
    setEditing(null);
    setFormData({ ...emptyForm, campaignId: defaultCampaignId || '' });
    setShowForm(true);
  };

  return (
    <div className="space-y-4">
      {/* Toolbar */}
      <div className="flex justify-between items-center">
        <p className="text-sm text-gray-600">{filteredItems.length} contato(s)</p>
        <button
          onClick={openNewForm}
          className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition-colors"
        >
          <Plus className="w-4 h-4" />
          Novo Contato
        </button>
      </div>

      {/* Summary counters */}
      {items.length > 0 && (
        <div className="flex gap-4 text-sm">
          <span className="text-gray-500">{statusCounts.Pending} pendente(s)</span>
          <span className="text-green-600">{statusCounts.Sent} enviado(s)</span>
          <span className="text-red-600">{statusCounts.Failed} falhou(aram)</span>
        </div>
      )}

      {/* Filters */}
      <div className="flex gap-3 items-center flex-wrap">
        <div className="flex gap-2">
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
        <div className="w-64">
          <SearchInput
            onChange={handleSearchChange}
            placeholder="Buscar por empresa ou e-mail..."
          />
        </div>
      </div>

      {/* Form */}
      {showForm && (
        <form onSubmit={handleSubmit} className="bg-white p-4 rounded-lg border border-gray-200 space-y-3">
          <h3 className="font-semibold text-gray-800">{editing ? 'Editar Contato' : 'Novo Contato'}</h3>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Campanha *</label>
            <select
              required
              value={formData.campaignId}
              onChange={(e) => setFormData({ ...formData, campaignId: e.target.value })}
              disabled={!!editing}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-100 disabled:cursor-not-allowed"
            >
              <option value="">Selecione uma campanha</option>
              {campaigns.map((c) => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Nome da empresa *</label>
              <input
                type="text"
                required
                value={formData.companyName}
                onChange={(e) => setFormData({ ...formData, companyName: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Ex: Google Brasil"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">E-mail *</label>
              <input
                type="email"
                required
                value={formData.email}
                onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Ex: rh@empresa.com"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Regiao</label>
            <input
              type="text"
              value={formData.region}
              onChange={(e) => setFormData({ ...formData, region: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Ex: Sao Paulo - SP"
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
          title="Nenhum contato encontrado"
          description={
            searchTerm || statusFilter
              ? 'Nenhum contato corresponde aos filtros aplicados.'
              : 'Nenhum contato nesta campanha. Adicione contatos para iniciar o envio.'
          }
          actionLabel={!searchTerm && !statusFilter ? 'Novo Contato' : undefined}
          onAction={!searchTerm && !statusFilter ? openNewForm : undefined}
        />
      ) : (
        <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Empresa</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">E-mail</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Regiao</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Status do envio</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Enviado em</th>
                <th className="px-4 py-3 text-right font-semibold text-gray-600">Acoes</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {filteredItems.map((item) => (
                <tr key={item.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-4 py-3 font-medium text-gray-800">{item.companyName}</td>
                  <td className="px-4 py-3 text-gray-500">{item.email}</td>
                  <td className="px-4 py-3 text-gray-500">{item.region || '—'}</td>
                  <td className="px-4 py-3">
                    <StatusBadge status={item.emailStatus} config={emailStatusConfig} />
                  </td>
                  <td className="px-4 py-3 text-gray-500">
                    {item.emailSentAt ? formatDate(item.emailSentAt) : 'Nao enviado'}
                  </td>
                  <td className="px-4 py-3 text-right">
                    <button
                      onClick={() => handleEdit(item)}
                      className="p-1.5 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded transition-colors mr-1"
                      title="Editar"
                    >
                      <Pencil className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => confirm(() => onDelete(item.id))}
                      className="p-1.5 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded transition-colors"
                      title="Excluir"
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
        title="Excluir contato"
        message="Tem certeza que deseja excluir este contato? Esta acao nao pode ser desfeita."
      />
    </div>
  );
}

export default ContactComponent;
