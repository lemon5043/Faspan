namespace FoodDlvAPI.Models.ViewModels
{
	public class SendReqToDeliverVM
	{
		public string AlertString { get; set; }
		public int OrderId { get; set; }
		public int deliveryDriverId { get; set; }
	}
}
