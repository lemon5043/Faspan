import React, { Fragment, useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { OverlayBg, OverlayMain } from "../../components/Style/overlay-styling";
import UserAddressService from "../../services/User/userAddress.service";
import { WhiteBtn } from "../../components/Style/button-styling";

const AddressOverlay = ({
  isOpen,
  toggleOverlay,
  bubblePreventer,
  currentAddress,
  currentUser,
  setCurrentAddress,
  magnifyingGlass,
  FmdGoodIcon,
}) => {
  const navigate = useNavigate();
  const [address, setAddress] = useState([]);
  const [input, setInput] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const enterHandler = (e) => {
    if (e.key === "Enter") {
      searchHandler();
    }
  };

  const searchHandler = async () => {
    if (!currentUser) {
      navigate("/login");
      toggleOverlay();
      return;
    }
    navigate("/store/" + "");
    toggleOverlay();
  };

  const chooseAddress = useCallback(async () => {
    if (!currentUser) {
      return;
    }
    const response = await UserAddressService.getAddress(currentUser.userId);
    setAddress(response.data);
  });

  const CreateNewAddress = useCallback(async () => {
    try {
      await UserAddressService.createAddress(currentUser.userId, input);
    } catch (e) {
      setErrorMessage("請輸入正確的地址");
      console.log(e);
    }
  });

  // useEffect(() => {
  //   chooseAddress();
  // }, [CreateNewAddress]);

  // useEffect(() => {
  //   chooseAddress();
  // }, [address]);

  // useEffect(() => {
  //   chooseAddress();
  // }, [currentUser]);

  return (
    <Fragment>
      {isOpen && (
        <div>
          <OverlayBg onClick={toggleOverlay}>
            <OverlayMain className="p-4 w-96" onClick={bubblePreventer}>
              <div>
                <h1 className="text-3xl mb-2 font-semibold">外送詳細資訊</h1>
                <div className="mb-3">
                  <div className="relative text-gray-600 flex items-center">
                    <input
                      //   onChange={(e) => setAddress(e.target.value)}
                      onKeyDown={enterHandler}
                      className="border-2 border-gray-300 bg-white h-10 w-full px-2 rounded-lg text-sm focus:border-neutral-400 focus:ring-neutral-300 focus:outline-none focus:ring focus:ring-opacity-40"
                      type="address"
                      name="address"
                      placeholder="要送到哪呢?"
                      onChange={setInput}
                    />
                    <button className="absolute right-0 top-0 mr-3">
                      <img
                        onClick={CreateNewAddress}
                        src={magnifyingGlass}
                        alt="magnifyingGlass.svg"
                        className="py-3 w-4"
                      />
                    </button>
                  </div>
                  <p className="text-red-500 pl-2 text-sm">{errorMessage}</p>
                </div>
                <div>
                  {address.length !== 0 &&
                    address.map((d) => {
                      return (
                        <WhiteBtn
                          key={d.id}
                          onClick={searchHandler}
                          className="w-full flex my-1 py-2"
                        >
                          <FmdGoodIcon className="mr-2" />
                          {d.address}
                        </WhiteBtn>
                      );
                    })}
                </div>
              </div>
            </OverlayMain>
          </OverlayBg>
        </div>
      )}
    </Fragment>
  );
};

export default AddressOverlay;
