import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ProductListing } from './pages/ProductListing';
import { ProductForm } from './pages/ProductForm';
import { CategoryListing } from './pages/CategoryListing';

function App() {
  return (
    <BrowserRouter>
      <div className="min-h-screen bg-gray-100">
        <Routes>
          <Route path="/" element={<ProductListing />} />
          <Route path="/products/new" element={<ProductForm />} />
          <Route path="/products/:id/edit" element={<ProductForm />} />
          <Route path="/categories" element={<CategoryListing />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
