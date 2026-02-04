const baseUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5000';

export interface CategoryDto {
  id: number;
  name: string;
}

export interface ProductDto {
  id: number;
  name: string;
  description: string | null;
  price: number;
  stockQuantity: number;
  categories: CategoryDto[];
}

export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CreateProductDto {
  name: string;
  description?: string | null;
  price: number;
  stockQuantity: number;
  categoryIds: number[];
}

export interface UpdateProductDto {
  name: string;
  description?: string | null;
  price: number;
  stockQuantity: number;
  categoryIds: number[];
}

export async function getProducts(params: {
  page?: number;
  pageSize?: number;
  search?: string | null;
}): Promise<PagedResultDto<ProductDto>> {
  const sp = new URLSearchParams();
  if (params.page != null) sp.set('page', String(params.page));
  if (params.pageSize != null) sp.set('pageSize', String(params.pageSize));
  if (params.search != null && params.search.trim() !== '') sp.set('search', params.search.trim());
  const res = await fetch(`${baseUrl}/product-management/managed-products?${sp.toString()}`);
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export async function getProduct(id: number): Promise<ProductDto | null> {
  const res = await fetch(`${baseUrl}/product-management/managed-products/${id}`);
  if (res.status === 404) return null;
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export async function createProduct(dto: CreateProductDto): Promise<ProductDto> {
  const res = await fetch(`${baseUrl}/product-management/managed-products`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export async function updateProduct(id: number, dto: UpdateProductDto): Promise<ProductDto> {
  const res = await fetch(`${baseUrl}/product-management/managed-products/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export async function deleteProduct(id: number): Promise<void> {
  const res = await fetch(`${baseUrl}/product-management/managed-products/${id}`, { method: 'DELETE' });
  if (!res.ok) throw new Error(await res.text());
}

export async function getCategories(): Promise<CategoryDto[]> {
  const res = await fetch(`${baseUrl}/product-management/product-categories`);
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export async function getCategory(id: number): Promise<CategoryDto | null> {
  const res = await fetch(`${baseUrl}/product-management/product-categories/${id}`);
  if (res.status === 404) return null;
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export interface CreateCategoryDto {
  name: string;
}

export interface UpdateCategoryDto {
  name: string;
}

export async function createCategory(dto: CreateCategoryDto): Promise<CategoryDto> {
  const res = await fetch(`${baseUrl}/product-management/product-categories`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export async function updateCategory(id: number, dto: UpdateCategoryDto): Promise<CategoryDto> {
  const res = await fetch(`${baseUrl}/product-management/product-categories/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export async function deleteCategory(id: number): Promise<void> {
  const res = await fetch(`${baseUrl}/product-management/product-categories/${id}`, { method: 'DELETE' });
  if (!res.ok) throw new Error(await res.text());
}
