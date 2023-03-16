import { useState } from "react";
import Item from "./Item";

const List = ({ items, toggleItem }) => {
  return (
    <div className="p-6 border-b-2">
      <h1 className="text-lg font-semibold">加購選項</h1>
      {items.map((item) => {
        return <Item key={item.id} item={item} toggleItem={toggleItem} />;
      })}
    </div>
  );
};

export default List;
