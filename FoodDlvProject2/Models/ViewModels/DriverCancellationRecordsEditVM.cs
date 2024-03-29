﻿using FoodDlvProject2.EFModels;
using FoodDlvProject2.Models.DTOs;
using FoodDlvProject2.Models.Infrastructures.ExtensionMethods;
using System.ComponentModel.DataAnnotations;

namespace FoodDlvProject2.Models.ViewModels
{
    public class DriverCancellationRecordsEditVM
    {
        public int Id { get; set; }
        [Display(Name = "訂單編號")]
        public long OrderId { get; set; }
        [Display(Name = "外送員編號")]
        public int DriverId { get; set; }
        [Display(Name = "姓名")]
        public string DriverName { get; set; }
        [Display(Name = "取消原因")]
        public int CancellationId { get; set; }
        [Display(Name = "取消日期")]
        [Required(ErrorMessage = "請輸入{0}")]
        [DateNow(ErrorMessage ="{0}不可大於今日")]
        public DateTime CancellationDate { get; set; }
    }

    public static class DriverCancellationRecordsEditVMExts
    {
        public static DriverCancellationRecordDTO ToDriverCancellationRecordDTO(this DriverCancellationRecordsEditVM source)
        {
            return new DriverCancellationRecordDTO
			{
                Id = source.Id,
                CancellationId = source.CancellationId,
                CancellationDate = source.CancellationDate,
            };
        }

		public static DriverCancellationRecordsEditVM ToDriverCancellationRecordsEditVM(this DriverCancellationRecordDTO source)
		{
			return new DriverCancellationRecordsEditVM
			{
				Id = source.Id,
				OrderId = source.OrderId,
				DriverId = source.DriverId,
				DriverName = source.DriverName,
                CancellationId= source.CancellationId,
				CancellationDate = source.CancellationDate,
			};
		}
	}
}