import type { ProductDto } from '../api/client';

interface ProductCardProps {
  product: ProductDto;
  onEdit: (id: number) => void;
  onDelete: (id: number) => void;
}

export function ProductCard({ product, onEdit, onDelete }: ProductCardProps) {
  return (
    <div className="border rounded-lg p-4 bg-white shadow-sm">
      <h3 className="font-semibold text-lg text-gray-900">{product.name}</h3>
      <p className="text-gray-600 mt-1">{product.price.toFixed(2)}</p>
      {product.description != null && product.description !== '' && (
        <p className="text-gray-500 text-sm mt-2 line-clamp-2">{product.description}</p>
      )}
      {product.categories != null && product.categories.length > 0 && (
        <p className="text-xs text-gray-400 mt-2">
          {product.categories.map((c) => c.name).join(', ')}
        </p>
      )}
      <div className="flex gap-2 mt-3">
        <button
          type="button"
          onClick={() => onEdit(product.id)}
          className="px-3 py-1.5 text-sm bg-blue-600 text-white rounded hover:bg-blue-700"
        >
          Edit
        </button>
        <button
          type="button"
          onClick={() => onDelete(product.id)}
          className="px-3 py-1.5 text-sm bg-red-600 text-white rounded hover:bg-red-700"
        >
          Delete
        </button>
      </div>
    </div>
  );
}
