using FoodDlvAPI.Models;

namespace FoodDlvAPI.Models.DTOs
{
    public class OrderDTO
    {
        public long Id { get; set; }
        public int? DeliveryDriversId { get; set; }
        public int MemberId { get; set; }
        public string? MemberName { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public int DeliveryFee { get; set; }
        public int? DetailQty { get; set; }
        public int? Total { get; set; }
        public string? DeliveryAddress { get; set; }
        public int? DriverRating { get; set; }
        public int? StoreRating { get; set; }
        public string? DriverComment { get; set; }
        public string? StoreComment { get; set; }
        public decimal Milage { get; set; }
        public bool CreateMark { get; set; }
        public CartDTO? Cart { get; set; }
        public List<OrderDetailDTO>? Details { get; set; }
        public List<OrderScheduleDTO>? Schedules { get; set; }
    }

    public static partial class OrderExts
    {
        public static OrderDTO ToOrderDTO(this Order source)
        {
            var orderDTO = new OrderDTO()
            {
                Id = source.Id,
                DeliveryDriversId = source.DeliveryDriversId,
                MemberId = source.MemberId,
                StoreId = source.StoreId,
                DeliveryFee = source.DeliveryFee,
                DeliveryAddress = source.DeliveryAddress,
                DriverRating = source.DriverRating,
                StoreRating = source.StoreRating,
                DriverComment = source.DriverComment,
                StoreComment = source.StoreComment,
                Milage = source.Milage,
            };
            return orderDTO;
        }

        public static Order ToOrderEF(this OrderDTO source)
        {
            var orderEF = new Order
            {                
                MemberId = source.MemberId,
                StoreId = source.StoreId,
                DeliveryFee = source.DeliveryFee,
                DeliveryAddress = source.DeliveryAddress,
                Milage = source.Milage,
                CreateMark = source.CreateMark,
                //OrderDetails = source.Details.Select(d => d.ToOrderDetailEF()).ToList(),
            };
            return orderEF;
        }
    }
}
