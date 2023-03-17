import axios from "axios";

const API_URL = "https://localhost:7093/api/MemberAddress";

class UserAddressService {
  getAddress(memberId) {
    const response = axios.get(API_URL + "/index" + `?memberId=${memberId}`);
    return response;
  }
  createAddress(memberId, address) {
    const respones = axios.post(
      API_URL + "/Create" + `?memberId=${memberId}` + `&address=${address}`
    );
    return respones;
  }
}

export default new UserAddressService();
