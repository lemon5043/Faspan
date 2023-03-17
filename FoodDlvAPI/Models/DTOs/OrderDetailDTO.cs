using FoodDlvAPI.Models;

namespace FoodDlvAPI.Models.DTOs
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public int IdentifyNum { get; set; }
        public long ProductId { get; set; }
        public string? ProductName { get; set; }
        public int ProductPrice { get; set; }
        public int? ItemId { get; set; }
        public List<int?>? ItemsId { get; set; }
        public string? ItemName { get; set; }
        public int ItemPrice { get; set; }
        public int Qty { get; set; }
        public int SubTotal { get; set; }
        public long OrderId { get; set; }
    }

    public static partial class OrderDetailExts
    {
        public static OrderDetailDTO ToOrderDetailDTO(this OrderDetail source)
        {
            var orderDetailDTO = new OrderDetailDTO
            {
                Id = source.Id,
                IdentifyNum = source.IdentifyNum,
                ProductId = source.ProductId,
                ProductPrice = source.ProductPrice,
                ItemId = source.ItemId,
                ItemPrice = source.ItemPrice,
                Qty = source.Qty,
                OrderId = source.OrderId,
            };
            return orderDetailDTO;
        }

        public static OrderDetail ToOrderDetailEF(this OrderDetailDTO source)
        {
            var orderDetailEF = new OrderDetail
            {
                Id = source.Id,
                IdentifyNum = source.IdentifyNum,
                ProductId = source.ProductId,
                ProductPrice = source.ProductPrice,
                ItemId = source.ItemId,
                ItemPrice = source.ItemPrice,
                Qty = source.Qty,
                OrderId = source.OrderId,
            };
            return orderDetailEF;
        }
    }
}
