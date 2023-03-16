import { useState, useEffect } from "react";

import CartService from "../../services/Cart/cart.service";
import ShoppingCart from "../../assets/images/shopping_cart.png";
import { LayoutBtn } from "../../components/Style/button-styling";
import { Link } from "react-router-dom";
import useCart from "../../hooks/useCart";

const Cart = ({ currentUser, storeId }) => {
  const { cartDetail, setCartDetail, CartInfo, refreshCart, setRefreshCart } =
    useCart();

  const [identifyNum, setIdentifyNum] = useState("");

  //展示購物車內容

  //條件:剛開啟網頁時改變購物車的內容
  useEffect(function () {
    if (currentUser) {
      CartInfo(0, currentUser.userId);
    }
  }, []);

  //條件:在cartDetail改變時, 重新獲取購物車的產品明細
  useEffect(
    function () {
      if (currentUser) {
        CartInfo(0, currentUser.userId);
      } else {
        setCartDetail([]);
      }
    },
    [currentUser]
  );

  useEffect(
    function () {
      CartInfo(0, currentUser.userId);
    },
    [refreshCart]
  );

  //修改購物車'被選取'商品明細
  function UpdateDetail(detail) {
    console.log(
      detail.identifyNum,
      detail.storeId,
      detail.productId,
      detail.itemId,
      detail.qty
    );
    CartService.postUpdateCart(
      detail.identifyNum,
      detail.storeId,
      detail.productId,
      detail.itemId,
      detail.qty
    )
      .then(function (response) {
        console.log(response.data);
      })
      .catch(function (error) {
        console.log(error);
      });
  }

  //刪除購物車'被選取'商品明細
  function RemoveDetail(identifyNum) {
    console.log(identifyNum);
    CartService.postRemoveDetail(identifyNum)
      .then(function (response) {
        console.log(response.data);
      })
      .catch(function (error) {
        console.log(error);
      });
  }

  //清空購物車
  async function DeleteCart(memberId, storeId) {
    await CartService.deleteDeleteCart(memberId, storeId)
      .then(function (response) {
        console.log(response);
        setCartDetail(null);
      })
      .catch(function (error) {
        console.log(error);
      });
  }

  return (
    <div>
      {/* 如果購物車有東西，就顯示資訊 */}
      {cartDetail && cartDetail.length !== 0 && (
        <div>
          <p className="text-3xl font-semibold">{cartDetail.storeName}</p>
          <button
            onClick={() => DeleteCart(currentUser.userId, cartDetail.storeId)}
          >
            DeleteCart
          </button>
          <div>
            {cartDetail.cartDetails.map((detail) => {
              return (
                <div key={detail.identifyNum}>
                  <p>{detail.productName}</p>
                  <p>{detail.itemName}</p>
                  <p>{detail.qty}</p>
                  <p>{detail.subTotal}</p>

                  {/* 按鈕Update:回到'ProductSelection'頁面, 並記憶該筆商品明細的'客製化選項'與'商品數量' */}
                  {/* 在該頁面重新選擇完成後, 按鈕'確認修改':onClick={UpdateDetail} */}
                  <button onClick={null}>Update</button>
                  <button onClick={() => RemoveDetail(detail.identifyNum)}>
                    Remove
                  </button>
                </div>
              );
            })}
          </div>
          <p>小計</p>
          <p>{cartDetail.total}</p>
        </div>
      )}
      {cartDetail.length === 0 && (
        <div className="flex justify-center items-center h-screen flex-col">
          <img src={ShoppingCart} alt="shoppingCart.png" className="w-2/3" />
          <h2 className="text-xl font-bold my-2">購物車是空的!</h2>
          <p className="mb-4">來把錢錢變成喜歡的東西吧~</p>
          <LayoutBtn>
            <Link to="/store">開逛</Link>
          </LayoutBtn>
        </div>
      )}
    </div>
  );
};

export default Cart;
