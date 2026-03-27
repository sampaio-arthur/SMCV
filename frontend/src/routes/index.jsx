import { Navigate, Route, Routes } from 'react-router-dom';
import MainLayout from '../layouts/MainLayout';
import ExamplePage from '../pages/ExamplePage';

// Central route configuration.
// Add new routes here as you expand the application.
export function AppRoutes() {
  return (
    <Routes>
      <Route element={<MainLayout />}>
        <Route path="/" element={<Navigate to="/example" replace />} />
        <Route path="/example" element={<ExamplePage />} />
        {/* <Route path="/your-resource" element={<YourPage />} /> */}
      </Route>
    </Routes>
  );
}
