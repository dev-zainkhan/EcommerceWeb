namespace EcommerceWeb.Models.Shipping
{
    public class ShippingAddress
    {
        public int AddressId { get; set; }
        public int OrderId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string AddressLine { get; set; }
    }
}