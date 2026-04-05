import { useState } from 'react';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { formatDate } from '../../utils';
import EmptyState from '../ui/EmptyState';
import ConfirmDialog from '../ui/ConfirmDialog';
import { useConfirm } from '../../hooks/useConfirm';

const emptyForm = { name: '', email: '', password: '' };

function UserComponent({ items = [], onCreate, onUpdate, onDelete }) {
  const [showForm, setShowForm] = useState(false);
  const [editing, setEditing] = useState(null);
  const [formData, setFormData] = useState(emptyForm);
  const [submitting, setSubmitting] = useState(false);
  const { confirm, dialogProps } = useConfirm();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      if (editing) {
        const payload = { name: formData.name, email: formData.email };
        if (formData.password) payload.password = formData.password;
        await onUpdate(editing.id, payload);
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
    setFormData({ name: item.name, email: item.email, password: '' });
    setShowForm(true);
  };

  const handleCancel = () => {
    setEditing(null);
    setFormData(emptyForm);
    setShowForm(false);
  };

  return (
    <div className="space-y-4">
      {/* Toolbar */}
      <div className="flex justify-between items-center">
        <p className="text-sm text-gray-600">{items.length} usuario(s)</p>
        <button
          onClick={() => { setEditing(null); setFormData(emptyForm); setShowForm(true); }}
          className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition-colors"
        >
          <Plus className="w-4 h-4" />
          Novo Usuario
        </button>
      </div>

      {/* Form */}
      {showForm && (
        <form onSubmit={handleSubmit} className="bg-white p-4 rounded-lg border border-gray-200 space-y-3">
          <h3 className="font-semibold text-gray-800">{editing ? 'Editar Usuario' : 'Novo Usuario'}</h3>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Nome *</label>
            <input
              type="text"
              required
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Nome completo"
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
              placeholder="email@exemplo.com"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Senha {editing ? '' : '*'}
            </label>
            <input
              type="password"
              required={!editing}
              value={formData.password}
              onChange={(e) => setFormData({ ...formData, password: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder={editing ? 'Deixe vazio para manter a atual' : 'Minimo 8 caracteres'}
            />
            {editing && (
              <p className="text-xs text-gray-400 mt-1">Deixe em branco para nao alterar a senha.</p>
            )}
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
      {items.length === 0 ? (
        <EmptyState
          title="Nenhum usuario encontrado"
          description="Crie o primeiro usuario para comecar."
          actionLabel="Novo Usuario"
          onAction={() => { setEditing(null); setFormData(emptyForm); setShowForm(true); }}
        />
      ) : (
        <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Nome</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">E-mail</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Criado em</th>
                <th className="px-4 py-3 text-right font-semibold text-gray-600">Acoes</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {items.map((item) => (
                <tr key={item.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-4 py-3 font-medium text-gray-800">{item.name}</td>
                  <td className="px-4 py-3 text-gray-500">{item.email}</td>
                  <td className="px-4 py-3 text-gray-500">{item.createdAt ? formatDate(item.createdAt) : '—'}</td>
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
        title="Excluir usuario"
        message="Tem certeza que deseja excluir este usuario? Esta acao nao pode ser desfeita."
      />
    </div>
  );
}

export default UserComponent;
