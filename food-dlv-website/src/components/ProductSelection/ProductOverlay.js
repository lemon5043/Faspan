import { Fragment, useEffect } from "react";
import ProductSelection from "./ProductSelection";
import { OverlayBg, OverlayMain } from "../Style/overlay-styling";

const ProductOverlay = ({
  isOpen,
  onClose,
  bubblePreventer,
  currentUser,
  data,
  id,
  setCartDetail,
}) => {
  return (
    <Fragment>
      {isOpen && (
        <div>
          <OverlayBg onClick={onClose}>
            <OverlayMain style={{ width: "37.5rem" }} onClick={bubblePreventer}>
              <div className="flex items-center">
                <ProductSelection
                  data={data}
                  productId={id}
                  currentUser={currentUser}
                  onClose={onClose}
                  setCartDetail={setCartDetail}
                />
              </div>
            </OverlayMain>
          </OverlayBg>
        </div>
      )}
    </Fragment>
  );
};

export default ProductOverlay;
