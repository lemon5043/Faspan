using FoodDlvAPI.Models.DTOs;

namespace FoodDlvAPI.Models.ViewModels
{
	public class BalanceVM
	{
		public int Balance { get; set; }
	}

	public static class GetBalanceVmExts
	{
		public static BalanceVM ToBalanceVM(this PaymentDTO dTO)
		{
			return new BalanceVM { Balance = dTO.Balance, };
		}
	}
}
