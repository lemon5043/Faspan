using FoodDlvAPI.Models;
using FoodDlvAPI.Models.DTOs;
using FoodDlvAPI.Models.Services.Interfaces;
using FoodDlvAPI.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace FoodDlvAPI.Models.Repositories
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private AppDbContext db;

        public DeliveryRepository(AppDbContext db)
        {
            this.db = db;
        }

        //外送員切換上下線狀態
        public async Task ChangeWorkingStatus(LocationDTO location)
        {
            if (db.DeliveryDrivers == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            var EFModel = await db.DeliveryDrivers
                .Where(x => x.Id == location.DriverId)
                .Select(x => new DeliveryDriver
                {
                    Id = x.Id,
                    WorkStatuseId = x.WorkStatuseId,
                    AccountStatusId = x.AccountStatusId
                })
                .FirstOrDefaultAsync();

            if (EFModel == null) throw new Exception("您的帳號處於未開通或是禁止使用狀態，請聯絡客服了解詳細情況");

            //上線
            if (EFModel.WorkStatuseId < 3)
            {
                int AccountAvailableId = 2;
                if (EFModel.AccountStatusId != AccountAvailableId) throw new Exception("您的帳號處於未開通或是禁止使用狀態，請聯絡客服了解詳細情況");

                int onlineWorkStatusId = 3;
                EFModel.WorkStatuseId = onlineWorkStatusId;
                EFModel.Latitude= location.Latitude;
                EFModel.Longitude= location.Longitude;

                string[] updateModel = { "WorkStatuseId", "Longitude", "Latitude" };
                db.Attach(EFModel);

                foreach (var property in updateModel)
                {
                    db.Entry(EFModel).Property(property).IsModified = true;
                }

                await db.SaveChangesAsync();

                return;
            }

            //下線
            if (EFModel.WorkStatuseId == 3)
            {
                int offlineWorkStatusId = 2;

                EFModel.WorkStatuseId = offlineWorkStatusId;

                string[] updateModel = { "WorkStatuseId", "Longitude", "Latitude" };
                db.Attach(EFModel);

                foreach (var property in updateModel)
                {
                    db.Entry(EFModel).Property(property).IsModified = true;
                }

                await db.SaveChangesAsync();

                return;
            }

            throw new Exception("抱歉，找不到指定資料，請確認後再試一次");
        }
        //回傳訂單
        public async Task<AasignmentOrderDTO> GetOrderDetail(int orderId)
        {
            if (db.Orders == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            var query = await db.Orders
                .Where(x => x.Id == orderId)
                .Select(x => new AasignmentOrderDTO
                {
                    OrderId = x.Id,
                    StoreAddress = x.Store.Address,
                })
                .FirstOrDefaultAsync();

            if (query == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            return query;
        }

        public async Task<IEnumerable<DriverCancellationsDTO>> GetListAsync()
        {
            if (db.DriverCancellations == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");
            var query = await db.DriverCancellations.Select(x => new DriverCancellationsDTO
            {
                Id = x.Id,
                Reason = x.Reason,
                Content = x.Content,
            }).ToListAsync();

            return query;
        }

        public async Task<ActionResult<string>> SaveCancellationRecord(DriverCancellationRecordsDTO model)
        {
            try
            {
                db.Add(model.ToEFModel());
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CancellationRecordExists(model.OrderId)) throw new Exception("在更新資料時發生衝突。這可能是因為已經更新了相同的資料，請重新載入頁面後再進行修改。");
            }

            return "新增成功";
        }

        private bool CancellationRecordExists(long id)
        {
            return db.DriverCancellationRecords.Any(e => e.OrderId == id);
        }

        //傳送店家資料
        public Task<AasignmentOrderDTO> NavigationToStore(int orderId)
        {
            if (db.Orders == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            var query = db.Orders
                .Where(x => x.Id == orderId)
                .Select(x => new AasignmentOrderDTO
                {
                    OrderId = x.Id,
                    StoreAddress = x.Store.Address,
                    StoreName = x.Store.StoreName,
                    MemberId = x.MemberId
                    //todo OrderDetails= x.OrderDetails
                }).FirstOrDefaultAsync();


            if (query == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            return query;
        }

        //取出外送、店家地址
        public async Task<AasignmentOrderDTO> NavigationToCustomer(int orderId)
        {
            if (db.Orders == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            var query = await db.Orders
                .Where(x => x.Id == orderId)
                .Select(x => new AasignmentOrderDTO
                {
                    StoreAddress = x.Store.Address,
                    DeliveryAddress = x.DeliveryAddress,
                    MemberId= x.MemberId,
                }).FirstOrDefaultAsync();

            if (query == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            return query;
        }

        //外送員確認接單後，更新訂單
        public async Task UpdateOrder(int orderId, int driverId)
        {
            if (db.Orders == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");
            var query = await db.Orders
                .Where(x => x.Id == orderId)
                .Select(x => new Order
                {
                    Id = x.Id,
                    DeliveryDriversId = driverId,
                })
                .FirstOrDefaultAsync();

            string updateModel = "DeliveryDriversId";

            db.Attach(query);

            db.Entry(query).Property(updateModel).IsModified = true;

            db.SaveChanges();
        }
        //追加:補上OrderSchedule
        public async Task SupMarkOrderStatus(int orderId)
        {
           for (int i = 0; i < 2; i++)
            {
            if (db.OrderSchedules == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            var query = await db.OrderSchedules
                .Where(x => x.OrderId == orderId)
                .Select(x => new OrderSchedule
                {
                    OrderId = x.OrderId,
                    StatusId = x.StatusId,
                })
                .OrderBy(x => x.StatusId)
                .LastOrDefaultAsync();

            if (query == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");
           // if (query.StatusId < 3 || query.StatusId > 5) throw new Exception("抱歉，指定為不可外送狀態，請重新確認訂單狀態");
 
            query.StatusId++;
            query.MarkTime = DateTime.Now;

            db.Add(query);
            db.SaveChanges();
            }

        }
        //更新訂單狀態
        public async Task MarkOrderStatus(int orderId)
        {
            if (db.OrderSchedules == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            var query = await db.OrderSchedules
                .Where(x => x.OrderId == orderId)
                .Select(x => new OrderSchedule
                {
                    OrderId = x.OrderId,
                    StatusId = x.StatusId,
                })
                .OrderBy(x=>x.StatusId)
                .LastOrDefaultAsync();

            if (query == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");
            if (query.StatusId < 3 || query.StatusId > 5) throw new Exception("抱歉，指定為不可外送狀態，請重新確認訂單狀態");

            query.StatusId++;
            query.MarkTime = DateTime.Now;

            db.Add(query);
            db.SaveChanges();
        }
        //外送狀態切換
        public async Task ChangeDeliveryStatus(int dirverId)
        {
            if (db.DeliveryDrivers == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            var EFModel = await db.DeliveryDrivers
                .Where(x => x.Id == dirverId)
                .Select(x => new DeliveryDriver
                {
                    Id = x.Id,
                    WorkStatuseId = x.WorkStatuseId,
                })
                .FirstOrDefaultAsync();

            if (EFModel == null) throw new Exception("您的帳號處於未開通或是禁止使用狀態，請聯絡客服了解詳細情況");

            //切換為外送中
            if (EFModel.WorkStatuseId == 3)
            {
                int DeliveringId = 4;
                EFModel.WorkStatuseId = DeliveringId;

                string updateModel = "WorkStatuseId";
                db.Attach(EFModel);

                db.Entry(EFModel).Property(updateModel).IsModified = true;

                db.SaveChanges();
                return;
            }

            //切換為等待接單
            if (EFModel.WorkStatuseId == 4)
            {
                int onlineWorkStatusId = 3;

                EFModel.WorkStatuseId = onlineWorkStatusId;

                string updateModel = "WorkStatuseId";
                db.Attach(EFModel);

                db.Entry(EFModel).Property(updateModel).IsModified = true;

                db.SaveChanges();

                return;
            }
            throw new Exception("抱歉，找不到指定資料，請確認後再試一次");
        }

        public async Task UpateLocation(LocationDTO location)
        {
            if (db.DeliveryDrivers == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            var EFModel = await db.DeliveryDrivers.Where(x => x.Id == location.DriverId && x.WorkStatuseId >= 3).FirstOrDefaultAsync();

            if (EFModel == null) throw new Exception("抱歉，找不到指定外送員或此外送員為非工作狀態");

            EFModel.Latitude= location.Latitude;
            EFModel.Longitude= location.Longitude;

            string[] updateModel = { "Longitude", "Latitude" };
            db.Attach(EFModel);

            foreach (var property in updateModel)
            {
                db.Entry(EFModel).Property(property).IsModified = true;
            }

            await db.SaveChangesAsync();
        }

        public async Task<string> GetKey(string APIName)
        {
            if (db.Apis == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            var apiKey = await db.Apis.Where(x => x.Apiname == APIName).FirstOrDefaultAsync();

            if (apiKey == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            return apiKey.Apikey;
        }

        public async Task UpateOrder(DeliveryEndDTO dTO)
        {
            if (db.Orders == null) throw new Exception("抱歉，找不到指定資料，請確認後再試一次");

            var EFModel = dTO.ToEFModel();

            string updateModel = "Milage";

            db.Attach(EFModel);

            db.Entry(EFModel).Property(updateModel).IsModified = true;

            await db.SaveChangesAsync();
        }
    }
}