﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FoodDlvProject2.EFModels
{
    public partial class Products
    {
        public Products()
        {
            Carts = new HashSet<Carts>();
            OrderDetails = new HashSet<OrderDetails>();
            ProductRatings = new HashSet<ProductRatings>();
        }

        public long Id { get; set; }
        public long StoreId { get; set; }
        public string ProductName { get; set; }
        public string ProductContent { get; set; }

        public virtual Stores Store { get; set; }
        public virtual ICollection<Carts> Carts { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
        public virtual ICollection<ProductRatings> ProductRatings { get; set; }
    }
}