namespace FoodDlvAPI.Models.Services
{
    public class UserConnection
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Room =>Role.ToLower()+Id.ToString();
    }
}
