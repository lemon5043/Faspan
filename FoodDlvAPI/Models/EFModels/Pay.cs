﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FoodDlvAPI.Models
{
    public partial class Pay
    {
        public int Id { get; set; }
        public int DeliveryDriversId { get; set; }
        public int DeliveryCount { get; set; }
        public int TotalMilage { get; set; }
        public int Bouns { get; set; }
        public int TotalPay { get; set; }
        public DateOnly SettlementMonth { get; set; }

        public virtual DeliveryDriver DeliveryDrivers { get; set; }
    }
}