import { useState } from "react";
import CartService from "../services/Cart/cart.service";

const useCart = () => {
  let [cartDetail, setCartDetail] = useState([]);

  //展示購物車內容
  async function CartInfo(index, userId) {
    try {
      const response = await CartService.getCartInfo(userId);
      if (response.data.length !== 0) {
        setCartDetail(response.data[index]);
        return;
      }
      setCartDetail([]);
    } catch (error) {
      console.log(error);
    }
  }

  return {
    cartDetail,
    setCartDetail,
    CartInfo,
  };
};

export default useCart;
