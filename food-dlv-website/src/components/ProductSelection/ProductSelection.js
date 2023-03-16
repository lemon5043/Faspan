import { useEffect, useState } from "react";
import ProductSelectionService from "../../services/Cart/productSelection.service";
import CartService from "../../services/Cart/cart.service";
import ProductInfo from "./ProductInfo";
import List from "./List";
import { Btn } from "../Style/button-styling";
import AddIcon from "@mui/icons-material/Add";
import RemoveIcon from "@mui/icons-material/Remove";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import useCart from "../../hooks/useCart";

const ProductSelection = ({ productId, currentUser, onClose }) => {
  const [product, setProduct] = useState(null);
  const [selectItems, setSelectItems] = useState([]);
  const [qty, setQty] = useState(1);

  const navigate = useNavigate();
  const { refreshCart, setRefreshCart } = useCart();

  // 點擊後彈出視窗顯示產品內容
  async function getProduct() {
    try {
      const response = await ProductSelectionService.getProductSelect(
        productId
      );
      setProduct(response.data);
    } catch (error) {
      console.log(error);
    }
  }

  const minusOne = () => {
    if (qty === 1) return;
    setQty((currentQty) => {
      return currentQty - 1;
    });
  };

  const plusOne = () => {
    setQty((currentQty) => {
      return currentQty + 1;
    });
  };

  const toggleItem = (id) => {
    const newitems = [...selectItems];
    const item = newitems.find((item) => item === id);
    if (typeof item === "undefined") {
      setSelectItems([...selectItems, id]);
    } else {
      setSelectItems(newitems.filter((item) => item !== id));
    }
  };

  async function AddToCart() {
    if (!currentUser) {
      Swal.fire({
        text: "請先登入才能點餐喔!",
        heightAuto: false,
        showCancelButton: true,
        confirmButtonColor: "#050505",
        cancelButtonColor: "#aeb2b6",
        confirmButtonText: "登入",
        cancelButtonText: "取消",
      }).then((result) => {
        if (result.isConfirmed) {
          navigate("/login");
        }
      });
      return;
    }
    try {
      await CartService.postAddToCart(
        currentUser.userId,
        product.storeId,
        productId,
        selectItems,
        qty
      );
      setRefreshCart("123");
      console.log(refreshCart);
      Swal.fire({
        title: "已加入購物車!",
        icon: "success",
        timer: 2000,
        showConfirmButton: false,
      }).then(() => onClose());
    } catch (error) {
      console.log(error);
    }
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
          <div className="flex justify-between items-center p-4">
            <div className="flex items-center">
              <RemoveIcon onClick={minusOne} className="cursor-pointer m-2" />
              <span className="text-slate-700 font-bold text-lg">{qty}</span>
              <AddIcon onClick={plusOne} className="cursor-pointer m-2" />
            </div>
            <Btn onClick={AddToCart}>AddToCart</Btn>
          </div>
        </div>
      )}
    </div>
  );
};

export default ProductSelection;
