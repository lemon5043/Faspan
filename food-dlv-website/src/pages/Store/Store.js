import React, { useEffect, useState } from "react";
import InfiniteScroll from "react-infinite-scroller";
import StoreService from "../../services/Store/store.service";
import StoreCard from "../../components/Store/StoreCard";
import { useParams } from "react-router-dom";
import { Input } from "../../components/Style/form-styling";

const Store = () => {
  const params = useParams();
  const addressName = params.addressName;

  let [data, setData] = useState([]);
  let [ctg, setCtg] = useState([]);
  const [selectedCtg, setSelectedCtg] = useState(0);
  const [input, setInput] = useState("");
  // let [page, setPage] = useState(1);
  // let [enableLoadMoreData, setEnableLoadMoreData] = useState(false);

  const displayCategory = async () => {
    const category = await StoreService.getCategories();
    setCtg(category.data);
  };

  const search = async () => {
    const res = await StoreService.getByAddress(addressName);
    setData(res.data);
    setSelectedCtg(0);
  };

  const chooseCategory = async (categoryId) => {
    setSelectedCtg(categoryId);
    const res = await StoreService.getByCategories(addressName, categoryId);
    setData(res.data);
  };

  const searchByType = async (e) => {
    if (e.key === "Enter") {
      const res = await StoreService.getBySearch(addressName, e.target.value);
      setData(res.data);
    }
  };

  const calcFee = (distance) => {
    let feePerKm = 9999;

    if (distance < 1) {
      feePerKm = 20;
    } else if (distance >= 1 && distance < 2) {
      feePerKm = 25;
    } else if (distance >= 2 && distance < 4) {
      feePerKm = 30;
    } else if (distance >= 4 && distance < 6) {
      feePerKm = 40;
    } else {
      feePerKm = 100;
    }
    return feePerKm;
  };

  useEffect(() => {
    search(addressName);
    displayCategory();
  }, [addressName]);

  return (
    <div className="mx-12 min-h-screen">
      <main className="flex mt-6 min-h-screen">
        {/* 篩選部分 */}
        <section className="max-w-xs mr-28 h-3/4 p-2 sticky top-60 left-3 border-2 border-solid border-black">
          <label className="text-lg font-bold">搜尋</label>
          <Input
            placeholder="搜尋想吃的"
            onChange={setInput}
            name="searchString"
            onKeyDown={searchByType}
            className="mb-3"
          />
          <fieldset>
            <legend className="text-lg font-bold">分類</legend>
            <div>
              <input
                type="radio"
                name="categories"
                id={0}
                value={0}
                className="mr-2 my-2"
                onChange={search}
              />
              <label htmlFor={0}>所有店家</label>
            </div>
            {ctg &&
              ctg.map((d) => {
                return (
                  <div key={d.id}>
                    <input
                      type="radio"
                      name="categories"
                      id={d.id}
                      value={d.id}
                      className="mr-2 my-2"
                      onChange={(e) => chooseCategory(e.target.value)}
                    />
                    <label htmlFor={d.id}>{d.categoryName}</label>
                  </div>
                );
              })}
            {/**下面的這個 div 做完要刪掉 */}
            <div className=" w-64"></div>
          </fieldset>
        </section>

        {/* 餐廳選擇部分 */}
        <section className="flex flex-wrap justify-center">
          {data &&
            data.map((d) => {
              return <StoreCard calcFee={calcFee} data={d} key={d.id} />;
            })}
        </section>
      </main>
      <footer></footer>
    </div>
  );
};

export default Store;
