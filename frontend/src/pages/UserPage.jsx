import { useEffect, useState, useCallback, useRef } from 'react';
import UserComponent from '../components/user/UserComponent';
import { getAll, create, update, remove } from '../services/userService';
import { useToast } from '../hooks/useToast';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import { getErrorMessage } from '../utils';

function UserPage() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const { success: toastSuccess, error: toastError } = useToast();
  const toastErrorRef = useRef(toastError);

  useEffect(() => {
    toastErrorRef.current = toastError;
  }, [toastError]);

  const loadItems = useCallback(async () => {
    try {
      setLoading(true);
      const data = await getAll();
      setItems(data);
    } catch (err) {
      toastErrorRef.current(await getErrorMessage(err, 'Nao foi possivel carregar os usuarios. Verifique se a API esta em execucao.'));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadItems();
  }, [loadItems]);

  const handleCreate = async (item) => {
    try {
      const novoUsuario = await create(item);
      setItems(prev => [...prev, novoUsuario]);
      toastSuccess('Usuario criado com sucesso!');
    } catch (err) {
      toastError(await getErrorMessage(err, 'Nao foi possivel criar o usuario.'));
    }
  };

  const handleUpdate = async (id, item) => {
    try {
      const usuarioAtualizado = await update(id, item);
      setItems(prev => prev.map(u => u.id === id ? usuarioAtualizado : u));
      toastSuccess('Usuario atualizado com sucesso!');
    } catch (err) {
      toastError(await getErrorMessage(err, 'Nao foi possivel atualizar o usuario.'));
    }
  };

  const handleDelete = async (id) => {
    try {
      await remove(id);
      setItems(prev => prev.filter(u => u.id !== id));
      toastSuccess('Usuario excluido com sucesso!');
    } catch (err) {
      toastError(await getErrorMessage(err, 'Nao foi possivel excluir o usuario.'));
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
