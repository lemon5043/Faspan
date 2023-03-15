import React, { useState } from "react";
import BgImage from "../../components/Product/BgImage";
import ProductDisplay from "../../components/Product/ProductDisplay";
import StoreInfo from "../../components/Product/StoreInfo";
import ProductOverlay from "../../components/Product/ProductOverlay";
import useOverlay from "../../hooks/useOverlay";

const ProductPage = ({ data, currentUser }) => {
  const { isOpen, setIsOpen, bubblePreventer } = useOverlay();
  const [id, setId] = useState(0);

  const openOverlay = (e) => {
    setIsOpen(true);
    setId(e);
  };

  const closeOverlay = () => {
    setIsOpen(!isOpen);
  };

  return (
    <div>
      <BgImage data={data} />
      <StoreInfo data={data} />
      <ProductOverlay
        isOpen={isOpen}
        onClose={closeOverlay}
        currentUser={currentUser}
        bubblePreventer={bubblePreventer}
        data={data}
        id={id}
      />
      <ul className="flex flex-wrap justify-center">
        {data.products.length !== 0 &&
          data.products.map((d) => {
            return (
              <ProductDisplay
                openOverlay={() => openOverlay(d.id)}
                products={d}
                key={d.id}
              />
            );
          })}
      </ul>
      {data.products.length === 0 && (
        <div className="flex items-center justify-center">
          該店家目前沒有產品上架喔!
        </div>
      )}
    </div>
  );
};

export default ProductPage;
