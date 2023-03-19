import React, { useEffect, useState } from "react";
import orderService from "../services/order.service";
import { Label, Input, Button, Box } from "../components/Style/form-styling";
import { Link } from "react-router-dom";

const Order = ({ currentUser, currentAddress }) => {
  const [data, setData] = useState(null);
  const DisplayOrder = async () => {
    const res = await orderService.orderTracking(currentUser.userId);
    setData(res.data);
    console.log(res.data);
  };
  useEffect(() => {
    DisplayOrder();
  }, []);

  const displayStatue = (id) => {
    let result = "";
    switch (id) {
      case 1:
        result = "訂單已建立，等待店家確認中!";
        break;
      case 2:
        result = "餐點正在準備中";
        break;
      case 3:
        result = "餐點完成，正等待店家配送";
        break;
      case 4:
        result = "餐點正在路上囉!";
        break;
      case 5:
        result = "餐點已抵達";
        break;
      case 6:
        result = "訂單完成，請享用餐點吧!";
        break;
      case 9:
        result = "訂單失敗";
        break;
    }
    return result;
  };

  return (
    <div>
      {data && (
        <div className="h-screen flex justify-center items-center">
          <Box>
            <h2 className="text-3xl font-semibold mb-6">
              {displayStatue(data.schedules[0].statusId)}
            </h2>
            <h3 className="text-xl font-semibold mb-2">外送詳細資訊</h3>
            <p className="mb-2">
              <span className="font-semibold ">
                外送地址:{currentAddress.address}
              </span>
            </p>
            <p className="mb-2">
              <span className="font-semibold ">用戶姓名:{data.memberName}</span>
            </p>
            {/* <h4 className="text-lg font-semibold mb-2">訂單摘要</h4>
            <div className="flex justify-between mb-2">
              <p>小計</p>
              <p></p>
            </div> */}
            {/* <div className="flex justify-between">
              <p>外送費</p>
              <p></p>
            </div> */}
            {/* <Button className="mt-4">一鍵下訂單</Button> */}
          </Box>
        </div>
      )}
    </div>
  );
};

export default Order;
