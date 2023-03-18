using FoodDlvAPI.Models.DTOs;
using FoodDlvAPI.Interfaces;

namespace FoodDlvAPI.Models.Services
{
    public class OrderService
    {
        //Fields
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly AppDbContext _context;

        //Constructors
        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, AppDbContext context)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _context = context;
        }

        public OrderDTO OrderInfo(long cartId, int addressId)
        {           
            var orderInfo = _orderRepository.GetOrderInfo(cartId, addressId);
            return orderInfo;
        }

        public void CheckOutTime(int storeId)
        {
            _orderRepository.CheckOutTime(storeId);
        }

        public void OrderEstablished(long cartId, int addressId)
        {
            var memberId = _context.Carts.Where(c => c.Id == cartId).Select(c => c.MemberId).First();
            var storeId = _context.Carts.Where(c => c.Id == cartId).Select(c => c.StoreId).First();
            var fee = _orderRepository.getFee(cartId, addressId);
            _orderRepository.CashTransfer(memberId, storeId, fee);
            _orderRepository.CreateNewOrder(memberId, storeId, fee, addressId);            
            _cartRepository.EmptyCart(memberId, storeId);
        }

        public OrderDTO OrderTracking(long orderId)
        {
            var orderTracking = _orderRepository.GetOrderTrack(orderId);
            return orderTracking;
        }
    }
}
