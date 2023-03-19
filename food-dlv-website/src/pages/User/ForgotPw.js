import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Label, Input, Button, Box } from "../../components/Style/form-styling";
import userAuthService from "../../services/User/userAuth.service";

const ForgotPw = () => {
  const navigate = useNavigate();
  //states
  let [account, setAccount] = useState("");
  let [errorMessage, setErrorMessage] = useState("");

  //判斷是否登入成功
  const forgotPasswordHandler = (e) => {
    try {
      e.preventDefault();
      console.log(account);
      userAuthService.forgotPassword(account).then(() => {
        alert("已成功發送重設密碼連結，請至信箱查收");
        navigate("/");
      });
    } catch (e) {
      console.log(e);
    }
  };

  return (
    <div className="relative flex flex-col justify-center min-h-screen overflow-hidden">
      <Box>
        <form className="mt-2" onSubmit={forgotPasswordHandler}>
          <p className="text-sm text-red-600">{errorMessage}</p>
          <div className="mb-2">
            <h1 className="text-2xl font-semibold mb-3">忘記密碼了嗎?</h1>
            <h3 className="text-sm text-zinc-700">
              輸入你的email，我們發送重設密碼連結
            </h3>
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
          <div className="mt-6">
            <Button type="submit">寄送驗證信</Button>
          </div>
        </form>
      </Box>
    </div>
  );
};

export default ForgotPw;
