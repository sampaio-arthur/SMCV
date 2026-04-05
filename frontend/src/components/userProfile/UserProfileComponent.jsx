import { useState } from 'react';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import EmptyState from '../ui/EmptyState';
import ConfirmDialog from '../ui/ConfirmDialog';
import { useConfirm } from '../../hooks/useConfirm';

const emptyForm = {
  userId: '',
  resumeFileName: '',
  smtpHost: '',
  smtpPort: '',
  smtpEmail: '',
  smtpPassword: '',
};

function UserProfileComponent({ items = [], users = [], onCreate, onUpdate, onDelete }) {
  const [showForm, setShowForm] = useState(false);
  const [editing, setEditing] = useState(null);
  const [formData, setFormData] = useState(emptyForm);
  const [submitting, setSubmitting] = useState(false);
  const { confirm, dialogProps } = useConfirm();

  const getUserName = (userId) => {
    const user = users.find((u) => u.id === userId);
    return user ? user.name : `ID ${userId}`;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      const payload = {
        ...formData,
        smtpPort: formData.smtpPort ? Number(formData.smtpPort) : null,
      };
      if (editing) {
        await onUpdate(editing.id, payload);
        setEditing(null);
      } else {
        await onCreate(payload);
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
      userId: item.userId ?? '',
      resumeFileName: item.resumeFileName ?? '',
      smtpHost: item.smtpHost ?? '',
      smtpPort: item.smtpPort ?? '',
      smtpEmail: item.smtpEmail ?? '',
      smtpPassword: '',
    });
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
        <p className="text-sm text-gray-600">{items.length} perfil(is)</p>
        <button
          onClick={() => { setEditing(null); setFormData(emptyForm); setShowForm(true); }}
          className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition-colors"
        >
          <Plus className="w-4 h-4" />
          Novo Perfil
        </button>
      </div>

      {/* Form */}
      {showForm && (
        <form onSubmit={handleSubmit} className="bg-white p-4 rounded-lg border border-gray-200 space-y-3">
          <h3 className="font-semibold text-gray-800">{editing ? 'Editar Perfil' : 'Novo Perfil'}</h3>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Usuario *</label>
            <select
              required
              value={formData.userId}
              onChange={(e) => setFormData({ ...formData, userId: e.target.value })}
              disabled={!!editing}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-100 disabled:cursor-not-allowed"
            >
              <option value="">Selecione um usuario</option>
              {users.map((u) => (
                <option key={u.id} value={u.id}>{u.name}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Nome do curriculo</label>
            <input
              type="text"
              value={formData.resumeFileName}
              onChange={(e) => setFormData({ ...formData, resumeFileName: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Ex: meu_curriculo.pdf"
            />
            <p className="text-xs text-gray-400 mt-1">Nome do arquivo PDF enviado</p>
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Servidor SMTP</label>
              <input
                type="text"
                value={formData.smtpHost}
                onChange={(e) => setFormData({ ...formData, smtpHost: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Ex: smtp.gmail.com"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Porta SMTP</label>
              <input
                type="number"
                min="1"
                max="65535"
                value={formData.smtpPort}
                onChange={(e) => setFormData({ ...formData, smtpPort: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Ex: 587"
              />
              <p className="text-xs text-gray-400 mt-1">Portas comuns: 587 (TLS), 465 (SSL)</p>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">E-mail SMTP</label>
              <input
                type="email"
                value={formData.smtpEmail}
                onChange={(e) => setFormData({ ...formData, smtpEmail: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="email@gmail.com"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Senha SMTP</label>
              <input
                type="password"
                value={formData.smtpPassword}
                onChange={(e) => setFormData({ ...formData, smtpPassword: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="App Password"
              />
              <p className="text-xs text-gray-400 mt-1">Use uma App Password do Gmail, nao sua senha pessoal</p>
            </div>
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
          title="Nenhum perfil encontrado"
          description="Crie o primeiro perfil de usuario para configurar curriculo e SMTP."
          actionLabel="Novo Perfil"
          onAction={() => { setEditing(null); setFormData(emptyForm); setShowForm(true); }}
        />
      ) : (
        <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Usuario</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Curriculo</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">SMTP Host</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">SMTP E-mail</th>
                <th className="px-4 py-3 text-right font-semibold text-gray-600">Acoes</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {items.map((item) => (
                <tr key={item.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-4 py-3 font-medium text-gray-800">{getUserName(item.userId)}</td>
                  <td className="px-4 py-3 text-gray-500">{item.resumeFileName || 'Nenhum'}</td>
                  <td className="px-4 py-3 text-gray-500">{item.smtpHost || 'Nao configurado'}</td>
                  <td className="px-4 py-3 text-gray-500">{item.smtpEmail || 'Nao configurado'}</td>
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
        title="Excluir perfil"
        message="Tem certeza que deseja excluir este perfil? Esta acao nao pode ser desfeita."
      />
    </div>
  );
}

export default UserProfileComponent;
