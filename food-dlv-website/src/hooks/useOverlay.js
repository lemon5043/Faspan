import { useState } from "react";

const useOverlay = () => {
  const [isOpen, setIsOpen] = useState(false);

  const toggleOverlay = () => {
    setIsOpen((currentOpen) => !currentOpen);
  };

  const bubblePreventer = (e) => {
    e.stopPropagation();
  };

  return { isOpen, setIsOpen, toggleOverlay, bubblePreventer };
};

export default useOverlay;
