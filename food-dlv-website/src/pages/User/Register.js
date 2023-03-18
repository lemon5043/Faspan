import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Label, Input, Button, Box } from "../../components/Style/form-styling";
import UserAuthService from "../../services/User/userAuth.service";

const Register = () => {
  const navigate = useNavigate();
  let [account, setAccount] = useState("");
  let [password, setPassword] = useState("");
  let [firstName, setFirstName] = useState("");
  let [lastName, setLastName] = useState("");
  let [phone, setPhone] = useState("");
  let [errorMessage, setErrorMessage] = useState("");
  let [serverErrorMessage, setServerErrorMessage] = useState("");

  //判斷是否登入成功
  const registerHandler = async (e) => {
    try {
      e.preventDefault();
      if (!isMatch) return;
      let formData = new FormData();
      formData.append("account", account);
      formData.append("password", password);
      formData.append("firstName", firstName);
      formData.append("lastName", lastName);
      formData.append("phone", phone);
      await UserAuthService.register(formData).then(() => {
        UserAuthService.emailConfirmation();
        alert("註冊成功，已寄 email 到您的信箱，");
        navigate("/login");
      });
    } catch (e) {
      console.log("e");
    }
  };

  const isMatch = (e) => {
    if (e.target.value !== password) {
      setErrorMessage("密碼不相符");
      return false;
    }
    setErrorMessage("");
    return true;
  };

  return (
    <div className="relative flex flex-col justify-center min-h-screen overflow-hidden">
      <Box>
        <form className="mt-6" onSubmit={registerHandler}>
          <div className="mb-2">
            <Label htmlFor="account">email / 帳號</Label>
            <Input
              pattern="[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$"
              required
              autoComplete="username email"
              name="email"
              type="text"
              onChange={(e) => setAccount(e.target.value)}
            />
            <p className="text-sm text-red-600">{serverErrorMessage}</p>
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
          </div>
          <div className="mb-2">
            <Label htmlFor="lastName">姓</Label>
            <Input
              required
              name="lastName"
              type="text"
              maxLength="20"
              onChange={(e) => setLastName(e.target.value)}
            />
          </div>
          <div className="mb-2">
            <Label htmlFor="lastName">名</Label>
            <Input
              required
              name="lastName"
              type="text"
              maxLength="20"
              onChange={(e) => setFirstName(e.target.value)}
            />
          </div>
          <div className="mb-2">
            <Label htmlFor="phone">手機號碼</Label>
            <Input
              required
              name="phone"
              type="text"
              maxLength={10}
              pattern="09\d{8}"
              onChange={(e) => setPhone(e.target.value)}
            />
          </div>
          <div className="mt-6">
            <Button onClick={registerHandler}>註冊</Button>
          </div>
        </form>
      </Box>
    </div>
  );
};

export default Register;
