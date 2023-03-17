using FoodDlvAPI.Controllers;
using FoodDlvAPI.Interfaces;
using FoodDlvAPI.Models;
using FoodDlvAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

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
                DeliveryFee = Convert.ToInt32(_addressClac.GetDeliveryFee(cartId).Result),
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

        public void CreateNewOrder(int memberId, int storeId, int fee, string address)
        {
            var cart = _context.Carts.Include(c=> c.CartDetails).FirstOrDefault(c => c.Id == memberId && c.StoreId == storeId);
            var order = new OrderDTO
            {
                MemberId = cart.MemberId,
                StoreId = cart.StoreId,
                DeliveryAddress = address,
                DeliveryFee = fee,
            };
            _context.Orders.Add(order.ToOrderEF());
            _context.SaveChanges();

            var createMark = _context.Orders.First(o => o.MemberId == cart.MemberId && o.StoreId == cart.StoreId && o.CreateMark == true);
            foreach (var detail in cart.CartDetails.OrderBy(cd => cd.IdentifyNum).ThenBy(cd => cd.ItemId))
            {
                var orderDetail = new OrderDetailDTO
                {
                    IdentifyNum = detail.IdentifyNum,
                    ProductId = detail.ProductId,
                    ProductPrice = _context.Products.First(p => p.Id == detail.ProductId).UnitPrice,
                    ItemId = detail.ItemId,
                    ItemPrice = _context.ProductCustomizationItems.First(pci => pci.Id == detail.ItemId).UnitPrice,
                    Qty = detail.Qty,
                    OrderId = createMark.Id,
                };
                _context.OrderDetails.Add(orderDetail.ToOrderDetailEF());
            }

            var orderSechedule = new OrderScheduleDTO
            {
                OrderId = createMark.Id,
                StatusId = _context.OrderStatues.Min().Id,
                MarkTime = DateTime.Now,
            };
            _context.OrderSchedules.Add(orderSechedule.ToOrderScheduleEF());

            createMark.CreateMark = false;
            _context.SaveChanges();
        }

        public OrderDTO GetOrderTrack(long orderId)
        {
            var order = _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.Id == orderId).ToOrderDTO();
            var identifyGroup = order.Details.GroupBy(d => d.IdentifyNum); ;
            var orderDetail = identifyGroup.Select(gd => new OrderDetailDTO
            {
                IdentifyNum = gd.Key,
                ProductId = gd.First().ProductId,
                ProductName = _context.Products.First(p => p.Id == gd.First().ProductId).ProductName,
                ItemIds = gd.Select(d => d.ItemId).ToList(),
                ItemName = string.Join(", ", _context.ProductCustomizationItems
                    .Where(pci => gd.Select(d => d.ItemId).Contains(pci.Id))
                    .Select(pci => pci.ItemName).ToList()),
                Qty = gd.First().Qty,
                SubTotal = (
                           _context.Products.Single(p => p.Id == gd.First().ProductId).UnitPrice +
                           _context.ProductCustomizationItems.Where(pci => gd.Select(d => d.ItemId).Contains(pci.Id)).Sum(pci => pci.UnitPrice)
                           ) * gd.First().Qty,
                OrderId = order.Id,
            }).ToList();

            var orderSchedule = _context.OrderSchedules
                .Where(os => os.OrderId == order.Id)
                .Select(os => new OrderScheduleDTO
                {
                    StatusId = os.StatusId,
                    StatusName = os.Status.Status,
                    MarkTime = os.MarkTime,
                }).ToList();

            var orderTrack = new OrderDTO
            {
                Id = order.Id,
                MemberId = order.MemberId,
                MemberName = _context.Members.Where(m => m.Id == order.MemberId).Select(m => m.FirstName + " " + m.LastName).FirstOrDefault(),
                StoreId = order.StoreId,
                StoreName = _context.Stores.First(s => s.Id == order.StoreId).StoreName,
                DetailQty = orderDetail.Where(d => d.OrderId == order.Id).Sum(d => d.Qty),
                DeliveryFee = order.DeliveryFee,
                Total = orderDetail.Where(d => d.OrderId == order.Id).Sum(d => d.SubTotal) + order.DeliveryFee,
                Details = orderDetail,
                Schedules = orderSchedule,
            };

            return orderTrack;
        }
    }
}
