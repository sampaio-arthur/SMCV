import { Search } from 'lucide-react';
import { useState, useRef, useEffect, useCallback } from 'react';

function SearchInput({ onChange, placeholder = 'Buscar...', delay = 300 }) {
  const [internal, setInternal] = useState('');
  const timerRef = useRef(null);

  const stableOnChange = useCallback((...args) => onChange(...args), [onChange]);

  const handleChange = (e) => {
    const val = e.target.value;
    setInternal(val);
    clearTimeout(timerRef.current);
    timerRef.current = setTimeout(() => stableOnChange(val), delay);
  };

  useEffect(() => () => clearTimeout(timerRef.current), []);

  return (
    <div className="relative">
      <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
      <input
        type="text"
        value={internal}
        onChange={handleChange}
        placeholder={placeholder}
        className="w-full pl-9 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
      />
    </div>
  );
}

export default SearchInput;
