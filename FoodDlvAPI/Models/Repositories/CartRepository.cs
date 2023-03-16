using FoodDlvAPI.Interfaces;
using FoodDlvAPI.Models;
using FoodDlvAPI.Models.DTOs;
using FoodDlvAPI.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace FoodDlvAPI.Models.Repositories
{
    public class CartRepository : ICartRepository
    {
        //Fields
        private readonly AppDbContext _context;

        //Constructors
        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool IsExists(int memberId, int storeId)
        {
            var memberCheck = _context.Members.Any(m => m.Id == memberId);
            var storeCheck = _context.Stores.Any(s => s.Id == storeId);

            if (memberCheck && storeCheck)
            {
                var isExists = _context.Carts.Any(c => c.MemberId == memberId && c.StoreId == storeId);
                return isExists;
            }
            else
            {
                throw new Exception("帳號或商店不存在");
            }
        }

        public CartDTO Load(int memberId, int storeId)
        {
            var data = _context.Carts
                .AsNoTracking()
                .Include(c => c.CartDetails)
                .Single(c => c.MemberId == memberId && c.StoreId == storeId)
                .ToCartDTO();

            return data;
        }

        public CartDTO CreateNewCart(int memberId, int storeId)
        {
            var cart = new Cart
            {
                MemberId = memberId,
                StoreId = storeId,
            };
            _context.Carts.Add(cart);
            _context.SaveChanges();

            return Load(memberId, storeId);
        }

        public void EmptyCart(int memberId, int storeId)
        {
            var cart = _context.Carts.SingleOrDefault(c => c.MemberId == memberId && c.StoreId == storeId);
            if (cart == null) return;
            _context.Carts.Remove(cart);
            _context.SaveChanges();
        }

        public void AddDetail(CartDTO cart, CartDTO request)
        {
            if (_context.Stores.Any(m => m.Id == cart.StoreId) == false)
            {
                throw new Exception("此商店不存在");
            }
            if (_context.Products.Any(p => p.StoreId == cart.StoreId && p.Id == request.Details.First().ProductId) == false)
            {
                throw new Exception("商店無此商品");
            }
            if (request.Details.First().Qty <= 0)
            {
                throw new Exception("商品數量不可小於0");
            }

            List<int?> listItemId = request.Details.SelectMany(d => d.ItemsId ?? Enumerable.Empty<int?>()).ToList();

            if (!(listItemId?.Count == 1 && listItemId.Contains(null)))
            {
                var invalidItemIds = listItemId.Where(itemId => _context.ProductCustomizationItems
                                    .Any(pci => pci.ProuctId == request.Details.First().ProductId && pci.Id == itemId) == false)
                                    .ToList();

                if (invalidItemIds.Count > 0)
                {
                    throw new Exception($"客製化編號{string.Join(", ", invalidItemIds)}號不屬於該產品");
                }
            }

            if (listItemId.Count == 0)
            {
                listItemId.Add(null);
            }

            var details = cart.Details.Where(d => d.ProductId == request.Details.First().ProductId);
            var selectDetailItem = details.OrderBy(d => d.IdentifyNum).ThenBy(d => d.ItemsId).GroupBy(d => d.IdentifyNum).Select(gd => gd.Select(d => d.ItemId).ToList()).ToList();
            var identifyNum = details.OrderBy(d => d.IdentifyNum).GroupBy(d => d.IdentifyNum).Select(gd => gd.Key).ToList();
            List<int?> item = new List<int?>();
            int count = -1;
            foreach (var items in selectDetailItem)
            {
                count++;
                var sameDetail = items.SequenceEqual(listItemId);
                if (sameDetail)
                {
                    item = items;
                    break;
                }
            }

            var targetDtail = details.Where(d => d.IdentifyNum == identifyNum[count]);
            if (item.Count == 0 || !targetDtail.Any())
            {
                foreach (int? itemId in listItemId)
                {
                    var newDetail = new CartDetailDTO
                    {
                        IdentifyNum = IdentifyNumSelector(cart.Id),
                        ProductId = request.Details.First().ProductId,
                        ItemId = itemId,
                        Qty = request.Details.First().Qty,
                        CartId = cart.Id
                    };
                    _context.CartDetails.Add(newDetail.ToCartDetailEF());
                }
                _context.SaveChanges();
            }
            else
            {
                foreach (var detail in targetDtail)
                {
                    detail.Qty += request.Details.First().Qty;

                    _context.CartDetails.Update(detail.ToCartDetailEF());
                }
                _context.SaveChanges();
            }
        }

        public int IdentifyNumSelector(long cartId)
        {
            var selectIdentifyNum = _context.CartDetails.Where(d => d.CartId == cartId).Select(cd => cd.IdentifyNum);

            int identifyNum;
            if (!selectIdentifyNum.Any())
            {
                identifyNum = 1;
            }
            else
            {
                identifyNum = selectIdentifyNum.Max() + 1;
            }

            return identifyNum;
        }

        public List<CartDTO> GetCartInfos(int memberId)
        {
            var carts = _context.Carts
                .Include(c => c.CartDetails)
                .Where(c => c.MemberId == memberId).ToList();

            var cartInfos = carts.GroupBy(c => c.StoreId)
                .Select(gc => new CartDTO
                {
                    Id = gc.First().Id,
                    MemberId = gc.First().MemberId,
                    MemberName = _context.Members.Where(m => m.Id == gc.First().MemberId)
                        .Select(m => m.FirstName + " " + m.LastName).FirstOrDefault(),
                    StoreId = gc.First().StoreId,
                    StoreName = _context.Stores.Where(s => s.Id == gc.First().StoreId).Select(s => s.StoreName).FirstOrDefault(),
                    DetailQty = gc.SelectMany(c => c.CartDetails).GroupBy(d => d.IdentifyNum).Sum(gd => gd.First().Qty),
                    Total = gc.SelectMany(c => c.CartDetails).GroupBy(d => d.IdentifyNum).Sum(gd =>
                    {
                        var productId = gd.First().ProductId;
                        var itemsId = gd.Select(d => d.ItemId).ToList();
                        var productPrice = _context.Products.First(p => p.Id == productId).UnitPrice;
                        var itemsPrice = _context.ProductCustomizationItems.Where(pci => itemsId.Contains(pci.Id)).Sum(pci => pci.UnitPrice);
                        var qty = gd.First().Qty;

                        return (productPrice + itemsPrice) * qty;
                    }),

                    Details = gc.SelectMany(c => c.CartDetails).GroupBy(d => d.IdentifyNum).Select(gd =>
                    {
                        var productId = gd.First().ProductId;
                        var itemsId = gd.Select(d => d.ItemId).ToList();
                        var productPrice = _context.Products.First(p => p.Id == productId).UnitPrice;
                        var itemsPrice = _context.ProductCustomizationItems.Where(pci => itemsId.Contains(pci.Id)).Sum(pci => pci.UnitPrice);
                        var qty = gd.First().Qty;

                        return new CartDetailDTO
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
                            CartId = gd.First().CartId,
                        };
                    }).ToList()
                }).ToList();

            return cartInfos;
        }
        public void RemoveDetail(int identifyNum)
        {
            var target = _context.CartDetails.Where(cd => cd.IdentifyNum == identifyNum).ToList();
            if (target.Count == 0) return;

            _context.CartDetails.RemoveRange(target);
            _context.SaveChanges();
        }
    }
}
