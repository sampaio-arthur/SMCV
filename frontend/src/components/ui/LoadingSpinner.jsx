function LoadingSpinner({ message = 'Carregando...' }) {
  return (
    <div className="flex items-center justify-center py-20">
      <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
      <span className="ml-3 text-gray-500">{message}</span>
    </div>
  );
}

export default LoadingSpinner;
