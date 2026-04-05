function StatusBadge({ status, config }) {
  const entry = config[status] || { label: status, bg: 'bg-gray-100', text: 'text-gray-600' };

  return (
    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${entry.bg} ${entry.text} ${entry.extra || ''}`}>
      {entry.label}
    </span>
  );
}

export default StatusBadge;
