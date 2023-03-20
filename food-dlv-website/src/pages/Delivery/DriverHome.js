import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { Label, Input, Button } from "../../components/Style/form-styling2";
import driverAuthService from "../../services/Delivery/driverAuth.service";
import driverService from "../../services/Delivery/driver.service";

const DriverHome = () => {
  const navigate = useNavigate();
  let [errorMessage, setErrorMessage] = useState("");
  let [account, setAccount] = useState("");
  let [password, setPassword] = useState("");
  let [firstName, setFirstName] = useState("");
  let [lastName, setLastName] = useState("");
  let [phone, setPhone] = useState("");
  let [bankAccount, setBankAccount] = useState("");
  let [Idcard, setIdCard] = useState("");
  let [vehicleRegistration, setVehicleRegistration] = useState("");
  let [driverLicense, setDriverLicense] = useState("");
  let [serverErrorMessage, setServerErrorMessage] = useState("");

  //測試用
  // const token = localStorage.getItem("driver")
  // const driverId =  driverAuthService.GetDriver(token).then(
  //   function(response){
  //     console.log(response.data.driverId)
  //     const driverDetail = driverService.GetDetails(response.data.driverId,token).then(
  //       function(details){
  //         console.log(details.data)
  //       }
  //     )
  //   }
  // )

  useEffect(() => {
    GetDetails();
  }, []);

  const GetDetails = async (e) => {
    try {
      const token = localStorage.getItem("driver");
      const driver = await driverAuthService.GetDriver(token);
      const driverDetail = await driverService.GetDetails(
        driver.data.driverId,
        token
      );
      setPassword(driverDetail.password);
      setFirstName(driverDetail.firstName);
      setLastName(driverDetail.lastName);
      setPhone(driverDetail.phone);
      setBankAccount(driverDetail.bankAccount);
      setIdCard(driverDetail.Idcard);
      setVehicleRegistration(driverDetail.vehicleRegistration);
      setDriverLicense(driverDetail.driverLicense);
    } catch {
      setErrorMessage(e.response.data.wrongAccountOrPassword[0]);
    }
  };

  const EditHandler = async (e) => {
    try {
      e.preventDefault();
      if (!isMatch) return;
      let formData = new FormData();
      formData.append("password", password);
      formData.append("firstName", firstName);
      formData.append("lastName", lastName);
      formData.append("phone", phone);
      formData.append("bankAccount", bankAccount);
      formData.append("idcard", Idcard);
      formData.append("vehicleRegistration", vehicleRegistration);
      formData.append("driverLicense", driverLicense);
      await driverService.DriverEditSaveChange(formData);
      alert("修改成功，還需要審核約1~2個工作天，屆時將以email通知，請耐心等候");
      navigate("/delivery/Home");
    } catch (e) {
      setServerErrorMessage("信箱已被使用!");
      console.log(e);
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
    <div className="flex justify-center h-full% bg-black">
      <div className="bg-black h-screen w-full max-w-md overflow-scroll pb-12">
        <div className="relative flex flex-col justify-center overflow-hidden">
          <div className=" w-full px-6 m-auto rounded-md max-w-md">
            <p className="mt-6 text-2xl font-semibold text-center text-white">
              Hi! {lastName} {firstName}
            </p>
            {/* <form className="mt-6" onSubmit={EditHandler}>
              <div className="mb-2">
                <Label htmlFor="account">密碼</Label>
                <Input
                  required
                  autoComplete="new-password"
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
              <div className="mb-2">
                <Label htmlFor="bankAccount">銀行帳戶</Label>
                <Input
                  required
                  name="bankAccount"
                  type="text"
                  onChange={(e) => setBankAccount(e.target.value)}
                />
              </div>
              <div className="mb-2">
                <Label htmlFor="idCard">身分證</Label>
                <input
                  className="text-white"
                  required
                  type="file"
                  name="idCard"
                  onChange={(e) => setIdCard(e.target.files[0])}
                />
              </div>
              <div className="mb-2">
                <Label htmlFor="vehicleRegistration">行照</Label>
                <input
                  className="text-white"
                  required
                  type="file"
                  name="vehicleRegistration"
                  onChange={(e) => setVehicleRegistration(e.target.files[0])}
                />
              </div>
              <div className="mb-2">
                <Label htmlFor="driverLicense">駕照</Label>
                <input
                  className="text-white"
                  required
                  type="file"
                  name="driverLicense"
                  onChange={(e) => setDriverLicense(e.target.files[0])}
                />
              </div>
              <div className="mt-6">
                <Button type="submit">下一步</Button>
              </div>
            </form> */}
          </div>
        </div>
      </div>
    </div>
  );
};

export default DriverHome;
