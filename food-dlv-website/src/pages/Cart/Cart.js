import React, { useState, useEffect } from "react";

import CartService from "../../services/Cart/cart.service";
import ShoppingCart from "../../assets/images/shopping_cart.png";
import { LayoutBtn, Btn } from "../../components/Style/button-styling";
import { Link, useNavigate } from "react-router-dom";
import cartService from "../../services/Cart/cart.service";
import Swal from "sweetalert2";

const Cart = ({ currentUser, currentAddress, setCartDetail, cartDetail }) => {
  const navigate = useNavigate();

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
  function RemoveDetail(memberId, identifyNum) {
    CartService.postRemoveDetail(identifyNum)
      .then(function (response) {
        console.log(response.data);
        cartService.getCartInfo(memberId);
        Swal.fire({
          title: "刪除成功!",
          icon: "success",
          timer: 2000,
          showConfirmButton: false,
        }).then(() => {
          setCartDetail(CartService.getCurrentCart());
        });
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
        localStorage.removeItem("cartInfo");
      })
      .catch(function (error) {
        console.log(error);
      });
  }

  const checkoutHandler = () => {
    if (cartDetail.length === 0) return;
    navigate("/checkout");
  };

  return (
    <div>
      {/* 如果購物車有東西，就顯示資訊 */}
      {cartDetail && cartDetail.length !== 0 && (
        <div className="mx-2">
          <p className="text-3xl mb-2 font-semibold">{cartDetail.storeName}</p>
          {/* 清空購物車 */}
          <button
            onClick={() => DeleteCart(currentUser.userId, cartDetail.storeId)}
            className="text-red-600 pb-4"
          >
            清空購物車
          </button>
          <div className="flex justify-between pb-3 border-b-2">
            <p>
              <span className="font-bold">{cartDetail.qty}</span>項商品
            </p>
            <p>
              小計: <span className="font-bold">${cartDetail.total}</span>
            </p>
          </div>
          <div>
            {cartDetail.cartDetails.map((detail) => {
              return (
                <div key={detail.identifyNum} className="border-b-2 py-2">
                  <p className="text-lg pb-2">{detail.productName}</p>
                  <p className="pb-2">{detail.itemName}</p>
                  <div className="flex justify-between">
                    <p>{detail.qty} 項</p>
                    <p>$ {detail.subTotal}</p>
                  </div>

                  {/* 按鈕Update:回到'ProductSelection'頁面, 並記憶該筆商品明細的'客製化選項'與'商品數量' */}
                  {/* 在該頁面重新選擇完成後, 按鈕'確認修改':onClick={UpdateDetail} */}
                  <div className="flex justify-between">
                    <button onClick={null}>更新</button>
                    <button
                      className="text-red-600"
                      onClick={() =>
                        RemoveDetail(currentUser.userId, detail.identifyNum)
                      }
                    >
                      刪除
                    </button>
                  </div>
                </div>
              );
            })}
          </div>
          <Btn onClick={checkoutHandler} className="w-full mt-4">
            結帳
          </Btn>
        </div>
      )}
      {/* 購物車沒東西時顯示的畫面 */}
      {currentUser && currentAddress && !cartDetail && (
        <div className="flex justify-center items-center h-screen flex-col">
          <img src={ShoppingCart} alt="shoppingCart.png" className="w-2/3" />
          <h2 className="text-xl font-bold my-2">購物車是空的!</h2>
          <p className="mb-4">來把錢錢變成喜歡的東西吧~</p>
          <LayoutBtn>
            <Link to={"/store/" + currentAddress.address}>開逛</Link>
          </LayoutBtn>
        </div>
      )}
    </div>
  );
};

export default Cart;
