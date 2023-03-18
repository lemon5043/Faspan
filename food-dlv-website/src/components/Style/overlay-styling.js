import styled from "styled-components";
// layout 的 z-index 是10
export const OverlayBg = styled.div`
  background-color: rgba(0, 0, 0, 0.5);
  width: 100vw;
  height: 100vh;
  position: fixed;
  top: 0;
  left: 0;
  z-index: 20;
`;

export const OverlayMain = styled.div`
  background-color: #fff;
  position: fixed;
  top: 0;
  left: 0;
  bottom: 0;
  right: 0;
  margin: auto;
  z-index: 30;
  height: fit-content;
  border-radius: 0.5rem;
`;
