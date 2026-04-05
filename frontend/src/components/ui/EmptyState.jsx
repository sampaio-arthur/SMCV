import { Inbox } from 'lucide-react';

function EmptyState({ icon, title, description, actionLabel, onAction }) {
  const ResolvedIcon = icon || Inbox;
  return (
    <div className="text-center py-12 bg-white rounded-lg border border-gray-200">
      <ResolvedIcon className="w-12 h-12 text-gray-300 mx-auto mb-4" />
      <p className="text-lg font-medium text-gray-400">{title}</p>
      {description && (
        <p className="text-sm text-gray-400 mt-1">{description}</p>
      )}
      {actionLabel && onAction && (
        <button
          onClick={onAction}
          className="mt-4 px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition-colors"
        >
          {actionLabel}
        </button>
      )}
    </div>
  );
}

export default EmptyState;
