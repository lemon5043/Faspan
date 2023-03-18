import { useState } from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
//*用戶介面
//layout
import Layout from "./components/Layout";
//登入註冊
import Login from "./pages/User/Login";
import Register from "./pages/User/Register";
import ForgotPw from "./pages/User/ForgotPw";
//頁面
import User from "./pages/User/User";
import Error from "./pages/Error";
import Home from "./pages/Home/Home";
import Store from "./pages/Store/Store";
import Product from "./pages/Product/Product";
import Checkout from "./pages/Checkout";
// *外送員介面
//layout
import DriverLayout from "./components/DriverLayout";
//登入註冊
import DriverRegister from "./pages/Delivery/DriverRegister";
import DriverLogin from "./pages/Delivery/DriverLogin";
//頁面
import DriverHome from "./pages/Delivery/DriverHome";
import DriverMap from "./pages/Delivery/DriverMap";
import DriverOrder from "./pages/Delivery/DriverOrder";
import DriverWallet from "./pages/Delivery/DriverWallet";
import DriverHistory from "./pages/Delivery/DriverHistory";
//css
import "./assets/styles/tailwind.css";
//services
import UserAuthService from "./services/User/userAuth.service";
import Test from "./pages/___test___/Sidebar.test";
import UserAddressService from "./services/User/userAddress.service";
import cartService from "./services/Cart/cart.service";

function App() {
  const [currentUser, setCurrentUser] = useState(
    UserAuthService.getCurrentUser()
  );
  const [currentAddress, setCurrentAddress] = useState(
    UserAddressService.getCurrentAddress()
  );
  let [cartDetail, setCartDetail] = useState(cartService.getCurrentCart());

  return (
    <BrowserRouter>
      <Routes>
        {/* 使用者頁面 */}
        setCartDetail{" "}
        <Route
          path="/"
          element={
            <Layout
              currentUser={currentUser}
              setCurrentUser={setCurrentUser}
              currentAddress={currentAddress}
              setCurrentAddress={setCurrentAddress}
              cartDetail={cartDetail}
              setCartDetail={setCartDetail}
            />
          }
        >
          <Route index element={<Home />}></Route>
          <Route
            path="user"
            element={
              <User currentUser={currentUser} setCurrentUser={setCurrentUser} />
            }
          ></Route>
          <Route
            path="login"
            element={
              <Login
                setCurrentUser={setCurrentUser}
                setCurrentAddress={setCurrentAddress}
                setCartDetail={setCartDetail}
              />
            }
          ></Route>
          <Route path="register" element={<Register />}></Route>
          <Route path="store/:addressName" element={<Store />}></Route>
          <Route
            path="product/:storeId"
            element={
              <Product
                currentUser={currentUser}
                setCartDetail={setCartDetail}
              />
            }
          ></Route>
          <Route path="/forgotPassword" element={<ForgotPw />}></Route>
          <Route path="*" element={<Error />}></Route>
        </Route>
        <Route
          path="checkout"
          element={
            <Checkout
              currentAddress={currentAddress}
              cartDetail={cartDetail}
              setCartDetail={setCartDetail}
            />
          }
        ></Route>
        {/* 外送員頁面 */}
        {/* 註冊登入 */}
        <Route path="/test" element={<Test />}></Route>
        <Route path="/delivery/register" element={<DriverRegister />}></Route>
        <Route path="/delivery/login" element={<DriverLogin />}></Route>
        <Route element={<DriverLayout />}>
          <Route path="/delivery" element={<DriverHome />}></Route>
          <Route path="/deliveryOrder" element={<DriverOrder />}></Route>
          <Route path="/deliveryMap" element={<DriverMap />}></Route>
          <Route path="/deliveryHistory" element={<DriverHistory />}></Route>
          <Route path="/deliveryWallet" element={<DriverWallet />}></Route>
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
