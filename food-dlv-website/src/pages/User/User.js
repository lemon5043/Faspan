import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Label, Input, Button, Box } from "../../components/Style/form-styling";
import cartService from "../../services/Cart/cart.service";
import userAddressService from "../../services/User/userAddress.service";
import userAuthService from "../../services/User/userAuth.service";

const User = ({ currentUser, setCurrentUser }) => {
  const [data, setData] = useState([]);
  let [password, setPassword] = useState("");
  let [errorMessage, setErrorMessage] = useState("");
  const displayData = async (userId) => {
    const res = await userAuthService.displayuserId(userId);
    setData(res.data);
  };

  const isMatch = (e) => {
    if (e.target.value !== password) {
      setErrorMessage("密碼不相符");
      return false;
    }
    setErrorMessage("");
    return true;
  };

  useEffect(() => {
    displayData(currentUser.userId);
  }, []);

  return (
    <div className="relative flex flex-col justify-center min-h-screen overflow-hidden">
      <Box>
        <h2 className="text-3xl font-semibold mb-6">我的帳戶</h2>

        {/* <p className="text-sm text-red-600">{errorMessage}</p> */}
        <div className="mb-2">
          <p>email: {data.account}</p>
        </div>
        <div className="mb-2">
          <p>姓: {data.lastName}</p>
        </div>
        <div className="mb-2">
          <p>名: {data.firstName}</p>
        </div>
        <div className="mb-2">
          <p>電話號碼: {data.phone}</p>
        </div>
        {/* <div className="mb-2">
          <Label htmlFor="password">密碼</Label>
          <Input
            required
            autoComplete="current-password"
            name="password"
            type="password"
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
        <div className="mb-2">
          <Label htmlFor="conFirmpassword">確認密碼</Label>
          <Input
            required
            autoComplete="new-password"
            name="conFirmpassword"
            type="password"
            onChange={isMatch}
          />
          <p className="text-sm text-red-600">{errorMessage}</p>
          <Button className="mt-6">更換密碼</Button>
        </div> */}
        {/* <Input
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
        <div className="mt-6">
          <Button type="submit">登入</Button> */}
      </Box>
    </div>
  );
};

export default User;
