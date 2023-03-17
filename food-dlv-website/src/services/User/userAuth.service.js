import axios from "axios";

const API_URL = "https://localhost:7093/api/Members";

class UserAuthService {
  register(formData) {
    return axios.post(API_URL + "/register", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  }

  login(account, password) {
    const response = axios.post(API_URL + "/login", { account, password });
    return response;
  }

  getCurrentUser() {
    const data = JSON.parse(localStorage.getItem("user"));
    return data;
  }

  logout() {
    localStorage.removeItem("user");
    localStorage.removeItem("addressId");
  }
}

export default new UserAuthService();
