import axios from "axios";

const API_URL = "https://localhost:7093/api/MemberAddress";

class UserAddressService {
  getAddress(memberId) {
    const response = axios.get(API_URL + `/index?memberId=${memberId}`);
    return response;
  }
  createAddress(memberId, address) {
    const respones = axios.post(
      API_URL + `/Create?memberId=${memberId}&address=${address}`
    );
    return respones;
  }

  async displayAddress(memberId) {
    const res = await this.getAddress(memberId);
    if (res.data.length === 0) return;
    const firstAddress = res.data[0];
    localStorage.setItem("address", JSON.stringify(firstAddress));
  }

  getCurrentAddress() {
    const data = JSON.parse(localStorage.getItem("address"));
    return data;
  }
}

export default new UserAddressService();
