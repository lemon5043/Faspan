using FoodDlvAPI.Models.DTOs;

namespace FoodDlvAPI.Interfaces
{
	public interface IPaymentRepository
	{
		Task<PaymentDTO> GetBalanceAsync(int id);
	}
}
