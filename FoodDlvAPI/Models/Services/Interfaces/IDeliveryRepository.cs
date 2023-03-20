using FoodDlvAPI.Models.DTOs;
using FoodDlvAPI.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FoodDlvAPI.Models.Services.Interfaces
{
    public interface IDeliveryRepository
    {
        Task ChangeWorkingStatus(LocationDTO location);
        //void ChangeToOnline(int dirverId);
        Task<AasignmentOrderDTO> GetOrderDetail(int orderId);
        Task<AasignmentOrderDTO> NavigationToCustomer(int orderId);
        Task<AasignmentOrderDTO> NavigationToStore(int orderId);
        Task MarkOrderStatus(int orderId);
        Task ChangeDeliveryStatus(int dirverId);
        Task UpdateOrder(int orderId, int driverId);
        Task<ActionResult<string>> SaveCancellationRecord(DriverCancellationRecordsDTO driverCancellation);
        Task<IEnumerable<DriverCancellationsDTO>> GetListAsync();
        Task UpateLocation(LocationDTO location);
        Task<string> GetKey(string APIName);
        Task UpateOrder(DeliveryEndDTO dTO);
        Task SupMarkOrderStatus(int orderId);
    }
}