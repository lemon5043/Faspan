import { Fragment, useEffect } from "react";
import ProductSelection from "../../pages/Product/ProductSelection";
import { OverlayBg, OverlayMain } from "../Style/overlay-styling";

const ProductOverlay = ({
  isOpen,
  onClose,
  bubblePreventer,
  currentUser,
  data,
  id,
}) => {
  useEffect(() => {
    console.log(data);
    console.log(id);
    console.log(currentUser.userId);
  }, [id]);

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
