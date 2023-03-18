import { Outlet, Link, useNavigate, useSearchParams } from "react-router-dom";
import React, { useEffect, useState } from "react";
// icons
import Logo from "../assets/images/logo.svg";
import bag from "../assets/icons/bag.svg";
import magnifyingGlass from "../assets/icons/magnifying-glass-solid.svg";
import User from "../assets/icons/user.svg";
import X from "../assets/icons/x-solid.svg";
//components
import Swal from "sweetalert2";
// services
import userAuthService from "../services/User/userAuth.service";
// styled components
import { DropdownItem, DropdownMenu, Trigger } from "./Style/dropdown-styling";
import { LayoutBtn } from "./Style/button-styling";
// mui
import Drawer from "@mui/material/Drawer";
import Cart from "../pages/Cart/Cart";
import useOverlay from "../hooks/useOverlay";
import AddressOverlay from "../pages/User/AddressOverlay";
import userAddressService from "../services/User/userAddress.service";
import FmdGoodIcon from "@mui/icons-material/FmdGood";

//mui
const drawerWidth = 360;

const Layout = ({
  currentUser,
  setCurrentUser,
  currentAddress,
  setCurrentAddress,
  cartDetail,
  setCartDetail,
}) => {
  const navigate = useNavigate();
  const { isOpen, toggleOverlay, bubblePreventer } = useOverlay();

  const logoutHandler = () => {
    Swal.fire({
      title: "您確定要登出嗎?",
      text: "期待下次跟您相見!",
      heightAuto: false,
      icon: "question",
      showCancelButton: true,
      confirmButtonColor: "#050505",
      cancelButtonColor: "#aeb2b6",
      confirmButtonText: "登出",
      cancelButtonText: "取消",
    }).then((result) => {
      if (result.isConfirmed) {
        userAuthService.logout();
        setCurrentUser(null);
        setCurrentAddress(null);
        setCartDetail(null);
        navigate("/");
      }
    });
  };
  const [open, setOpen] = useState(false);

  const handleDrawerOpen = () => {
    if (currentUser) setOpen(true);
  };

  const handleDrawerClose = () => {
    setOpen(false);
  };

  return (
    <div>
      <nav
        style={{ height: "6vh" }}
        className="bg-theme-color sticky top-0 z-10"
      >
        <ul className="flex h-full justify-between items-center w-full mr-4">
          {/* logo 標題 搜尋欄 */}
          <li className="flex items-center">
            <Link to="" className="text-2xl flex items-center px-4">
              {/* logo */}
              <div>
                <img src={Logo} alt="logo.svg" className="w-10 -rotate-12" />
              </div>
              {/* 標題 */}
              <div className="text-2xl pl-4 font-extrabold font-nunito">
                FASPAN
              </div>
            </Link>
            {/* 搜尋欄 */}
            <LayoutBtn
              className="ml-16 flex items-center"
              onClick={toggleOverlay}
            >
              <FmdGoodIcon className="mr-2" />
              {!currentAddress && <p>要送到哪呢?</p>}
              {currentAddress && <p>{currentAddress.address}</p>}
            </LayoutBtn>
            <AddressOverlay
              isOpen={isOpen}
              toggleOverlay={toggleOverlay}
              bubblePreventer={bubblePreventer}
              currentUser={currentUser}
              currentAddress={currentAddress}
              setCurrentAddress={setCurrentAddress}
              magnifyingGlass={magnifyingGlass}
              FmdGoodIcon={FmdGoodIcon}
            />
          </li>

          {/* 導覽列右側 nav */}
          <li className="flex items-center mr-4">
            <div className="relative group">
              {/* (沒登入)登入超連結*/}
              {!currentUser && (
                <div className="pr-4">
                  <LayoutBtn>
                    <Link to="/login" className="flex justify-center">
                      <img
                        src={User}
                        alt="user.svg"
                        className="w-4 mr-2 inline"
                      />
                      登入/ 註冊
                    </Link>
                  </LayoutBtn>
                </div>
              )}
              {currentUser && (
                <div>
                  <button className=" w-full px-4 font-medium">
                    <span>Hi!, {currentUser.userAccount}</span>
                  </button>
                  <Trigger className="group-hover:block">
                    <DropdownMenu className="w-28 shadow-lg">
                      <ul className="w-full text-center">
                        <DropdownItem>
                          <Link to="/cart" className="text-base">
                            個人檔案
                          </Link>
                        </DropdownItem>
                        <DropdownItem>
                          <Link to="/cart" className="text-base">
                            我的訂單
                          </Link>
                        </DropdownItem>
                        <DropdownItem>
                          <Link to="/cart" className="text-base">
                            客服中心
                          </Link>
                        </DropdownItem>
                        <DropdownItem>
                          <Link onClick={logoutHandler} className="text-base">
                            登出
                          </Link>
                        </DropdownItem>
                      </ul>
                    </DropdownMenu>
                  </Trigger>
                </div>
              )}
            </div>
            {/* 購物車超連結 */}
            <div className="p-4">
              <button>
                <img
                  src={bag}
                  alt="bag.svg"
                  className="w-4"
                  onClick={handleDrawerOpen}
                  sx={{ ...(open && { display: "none" }) }}
                />
              </button>
            </div>
          </li>
        </ul>
      </nav>
      <div className="h-full">
        <Drawer
          sx={{
            width: drawerWidth,
            flexShrink: 0,
            "& .MuiDrawer-paper": {
              width: drawerWidth,
            },
          }}
          variant="persistent"
          anchor="right"
          open={open}
        >
          <div className="flex justify-end mr-8 mt-4">
            <button onClick={handleDrawerClose}>
              <img src={X} alt="x.svg" className="w-4" />
            </button>
          </div>
          {/* <Divider /> */}
          <Cart
            currentUser={currentUser}
            currentAddress={currentAddress}
            setCartDetail={setCartDetail}
            cartDetail={cartDetail}
          />
        </Drawer>
        <Outlet />
      </div>
    </div>
  );
};

export default Layout;
