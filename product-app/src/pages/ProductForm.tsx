import { useEffect, useState, useRef } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  getProduct,
  createProduct,
  updateProduct,
  getCategories,
  type CreateProductDto,
  type CategoryDto,
} from '../api/client';

interface FormErrors {
  name?: string;
  price?: string;
  stockQuantity?: string;
}

export function ProductForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEdit = id != null && id !== 'new';
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState('');
  const [stockQuantity, setStockQuantity] = useState('');
  const [categoryIds, setCategoryIds] = useState<number[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(isEdit);
  const [submitting, setSubmitting] = useState(false);
  const [errors, setErrors] = useState<FormErrors>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [categoriesOpen, setCategoriesOpen] = useState(false);
  const categoriesRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (categoriesRef.current != null && !categoriesRef.current.contains(event.target as Node)) {
        setCategoriesOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  useEffect(() => {
    getCategories()
      .then(setCategories)
      .catch(() => setCategories([]));
  }, []);

  useEffect(() => {
    if (!isEdit) return;
    const numId = parseInt(id!, 10);
    if (Number.isNaN(numId)) {
      setLoading(false);
      return;
    }
    getProduct(numId)
      .then((p) => {
        if (p == null) {
          setLoading(false);
          return;
        }
        setName(p.name);
        setDescription(p.description ?? '');
        setPrice(String(p.price));
        setStockQuantity(String(p.stockQuantity));
        setCategoryIds(p.categories?.map((c) => c.id) ?? []);
      })
      .finally(() => setLoading(false));
  }, [id, isEdit]);

  const validate = (): boolean => {
    const next: FormErrors = {};
    if (name.trim() === '') next.name = 'Name is required.';
    const p = parseFloat(price);
    if (Number.isNaN(p) || p <= 0) next.price = 'Price must be greater than 0.';
    const s = parseInt(stockQuantity, 10);
    if (Number.isNaN(s) || s < 0) next.stockQuantity = 'Stock quantity must be 0 or greater.';
    setErrors(next);
    return Object.keys(next).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitError(null);
    if (!validate()) return;
    setSubmitting(true);
    try {
      const dto: CreateProductDto = {
        name: name.trim(),
        description: description.trim() || undefined,
        price: parseFloat(price),
        stockQuantity: parseInt(stockQuantity, 10),
        categoryIds: [...categoryIds],
      };
      if (isEdit) {
        await updateProduct(parseInt(id!, 10), dto);
      } else {
        await createProduct(dto);
      }
      navigate('/');
    } catch (err) {
      setSubmitError(err instanceof Error ? err.message : 'Request failed');
    } finally {
      setSubmitting(false);
    }
  };

  const toggleCategory = (categoryId: number) => {
    setCategoryIds((prev) =>
      prev.includes(categoryId) ? prev.filter((c) => c !== categoryId) : [...prev, categoryId]
    );
  };

  const selectedNames = categories.filter((c) => categoryIds.includes(c.id)).map((c) => c.name);
  const triggerLabel = selectedNames.length === 0
    ? 'Select categories...'
    : selectedNames.length <= 2
      ? selectedNames.join(', ')
      : `${selectedNames.length} categories selected`;

  if (loading) {
    return (
      <div className="flex justify-center py-12">
        <div className="w-10 h-10 border-4 border-gray-200 border-t-blue-600 rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <div className="max-w-xl mx-auto p-6">
      <h1 className="text-2xl font-bold text-gray-900 mb-6">
        {isEdit ? 'Edit Product' : 'New Product'}
      </h1>
      <form onSubmit={handleSubmit} className="space-y-4">
        {submitError != null && (
          <div className="p-3 bg-red-100 text-red-700 rounded">{submitError}</div>
        )}
        <div>
          <label htmlFor="name" className="block text-sm font-medium text-gray-700">
            Name *
          </label>
          <input
            id="name"
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="mt-1 block w-full border rounded px-3 py-2 text-gray-900"
          />
          {errors.name != null && <p className="text-red-600 text-sm mt-1">{errors.name}</p>}
        </div>
        <div>
          <label htmlFor="description" className="block text-sm font-medium text-gray-700">
            Description
          </label>
          <textarea
            id="description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            rows={3}
            className="mt-1 block w-full border rounded px-3 py-2 text-gray-900"
          />
        </div>
        <div>
          <label htmlFor="price" className="block text-sm font-medium text-gray-700">
            Price *
          </label>
          <input
            id="price"
            type="number"
            step="0.01"
            min="0"
            value={price}
            onChange={(e) => setPrice(e.target.value)}
            className="mt-1 block w-full border rounded px-3 py-2 text-gray-900"
          />
          {errors.price != null && <p className="text-red-600 text-sm mt-1">{errors.price}</p>}
        </div>
        <div>
          <label htmlFor="stockQuantity" className="block text-sm font-medium text-gray-700">
            Stock quantity *
          </label>
          <input
            id="stockQuantity"
            type="number"
            min="0"
            value={stockQuantity}
            onChange={(e) => setStockQuantity(e.target.value)}
            className="mt-1 block w-full border rounded px-3 py-2 text-gray-900"
          />
          {errors.stockQuantity != null && (
            <p className="text-red-600 text-sm mt-1">{errors.stockQuantity}</p>
          )}
        </div>
        <div ref={categoriesRef} className="relative">
          <label className="block text-sm font-medium text-gray-700 mb-2">Categories</label>
          <button
            type="button"
            onClick={() => setCategoriesOpen((open) => !open)}
            className="w-full flex items-center justify-between gap-2 border border-gray-300 rounded px-3 py-2.5 bg-white text-left text-gray-900 hover:border-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <span className={selectedNames.length === 0 ? 'text-gray-500' : ''}>
              {triggerLabel}
            </span>
            <svg
              className={`w-5 h-5 text-gray-500 shrink-0 transition-transform ${categoriesOpen ? 'rotate-180' : ''}`}
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
            </svg>
          </button>
          {categoriesOpen && (
            <div className="absolute z-10 mt-1 w-full rounded-lg border border-gray-200 bg-white shadow-lg max-h-48 overflow-auto">
              {categories.length === 0 ? (
                <div className="px-3 py-2 text-sm text-gray-500">No categories available</div>
              ) : (
                <ul className="py-1">
                  {categories.map((c) => (
                    <li key={c.id}>
                      <button
                        type="button"
                        onClick={() => toggleCategory(c.id)}
                        className="w-full flex items-center gap-2 px-3 py-2 text-left text-sm text-gray-900 hover:bg-gray-100"
                      >
                        <span
                          className={`flex h-4 w-4 shrink-0 items-center justify-center rounded border ${
                            categoryIds.includes(c.id) ? 'bg-blue-600 border-blue-600' : 'border-gray-300'
                          }`}
                        >
                          {categoryIds.includes(c.id) && (
                            <svg className="h-3 w-3 text-white" fill="currentColor" viewBox="0 0 12 12">
                              <path d="M10 3L4.5 8.5 2 6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" fill="none" />
                            </svg>
                          )}
                        </span>
                        {c.name}
                      </button>
                    </li>
                  ))}
                </ul>
              )}
            </div>
          )}
        </div>
        <div className="flex gap-2 pt-4">
          <button
            type="submit"
            disabled={submitting}
            className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
          >
            {submitting ? 'Saving...' : isEdit ? 'Update' : 'Create'}
          </button>
          <button
            type="button"
            onClick={() => navigate('/')}
            className="px-4 py-2 border rounded hover:bg-gray-100"
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
