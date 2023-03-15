import { useEffect, useState } from "react";
import ProductSelectionService from "../../services/Cart/productSelection.service";
import CartService from "../../services/Cart/cart.service";
import ProductInfo from "../../components/ProductSelection/ProductInfo";
import List from "../../components/ProductSelection/List";

const ProductSelection = ({ data, productId, currentUser }) => {
  const memberId = currentUser.userId;
  const [product, setProduct] = useState(null);
  const [selectItems, setSelectItems] = useState([]);
  const [qty, setQty] = useState(1);

  async function getProduct() {
    try {
      const response = await ProductSelectionService.getProductSelect(
        productId
      );
      setProduct(response.data);
      console.log(response.data);
    } catch (error) {
      console.log(error);
    }
  }

  function numberQty(e) {
    setQty(parseInt(e.target.value));
  }

  const toggleItem = (id) => {
    const newitems = [...selectItems];
    const item = newitems.find((item) => item === id);
    if (typeof item === "undefined") {
      setSelectItems([...selectItems, id]);
    } else {
      setSelectItems(newitems.filter((item) => item !== id));
    }
  };

  function AddToCart() {
    console.log(memberId, product.storeId, productId, selectItems, qty);
    CartService.postAddToCart(
      memberId,
      product.storeId,
      productId,
      selectItems,
      qty
    )
      .then(function (response) {
        console.log(response.data);
      })
      .catch(function (error) {
        console.log(error);
      });
  }

  useEffect(() => {
    getProduct();
  }, [productId]);

  return (
    <div>
      {product && (
        <div>
          {/* 紀錄產品訊息 */}
          <ProductInfo product={product} />
          {/* 增加客製化選項的 checkbox */}
          <List items={product.customizationItems} toggleItem={toggleItem} />
          <div>
            <label>Quantity:</label>
            <input type="number" value={qty} min={1} onChange={numberQty} />
          </div>
          <button onClick={AddToCart}>AddToCart</button>
        </div>
      )}
    </div>
  );
};

export default ProductSelection;
