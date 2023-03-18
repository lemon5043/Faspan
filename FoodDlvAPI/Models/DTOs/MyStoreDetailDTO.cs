using FoodDlvAPI.Models;

namespace FoodDlvAPI.Models.DTOs
{
    public class MyStoreDetailDTO
    {
        public int Id { get; set; }
        public int StorePrincipalId { get; set; }
        public string StoreName { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string? Photo { get; set; }


		public IEnumerable<int> CategoryId { get; set; }
		public IEnumerable<string> CategoryName { get; set; }



		public IEnumerable<Product> Products { get; set; }

	}
}
