import { useState } from "react";

const Item = ({ item, toggleItem }) => {
  function handleItemClick() {
    toggleItem(item.id);
  }

  return (
    <div>
      <input type="checkbox" onChange={handleItemClick} />
      <p>{item.itemName}</p>
      <p>{item.customizationItemPrice}</p>
    </div>
  );
};

export default Item;
