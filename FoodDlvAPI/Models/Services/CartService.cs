using FoodDlvAPI.Interfaces;
using FoodDlvAPI.Models;
using FoodDlvAPI.Models.DTOs;
using FoodDlvAPI.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace FoodDlvAPI.Models.Services
{
    public class CartService
    {
        //Fields       
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;


        //Constructors
        public CartService(ICartRepository cartRepository,
                            IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public void AddToCart(CartDTO request)
        {
            var cart = Current(request.MemberId, request.StoreId);
            _cartRepository.AddDetail(cart, request);
        }

        public List<CartDTO> CartInfos(int memberId)
        {            
            var cartInfo = _cartRepository.GetCartInfos(memberId);
            return cartInfo;
        }


        public CartDTO Current(int memberId, int storeId)
        {
            if (_cartRepository.IsExists(memberId, storeId))
            {
                return _cartRepository.Load(memberId, storeId);
            }
            else
            {
                return _cartRepository.CreateNewCart(memberId, storeId);
            }
        }

        public void UpdateCart(CartDTO request)
        {
            _cartRepository.RemoveDetail(request.Details.First().IdentifyNum);
            if (request.Details.First().Qty >= 1)
            {
                AddToCart(request);
            }
        }

        public void RemoveDetail(int identifyNum)
        {
            _cartRepository.RemoveDetail(identifyNum);
        }

        public void DeleteCart(int memberId, int storeId)
        {
            _cartRepository.EmptyCart(memberId, storeId);
        }

        public void CheckOutCart(long cartId)
        {              
            var cart = _cartRepository.CheckOutCart(cartId);
            if (cart.Details.Count == 0 || cart.Details == null)
            {
                throw new Exception("購物車內無商品, 無法進行結帳");
            }
        }


    }
}
