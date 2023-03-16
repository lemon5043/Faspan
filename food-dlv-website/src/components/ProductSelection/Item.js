import { useState } from "react";

const Item = ({ item, toggleItem }) => {
  function handleItemClick() {
    toggleItem(item.id);
  }

  return (
    <div className="flex justify-between">
      <div className="flex my-3">
        <input
          className="w-4"
          type="checkbox"
          name={item.itemName}
          onChange={handleItemClick}
        />
        <label className="pl-3" htmlFor={item.itemName}>
          {item.itemName}
        </label>
      </div>
      <p className="text-slate-500">${item.customizationItemPrice}</p>
    </div>
  );
};

export default Item;
