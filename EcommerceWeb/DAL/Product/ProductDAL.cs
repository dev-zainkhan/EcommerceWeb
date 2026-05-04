using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ProductModel = EcommerceWeb.Models.Product.Product;

namespace EcommerceWeb.DAL.Product
{
    public class ProductDAL
    {
        private readonly DbHelper _db;
        public ProductDAL(DbHelper db) { _db = db; }

        public async Task<List<ProductModel>> GetAllAsync()
        {
            var list = new List<ProductModel>();
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM Products WHERE Stock > 0", conn);
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync()) list.Add(MapProduct(r));
                }
            }
            return list;
        }

        public async Task<List<ProductModel>> GetBySellerAsync(int sellerId)
        {
            var list = new List<ProductModel>();
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("EXEC GetProductsBySeller @SellerId", conn);
                cmd.Parameters.AddWithValue("@SellerId", sellerId);
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync()) list.Add(MapProduct(r));
                }
            }
            return list;
        }

        public async Task<ProductModel> GetByIdAsync(int id)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM GetProductById(@Id)", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    return await r.ReadAsync() ? MapProduct(r) : null;
                }
            }
        }

        public void Insert(ProductModel p)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("exec InsertProduct", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SellerId", p.SellerId);
                cmd.Parameters.AddWithValue("@Name", p.Name);
                cmd.Parameters.AddWithValue("@ProductDescription", p.Description ?? "");
                cmd.Parameters.AddWithValue("@Price", p.Price);
                cmd.Parameters.AddWithValue("@Stock", p.Stock);
                cmd.Parameters.AddWithValue("@Img", p.ImagePath ?? "");
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateStock(int productId, int newStock, SqlConnection conn, SqlTransaction tx)
        {
            var cmd = new SqlCommand(
                "UPDATE Products SET Stock = @Stock WHERE ProductId = @Id", conn, tx);
            cmd.Parameters.AddWithValue("@Stock", newStock);
            cmd.Parameters.AddWithValue("@Id", productId);
            cmd.ExecuteNonQuery();
        }

        private ProductModel MapProduct(SqlDataReader r) => new ProductModel
        {
            ProductId = (int)r["ProductId"],
            SellerId = (int)r["SellerId"],
            Name = r["Name"].ToString(),
            Description = r["Description"].ToString(),
            Price = (decimal)r["Price"],
            Stock = (int)r["Stock"],
            ImagePath = r["ImagePath"].ToString()
        };
    }
}