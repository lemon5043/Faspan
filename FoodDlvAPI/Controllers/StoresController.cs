using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDlvAPI.Models;
using Newtonsoft.Json;
using FoodDlvAPI.Models.DTOs;
using FoodDlvAPI.Models.ViewModels;
using System.Drawing.Text;

namespace FoodDlvAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StoresController : ControllerBase
	{
		private readonly AppDbContext _context;

		public StoresController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet("getStoreCategories")]
		public async Task<ActionResult<IEnumerable<StoreCategory>>> GetStoreCategories()
		{
			return await _context.StoreCategories.ToListAsync();
		}



		/// <summary>
		/// 傳入搜尋字串、頁碼、顯示數量、所在地點、餐廳類別ID，先依據搜尋字串與類別ID選出商店名稱、商店類別名稱、商店的商品名稱與搜尋字串有關的商店
		/// ，如果與搜尋字串有關的商品狀態為FALSE則不列出，然後利用所在地與商店的經緯度計算距離後依距排列(如果沒傳入類別ID則顯示全部餐廳)
		/// </summary>
		/// <param name="pageNum">頁碼</param>
		/// <param name="storeNum">顯示數量</param>
		/// <param name="origin">所在地點</param>
		/// <param name="categoryId">餐廳類別ID</param>
		/// <param name="searchString">搜尋字串</param>
		/// <returns>List<StoreGetDTO>getSomeStoresOrderByDistance</returns>


		[HttpGet("getSomeStoresIfIMAt/")]
		public async Task<ActionResult<IEnumerable<StoreGetDTO>>> GetSomeThisCategoryOfStoresIfIMAt(int pageNum, int storeNum, string? origin, int? categoryId, string? searchString)
		{
			if (origin == null)
			{
				origin = "聖德基督學院";
			}
			List<StoreGetDTO> getSomeStores;
			//如果有搜尋字串
			if (!string.IsNullOrEmpty(searchString) || !string.IsNullOrWhiteSpace(searchString))
			{
				//如果有搜尋字串有類別ID
				if (categoryId != null)
				{

					getSomeStores = await _context.Stores.Include(x => x.StoresCategoriesLists).ThenInclude(x => x.Category).Include(x => x.Products).
				   Where(s => s.StoreName.Contains(searchString)
				   || s.StoresCategoriesLists.Any(scl => scl.Category.CategoryName.Contains(searchString))
				   || s.Products.Any(p => p.ProductName.Contains(searchString) && s.Products.Any(p => p.Status == true))).Where(x => x.StoresCategoriesLists.Any(scl => scl.CategoryId
				   == categoryId)).Select(x => new StoreGetDTO
				   {
					   Id = x.Id,
					   StorePrincipalId = x.StorePrincipalId,
					   StoreName = x.StoreName,
					   Address = x.Address,
					   ContactNumber = x.ContactNumber,
					   Photo = x.Photo,
					   CategoryName = x.StoresCategoriesLists.Select(s => s.Category.CategoryName),
					   Longitude = x.Longitude,
					   Latitude = x.Latitude,
				   }).ToListAsync();
				}
				//如果有搜尋字串沒有類別ID
				else
				{
					getSomeStores = await _context.Stores.Include(x => x.StoresCategoriesLists).ThenInclude(x => x.Category).Include(x => x.Products).
				   Where(s => s.StoreName.Contains(searchString)
				   || s.StoresCategoriesLists.Any(scl => scl.Category.CategoryName.Contains(searchString))
				   || s.Products.Any(p => p.ProductName.Contains(searchString) && s.Products.Any(p => p.Status == true))).Select(x => new StoreGetDTO
				   {
					   Id = x.Id,
					   StorePrincipalId = x.StorePrincipalId,
					   StoreName = x.StoreName,
					   Address = x.Address,
					   ContactNumber = x.ContactNumber,
					   Photo = x.Photo,
					   CategoryName = x.StoresCategoriesLists.Select(s => s.Category.CategoryName),
					   Longitude = x.Longitude,
					   Latitude = x.Latitude,
				   }).ToListAsync();
				}
			}
			//如果沒有搜尋字串
			else
			{
				//如果沒有搜尋字串有類別ID
				if (categoryId != null)
				{
					getSomeStores = await _context.Stores
						.Include(s => s.StoresCategoriesLists)
						.ThenInclude(x => x.Category)
						.Where(x => x.StoresCategoriesLists.Any(scl => scl.CategoryId == categoryId))
						.Select(x => new StoreGetDTO
						{
							Id = x.Id,
							StorePrincipalId = x.StorePrincipalId,
							StoreName = x.StoreName,
							Address = x.Address,
							ContactNumber = x.ContactNumber,
							Photo = x.Photo,
							CategoryName = x.StoresCategoriesLists.Select(s => s.Category.CategoryName),
							Longitude = x.Longitude,
							Latitude = x.Latitude,

						})
						.ToListAsync();
				}
				//如果沒有搜尋字串沒有類別ID
				else
				{
					getSomeStores = await _context.Stores
						.Include(s => s.StoresCategoriesLists)
						.ThenInclude(x => x.Category)
						.Select(x => new StoreGetDTO
						{
							Id = x.Id,
							StorePrincipalId = x.StorePrincipalId,
							StoreName = x.StoreName,
							Address = x.Address,
							ContactNumber = x.ContactNumber,
							Photo = x.Photo,
							CategoryName = x.StoresCategoriesLists.Select(s => s.Category.CategoryName),
							Longitude = x.Longitude,
							Latitude = x.Latitude,
						})
						.ToListAsync();
				}
			}
			var OriginsLongitudeNLatitude = await GetOriginsLongitudeNLatitude(origin);
			var getSomeStoresWithDistance = new List<StoreGetDTO>();

			foreach (var store in getSomeStores)
			{
				var distance = await GetDistance(store.Longitude, store.Latitude, OriginsLongitudeNLatitude[0], OriginsLongitudeNLatitude[1]);
				store.Distance = distance;
				getSomeStoresWithDistance.Add(store);
			}

			var getSomeStoresOrderByDistance = getSomeStoresWithDistance.OrderBy(x => x.Distance).Skip((pageNum - 1) * storeNum).Take(storeNum).ToList();
			return getSomeStoresOrderByDistance;
		}

		//取得所在地經緯度
		private async Task<List<double>> GetOriginsLongitudeNLatitude(string origin)
		{
			var apiKey = await _context.Apis.Where(x => x.Id == 1).Select(x => x.Apikey).FirstOrDefaultAsync();
			var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={origin}&key={apiKey}";
			using var client = new HttpClient();
			var response = await client.GetAsync(url);
			var content = await response.Content.ReadAsStringAsync();
			dynamic result = JsonConvert.DeserializeObject(content);

			var OriginsLongitude = Convert.ToDouble(result.results[0].geometry.location.lng);
			var OriginsLatitude = Convert.ToDouble(result.results[0].geometry.location.lat);
			var OriginsLongitudeNLatitude = new List<double>() { OriginsLongitude, OriginsLatitude };

			return OriginsLongitudeNLatitude;
		}
		//取得店家與所在地距離
		private async Task<double> GetDistance(double storeLng, double storeLat, double originLng, double originLat)
		{
			double R = 6371; // 地球平均半徑，單位為公里
			double dLat = Math.Abs(storeLat - originLat) * Math.PI / 180;
			double dLon = Math.Abs(storeLng - originLng) * Math.PI / 180;
			double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(originLat * Math.PI / 180) * Math.Cos(storeLat * Math.PI / 180) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
			double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			double distance_km = R * c;



			//double dLat = Math.Abs(storeLat - originLat);
			//double dLon = Math.Abs(storeLng - originLng);

			//var distance_km = Math.Sqrt(Math.Pow(dLon, 2) + Math.Pow(dLat, 2)) * 110.9362;


			return distance_km;

		}

		//進入商店頁面顯示其資訊包含商品資訊

		[HttpGet("storeDetail/{storeId}")]

		public async Task<ActionResult<IEnumerable<StoreDetailDTO>>> GetStoreDetail(int storeId)
		{

			var getStoreDetail = await _context.Stores.Include(s => s.StoresCategoriesLists).ThenInclude(x => x.Category).Include(x => x.Products).Where(x => x.Id == storeId).Select(x => new StoreDetailDTO
			{
				Id = x.Id,
				StorePrincipalId = x.StorePrincipalId,
				StoreName = x.StoreName,
				Address = x.Address,
				ContactNumber = x.ContactNumber,
				Photo = x.Photo,

				CategoryName = x.StoresCategoriesLists.Select(s => s.Category.CategoryName)
			   ,
				Products=x.Products.Where(x=>x.StoreId== storeId).Where(x=>x.Status!=false)

			})
				  .ToListAsync();

			return getStoreDetail;

		}




		//4店家擁有商店
		[HttpGet("myStores/{storePrincipalId}")]

		public async Task<ActionResult<IEnumerable<Store>>> GetMyStores(int storePrincipalId)
		{
			return await _context.Stores.Where(x => x.StorePrincipalId == storePrincipalId).ToListAsync();

		}


		//5店家擁有商店頁面

		[HttpGet("myStoreDetail/{storeId}")]
		public async Task<ActionResult<IEnumerable<MyStoreDetailDTO>>> GetMyStoreDetail(int storeId)
		{

			var getMyStoreDetail = await _context.Stores.Include(s => s.StoresCategoriesLists).ThenInclude(x => x.Category).Where(x => x.Id == storeId).Include(x => x.Products).Select(x => new MyStoreDetailDTO
			{
				Id = x.Id,
				StorePrincipalId = x.StorePrincipalId,
				StoreName = x.StoreName,
				Address = x.Address,
				ContactNumber = x.ContactNumber,
				Photo = x.Photo,


				CategoryId = x.StoresCategoriesLists.Select(s => s.Category.Id),
				CategoryName = x.StoresCategoriesLists.Select(s => s.Category.CategoryName)

			   ,
				Products = x.Products.Where(x => x.StoreId == storeId)


			})
				  .ToListAsync();

			return getMyStoreDetail;


		}

		/// <summary>
		/// 取得自己的店家輸入Id取得，傳入更改的資訊myStoreDetailEditDTO修改以下資料
		/// </summary>
		/// <param name="id">我的店家Id</param>
		/// <param name="myStoreDetailEditDTO">修改的值</param>
		/// 
		/// <returns>"修改成功"</returns>
	


		//6商店內部資訊修改
		[HttpPut("myStoreDetail/Edit/{id}")]
		public async Task<string> PutStore(int id, MyStoreDetailEditDTO myStoreDetailEditDTO)
		{
			try
			{
				var store = await _context.Stores.FindAsync(id);

				store.StoreName = myStoreDetailEditDTO.StoreName;
				store.Address = myStoreDetailEditDTO.Address;
				store.ContactNumber = myStoreDetailEditDTO.ContactNumber;



				//如果傳入照片有值
				if (myStoreDetailEditDTO.Photo != null)
				{
					// 刪除舊圖片
					if (store.Photo != null)
					{
						var oldFilePath = Path.Combine(
							Directory.GetCurrentDirectory(), "../food-dlv-website/src/assets/images/public/Stores/",
							store.Photo);

						if (System.IO.File.Exists(oldFilePath))
						{
							System.IO.File.Delete(oldFilePath);
						}
					}


					var now = DateTime.Now;
					var fileName = $"{myStoreDetailEditDTO.StoreName}{now.Year}{now.Month}{now.Day}{now.Hour}{now.Minute}{now.Second}{myStoreDetailEditDTO.Address}.jpg";
					var filePath = Path.Combine(
						Directory.GetCurrentDirectory(), "../food-dlv-website/src/assets/images/public/Stores/",
						fileName);
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await myStoreDetailEditDTO.Photo.CopyToAsync(stream);
					}
					store.Photo = fileName;
				}


				//目前我的店家的類別
				var Categories = await _context.StoresCategoriesLists.Where(x => x.StoreId == id).ToListAsync();
				//新類別
				var newCategories = myStoreDetailEditDTO.CategoryIds.Select(x => new StoresCategoriesList { StoreId = id, CategoryId = x });
				_context.StoresCategoriesLists.RemoveRange(Categories);
				_context.StoresCategoriesLists.AddRange(newCategories);







				//我的店家的商品
				var products = await _context.Products.Where(x => x.StoreId == id).ToListAsync();
				foreach (var product in products)
				{
					//修改已有的商品
					var productFromDto = myStoreDetailEditDTO.Products.FirstOrDefault(x => x.Id == product.Id);
					//如果更新的資料有值
					if (productFromDto != null)
					{
						product.ProductName = productFromDto.ProductName;
						product.ProductContent = productFromDto.ProductContent;
						product.Status = productFromDto.Status;
						product.UnitPrice = productFromDto.UnitPrice;


						if (productFromDto.Photo != null)
						{
							// 刪除舊圖片
							if (product.Photo != null)
							{
								var oldFilePath = Path.Combine(
								Directory.GetCurrentDirectory(), "../food-dlv-website/src/assets/images/public/Products/",
								product.Photo);

								if (System.IO.File.Exists(oldFilePath))
								{
									System.IO.File.Delete(oldFilePath);
								}

							}

							var now = DateTime.Now;
							var fileName = $"{productFromDto.ProductName}{now.Year}{now.Month}{now.Day}{now.Hour}{now.Minute}{now.Second}.jpg";
							var filePath = Path.Combine(
								Directory.GetCurrentDirectory(), "../food-dlv-website/src/assets/images/public/Products/",
								fileName);
							using (var stream = new FileStream(filePath, FileMode.Create))
							{
								await productFromDto.Photo.CopyToAsync(stream);
							}
							product.Photo = fileName;
						}

					}
					//如果更新的資料有沒值就刪除商品
					else
					{
						_context.Products.Remove(product);
					}
				}

				//新增新商品進資料庫
				var newProducts = myStoreDetailEditDTO.Products.Where(x => x.Id == null).Select(x => new Product
				{
					StoreId = id,
					ProductName = x.ProductName,
					Photo = PruductPicToString(x.Photo, x.ProductName),
					ProductContent = x.ProductContent,
					Status = x.Status,
					UnitPrice = x.UnitPrice
				});

				_context.Products.AddRange(newProducts);

				await _context.SaveChangesAsync();

			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}


			return "修改成功";
		}


		private string? PruductPicToString(IFormFile? pic, string ProductName)
		{
			if (pic != null)
			{
				var now = DateTime.Now;
				var fileName = $"{ProductName}{now.Year}{now.Month}{now.Day}{now.Hour}{now.Minute}{now.Second}.jpg";
				var filePath = Path.Combine(
					Directory.GetCurrentDirectory(), "../food-dlv-website/src/assets/images/public/Products/",
					fileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
				pic.CopyTo(stream);
				}
				return fileName;
			}
			else
			{
				return null;
			}
		}



		//1訂單建立
		//2餐點準備中
		//3餐點完成待配送
		//4配送中
		//5配送到達
		//6訂單完成
		//9配送失敗



		//取得該店家狀態介於1到3的訂單資訊

		[HttpGet("StoreGetOrderList")]
		public async Task<ActionResult<IEnumerable<OrderDetail>>> StoreGetOrderList(int storeId)
		{
			//var orders =_context.Orders.Include(x => x.OrderSchedules).Include(x => x.OrderDetails).ThenInclude(x => x.Product)
			//	.ThenInclude(x => x.ProductCustomizationItems).Where(x => x.StoreId == storeId)
			//	.Where(x => x.OrderSchedules.Any(x => x.StatusId > 0 && x.StatusId < 4))
			//	;

			var ordersDetail = await _context.OrderDetails.Include(x => x.Order).ThenInclude(x => x.OrderSchedules).Include(x => x.Product).ThenInclude(x => x.ProductCustomizationItems).Where(x => x.Order.StoreId == storeId)
				.Where(x => x.Order.OrderSchedules.Any(x => x.StatusId > 0 && x.StatusId < 4)).ToListAsync();

			return ordersDetail;


		}
		//店家歷史訂單列表



		//訂單接收同意
		[HttpPut("AgreeReqAfterGetOrderRequestFromCutomer")]
		
		public async Task<string> AgreeReqAfterGetReq(int orderId)
		{
			//var orderSchedules = await _context.OrderSchedules.FirstOrDefaultAsync(x => x.OrderId == orderId);
			//orderSchedules.StatusId = 2;
			//_context.Update(orderSchedules);
			//await _context.SaveChangesAsync();

			if (_context.OrderSchedules == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

			var query = await _context.OrderSchedules
				.Where(x => x.OrderId == orderId)
				.Select(x => new OrderSchedule
				{
					OrderId = x.OrderId,
					StatusId = x.StatusId,
				}).OrderBy(x=>x.StatusId).LastOrDefaultAsync();

			if (query == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");
			if (query.StatusId !=1) throw new Exception("抱歉，該筆為不可讓店家接收餐點狀態，請重新確認訂單狀態");

			query.StatusId++;
			query.MarkTime = DateTime.Now;

			_context.Add(query);
            await _context.SaveChangesAsync();
            return "餐點準備中";
		}
		////訂單接收拒絕
		//public async Task<string> DeclineReqAfterGetReq(int orderId)
		//{
		//	if (_context.OrderSchedules == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

		//	var query = await _context.OrderSchedules
		//		.Where(x => x.OrderId == orderId)
		//		.Select(x => new OrderSchedule
		//		{
		//			OrderId = x.OrderId,
		//			StatusId = x.StatusId,
		//		}).OrderBy(x => x.StatusId).LastOrDefaultAsync();

		//	if (query == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");
		//	//if (query.StatusId != 1) throw new Exception("抱歉，該筆訂單未建立，不可讓店家接收餐點狀態，請重新確認訂單狀態");

		//	query.StatusId=9;
		//	query.MarkTime = DateTime.Now;

		//	_context.Add(query);
		//	_context.SaveChanges();
		//	return "訂單接收已取消";
		//}
		//訂單接收同意後取消
		//public async Task<string> CancelReqAfterAgreeReq(int orderId)
		//{


		//}

		//訂單傳送
		[HttpPut("SendReqToDeliver")]
		public async Task<SendReqToDeliverVM> SendReqToDeliver(int orderId)
		{
			if (_context.OrderSchedules == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

			var query = await _context.OrderSchedules
				.Where(x => x.OrderId == orderId)
				.Select(x => new OrderSchedule
				{
					OrderId = x.OrderId,
					StatusId = x.StatusId,
				}).OrderBy(x => x.StatusId).LastOrDefaultAsync();

			if (query == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");
			if (query.StatusId != 2) throw new Exception("抱歉，該筆訂單未在準備中，不可讓該餐點狀態變為待配中，請重新確認訂單狀態");


			var order = await _context.Orders.Where(x => x.Id == orderId).FirstOrDefaultAsync();
			var storeId = order.StoreId;
			var store =await _context.Stores.FirstOrDefaultAsync(x => x.Id == storeId);


         var deliveryDrivers = await _context.DeliveryDrivers.Where(x => x.Latitude.HasValue && x.Longitude.HasValue)
    .Where(x => x.Longitude != null && x.Latitude != null).ToListAsync();




			var deliveryDriver = deliveryDrivers.Select(x => new StoreToDeliverVM
			{
				Id = x.Id,
				AccountStatusId = x.AccountStatusId,
				WorkStatuseId = x.WorkStatuseId,
				FirstName = x.FirstName,
				LastName = x.LastName,
				Phone = x.Phone,
				BankAccount = x.BankAccount,
				Idcard = x.Idcard,
				RegistrationTime = x.RegistrationTime,
				VehicleRegistration = x.VehicleRegistration,
				Account = x.Account,
				Password = x.Password,
				DriverLicense = x.DriverLicense,
				Latitude = x.Latitude,
				Longitude = x.Longitude,

				distance = GetDistanceForStoreToDriver(store.Longitude, store.Latitude, x.Longitude, x.Latitude)

			}).OrderBy(x => x.distance).FirstOrDefault();


            if (deliveryDriver == null) throw new Exception("目前沒有可用的外送員");
			query.StatusId++;
			query.MarkTime = DateTime.Now;

			_context.Add(query);
			await _context.SaveChangesAsync();

			var sendReqToDeliverVM = new SendReqToDeliverVM
			{
				AlertString = "餐點待配中",
				OrderId = orderId,
				deliveryDriverId = deliveryDriver.Id

			};


			return sendReqToDeliverVM;
		}
		private double GetDistanceForStoreToDriver(double storeLng, double storeLat, double? originLng, double? originLat)
		{

            if (storeLng == 0 || storeLat == 0 || !originLng.HasValue || !originLat.HasValue)
            {
                throw new Exception("傳入經緯度有問題請查詢店家經緯度或是外送員經緯度是否有誤");
            }
            double R = 6371; // 地球平均半徑，單位為公里

			

			double dLat = Math.Abs((double)(storeLat - originLat)) * Math.PI / 180;
			double dLon = Math.Abs((double)(storeLng - originLng)) * Math.PI / 180;
			double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos((double)(originLat * Math.PI / 180)) * Math.Cos(storeLat * Math.PI / 180) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
			double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			double distance_km = R * c;



			//double dLat = Math.Abs(storeLat - originLat);
			//double dLon = Math.Abs(storeLng - originLng);

			//var distance_km = Math.Sqrt(Math.Pow(dLon, 2) + Math.Pow(dLat, 2)) * 110.9362;


			return distance_km;

		}
	}
}















