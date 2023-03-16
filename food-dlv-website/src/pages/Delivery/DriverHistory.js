import { useState, useEffect } from "react";
import driverAuthService from "../../services/Delivery/driverAuth.service";
import delieveryRecordsService from "../../services/Delivery/delieveryRecords.service";


const DriverHistory = () => {
  let [errorMessage, setErrorMessage] = useState("");
  let [delieveryRecords,setDelieveryRecords] = useState([])
  const token = localStorage.getItem("driver")
  
  useEffect(()=>{
    GetAllRecrodes()
  },[])

  const GetAllRecrodes = async (e) => {
    try {
      let driver = await driverAuthService.GetDriver(token)
      const delieveryRecords = await (await delieveryRecordsService.GetAllRecrodes(driver.data.driverId, token)).data
      console.log(delieveryRecords)
      setDelieveryRecords(delieveryRecords)
    }
    catch(e) {
      setErrorMessage(e.response.data.errorMessage[0]);
    }
  }



  async function GetRecrodesByMonth(year,month){
    try {
      const token = localStorage.getItem("driver")
      let driver = await driverAuthService.GetDriver(token)
      const driverDetail = await (await delieveryRecordsService.GetRecrodesByMonth(year, month, driver.data.driverId, token)).data
    }
    catch(e) {
      setErrorMessage(e.response.data.errorMessage[0]);
    }
  }
  return <div className="text-white">
    <thead>
      <tr>
        <th>外送次數</th>
        <th>總里程數</th>
        <th>結算月份</th>
        <th></th>
      </tr>
    </thead>
    <tbody> 
      {delieveryRecords.map((Records) => {
    return (
      <tr>
        <td>{Records.totalDelievery}</td>
        <td>{Records.totalMilage}</td>
        <td>{Records.orderDate}</td>
        <td >詳細</td>
      </tr>                      
      );
  })}
      </tbody>  
 
              
</div>;

};
export default DriverHistory;
