﻿using FoodDlvAPI.Models;
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
        public async Task<List<double>> GetMemberLongitudeNLatitude(int orderId)
        {
            
                var address = await _context.Orders
                .Where(x => x.Id == orderId)
                .Select(x => _context.AccountAddresses
                    .Where(a => x.MemberId == a.MemberId)
                    .Select(a => a.Address)
                    .FirstOrDefault())
                .FirstOrDefaultAsync();


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
        public async Task<double> CalculateDistance(int orderId)
        {
            var MemberLongitudeNLatitude = await GetMemberLongitudeNLatitude(orderId);
            var MemberLongitude = MemberLongitudeNLatitude[0];
            var MemberLatitude = MemberLongitudeNLatitude[1];
          
            var order = await _context.Orders.Where(x => x.Id == orderId).FirstOrDefaultAsync();
            var store = await _context.Stores.Where(x => x.Id == order.StoreId).FirstOrDefaultAsync();
            var storeLat = store.Latitude;
            var storeLng = store.Longitude;
            var gerStoreDistance = new List<GetMemberPositionDto>();
            var distance = await GetDistance(storeLng, storeLat, MemberLongitude, MemberLatitude);
            return distance;
        }
        //    //計算外送費
        [HttpGet("GetDeliveryFee")]
        public async Task<decimal> GetDeliveryFee(int orderId)
        {
            var distance = await CalculateDistance(orderId);
            decimal feePerKm;
            if (distance < 1)
            {
                feePerKm = 20;
            }
            else if (distance >= 1 && distance < 5)
            {
                feePerKm = 25;
            }
            else if (distance >= 5 && distance < 10)
            {
                feePerKm = 30;
            }
            else if(distance >= 10 && distance < 15)
            {
                feePerKm = 40;
            }
            else
            {
                feePerKm= 100;
            }
            return feePerKm;
        }

    }

}
