import { useEffect, useState } from 'react';
import ExampleComponent from '../components/example/ExampleComponent';
import { getAll, create, update, remove } from '../services/exampleService';
import { useToast } from '../hooks/useToast';

function ExamplePage() {
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
    } catch {
      toast.error('Failed to load items. Check if the API is running.');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (item) => {
    try {
      await create(item);
      toast.success('Item created successfully!');
      await loadItems();
    } catch {
      toast.error('Failed to create item.');
    }
  };

  const handleUpdate = async (id, item) => {
    try {
      await update(id, item);
      toast.success('Item updated successfully!');
      await loadItems();
    } catch {
      toast.error('Failed to update item.');
    }
  };

  const handleDelete = async (id) => {
    try {
      await remove(id);
      toast.success('Item deleted successfully!');
      await loadItems();
    } catch {
      toast.error('Failed to delete item.');
    }
  };

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Example Page</h1>
        <p className="text-gray-500 text-sm mt-1">
          Template page — replace with your domain logic.
        </p>
      </div>

      {loading ? (
        <div className="flex items-center justify-center py-20">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
          <span className="ml-3 text-gray-500">Loading...</span>
        </div>
      ) : (
        <ExampleComponent
          items={items}
          onCreate={handleCreate}
          onUpdate={handleUpdate}
          onDelete={handleDelete}
        />
      )}
    </div>
  );
}

export default ExamplePage;
