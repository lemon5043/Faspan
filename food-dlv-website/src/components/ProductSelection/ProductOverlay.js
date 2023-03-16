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
}) => {
  return (
    <Fragment>
      {isOpen && (
        <div>
          <OverlayBg onClick={onClose}>
            <OverlayMain onClick={bubblePreventer}>
              <div className="flex items-center">
                <ProductSelection
                  data={data}
                  productId={id}
                  currentUser={currentUser}
                  onClose={onClose}
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
