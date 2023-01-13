﻿using FoodDlvProject2.EFModels;
using FoodDlvProject2.Models.DTOs;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;


namespace FoodDlvProject2.Models.ViewModels
{
    public class OrderVM
    {       
        [Display(Name = "訂單編號")]
        public long Id { get; set; }

        //[Display(Name = "訂單建立時間")]
        //public DateTime OrderTime { get; set; }

        //[Display(Name = "會員編號")]
        //public int MemberId { get; set; }

        [Display(Name = "會員姓名")]
        public string MemberName { get; set; }

        //[Display(Name = "商家編號")]
        //public int StoreId { get; set; }

        [Display(Name = "商家名稱")]
        public string StoreName { get; set; }

        
        public IEnumerable<OrderSchedule> orderSchedule { get; set; }

        [Display(Name = "外送地址")]
        public string DeliveryAddress { get; set; }

        [Display(Name = "外送費")]
        public int DeliveryFee { get; set; }

        [Display(Name = "訂單總價")]
        public int Total { get; set; }
                
    }

    public static partial class OrderDtoExts
    {
        public static OrderVM ToOrderVM(this OrderDto source)
        {
            return new OrderVM
            {
                Id = source.Id,
                //OrderTime = source.OrderTime,
                MemberName = source.MemberName,
                StoreName = source.StoreName,
                orderSchedule = source.orderSchedule,
                DeliveryAddress = source.DeliveryAddress,
                DeliveryFee = source.DeliveryFee,
                Total = source.Total,                
            };
        }
    }
}
