﻿using FoodDlvAPI.Models.DTOs;

namespace FoodDlvAPI.Models.ViewModels
{
    public class AasignmentOrderVM
    {
        public long OrderId { get; set; }
        public string StoreAddress { get; set; }
    }

    public static class AasignmentOrderVMExts
    {
        public static AasignmentOrderVM ToAasignmentOrderVM(this AasignmentOrderDTO dTO)
            => new AasignmentOrderVM
            {
                OrderId = dTO.OrderId,
                StoreAddress = dTO.StoreAddress,
            };
    }
}