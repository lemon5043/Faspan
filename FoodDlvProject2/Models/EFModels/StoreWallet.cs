﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FoodDlvProject2.EFModels
{
    public partial class StoreWallet
    {
        public int StoreId { get; set; }
        public long OrderId { get; set; }
        public int Total { get; set; }

        public virtual Order Order { get; set; }
        public virtual Store Store { get; set; }
    }
}