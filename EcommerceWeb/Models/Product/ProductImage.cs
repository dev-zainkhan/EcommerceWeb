namespace EcommerceWeb.Models.Product
{
    public class ProductImage
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; }
        public string ImagePath { get; set; }
        public int SortOrder { get; set; }
    }
}