import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Label, Input, Button, Box } from "../../components/Style/form-styling";
import cartService from "../../services/Cart/cart.service";
import userAddressService from "../../services/User/userAddress.service";
import userAuthService from "../../services/User/userAuth.service";

const Login = ({ setCurrentUser, setCurrentAddress, setCartDetail }) => {
  // navigate 是控制重新導向的東西
  const navigate = useNavigate();
  //states
  let [account, setAccount] = useState("");
  let [password, setPassword] = useState("");
  let [errorMessage, setErrorMessage] = useState("");

  //判斷是否登入成功
  const loginHandler = async (e) => {
    try {
      e.preventDefault();
      await userAuthService.login(account, password).then(async (res) => {
        localStorage.setItem("user", JSON.stringify(res.data));
        await userAddressService.displayAddress(res.data.userId);
        setCurrentAddress(userAddressService.getCurrentAddress());
        setCurrentUser(userAuthService.getCurrentUser());
        setCartDetail(cartService.getCurrentCart());
      });
      navigate("/");
    } catch (e) {
      if (e.response.status === 400) {
        setErrorMessage("帳號或密碼錯誤!");
      }
      console.log(e);
    }
  };

  return (
    <div className="relative flex flex-col justify-center min-h-screen overflow-hidden">
      <Box>
        <form className="mt-6" onSubmit={loginHandler}>
          <p className="text-sm text-red-600">{errorMessage}</p>
          <div className="mb-2">
            <Label htmlFor="account">email / 帳號</Label>
            <Input
              pattern="[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$"
              required
              autoComplete="username"
              name="account"
              type="email"
              onChange={(e) => setAccount(e.target.value)}
            />
          </div>
          <div className="mb-2">
            <Label htmlFor="password">密碼</Label>
            <Input
              required
              autoComplete="current-password"
              name="password"
              type="password"
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>
          <Link
            to="/forgotPassword"
            className="text-sm text-neutral-600 hover:underline"
          >
            忘記密碼?
          </Link>
          <div className="mt-6">
            <Button type="submit">登入</Button>
          </div>
        </form>
        <p className="mt-8 text-sm font-light text-center text-gray-700">
          還沒有帳號嗎?
          <Link
            to="/register"
            className="font-medium text-black hover:underline"
          >
            註冊
          </Link>
        </p>
      </Box>
    </div>
  );
};

export default Login;
