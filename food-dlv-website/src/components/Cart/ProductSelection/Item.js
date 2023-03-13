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
// const Item = ({item, ItemsForSelected}) => {

//     const[checked, setChecked] = useState(false);
//     function itemChange(e){
//         setChecked(e.target.checked);
//         ItemsForSelected(item.id , e.target.checked)
//     }

//     return(
//         <div>
//             <input type='checkbox' checked={checked} onChange={itemChange}/>
//             <p>{item.itemName}</p>
//             <p>{item.customizationItemPrice}</p>
//         </div>
//     )
// }

export default Item;
