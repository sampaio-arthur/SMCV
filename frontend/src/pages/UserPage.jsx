import { useEffect, useState } from 'react';
import UserComponent from '../components/user/UserComponent';
import { getAll, create, update, remove } from '../services/userService';
import { useToast } from '../hooks/useToast';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import { getErrorMessage } from '../utils';

function UserPage() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const toast = useToast();

  useEffect(() => {
    loadItems();
  }, []);

  const loadItems = async () => {
    try {
      setLoading(true);
      const data = await getAll();
      setItems(data);
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Nao foi possivel carregar os usuarios. Verifique se a API esta em execucao.'));
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (item) => {
    try {
      await create(item);
      toast.success('Usuario criado com sucesso!');
      await loadItems();
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Nao foi possivel criar o usuario.'));
    }
  };

  const handleUpdate = async (id, item) => {
    try {
      await update(id, item);
      toast.success('Usuario atualizado com sucesso!');
      await loadItems();
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Nao foi possivel atualizar o usuario.'));
    }
  };

  const handleDelete = async (id) => {
    try {
      await remove(id);
      toast.success('Usuario excluido com sucesso!');
      await loadItems();
    } catch (err) {
      toast.error(await getErrorMessage(err, 'Nao foi possivel excluir o usuario.'));
    }
  };

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Usuarios</h1>
        <p className="text-gray-500 text-sm mt-1">
          Gerencie os usuarios da plataforma.
        </p>
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : (
        <UserComponent
          items={items}
          onCreate={handleCreate}
          onUpdate={handleUpdate}
          onDelete={handleDelete}
        />
      )}
    </div>
  );
}

export default UserPage;
