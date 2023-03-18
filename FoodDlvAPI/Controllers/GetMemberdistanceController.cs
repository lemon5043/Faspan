using FoodDlvAPI.Models;
using FoodDlvAPI.Models.DTOs;
using FoodDlvAPI.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FoodDlvAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetMemberdistanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GetMemberdistanceController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetMemberLongitudeNLatitude")]
        public async Task<List<double>> GetMemberLongitudeNLatitude(int addressId)
        {
            //if (_context.Orders == null) throw new Exception("抱歉找不到訂單，請確認後再試一次");
            //if (address == null) throw new Exception("抱歉，找不到地址，請確認後再試一次");

            //var address = await _context.Orders
            //        .Where(x => x.Id == orderId)
            //        .Select(x => new GetMemberPositionDto
            //        {
            //            //StoreAddress = x.Store.Address,
            //            //MemberId = x.MemberId,
            //            Address = _context.AccountAddresses.First(a => x.MemberId == a.MemberId).Address,
            //        }).FirstOrDefaultAsync();
            //var memberId = await _context.Carts.Where(c => c.Id == cartId).Select(c => c.MemberId).FirstOrDefaultAsync();
            var address = await _context.AccountAddresses.Where(aa => aa.Id == addressId).Select(aa => aa.Address).FirstOrDefaultAsync();

            var apiKey = await _context.Apis.Where(x => x.Id == 1).Select(x => x.Apikey).FirstOrDefaultAsync();
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={apiKey}";
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(content);

            var MemberLongitude = Convert.ToDouble(result.results[0].geometry.location.lng);
            var MemberLatitude = Convert.ToDouble(result.results[0].geometry.location.lat);
            var MembersLongitudeNLatitude = new List<double>() { MemberLongitude, MemberLatitude };

            return MembersLongitudeNLatitude;
        }

        [HttpGet("GetDistance")]
        //取得店家與會員距離
        public async Task<double> GetDistance(double storeLng, double storeLat, double MemberLongitude, double MemberLatitude)
        {
            double R = 6371; // 地球平均半徑，單位為公里
            double dLat = Math.Abs(storeLat - MemberLatitude) * Math.PI / 180;
            double dLon = Math.Abs(storeLng - MemberLongitude) * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(MemberLatitude * Math.PI / 180) * Math.Cos(storeLat * Math.PI / 180) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance_km = R * c;

            return distance_km;
        }

        [HttpGet("CalculateDistance")]
        public async Task<double> CalculateDistance(long cartId, int addressId)
        {
            var MemberLongitudeNLatitude = await GetMemberLongitudeNLatitude(addressId);
            var MemberLongitude = MemberLongitudeNLatitude[0];
            var MemberLatitude = MemberLongitudeNLatitude[1];

            //var order = await _context.Orders.Where(x => x.Id == orderId).FirstOrDefaultAsync();
            var store = await _context.Carts.Where(c => c.Id == cartId)
                .Join( _context.Stores,c => c.StoreId,s => s.Id,(c, s) => s)
                .FirstOrDefaultAsync();

            var storeLat = store.Latitude;
            var storeLng = store.Longitude;
            var gerStoreDistance = new List<GetMemberPositionDto>();
            var distance = await GetDistance(storeLng, storeLat, MemberLongitude, MemberLatitude);
            return distance;
        }
        //    //計算外送費
        [HttpGet("GetDeliveryFee")]
        public async Task<decimal> GetDeliveryFee(long cartId, int addressId)
        {
            var distance = await CalculateDistance(cartId, addressId);
            decimal feePerKm;
            if (distance < 1)
            {
                feePerKm = 20;
            }
            else if (distance >= 1 && distance < 2)
            {
                feePerKm = 25;
            }
            else if (distance >= 2 && distance < 4)
            {
                feePerKm = 30;
            }
            else if (distance >= 4 && distance < 6)
            {
                feePerKm = 40;
            }
            else
            {
                feePerKm = 100;
            }
            return feePerKm;
        }

    }

}
