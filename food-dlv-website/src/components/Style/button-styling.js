import styled from "styled-components";

export const LayoutBtn = styled.button`
  background-color: rgb(212, 212, 216);
  font-weight: 600;
  padding: 0.5rem 1rem;
  border-radius: 9999px;
  &:hover {
    background-color: rgb(161, 161, 170);
  }
`;

export const Btn = styled.button`
  background-color: rgb(32, 27, 27);
  color: #fff;
  font-weight: 600;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  &:hover {
    background-color: rgb(137, 137, 155);
  }
`;

export const WhiteBtn = styled.button`
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 300ms;
  &:hover {
    background-color: rgb(228, 228, 231);
    color: rgb(5, 150, 105);
  }
`;
