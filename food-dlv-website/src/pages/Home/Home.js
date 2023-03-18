import React from "react";
import Slider from "react-slick";
import HomeCard from "./HomeCard";
import Macu from "../../assets/images/macu.jpg";
import PrinceValley from "../../assets/images/王子神谷.jpg";
import Threemom from "../../assets/images/threemom.jpeg";
import Mcdonald from "../../assets/images/mcdonald.jpg";
import Table from "../../assets/images/table.jpg";
import Honmono from "../../assets/images/honmono.jpg";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

const Home = () => {
  // slick 設定
  const settings = {
    arrows: false,
    infinite: true,
    speed: 500,
    slidesToShow: 5,
    slidesToScroll: 2,
    autoplay: true,
    autoplaySpeed: 5000,
    responsive: [
      {
        breakpoint: 1500,
        settings: {
          slidesToShow: 4,
          slidesToScroll: 3,
        },
      },
      {
        breakpoint: 1200,
        settings: {
          slidesToShow: 3,
          slidesToScroll: 3,
          initialSlide: 2,
        },
      },
      {
        breakpoint: 900,
        settings: {
          slidesToShow: 2,
          slidesToScroll: 2,
          initialSlide: 2,
        },
      },
      {
        breakpoint: 600,
        settings: {
          slidesToShow: 1,
          slidesToScroll: 1,
          centerMode: true,
        },
      },
    ],
  };
  // 測試用 array，等 API 寫好後會採用隨機店家挑選
  const stores = [
    {
      id: 1,
      image: Macu,
      title: "麻古茶坊-中壢中正店",
      type: "飲料",
      time: 25,
    },
    {
      id: 2,
      image: PrinceValley,
      title: "王子神谷",
      type: "甜點",
      time: 50,
    },
    {
      id: 3,
      image: Threemom,
      title: "三媽臭臭鍋-實踐店",
      type: "火鍋",
      time: 35,
    },
    {
      id: 4,
      image: Mcdonald,
      title: "麥當勞-中壢新生店cxc",
      type: "飲料",
      time: 30,
    },
    {
      id: 5,
      image: Honmono,
      title: "本物洋食cxcz",
      type: "義式",
      time: 50,
    },
    {
      id: 6,
      image: Macu,
      title: "麻古茶坊-中壢中正店cxzc",
      type: "飲料",
      time: 25,
    },
    {
      id: 7,
      image: PrinceValley,
      title: "王子神谷cxcz",
      type: "甜點",
      time: 50,
    },
    {
      id: 8,
      image: Threemom,
      title: "三媽臭臭鍋-實踐店cxzc",
      type: "火鍋",
      time: 35,
    },
    {
      id: 9,
      image: Mcdonald,
      title: "麥當勞-中壢新生店ccxz",
      type: "飲料",
      time: 30,
    },
    {
      id: 10,
      image: Honmono,
      title: "本物洋食cxzcz",
      type: "義式",
      time: 50,
    },
  ];

  return (
    <div className="bg-theme-color" style={{ height: "94vh" }}>
      <div className="pt-4 h-3/5">
        <Slider {...settings} className="h-full">
          {stores.map((d) => {
            return <HomeCard store={d} key={d.id} />;
          })}
        </Slider>
      </div>
      <div className="h-2/5">
        <div className="flex h-full relative bg-theme-color items-end">
          <div className="slogan absolute bottom-1/4 left-24">
            <h1 className=" text-4xl">想點什麼呢?</h1>
            <h5>
              以上的推薦您滿意嗎?
              <br />
              如不符合您需求請點這裡
            </h5>
          </div>
          <img className="w-80 mx-auto relative" src={Table} alt="" />
        </div>
      </div>
      {/* <About /> */}
    </div>
  );
};

export default Home;
