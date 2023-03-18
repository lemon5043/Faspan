namespace FoodDlvAPI.Models.DTOs
{
	public class MyStoreDetailEditDTO
	{

		public string StoreName { get; set; }
		public string Address { get; set; }
		public string ContactNumber { get; set; }
		public IFormFile? Photo { get; set; }
		public IEnumerable<int> CategoryIds { get; set; }
		public IEnumerable<ProductEditDTO> Products { get; set; }
	}
}
