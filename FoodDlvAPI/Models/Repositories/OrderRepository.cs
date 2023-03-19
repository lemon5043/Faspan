using FoodDlvAPI.Controllers;
using FoodDlvAPI.Interfaces;
using FoodDlvAPI.Models;
using FoodDlvAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;

namespace FoodDlvAPI.Models.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        //Fields
        private readonly AppDbContext _context;
        private readonly ICartRepository _cartRepository;
        private readonly GetMemberdistanceController _addressClac;


        //Constructors
        public OrderRepository(AppDbContext context)
        {
            _context = context;
            _cartRepository = new CartRepository(_context);
            _addressClac = new GetMemberdistanceController(_context);
        }


        public OrderDTO GetOrderInfo(long cartId, int addressId)
        {
            int memberId = _context.Carts.First(c => c.Id == cartId).MemberId;

            var orderInfo = new OrderDTO()
            {
                Cart = _cartRepository.GetCartInfos(memberId).First(c => c.Id == cartId),
                DeliveryAddress = _context.AccountAddresses.First(aa => aa.Id == addressId).Address,
                DeliveryFee = Convert.ToInt32(_addressClac.GetDeliveryFee(cartId, addressId).Result),
            };
            return orderInfo;
        }

        public void CheckOutTime(int storeId)
        {
            var nowTime = DateTime.Now.TimeOfDay;
            int nowDay = Convert.ToInt32(DateTime.Now.DayOfWeek);

            var storeState = _context.StorePrincipals.First(sp => sp.Id == _context.Stores.First(s => s.Id == storeId).StorePrincipalId).AccountStatusId == 2;

            if (storeState)
            {
                var targetStore = _context.StoreBusinessHours.Where(sbh => sbh.StoreId == storeId).ToList();
                if (targetStore == null)
                {
                    return;
                }
                var timeRange = targetStore.FirstOrDefault(sbh => sbh.OpeningDays == nowDay && sbh.OpeningTime <= nowTime && sbh.ClosingTime >= nowTime);
                if (timeRange == null)
                {
                    throw new Exception("目前剛商家非營業狀態");
                }
            }
            else
            {
                throw new Exception("目前剛商家非營業狀態");
            }
        }

        public void CashTransfer(int memberId, int storeId, int fee)
        {
            int memberWallet = _context.Members.First(m => m.Id == memberId).Balance;
            var cart = _context.Carts
                .AsNoTracking()
                .Include(c => c.CartDetails)
                .First(c => c.MemberId == memberId && c.StoreId == storeId);
            var identifyGroup = cart.CartDetails.GroupBy(d => d.IdentifyNum).ToList();
            int cartTotal = fee;

            foreach (var group in identifyGroup)
            {
                var product = _context.Products.First(p => p.Id == group.First().ProductId);
                var item = _context.ProductCustomizationItems.Where(pci => group.Select(d => d.ItemId).Contains(pci.Id)).ToList();
                int groupTotal = (product.UnitPrice + item.Sum(pci => pci.UnitPrice)) * group.First().Qty;
                cartTotal += groupTotal;
            }

            if (memberWallet >= cartTotal)
            {
                memberWallet -= cartTotal;
                var member = _context.Members.First(m => m.Id == memberId);
                member.Balance = memberWallet;
                _context.Members.Update(member);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("會員帳戶餘額不足");
            }
        }

        public long CreateNewOrder(int memberId, int storeId, int fee, int addressId)
        {
            try
            {
                var cart = _context.Carts.Include(c => c.CartDetails).Where(c => c.MemberId == memberId && c.StoreId == storeId);
                var targetCart = cart.First();
                var order = new OrderDTO
                {
                    MemberId = targetCart.MemberId,
                    StoreId = targetCart.StoreId,
                    DeliveryAddress = _context.AccountAddresses.First(aa => aa.Id == addressId).Address,
                    DeliveryFee = fee,
                    Milage = 0,
                    CreateMark = true,
                };

                _context.Orders.Add(order.ToOrderEF());
                _context.SaveChanges();

                var createMark = _context.Orders.First(o => o.MemberId == targetCart.MemberId && o.StoreId == targetCart.StoreId && o.CreateMark == true);
                foreach (var detail in targetCart.CartDetails.OrderBy(cd => cd.IdentifyNum).ThenBy(cd => cd.ItemId))
                {
                    var identifyNum = detail.IdentifyNum;
                    var productId = detail.ProductId;
                    var productPrice = _context.Products.First(p => p.Id == detail.ProductId).UnitPrice;
                    var itemId = detail.ItemId;
                    var itemPrice = _context.ProductCustomizationItems.Where(pci => pci.Id == detail.ItemId).Select(pci => pci.UnitPrice).FirstOrDefault();
                    var qty = detail.Qty;
                    var orderId = createMark.Id;
                    var orderDetail = new OrderDetailDTO
                    {
                        IdentifyNum = identifyNum,
                        ProductId = productId,
                        ProductPrice = productPrice,
                        ItemId = itemId,
                        ItemPrice = itemPrice,
                        Qty = qty,
                        OrderId = orderId,
                    };
                    _context.OrderDetails.Add(orderDetail.ToOrderDetailEF());
                }

                var orderSechedule = new OrderScheduleDTO
                {
                    OrderId = createMark.Id,
                    StatusId = _context.OrderStatues.Select(os => os.Id).Min(),
                    MarkTime = DateTime.Now,
                };
                _context.OrderSchedules.Add(orderSechedule.ToOrderScheduleEF());

                createMark.CreateMark = false;
                _context.SaveChanges();
                return createMark.Id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public OrderDTO GetOrderTrack(int memberId)
        {
            try
            {
                var order = _context.Orders
                    .Include(o => o.OrderDetails)
                    .Include(o => o.OrderSchedules)
                    .Where(o => o.MemberId == memberId && o.OrderSchedules.Max(os => os.StatusId) == 1)
                    .First();          
                
                var orderTrack = new OrderDTO
                {
                    Id = order.Id,
                    MemberId = order.MemberId,
                    MemberName = _context.Members.Where(m => m.Id == order.MemberId).Select(m => m.FirstName + " " + m.LastName).FirstOrDefault(),
                    StoreId = order.StoreId,
                    StoreName = _context.Stores.Where(s => s.Id == order.StoreId).Select(s => s.StoreName).First(),
                    DetailQty = order.OrderDetails.GroupBy(od => od.IdentifyNum).Sum(gd => gd.First().Qty),
                    DeliveryFee = order.DeliveryFee,
                    Total = order.OrderDetails.GroupBy(od => od.IdentifyNum).Sum(gd =>
                    {
                        var productId = gd.First().ProductId;
                        var itemsId = gd.Select(d => d.ItemId).ToList();
                        var productPrice = _context.Products.First(p => p.Id == productId).UnitPrice;
                        var itemsPrice = _context.ProductCustomizationItems.Where(pci => itemsId.Contains(pci.Id)).Sum(pci => pci.UnitPrice);
                        var qty = gd.First().Qty;

                        return (productPrice + itemsPrice) * qty + order.DeliveryFee;
                    }),
                    
                    Details = order.OrderDetails.GroupBy(od => od.IdentifyNum).Select(gd =>
                    {
                        var productId = gd.First().ProductId;
                        var itemsId = gd.Select(d => d.ItemId).ToList();
                        var productPrice = _context.Products.First(p => p.Id == productId).UnitPrice;
                        var itemsPrice = _context.ProductCustomizationItems.Where(pci => itemsId.Contains(pci.Id)).Sum(pci => pci.UnitPrice);
                        var qty = gd.First().Qty;

                        return new OrderDetailDTO
                        {
                            IdentifyNum = gd.Key,
                            ProductId = productId,
                            ProductName = _context.Products.First(p => p.Id == productId).ProductName,
                            ItemsId = itemsId,
                            ItemName = string.Join(", ", _context.ProductCustomizationItems
                            .Where(pci => itemsId.Contains(pci.Id))
                            .Select(pci => pci.ItemName).ToList()),
                            Qty = qty,
                            SubTotal = (productPrice + itemsPrice) * qty,

                        };
                    }).ToList(),

                    Schedules = order.OrderSchedules.Select(os => new OrderScheduleDTO
                    {
                        OrderId = os.OrderId,
                        StatusId = os.StatusId,
                        StatusName = _context.OrderStatues.Where(ost => ost.Id == os.StatusId).First().Status,
                        MarkTime = os.MarkTime,
                    }).ToList()
                };                                            

                return orderTrack;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int getFee(long cartId, int addressId)
        {
            var fee = Convert.ToInt32(_addressClac.GetDeliveryFee(cartId, addressId).Result);

            return fee;
        }
    }
}
