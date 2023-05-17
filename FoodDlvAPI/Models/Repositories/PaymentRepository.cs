using FoodDlvAPI.Interfaces;
using FoodDlvAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FoodDlvAPI.Models.Repositories
{
	public class PaymentRepository : IPaymentRepository
	{
		private AppDbContext db;

		public PaymentRepository(AppDbContext db)
		{
			this.db = db;
		}

		public async Task<PaymentDTO> GetBalanceAsync(int id)
		{
			var data = await db.Members.Select(x => new PaymentDTO { 
				Id = x.Id,
				Balance = x.Balance,
			}).FirstOrDefaultAsync(m =>m.Id == id);
			if (data == null) throw new Exception("cannot find data");
			return data;
		}
	}
}
