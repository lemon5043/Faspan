import axios from "axios";

const API_URL = "https://localhost:7093/api/Order/";

class OrderService {
  getOrderInfo(cartId, addressId) {
    const response = axios.get(
      API_URL + `OrderInfo?cartId=${cartId}&addressId=${addressId}`
    );
    return response;
  }

  postOrderEstablished(cartId, addressId) {
    const response = axios.post(
      API_URL + `OrderEstablished?cartId=${cartId}&addressId=${addressId}`
    );
    return response;
  }

  orderTracking(memberId, statusId) {
    const response = axios.get(
      API_URL + `OrderTracking?memberId=${memberId}&statusId=${statusId}`
    );
    return response;
  }
}

export default new OrderService();
