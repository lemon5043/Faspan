﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FoodDlvProject2.EFModels
{
    public partial class DeliveryViolationType
    {
        public int Id { get; set; }
        [Display(Name = "違規項目")]
        public string ViolationContent { get; set; }
        [Display(Name = "違規簡述")]
        public string Content { get; set; }
    }
}