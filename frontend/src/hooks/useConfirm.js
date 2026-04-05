import { useState, useCallback, useRef } from 'react';

export function useConfirm() {
  const [isOpen, setIsOpen] = useState(false);
  const callbackRef = useRef(null);

  const confirm = useCallback((onConfirm) => {
    callbackRef.current = onConfirm;
    setIsOpen(true);
  }, []);

  const handleConfirm = useCallback(() => {
    setIsOpen(false);
    callbackRef.current?.();
    callbackRef.current = null;
  }, []);

  const cancel = useCallback(() => {
    setIsOpen(false);
    callbackRef.current = null;
  }, []);

  return {
    isOpen,
    confirm,
    cancel,
    dialogProps: {
      isOpen,
      onClose: cancel,
      onConfirm: handleConfirm,
    },
  };
}
