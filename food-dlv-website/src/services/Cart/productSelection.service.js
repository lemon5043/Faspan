import axios from "axios";

const ProductSelection_API_URL = "https://localhost:7093/api/ProductSelection/";

class ProductSelectionService {
  getProductSelect(productId) {
    return axios.get(
      `${ProductSelection_API_URL}?productId=${productId}&status=true`
    );
  }
}

export default new ProductSelectionService();
