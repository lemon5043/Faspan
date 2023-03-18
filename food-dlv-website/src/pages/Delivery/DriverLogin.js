import { Link, useNavigate } from "react-router-dom";
import React, { useState } from "react";
import Bike from "../../assets/images/delivery/bike.svg";
import { Label, Input, Button } from "../../components/Style/form-styling";
import driverAuthService from "../../services/Delivery/driverAuth.service";

const DriverLogin = ({ currentDriver, setCurrentDriver }) => {
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
      const res = await driverAuthService.login(account, password);
      localStorage.setItem("driver", res.data);

      const data = await driverAuthService.GetDriver(res.data);
      navigate("/delivery");
      console.log(data.data);

      // setCurrentDriver(driverAuthService.getCurrentDriver());
      // navigate("/delivery");
    } catch (e) {
      setErrorMessage(e.response.data.wrongAccountOrPassword[0]);
    }
  };

  return (
    <div className="flex justify-center h-full bg-black">
      <div className="bg-white h-screen w-full max-w-md">
        <figure className=" flex justify-center pt-3">
          <img src={Bike} alt="bike" className="w-full max-w-xs h-72" />
        </figure>
        <h2 className="pl-6 text-xl mt-3 text-black">
          午安! <br />
          <span className="font-extrabold font-nunito">FASPAN</span> 的外送夥伴
        </h2>
        <div className="relative flex flex-col justify-center overflow-hidden">
          <div className=" w-full px-6 m-auto rounded-md max-w-md">
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
              <Link to="/" className="text-sm text-neutral-600 hover:underline">
                忘記密碼?
              </Link>
              <div className="mt-6">
                <Button type="submit">登入</Button>
              </div>
            </form>
            <p className="mt-8 text-sm font-light text-center text-gray-700">
              {" "}
              還沒有帳號嗎?{" "}
              <Link
                to="/delivery/register"
                className="font-medium text-black hover:underline"
              >
                註冊
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DriverLogin;
