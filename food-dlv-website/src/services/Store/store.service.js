import axios from "axios";

const API_URL = "https://localhost:7093/api/Stores/";

class StoreService {
  //找到所有店家，這邊之後會改寫成地址範圍內所有店家
  getByAddress(address) {
    if (address === "" || typeof address == "undefined") return;

    return axios.get(
      API_URL + "getSomeStoresIfIMAt/" + address + "?pageNum=1&storeNum=20"
    );
  }

  getByCategories(address, categoryId) {
    if (address === "" || typeof address == "undefined") return;

    return axios.get(
      API_URL +
        "getSomeStoresIfIMAt/" +
        address +
        "?pageNum=1&storeNum=20&categoryId=" +
        categoryId
    );
  }

  getBySearch(address, search) {
    return axios.get(
      API_URL +
        "getSomeStoresIfIMAt/" +
        address +
        "?pageNum=1&storeNum=20&searchString=" +
        search
    );
  }

  getCategories() {
    return axios.get(API_URL + "getStoreCategories");
  }

  getStoreDetail(id) {
    return axios.get(API_URL + "storeDetail/" + id);
  }
}

export default new StoreService();
