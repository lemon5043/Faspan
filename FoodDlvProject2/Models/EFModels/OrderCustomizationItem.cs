﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FoodDlvProject2.EFModels
{
    public partial class OrderCustomizationItem
    {
        public OrderCustomizationItem()
        {
            CartCustomizationItems = new HashSet<CartCustomizationItem>();
            OrderAssItems = new HashSet<OrderAssItem>();
        }

        public int Id { get; set; }
        public int ProuctId { get; set; }
        public string ItemName { get; set; }
        public int UnitPrice { get; set; }

        public virtual ICollection<CartCustomizationItem> CartCustomizationItems { get; set; }
        public virtual ICollection<OrderAssItem> OrderAssItems { get; set; }
    }
}