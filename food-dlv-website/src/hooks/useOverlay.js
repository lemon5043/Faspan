import { useState } from "react";

const useOverlay = () => {
  const [isOpen, setIsOpen] = useState(false);

  const toggleOverlay = (e) => {
    setIsOpen(!isOpen);
  };

  const bubblePreventer = (e) => {
    e.stopPropagation();
  };

  return { isOpen, setIsOpen, toggleOverlay, bubblePreventer };
};

export default useOverlay;
