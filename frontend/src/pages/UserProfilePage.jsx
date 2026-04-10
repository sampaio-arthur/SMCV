import { useEffect, useState } from 'react';
import UserProfileComponent from '../components/userProfile/UserProfileComponent';
import { getAll, create, remove, uploadResume } from '../services/userProfileService';
import { getAll as getAllUsers } from '../services/userService';
import { useToast } from '../hooks/useToast';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import { getErrorMessage } from '../utils';

function UserProfilePage() {
  const [items, setItems] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const toast = useToast();

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [profiles, userList] = await Promise.all([getAll(), getAllUsers()]);
      setItems(profiles);
      setUsers(userList);
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Nao foi possivel carregar os perfis. Verifique se a API esta em execucao.'));
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async () => {
    try {
      await create();
      toast.success('Perfil criado com sucesso!');
      await loadData();
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Nao foi possivel criar o perfil.'));
    }
  };

  const handleDelete = async (id) => {
    try {
      await remove(id);
      toast.success('Perfil excluido com sucesso!');
      await loadData();
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Nao foi possivel excluir o perfil.'));
    }
  };

  const handleUploadResume = async (id, file) => {
    try {
      await uploadResume(id, file);
      toast.success('Curriculo enviado com sucesso!');
      await loadData();
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Erro ao enviar o curriculo.'));
    }
  };

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Perfil de Usuario</h1>
        <p className="text-gray-500 text-sm mt-1">
          Gerencie seu perfil e envie seu curriculo em PDF.
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
