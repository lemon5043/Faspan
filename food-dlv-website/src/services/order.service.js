import axios from "axios";

const API_URL = "https://localhost:7093/api/Order/";

class OrderService {
  getOrderInfo(cartId, addressId) {
    const response = axios.get(
      API_URL + `OrderInfo?cartId=${cartId}&addressId=${addressId}`
    );
    return response;
  }

  postOrderEstablished(memberId, storeId, fee, address) {
    const response = axios.post(
      API_URL +
        `OrderEstablished?memberId=${memberId}&storeId=${storeId}&fee=${fee}&address=${address}`
    );
    return response;
  }
}

export default new OrderService();
