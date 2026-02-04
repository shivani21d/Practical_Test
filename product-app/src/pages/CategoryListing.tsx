import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  getCategories,
  createCategory,
  updateCategory,
  deleteCategory,
  type CategoryDto,
} from '../api/client';
import { LoadingSpinner } from '../components/LoadingSpinner';

export function CategoryListing() {
  const navigate = useNavigate();
  const [items, setItems] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [formName, setFormName] = useState('');
  const [formError, setFormError] = useState<string | null>(null);
  const [isCreating, setIsCreating] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  const loadCategories = async () => {
    setError(null);
    try {
      const list = await getCategories();
      setItems(list ?? []);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to load categories');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCategories();
  }, []);

  const handleCreate = () => {
    setIsCreating(true);
    setEditingId(null);
    setFormName('');
    setFormError(null);
  };

  const handleEdit = (cat: CategoryDto) => {
    setEditingId(cat.id);
    setFormName(cat.name);
    setIsCreating(false);
    setFormError(null);
  };

  const handleCancelEdit = () => {
    setEditingId(null);
    setIsCreating(false);
    setFormName('');
    setFormError(null);
  };

  const handleSave = async () => {
    const name = formName.trim();
    if (!name) {
      setFormError('Name is required.');
      return;
    }
    setSubmitting(true);
    setFormError(null);
    try {
      if (isCreating) {
        await createCategory({ name });
        await loadCategories();
        handleCancelEdit();
      } else if (editingId != null) {
        await updateCategory(editingId, { name });
        await loadCategories();
        handleCancelEdit();
      }
    } catch (e) {
      setFormError(e instanceof Error ? e.message : 'Request failed');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Delete this category?')) return;
    try {
      await deleteCategory(id);
      setItems((prev) => prev.filter((c) => c.id !== id));
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to delete');
    }
  };

  return (
    <div className="max-w-3xl mx-auto p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Categories</h1>
        <div className="flex gap-2">
          <button
            type="button"
            onClick={() => navigate('/')}
            className="px-4 py-2 border border-gray-600 text-gray-700 rounded hover:bg-gray-200"
          >
            Products
          </button>
          <button
            type="button"
            onClick={handleCreate}
            className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
          >
            Add Category
          </button>
        </div>
      </div>

      {(isCreating || editingId != null) && (
        <div className="mb-6 p-4 bg-gray-50 rounded-lg border">
          <h2 className="text-lg font-medium text-gray-900 mb-3">
            {isCreating ? 'New Category' : 'Edit Category'}
          </h2>
          {formError != null && (
            <div className="mb-2 p-2 bg-red-100 text-red-700 text-sm rounded">{formError}</div>
          )}
          <div className="flex gap-2 items-end">
            <div className="flex-1">
              <label htmlFor="cat-name" className="block text-sm font-medium text-gray-700 mb-1">
                Name
              </label>
              <input
                id="cat-name"
                type="text"
                value={formName}
                onChange={(e) => setFormName(e.target.value)}
                className="w-full border rounded px-3 py-2 text-gray-900"
              />
            </div>
            <button
              type="button"
              onClick={handleSave}
              disabled={submitting}
              className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
            >
              {submitting ? 'Saving...' : 'Save'}
            </button>
            <button
              type="button"
              onClick={handleCancelEdit}
              className="px-4 py-2 border rounded hover:bg-gray-200"
            >
              Cancel
            </button>
          </div>
        </div>
      )}

      {error != null && (
        <div className="mb-4 p-3 bg-red-100 text-red-700 rounded">{error}</div>
      )}

      {loading ? (
        <LoadingSpinner />
      ) : (
        <div className="bg-white rounded-lg border shadow overflow-hidden">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th scope="col" className="px-4 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">
                    Id
                  </th>
                  <th scope="col" className="px-4 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">
                    Name
                  </th>
                  <th scope="col" className="px-4 py-3 text-right text-xs font-semibold text-gray-700 uppercase tracking-wider">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {items.length === 0 ? (
                  <tr>
                    <td colSpan={3} className="px-4 py-8 text-center text-gray-500">
                      No categories. Add one above.
                    </td>
                  </tr>
                ) : (
                  items.map((cat) => (
                    <tr key={cat.id} className="hover:bg-gray-50">
                      <td className="px-4 py-3 text-sm text-gray-500">{cat.id}</td>
                      <td className="px-4 py-3 text-sm font-medium text-gray-900">{cat.name}</td>
                      <td className="px-4 py-3 text-right">
                        <button
                          type="button"
                          onClick={() => handleEdit(cat)}
                          className="text-blue-600 hover:text-blue-800 font-medium text-sm mr-3"
                        >
                          Edit
                        </button>
                        <button
                          type="button"
                          onClick={() => handleDelete(cat.id)}
                          className="text-red-600 hover:text-red-800 font-medium text-sm"
                        >
                          Delete
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
