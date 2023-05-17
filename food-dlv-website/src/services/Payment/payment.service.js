import axios from "axios";

const API_URL = "https://localhost:7093/api/Payment/";

class paymentService {
  getUserBalance(id) {
    const response = axios.get(API_URL + id);
    return response;
  }
}

export default new paymentService();
