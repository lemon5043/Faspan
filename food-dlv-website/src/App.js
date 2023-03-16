import { useState } from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
//*用戶介面
//layout
import Layout from "./components/Layout";
//登入註冊
import Login from "./pages/User/Login";
import Register from "./pages/User/Register";
//頁面
import User from "./pages/User/User";
import Error from "./pages/Error";
import Home from "./pages/Home/Home";
import Store from "./pages/Store/Store";
import Product from "./pages/Product/Product";
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

function App() {
  let [currentUser, setCurrentUser] = useState(
    UserAuthService.getCurrentUser()
  );
  return (
    <BrowserRouter>
      <Routes>
        {/* 使用者頁面 */}
        <Route
          path="/"
          element={
            <Layout currentUser={currentUser} setCurrentUser={setCurrentUser} />
          }
        >
          <Route index element={<Home />}></Route>
          <Route path="user" element={<User />}></Route>
          <Route
            path="login"
            element={<Login setCurrentUser={setCurrentUser} />}
          ></Route>
          <Route path="register" element={<Register />}></Route>
          <Route path="store/:addressName" element={<Store />}></Route>
          <Route
            path="product/:storeId"
            element={<Product currentUser={currentUser} />}
          ></Route>
          <Route path="*" element={<Error />}></Route>
        </Route>
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
