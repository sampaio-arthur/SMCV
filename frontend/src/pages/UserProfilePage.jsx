import { useCallback, useEffect, useRef, useState } from 'react';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import UserProfileComponent from '../components/userProfile/UserProfileComponent';
import { useToast } from '../hooks/useToast';
import { MAX_RESUME_FILE_SIZE_BYTES, create, getAll, remove, uploadResume } from '../services/userProfileService';
import { getAll as getAllUsers } from '../services/userService';
import { formatFileSize, getErrorMessage } from '../utils';

function UserProfilePage() {
  const [items, setItems] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const { success: toastSuccess, error: toastError } = useToast();
  const toastErrorRef = useRef(toastError);

  useEffect(() => {
    toastErrorRef.current = toastError;
  }, [toastError]);

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const [profiles, userList] = await Promise.all([getAll(), getAllUsers()]);
      setItems(profiles);
      setUsers(userList);
    } catch (err) {
      toastErrorRef.current(await getErrorMessage(err, 'Não foi possível carregar os perfis. Verifique se a API está em execução.'));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const handleCreate = async () => {
    try {
      const novoPerfil = await create();
      setItems(prev => [...prev, novoPerfil]);
      toastSuccess('Perfil criado com sucesso!');
    } catch (err) {
      toastError(await getErrorMessage(err, 'Não foi possível criar o perfil.'));
    }
  };

  const handleDelete = async (id) => {
    try {
      await remove(id);
      setItems(prev => prev.filter(p => p.id !== id));
      toastSuccess('Perfil excluído com sucesso!');
    } catch (err) {
      toastError(await getErrorMessage(err, 'Não foi possível excluir o perfil.'));
    }
  };

  const handleUploadResume = async (id, file) => {
    if (file.size > MAX_RESUME_FILE_SIZE_BYTES) {
      toastError(`Arquivo muito grande. O tamanho máximo permitido é ${formatFileSize(MAX_RESUME_FILE_SIZE_BYTES)}.`);
      return;
    }

    try {
      const perfilAtualizado = await uploadResume(id, file);
      setItems(prev => prev.map(p => p.id === id ? perfilAtualizado : p));
      toastSuccess('Currículo enviado com sucesso!');
    } catch (err) {
      toastError(await getErrorMessage(err, 'Erro ao enviar o currículo.'));
    }
  };

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Perfil de Usuário</h1>
        <p className="text-gray-500 text-sm mt-1">
          Gerencie seu perfil e envie seu currículo em PDF.
        </p>
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : (
        <UserProfileComponent
          items={items}
          users={users}
          onCreate={handleCreate}
          onDelete={handleDelete}
          onUploadResume={handleUploadResume}
        />
      )}
    </div>
  );
}

export default UserProfilePage;
