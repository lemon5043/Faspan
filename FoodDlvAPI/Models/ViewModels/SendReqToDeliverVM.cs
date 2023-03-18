namespace FoodDlvAPI.Models.ViewModels
{
    public class StoreToDeliverVM
    {
        public int Id { get; set; }
        public int AccountStatusId { get; set; }
        public int WorkStatuseId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string BankAccount { get; set; }
        public string Idcard { get; set; }
        public DateTime RegistrationTime { get; set; }
        public string VehicleRegistration { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string DriverLicense { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }




        public double distance { get; set; }
        
    }
}
