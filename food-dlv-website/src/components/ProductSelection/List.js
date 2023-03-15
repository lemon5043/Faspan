import { useState } from "react";
import Item from "./Item";

const List = ({ items, toggleItem }) => {

  return (
    <div>
      <h1>List</h1>
      {items.map((item) => {
        return <Item key={item.id} item={item} toggleItem={toggleItem} />;
      })}
    </div>
  );
};

export default List;
