import React from "react";

const ProductDisplay = ({ products, openOverlay }) => {
  const productImg = require(`../../assets/images/public/Products/${products.photo}`);
  return (
    <li
      className="p-6 border-b-2 cursor-pointer flex justify-between w-full lg:w-1/2 hover:bg-neutral-200 transition"
      onClick={openOverlay}
    >
      <div>
        <h3 className="font-semibold text-lg">{products.productName}</h3>
        <p className="text-slate-400">{products.productContent}</p>
        <h4 className="font-medium text-lg">${products.unitPrice}</h4>
      </div>
      <img className="h-24 w-24" src={productImg} alt="" />
    </li>
  );
};

export default ProductDisplay;
