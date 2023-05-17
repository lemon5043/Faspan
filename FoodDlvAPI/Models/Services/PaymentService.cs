using FoodDlvAPI.Interfaces;
using FoodDlvAPI.Models.DTOs;

namespace FoodDlvAPI.Models.Services
{
	public class PaymentService
	{
		private IPaymentRepository _repository;

		public PaymentService(IPaymentRepository repository)
		{
			_repository = repository;
		}

		public async Task<PaymentDTO> GetBalanceDTO(int id)
			=> await _repository.GetBalanceAsync(id);
	}
}
