using FoodDlvAPI.Models.DTOs;
using FoodDlvAPI.Interfaces;
using FoodDlvAPI.Models;
using FoodDlvAPI.Models.Services;
using FoodDlvAPI.Models.Repositories;
using FoodDlvAPI.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FoodDlvAPI.Controllers
{
    public class OrderController : Controller
    {
        //Fields
        private readonly AppDbContext _context;
        private OrderService _orderService;
        private CartService _cartService;

        //Constructors
        public OrderController(AppDbContext context)
        {
            _context = context;
            IOrderRepository orderRepo = new OrderRepository(_context);
            ICartRepository cartRepo = new CartRepository(_context);
            IProductRepository productRepo = new ProductRepository(_context);
            

            this._orderService = new OrderService(orderRepo, cartRepo, context);
            this._cartService = new CartService(cartRepo, productRepo);
        }

        [HttpGet("OrderInfo")]
        public IActionResult OrderInfo(long cartId, int addressId)
        {
            try
            {                
                var orderInfo = _orderService.OrderInfo(cartId, addressId).ToOrderInfoVM();
                return Json(orderInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost("OrderEstablished")]
        public IActionResult OrderEstablished(long cartId, int addressId) 
        {
            try
            {
                _cartService.CheckOutCart(cartId);
                //_orderService.CheckOutTime(storeId);
                _orderService.OrderEstablished(cartId, addressId);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("OrderTracking")]
        public IActionResult OrderTracking(long orderId)
        {
            try
            {
                var orderTracking = _orderService.OrderTracking(orderId);
                return Json(orderTracking);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
