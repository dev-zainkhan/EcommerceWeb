using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using EcommerceWeb.Models.Product;

namespace EcommerceWeb.DAL.Product
{
    public class ProductImageDAL
    {
        private readonly DbHelper _db;
        public ProductImageDAL(DbHelper db) { _db = db; }

        public void Insert(int productId, string imagePath, int sortOrder, SqlConnection conn, SqlTransaction tx)
        {
            var cmd = new SqlCommand(
                "INSERT INTO ProductImages (ProductId, ImagePath, SortOrder) " +
                "VALUES (@ProductId, @ImagePath, @SortOrder)", conn, tx);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@ImagePath", imagePath);
            cmd.Parameters.AddWithValue("@SortOrder", sortOrder);
            cmd.ExecuteNonQuery();
        }

        public List<ProductImage> GetByProduct(int productId)
        {
            var list = new List<ProductImage>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM ProductImages WHERE ProductId = @ProductId ORDER BY SortOrder", conn);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new ProductImage
                        {
                            ImageId = (int)r["ImageId"],
                            ProductId = (int)r["ProductId"],
                            ImagePath = r["ImagePath"].ToString(),
                            SortOrder = (int)r["SortOrder"]
                        });
                    }
                }
            }
            return list;
        }

        public List<ProductImage> GetAllProductImages()
        {
            var list = new List<ProductImage>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM ProductImages ORDER BY ProductId, SortOrder", conn);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new ProductImage
                        {
                            ImageId = (int)r["ImageId"],
                            ProductId = (int)r["ProductId"],
                            ImagePath = r["ImagePath"].ToString(),
                            SortOrder = (int)r["SortOrder"]
                        });
                    }
                }
            }
            return list;
        }
    }
}