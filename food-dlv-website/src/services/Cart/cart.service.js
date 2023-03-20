import axios from "axios";

const API_URL = "https://localhost:7093/api/Cart";

class CartService {
  postAddToCart(memberId, storeId, productId, itemId, qty) {
    const response = axios.post(API_URL + "/AddToCart", {
      MemberId: memberId,
      StoreId: storeId,
      ProductId: productId,
      ItemsId: itemId,
      Qty: qty,
    });
    return response;
  }

  async getCartInfo(memberId) {
    const response = await axios.get(
      API_URL + `/CartInfo?memberId=${memberId}`
    );
    if (response.data.length != 0) {
      localStorage.setItem("cartInfo", JSON.stringify(response.data[0]));
    }
    return response;
  }

  getCurrentCart() {
    const data = JSON.parse(localStorage.getItem("cartInfo"));
    return data;
  }

  postUpdateCart(memberId, storeId, productId, identifyNum, itemsId, qty) {
    const response = axios.post(API_URL + "/UpdateCart", {
      memberId: memberId,
      storeId: storeId,
      productId: productId,
      identifyNum: identifyNum,
      itemsId: itemsId,
      qty: qty,
    });
    return response;
  }

  async postRemoveDetail(identifyNum) {
    const response = await axios.post(
      API_URL + "/RemoveDetail" + `?identifyNum=${identifyNum}`
    );
    return response;
  }

  async deleteDeleteCart(memberId, storeId) {
    const response = await axios.delete(
      API_URL + "/DeleteCart" + `?memberId=${memberId}&storeId=${storeId}`
    );
    return response;
  }
}

export default new CartService();
