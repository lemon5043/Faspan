using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FoodDlvAPI.Models.ViewModels
{
	public class StorePrincipalEditVM
	{
		[Display(Name = "名字")]
		[Required(ErrorMessage = "{0}必填")]
		[StringLength(50)]
		public string FirstName { get; set; }
		[Display(Name = "姓氏")]
		[Required(ErrorMessage = "{0}必填")]
		[StringLength(50)]
		public string LastName { get; set; }
		[Display(Name = "電話")]
		[Required(ErrorMessage = "{0}必填")]
		[StringLength(10)]
		public string Phone { get; set; }
		[Display(Name = "性別")]
		[Required]
		public bool Gender { get; set; }
		[Display(Name = "生日")]
		[Required(ErrorMessage = "生日必填")]
		[DataType(DataType.Date)]
		public DateTime Birthday { get; set; }
		[Required(ErrorMessage = "{0}必填")]
		[StringLength(50)]
		[EmailAddress]
		public string Email { get; set; }
		[Display(Name = "密碼")]
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "{0}必填")]
		[StringLength(30)]
		public string Password { get; set; }
		[Display(Name = "確認密碼")]
		[Required(ErrorMessage = "{0}必填")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "{0}與密碼不相符")]
		[StringLength(30)]


		public string ConfirmedPassword { get; set; }

	}
}
