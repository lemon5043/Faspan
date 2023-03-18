using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FoodDlvProject2.Models.ViewModels
{
    public class StoreCreateVM
    {

        public int Id { get; set; }
        [Display(Name = "店家合作夥伴帳號")]
        [Required(ErrorMessage = "{0}必填")]
        public int StorePrincipalId { get; set; }
        [Display(Name = "商店名稱")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(50)]
        public string StoreName { get; set; }
        [Display(Name = "地址")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(100)]
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Display(Name = "聯絡電話")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(10)]
        public string ContactNumber { get; set; }
        [Display(Name = "圖片")]

        public IFormFile? Photo { get; set; }


    }
}
