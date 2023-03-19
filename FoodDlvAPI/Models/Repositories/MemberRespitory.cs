using FoodDlvAPI.Controllers;
using FoodDlvAPI.Interfaces;
using FoodDlvAPI.Models;
using FoodDlvAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;

namespace FoodDlvAPI.Models.Repositories
{
    public class MemberRespitory
    {
        public class MemberRepository : IMemberRepository
        {
            private readonly AppDbContext db;

            public MemberRepository(AppDbContext db)
            {
                this.db = db;
            }
            public async Task<string> CreateAsync(MemberRegisterDto model)
            {
                if (AccountExists(model.Account))
                { 
                  return ("此帳號已被使用，請重新輸入");
				} 
				try
                {
                    var EFModel = model.ToMember();

                    db.Members.Add(EFModel);
                    await db.SaveChangesAsync();
				}
                catch (Exception ex)
                {
					return ("註冊失敗");
				}

                return "新增成功";
            }
            public async Task<string> EditAsync(MemberRegisterDto model)
            {
                try
                {
                    var EFModel = model.ToMember();

                    List<string> updateModel = new List<string> { "LastName", "FirstName","Phone",
                    };
                    db.Members.Attach(EFModel);

                    foreach (var property in updateModel)
                    {
                        db.Entry(EFModel).Property(property).IsModified = true;
                    }

                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(model.Id)) throw new Exception("在修改時發生衝突，請再試一次。");
                }

                return "修改成功";

            }

            public async Task<MemberDTO> GetmemberAsync(int? id)
            {
                if (db.Members == null) throw new Exception("找不到會員資料，請確認後再試一次");

                var query = await db.Members.Select(x => new MemberDTO
                {
                    Id = x.Id,
                    AccountStatusId = x.AccountStatusId,
                    Account = x.Account,
                    LastName = x.LastName,
                    FirstName = x.FirstName,
                    Phone = x.Phone,
                    Balance = x.Balance,
                    RegistrationTime = x.RegistrationTime,
                }).FirstOrDefaultAsync(m => m.Id == id);

                if (query == null) throw new Exception("找不到會員資料");

                return query;
            }
            public async Task<MemberDTO> GetEditAsync(int? id)
            {
                if (db.Members == null) throw new Exception("找不到會員資料，請確認後再試一次");

                var query = await db.Members.Select(x => new MemberDTO
                {
                    Id = x.Id,
                    AccountStatusId = x.AccountStatusId,
                    Account = x.Account,
                    LastName = x.LastName,
                    FirstName = x.FirstName,
                    Phone = x.Phone,
                    RegistrationTime = x.RegistrationTime,
                }).FirstOrDefaultAsync(m => m.Id == id);

                if (query == null) throw new Exception("找不到會員資料");

                return query;
            }
            public bool AccountExists(string account)
            {
                return db.Members.Any(e => e.Account == account);
            }
			public async Task<string> GetKey(string APIName)
			{
				if (db.Apis == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

				var apiKey = await db.Apis.Where(x => x.Apiname == APIName).FirstOrDefaultAsync();

				if (apiKey == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

				return apiKey.Apikey;
			}
			//public async Task<GetMemberPositionDto> GetMemberPosition(int orderId)
			//{
			//	if (db.Orders == null) throw new Exception("抱歉找不到資料，請確認後再試一次");
			//	var query = await db.Orders
			//		.Where(x => x.Id == orderId)
			//		.Select(x => new GetMemberPositionDto
			//		{
			//			StoreAddress = x.Store.Address,
   //                     MemberId=x.MemberId,
   //                     Address = db.AccountAddresses.First(a=>x.MemberId==a.MemberId).Address,
			//		})
   //                 .FirstOrDefaultAsync();

			//	if (query == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

			//	return query;
			//}
   //         public async Task GetMemberPosition( location) 
   //         {
   //             if (db.AccountAddresses == null) throw new Exception("找不到指定資料,請確認後再試一次");
   //             var EFModel=location.ToMemberEFModel();
   //             string[] memberlocation = { "longitude", "latitude" };
   //             db.Attach(EFModel);
			//	foreach (var property in memberlocation)
			//	{
			//		db.Entry(EFModel).Property(property).IsModified = true;
			//	}

			//	await db.SaveChangesAsync();
			//}

			public bool MemberExists(int id)
            {
                return db.Members.Any(e => e.Id == id);
            }
            public MemberRegisterDto Load(string account)
                => db.Members.SingleOrDefault(x => x.Account == account).ToEntity();
			
           
			
            //將經緯度存回資料庫
			//public async Task<string> CreateMemberLongitudeNLatitudeAsync(MemberAccountAddressDto model)
			//{
			//	try
			//	{
			//		var EFModel = model.ToEFmodel();

			//		db.AccountAddresses.Add(EFModel);
			//		await db.SaveChangesAsync();
			//	}
			//	catch (DbUpdateConcurrencyException)
			//	{
			//		throw new Exception("發生異常狀況,請重新輸入");
			//	}

			//	return "新增成功";
			//}
		}

    }
}
