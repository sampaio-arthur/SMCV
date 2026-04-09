import { Navigate, Route, Routes } from 'react-router-dom';
import MainLayout from '../layouts/MainLayout';
import ProtectedRoute from '../components/auth/ProtectedRoute';
import LoginPage from '../pages/LoginPage';
import CampaignPage from '../pages/CampaignPage';
import ContactPage from '../pages/ContactPage';
import UserPage from '../pages/UserPage';
import UserProfilePage from '../pages/UserProfilePage';

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      <Route element={<ProtectedRoute />}>
        <Route element={<MainLayout />}>
          <Route path="/" element={<Navigate to="/campaign" replace />} />
          <Route path="/campaign" element={<CampaignPage />} />
          <Route path="/contact" element={<ContactPage />} />
          <Route path="/user" element={<UserPage />} />
          <Route path="/user-profile" element={<UserProfilePage />} />
        </Route>
      </Route>
    </Routes>
  );
}
