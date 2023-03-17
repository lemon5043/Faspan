import React, { useEffect, useState } from "react";
import useCart from "../hooks/useCart";
import { Label, Input, Button, Box } from "../components/Style/form-styling";
import Logo from "../assets/images/logo.svg";
import { Link, useParams } from "react-router-dom";
import OrderService from "../services/order.service.js";

const Checkout = ({ currentUser, currentAddress, currentAddressId }) => {
  const { cartDetail, CartInfo } = useCart();
  const [orderInfo, setOrderInfo] = useState(null);
  const params = useParams();
  const cartId = params.cartId;

  const getOrderInfo = async () => {
    const res = await OrderService.getOrderInfo(cartId, currentAddressId);
    console.log(res.data);
    setOrderInfo(res.data);
    return res;
  };

  useEffect(() => {
    if (currentUser) {
      CartInfo(0, currentUser.userId);
      getOrderInfo();
    }
  }, []);
  return (
    <div>
      <nav
        style={{ height: "6vh" }}
        className="bg-theme-color sticky top-0 z-10"
      >
        <ul className="flex h-full justify-between items-center w-full mr-4">
          {/* logo 標題 搜尋欄 */}
          <li className="flex items-center">
            <Link to="/" className="text-2xl flex items-center px-4">
              {/* logo */}
              <div>
                <img src={Logo} alt="logo.svg" className="w-10 -rotate-12" />
              </div>
              {/* 標題 */}
              <div className="text-2xl pl-4 font-extrabold font-nunito">
                FASPAN
              </div>
            </Link>
          </li>
        </ul>
      </nav>
      {cartDetail.length !== 0 && orderInfo && (
        <div className="h-screen flex justify-center items-center">
          <Box>
            <h2 className="text-3xl font-semibold mb-6">
              {cartDetail.storeName}
            </h2>
            <h3 className="text-xl font-semibold mb-2">外送詳細資訊</h3>
            <p className="mb-2">
              <span className="font-semibold ">外送地址:</span>
              {currentAddress}
            </p>
            <h4 className="text-lg font-semibold mb-2">訂單摘要</h4>
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
                  </div>
                );
              })}
            </div>
            <div className="flex justify-between">
              <p>小計</p>
              <p>{cartDetail.total}</p>
            </div>
            <div className="flex justify-between">
              <p>外送費</p>
              <p>{orderInfo.deliveryFee}</p>
            </div>
            <Button className="mt-4">一鍵下訂單</Button>
          </Box>
        </div>
      )}
    </div>
  );
};

export default Checkout;
