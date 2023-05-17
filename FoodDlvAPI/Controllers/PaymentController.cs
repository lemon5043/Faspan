using FoodDlvAPI.Interfaces;
using FoodDlvAPI.Models;
using FoodDlvAPI.Models.Repositories;
using FoodDlvAPI.Models.Services;
using FoodDlvAPI.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FoodDlvAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly PaymentService _paymentService;

        public PaymentController()
        {
			var db = new AppDbContext();
			IPaymentRepository repository = new PaymentRepository(db);
			_paymentService = new PaymentService(repository);
        }

		/// <summary>
		/// 找出特定 member 的餘額
		/// </summary>
		/// <param name="id"></param>
		/// <returns>該 member 的餘額</returns>
		/// <exception cref="Exception"></exception>
		[HttpGet("{id}")]

		public async Task<ActionResult<BalanceVM>> GetBalance(int id)
		{
			try
			{
				var data = await _paymentService.GetBalanceDTO(id);
				return data.ToBalanceVM();

			}
			catch (Exception ex)
			{

				throw new Exception(ex.Message);
			}
		}
    }
}
