﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FoodDlvProject2.EFModels
{
    public partial class DriverCancellation
    {
        public DriverCancellation()
        {
            DriverCancellationRecords = new HashSet<DriverCancellationRecord>();
        }

        public int Id { get; set; }
        [Display(Name = "取消原因")]
        public string Reason { get; set; }
        [Display(Name = "原因簡述")]
        public string Content { get; set; }

        public virtual ICollection<DriverCancellationRecord> DriverCancellationRecords { get; set; }
    }
}