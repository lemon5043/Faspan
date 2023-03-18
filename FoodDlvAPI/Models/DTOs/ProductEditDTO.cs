namespace FoodDlvAPI.Models.DTOs
{
	public class ProductEditDTO
	{
		public long Id { get; set; }
		public string ProductName { get; set; }
		public IFormFile? Photo { get; set; }
		public string? ProductContent { get; set; }
		public bool Status { get; set; }
		public int UnitPrice { get; set; }
	}
}
