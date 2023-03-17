import React, { useState, useEffect, useRef } from "react";
import { Label, Input, Button } from "../../components/Style/form-styling";
import driverAuthService from "../../services/Delivery/driverAuth.service";
import delieveryService from "../../services/Delivery/delievery.service";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr"; //signalr使用
import {
  useJsApiLoader,
  GoogleMap,
  Marker,
  DirectionsRenderer,
} from "@react-google-maps/api"; //googleMap使用
import {
  SweetAlertIcon,
  SweetAlertOptions,
  SweetAlertResult,
} from "sweetalert2";
import swal from "sweetalert2";

const DriverMap = () => {
  const { isLoaded, loadError } = useJsApiLoader({
    googleMapsApiKey: process.env.REACT_APP_GOOGLE_MAPS_API_KEY,
    //googleMapsApiKey: "",
  });
  let [orderId, setOrderId] = useState(0);
  let [driverId, setDriverId] = useState(0);
  let [memberId, setMemberId] = useState(0);
  let [storeName, setStoreName] = useState("");
  let [storeAddress, setStoreAddress] = useState("");
  let [customerAddress, setCustomerAddress] = useState("");
  let [workingStatus, setWorkingStatus] = useState(false);
  let [position, setPosition] = useState({ lat: 0, lng: 0 });
  let [errorMessage, setErrorMessage] = useState("");
  let [map, setMap] = useState(/** @type google.maps.Map */ (null));
  const [directionsResponse, setDirectionsResponse] = useState(null);
  const [distance, setDistance] = useState(0);
  const [duration, setDuration] = useState(0);
  const [connection,setConnection] = useState();
  //const dstTest = "桃園市中壢區新生路三段12號";
  const token = localStorage.getItem("driver");

  useEffect(() => {
    GetDriverId()
    //設定進入頁面時會不斷更新位置
    let timerId = null;
    if (navigator.geolocation) {
      timerId = setInterval(() => {
        navigator.geolocation.getCurrentPosition(
          (position) => {
            setPosition({
              lat: position.coords.latitude,
              lng: position.coords.longitude,
            });
          },
          (error) => console.error(error),
          { enableHighAccuracy: true, timeout: 5000, maximumAge: 0 }
        );
      }
      , 1000);
    } else {
      console.error("Geolocation is not supported by this browser.");
    }

    return () => clearInterval(timerId);  
  }, []);

  async function GetDriverId (){
    const driver = await driverAuthService.GetDriver(token);
    setDriverId(driver.data.driverId)
    }

  //地圖載入錯誤
  if (loadError) {
    return <div>Map cannot be loaded right now, sorry.</div>;
  }

  //路程計算
  async function calculateRoute(destination) {
    // eslint-disable-next-line no-undef
    const directionsService = new google.maps.DirectionsService();
    const results = await directionsService.route({
      origin: position,
      destination: destination,
      // eslint-disable-next-line no-undef
      travelMode: google.maps.TravelMode.DRIVING,
    });
    setDirectionsResponse(results);
    setDistance(results.routes[0].legs[0].distance.text);
    setDuration(results.routes[0].legs[0].duration.text);
  }

  //清除路線
  function clearRoute() {
    setDirectionsResponse(null);
    setDistance("");
    //setDuration("");
  }

  //加入HubGroup
  async function JoinGroup() {
    try {

      const role = "driver";
      const connection = new HubConnectionBuilder()
        .withUrl("https://localhost:7093/OrderHub")
        .configureLogging(LogLevel.Information)
        .build();
      //監聽由server傳來的OrderId
      connection.on("AssignOrder", async (OrderId) => {
         console.log(OrderId);
        const storeInfo = await delieveryService.OrderAasignment(OrderId);
        setOrderId(storeInfo.orderId);
        setStoreAddress(storeInfo.storeAddress);
        setDriverId(driverId);
        //todo 跳出視窗顯示=>接受
        //NewOrder();
        //todo 跳出視窗顯示=>取消
      });
      await connection.start();
      await connection.invoke("JoinGroup", { Id:driverId, Role:role });
      setConnection(connection)
    } catch (e) {
      console.log(e);
    }
  }

  //離開HubGroup
  async function LeaveGrop() {
    try {
      const role ="driver" 
      // const connection = new HubConnectionBuilder()
      //   .withUrl("https://localhost:7093/OrderHub")
      //   .configureLogging(LogLevel.Information)
      //   .build();
      
      // await connection.start();
      await connection.invoke("LeaveGroup", {Id:driverId,Role:role});
      setConnection()
    } catch (e) {
      console.log(e);
    }
  }

  //向餐廳導航
  async function NavationToStore(orderId, driverId) {
    try {
      const orderDetail = await delieveryService.OrderAccept(orderId, driverId);
      setStoreName(orderDetail.StoreName);
      setMemberId(orderDetail.MemberId);
      calculateRoute(orderDetail.StoreAddress);
      setStoreAddress("");
    } catch (e) {
      setErrorMessage(e.response.data.errorMessage[0]);
    }
  }

  //確認取餐，回傳外送地址
  async function NavationToCustomer(orderId) {
    try {
      clearRoute();
      const customerAddress = await delieveryService.NavationToCustomer(
        orderId
      );
      setCustomerAddress(customerAddress);
      calculateRoute(customerAddress);
    } catch (e) {
      setErrorMessage(e.response.data.wrongAccountOrPassword[0]);
    }
  }

  //餐點抵達，更新訂單，通知會員
  async function OrderArrive() {
    try {
      clearRoute();
      const driver = (await driverAuthService.GetDriver(token)).data;
      await delieveryService.DeliveryArrive(orderId, driver.driverId, distance);
      const targetRole = "Member";
      const connection = new HubConnectionBuilder()
        .withUrl("https://localhost:7093/OrderHub")
        .configureLogging(LogLevel.Information)
        .build();
      await connection.start();
      await connection.invoke("JoinGroup", { memberId, targetRole }); //連上MemberGroup
      await connection.invoke("OrderArrive", { memberId, orderId }); //傳送訂單
      await connection.invoke("LeaveGroup", { memberId, targetRole }); //離開MemberGroup
      setOrderId(0);
      setCustomerAddress("");
    } catch (e) {
      setErrorMessage(e.response.data.wrongAccountOrPassword[0]);
    }
  }


  //(由Button觸發)畫面定位至目前所在位置
  const handleCenterButton = () => {
    if (map) {
      map.panTo(position);
      map.setZoom(18);
    }
  };

  //(由Button觸發)(signalR)上、下線
  let timer = null
  const ChangeWorkingStatus = async () => {
    try {
      const driver = await (await driverAuthService.GetDriver(token)).data;
      await delieveryService.ChangeWorkingStatus(
        driver.driverId,
        position.lng,
        position.lat
        );
      if (workingStatus === false) {
        setWorkingStatus(true);
        //每分鐘更新位置
        timer = setInterval(()=>{
            delieveryService.UpdateLocation(driver.driverId,position.lng,position.lat)   
        },60000)       
      } else {
        setWorkingStatus(false);
        setDriverId(0)
        //停止每分鐘更新位置
        clearInterval(timer)
      }
    } catch (e) {
      console.log(e);
    }
  };

  //(swal:由商店觸發)(signalR)接收到新訂單，確認後後回傳商店地址導航資訊
  const NewOrder = () => {
    swal
      .fire({
        title: "新訂單!",
        text: "請至 " + storeAddress + " 取餐",
        icon: "warning",
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "開始導航",
        heightAuto: false,
        width: "20em",
      })
      .then(() => {
        NavationToStore(orderId, driverId);
        setDriverId(0);
        console.log("success");
      });
  };

  //(swal:由Button觸發)確認取餐，回傳外送地址的導航資訊
  const PickUpConfirmation = () => {
    swal
      .fire({
        title: "取餐確認",
        text: "請至" + { storeName } + "領取" + { orderId } + "號餐點",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "確認餐點無誤，開始外送",
        heightAuto: false,
        width: "20em",
      })
      .then((result) => {
        if (result.isConfirmed) {
          NavationToCustomer(orderId);
        }
      });
  };

  //(swal:由Button觸發)(signalR)向會員端回報餐點抵達
  const DeliveryArrive = () => {
    swal
      .fire({
        title: "送達確認",
        text: "請至將餐點送至" + { customerAddress },
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "確認餐點已送達，通知客戶取餐",
        heightAuto: false,
        width: "20em",
      })
      .then((result) => {
        if (result.isConfirmed) {
          OrderArrive();
        }
      });
  };

  async function AssignOrder() {
    try {
      const targetRole = "driver";
      const driverId = 1
      const orderId=1
      const connection = new HubConnectionBuilder()
        .withUrl("https://localhost:7093/OrderHub")
        .configureLogging(LogLevel.Information)
        .build();

      await connection.start();
      await connection.invoke("JoinGroup", {Id:driverId,Role:targetRole }); //連上driverGroup
      await connection.invoke("AssignOrder", driverId, orderId ); //傳送訂單
      await connection.invoke("LeaveGroup", {Id:driverId,Role:targetRole }); //離開driverGroup

    } catch (e) {
      console.log(e)
    }
  }

  return (
    <>
      {isLoaded && (
        <GoogleMap
          onLoad={(map) => setMap(map)}
          center={position}
          zoom={16}
          mapContainerStyle={{ width: "100%", height: "100%" }}
          options={{
            zoomControl: false,
            streetViewControl: false,
            mapTypeControl: false,
            fullscreenControl: false,
          }}
        >
          {position && <Marker position={position} />}
          {directionsResponse && (
            <DirectionsRenderer directions={directionsResponse} />
          )}
          {workingStatus && <Button onClick={() => {ChangeWorkingStatus();LeaveGrop();}}>下線</Button>}

          {!workingStatus && (
            <Button onClick={() => {ChangeWorkingStatus();JoinGroup();}}>上線</Button>
          )}
          <Button onClick={handleCenterButton}>置中?</Button>
          <Button onClick={PickUpConfirmation}>取餐回報</Button>
          <Button onClick={DeliveryArrive}>餐點送達回報</Button>
          <Button onClick={AssignOrder}>test</Button>

        </GoogleMap>
      )}
    </>
  );
};
export default DriverMap;
