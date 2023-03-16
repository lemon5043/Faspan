using FoodDlvAPI.Models.DTOs;
using FoodDlvAPI.Models.Services.Interfaces;
using FoodDlvAPI.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;

namespace FoodDlvAPI.Models.Services
{
    public class DeliveryService
    {
        private readonly IDeliveryRepository _repository;

        public DeliveryService(IDeliveryRepository repository)
        {
            _repository = repository;
        }

        public async Task ChangeWorkingStatus(LocationDTO location)     
           => await _repository.ChangeWorkingStatus(location);

        public async Task<AasignmentOrderDTO> GetOrderDetail(int orderId)
            => await _repository.GetOrderDetail(orderId);

        public async Task<AasignmentOrderDTO> UpdateOrder(int orderId, int driverId)
        {
            await _repository.UpdateOrder(orderId, driverId);
            await _repository.MarkOrderStatus(orderId);
            await _repository.ChangeDeliveryStatus(driverId);
            return await _repository.NavigationToStore(orderId);
        }

        public async Task MarkOrderStatus(DeliveryEndDTO dTO)
        {
            await _repository.MarkOrderStatus(dTO.OrderId);
            await _repository.ChangeDeliveryStatus(dTO.DriverId);
            await _repository.UpateOrder(dTO);
        }


        public async Task<string> NavigationToCustomer(int orderId)
        {
            var query = await _repository.NavigationToCustomer(orderId);
            //string token = await _repository.GetKey("GoogleMap");
            //string start = query.StoreAddress;
            string end = query.DeliveryAddress;
            // Create a request for the URL. 		
            //WebRequest request = WebRequest.Create("https://maps.googleapis.com/maps/api/directions/json?language=zh-TW&origin=" + start + "&destination=" + end + "&key=" + token);
            // If required by the server, set the credentials.
            //request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            //StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            //string responseFromServer = reader.ReadToEnd();

            // Cleanup the streams and the response.
            //reader.Close();
            //dataStream.Close();
            //response.Close();

            return end;
        }

        public async Task<IEnumerable<DriverCancellationsDTO>> GetListAsync()
             => await _repository.GetListAsync();

        public async Task<ActionResult<string>> SaveCancellationRecord(DriverCancellationRecordsDTO driverCancellation)
            => await _repository.SaveCancellationRecord(driverCancellation);

        public async Task UpateLocation(LocationDTO location)
            => await _repository.UpateLocation(location);
    }
}