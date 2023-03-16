import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import StoreService from "../../services/Store/store.service";
import ProductPage from "./ProductPage";
import UserAuthService from "../../services/User/userAuth.service";

const Product = ({ currentUser }) => {
  const params = useParams();
  const storeId = params.storeId;
  let [data, setData] = useState([]);

  const displayProduct = async (id) => {
    const res = await StoreService.getStoreDetail(id);
    setData(res.data[0]);
  };

  useEffect(() => {
    displayProduct(storeId);
  }, []);

  return (
    <div className="flex">
      {data.length !== 0 && (
        <section className="w-full">
          <ProductPage data={data} currentUser={currentUser} />
        </section>
      )}
    </div>
  );
};

export default Product;
