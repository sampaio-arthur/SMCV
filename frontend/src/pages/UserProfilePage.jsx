import { useEffect, useState } from 'react';
import UserProfileComponent from '../components/userProfile/UserProfileComponent';
import { getAll, create, update, remove } from '../services/userProfileService';
import { getAll as getAllUsers } from '../services/userService';
import { useToast } from '../hooks/useToast';
import LoadingSpinner from '../components/ui/LoadingSpinner';

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
    } catch {
      toast.error('Nao foi possivel carregar os perfis. Verifique se a API esta em execucao.');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (item) => {
    try {
      await create(item);
      toast.success('Perfil criado com sucesso!');
      await loadData();
    } catch {
      toast.error('Nao foi possivel criar o perfil. Verifique os campos e tente novamente.');
    }
  };

  const handleUpdate = async (id, item) => {
    try {
      await update(id, item);
      toast.success('Perfil atualizado com sucesso!');
      await loadData();
    } catch {
      toast.error('Nao foi possivel atualizar o perfil. Verifique sua conexao.');
    }
  };

  const handleDelete = async (id) => {
    try {
      await remove(id);
      toast.success('Perfil excluido com sucesso!');
      await loadData();
    } catch {
      toast.error('Nao foi possivel excluir o perfil.');
    }
  };

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Perfis de Usuario</h1>
        <p className="text-gray-500 text-sm mt-1">
          Configure o curriculo e as credenciais SMTP de cada usuario.
        </p>
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : (
        <UserProfileComponent
          items={items}
          users={users}
          onCreate={handleCreate}
          onUpdate={handleUpdate}
          onDelete={handleDelete}
        />
      )}
    </div>
  );
}

export default UserProfilePage;
