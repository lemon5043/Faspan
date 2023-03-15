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
    // googleMapsApiKey: process.env.REACT_APP_GOOGLE_MAPS_API_KEY,
    googleMapsApiKey: "",
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
  const dstTest = "桃園市中壢區新生路三段12號";
  const token = localStorage.getItem("driver");

  useEffect(() => {
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
      }, 1000);
    } else {
      console.error("Geolocation is not supported by this browser.");
    }

    return () => clearInterval(timerId);
  }, []);

  //路程計算
  async function calculateRoute(destination) {
    // eslint-disable-next-line no-undef
    const directionsService = new google.maps.DirectionsService();
    const results = await directionsService.route({
      origin: position,
      destination: dstTest,
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
    setDuration("");
  }

  const handleCenterButton = () => {
    if (map) {
      map.panTo(position);
      map.setZoom(18);
    }
  };

  if (loadError) {
    return <div>Map cannot be loaded right now, sorry.</div>;
  }

  //上、下線
  const ChangeWorkingStatus = async () => {
    try {
      const driver = (await driverAuthService.GetDriver(token)).data;
      await delieveryService.ChangeWorkingStatus(
        driver.driverId,
        position.lng,
        position.lat
      );
      if (workingStatus === false) {
        setWorkingStatus(true);
        JoinGroup(driver.id, driver.role);
      } else {
        setWorkingStatus(false);
        LeaveGrop(driver.id, driver.role);
      }
    } catch (e) {
      setErrorMessage(e.response.data.wrongAccountOrPassword[0]);
    }
  };

  //加入HubGroup
  async function JoinGroup(id, role) {
    try {
      const driver = (await driverAuthService.GetDriver(token)).data;
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
        setDriverId(driver.id);
        //todo 跳出視窗顯示=>接受
        NewOrder();
        //todo 跳出視窗顯示=>取消
      });
      await connection.start();
      await connection.invoke("JoinGroup", { id, role });
    } catch (e) {
      console.log(e);
    }
  }
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

  //離開HubGroup
  async function LeaveGrop(id, role) {
    try {
      const driver = (await driverAuthService.GetDriver(token)).data;
      const connection = new HubConnectionBuilder()
        .withUrl("https://localhost:7093/OrderHub")
        .configureLogging(LogLevel.Information)
        .build();

      await connection.invoke("LeaveGroup", { id, role });
    } catch (e) {
      console.log(e);
    }
  }

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

  // 餐點抵達
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
          {workingStatus && <Button onClick={ChangeWorkingStatus}>下線</Button>}

          {!workingStatus && (
            <Button onClick={ChangeWorkingStatus}>上線</Button>
          )}
          <Button onClick={handleCenterButton}>Center</Button>
          <Button onClick={PickUpConfirmation}>PickUp</Button>
          <Button onClick={calculateRoute}>calculateRoute</Button>
        </GoogleMap>
      )}
    </>
  );
};
export default DriverMap;
