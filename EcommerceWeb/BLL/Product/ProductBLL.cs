using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using EcommerceWeb.DAL;
using EcommerceWeb.DAL.Product;
using System.Threading.Tasks;
using ProductModel = EcommerceWeb.Models.Product.Product;
using ProductImageModel = EcommerceWeb.Models.Product.ProductImage;

namespace EcommerceWeb.BLL.Product
{
    public class ProductBLL
    {
        private readonly ProductDAL _productDAL;
        private readonly ProductImageDAL _productImageDAL;
        private readonly DbHelper _db;

        public ProductBLL(DbHelper db)
        {
            _db = db;
            _productDAL = new ProductDAL(db);
            _productImageDAL = new ProductImageDAL(db);
        }

        public async Task<List<ProductModel>> GetAllProductsAsync()
        {
            return await _productDAL.GetAllAsync();
        }

        public async Task<List<ProductModel>> GetSellerProductsAsync(int sellerId)
        {
            return await _productDAL.GetBySellerAsync(sellerId);
        }

        public async Task<ProductModel> GetProductAsync(int productId)
        {
            return await _productDAL.GetByIdAsync(productId);
        }

        public List<ProductImageModel> GetProductImages(int productId)
        {
            return _productImageDAL.GetByProduct(productId);
        }

        public Dictionary<int, List<ProductImageModel>> GetAllProductImages()
        {
            var allImages = _productImageDAL.GetAllProductImages();
            var dict = new Dictionary<int, List<ProductImageModel>>();
            foreach (var img in allImages)
            {
                if (!dict.ContainsKey(img.ProductId))
                    dict[img.ProductId] = new List<ProductImageModel>();
                dict[img.ProductId].Add(img);
            }
            return dict;
        }

        public async Task<bool> AddProductAsync(ProductModel product, List<IFormFile> images, S3Helper s3)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        // Insert product and get new ProductId
                        var cmd = new SqlCommand(
                            "INSERT INTO Products (SellerId, Name, Description, Price, Stock, ImagePath) " +
                            "OUTPUT INSERTED.ProductId " +
                            "VALUES (@SellerId, @Name, @Desc, @Price, @Stock, @Img)", conn, tx);
                        cmd.Parameters.AddWithValue("@SellerId", product.SellerId);
                        cmd.Parameters.AddWithValue("@Name", product.Name);
                        cmd.Parameters.AddWithValue("@Desc", product.Description ?? "");
                        cmd.Parameters.AddWithValue("@Price", product.Price);
                        cmd.Parameters.AddWithValue("@Stock", product.Stock);
                        cmd.Parameters.AddWithValue("@Img", "");
                        int productId = (int)cmd.ExecuteScalar();

                        // Upload images to S3 and save to ProductImages
                        string firstImageUrl = "";
                        for (int i = 0; i < images.Count; i++)
                        {
                            string imageUrl = await Task.Run(() => s3.UploadImage(images[i])); if (i == 0) firstImageUrl = imageUrl;
                            _productImageDAL.Insert(productId, imageUrl, i, conn, tx);
                        }

                        // Update main ImagePath with first image
                        var updateCmd = new SqlCommand(
                            "UPDATE Products SET ImagePath = @Img WHERE ProductId = @Id", conn, tx);
                        updateCmd.Parameters.AddWithValue("@Img", firstImageUrl);
                        updateCmd.Parameters.AddWithValue("@Id", productId);
                        updateCmd.ExecuteNonQuery();

                        tx.Commit();
                        return true;
                    }
                    catch
                    {
                        tx.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}