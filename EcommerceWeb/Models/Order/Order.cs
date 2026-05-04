using System.Collections.Generic;
namespace EcommerceWeb.Models.Order
{
    public class Order
    {
        public int OrderId { get; set; }
        public int BuyerId { get; set; }
        public System.DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
