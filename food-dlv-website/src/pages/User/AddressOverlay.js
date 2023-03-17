import React, { Fragment, useEffect } from "react";
import { OverlayBg, OverlayMain } from "../../components/Style/overlay-styling";
import userAddressService from "../../services/User/userAddress.service";

const AddressOverlay = ({
  isOpen,
  toggleOverlay,
  bubblePreventer,
  currentUser,
  setAddress,
  enterHandler,
  searchHandler,
  magnifyingGlass,
}) => {
  return (
    <Fragment>
      {isOpen && (
        <div>
          <OverlayBg onClick={toggleOverlay}>
            <OverlayMain onClick={bubblePreventer}>
              <div className="relative text-gray-600 flex items-center">
                <input
                  //   onChange={(e) => setAddress(e.target.value)}
                  onKeyDown={enterHandler}
                  className="border-2 border-gray-300 bg-white h-10 w-full px-2 rounded-lg text-sm focus:border-neutral-400 focus:ring-neutral-300 focus:outline-none focus:ring focus:ring-opacity-40"
                  type="address"
                  name="address"
                  placeholder="要送到哪呢"
                />
                <button className="absolute right-0 top-0 mr-3">
                  <img
                    onClick={searchHandler}
                    src={magnifyingGlass}
                    alt="magnifyingGlass.svg"
                    className="py-3 w-4"
                  />
                </button>
              </div>
            </OverlayMain>
          </OverlayBg>
        </div>
      )}
    </Fragment>
  );
};

export default AddressOverlay;
