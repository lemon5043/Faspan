﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FoodDlvProject2.EFModels
{
    public partial class Order
    {
        public Order()
        {
            DeliveryDriversRatings = new HashSet<DeliveryDriversRating>();
            DriverCancellationRecords = new HashSet<DriverCancellationRecord>();
            OrderDetails = new HashSet<OrderDetail>();
            OrderSchedules = new HashSet<OrderSchedule>();
            StoreCancellationRecords = new HashSet<StoreCancellationRecord>();
            StoreRatings = new HashSet<StoreRating>();
        }

        public long Id { get; set; }
        public int DeliveryDriversId { get; set; }
        public int MemberId { get; set; }
        public int StoreId { get; set; }
        public int DeliveryFee { get; set; }
        public string DeliveryAddress { get; set; }

        public virtual DeliveryDriver DeliveryDrivers { get; set; }
        public virtual Member Member { get; set; }
        public virtual Store Store { get; set; }
        public virtual ICollection<DeliveryDriversRating> DeliveryDriversRatings { get; set; }
        public virtual ICollection<DriverCancellationRecord> DriverCancellationRecords { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<OrderSchedule> OrderSchedules { get; set; }
        public virtual ICollection<StoreCancellationRecord> StoreCancellationRecords { get; set; }
        public virtual ICollection<StoreRating> StoreRatings { get; set; }
    }
}