import { useState } from 'react';
import { Plus, Trash2, Upload } from 'lucide-react';
import { formatDate } from '../../utils';
import EmptyState from '../ui/EmptyState';
import ConfirmDialog from '../ui/ConfirmDialog';
import { useConfirm } from '../../hooks/useConfirm';

function UserProfileComponent({ items = [], users = [], onCreate, onDelete, onUploadResume }) {
  const [submitting, setSubmitting] = useState(false);
  const { confirm, dialogProps } = useConfirm();

  const getUserName = (userId) => {
    const user = users.find((u) => u.id === userId);
    return user ? user.name : `ID ${userId}`;
  };

  const getFileName = (path) => {
    if (!path) return null;
    return path.split('/').pop().split('\\').pop();
  };

  const handleCreate = async () => {
    setSubmitting(true);
    try {
      await onCreate();
    } finally {
      setSubmitting(false);
    }
  };

  const handleFileChange = async (profileId, e) => {
    const file = e.target.files?.[0];
    if (!file) return;
    await onUploadResume(profileId, file);
    e.target.value = '';
  };

  return (
    <div className="space-y-4">
      {/* Toolbar */}
      <div className="flex justify-between items-center">
        <p className="text-sm text-gray-600">{items.length} perfil(is)</p>
        <button
          onClick={handleCreate}
          disabled={submitting}
          className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <Plus className="w-4 h-4" />
          {submitting ? 'Criando...' : 'Criar Meu Perfil'}
        </button>
      </div>

      {/* List */}
      {items.length === 0 ? (
        <EmptyState
          title="Nenhum perfil encontrado"
          description="Crie seu perfil para configurar o curriculo."
          actionLabel="Criar Meu Perfil"
          onAction={handleCreate}
        />
      ) : (
        <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Usuario</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Curriculo</th>
                <th className="px-4 py-3 text-left font-semibold text-gray-600">Criado em</th>
                <th className="px-4 py-3 text-right font-semibold text-gray-600">Acoes</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {items.map((item) => {
                const fileName = getFileName(item.resumeFilePath);
                return (
                  <tr key={item.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-4 py-3 font-medium text-gray-800">{getUserName(item.userId)}</td>
                    <td className="px-4 py-3 text-gray-500">
                      {fileName || <span className="text-gray-400 italic">Nenhum</span>}
                    </td>
                    <td className="px-4 py-3 text-gray-500">{item.createdAt ? formatDate(item.createdAt) : '—'}</td>
                    <td className="px-4 py-3 text-right">
                      <label
                        className="inline-flex p-1.5 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded transition-colors mr-1 cursor-pointer"
                        title="Enviar curriculo (PDF)"
                      >
                        <Upload className="w-4 h-4" />
                        <input
                          type="file"
                          accept=".pdf"
                          className="hidden"
                          onChange={(e) => handleFileChange(item.id, e)}
                        />
                      </label>
                      <button
                        onClick={() => confirm(() => onDelete(item.id))}
                        className="p-1.5 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded transition-colors"
                        title="Excluir"
                      >
                        <Trash2 className="w-4 h-4" />
                      </button>
                    </td>
                  </tr>
                );
              })}
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
