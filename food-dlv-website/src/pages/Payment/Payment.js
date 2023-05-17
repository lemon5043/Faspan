import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Label, Input, Button, Box } from "../../components/Style/form-styling";
import paymentService from "../../services/Payment/payment.service";

const Payment = ({ currentUser }) => {
  const [balance, setBalance] = useState(undefined);
  const displayBalance = async (userId) => {
    const res = await paymentService.getUserBalance(userId);
    setBalance(res.data);
  };

  useEffect(() => {
    displayBalance(currentUser.userId);
  }, []);

  return (
    <div className="relative flex flex-col justify-center min-h-screen overflow-hidden">
      <Box>
        <h2 className="text-3xl font-semibold mb-4">Faspay</h2>
        <div>
          <p>可用額度</p>
          {balance && <h1 className="text-2xl pt-2">${balance.balance}</h1>}
          <Button className="mt-2">儲值</Button>
        </div>
      </Box>
    </div>
  );
};

export default Payment;
