using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using CartModel = EcommerceWeb.Models.Cart.Cart;

namespace EcommerceWeb.DAL.Cart
{
    public class CartDAL
    {
        private readonly DbHelper _db;
        public CartDAL(DbHelper db) { _db = db; }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var checkCmd = new SqlCommand(
                    "SELECT CartId FROM Cart WHERE UserId = @UserId AND ProductId = @ProductId", conn);
                checkCmd.Parameters.AddWithValue("@UserId", userId);
                checkCmd.Parameters.AddWithValue("@ProductId", productId);
                var existing = await checkCmd.ExecuteScalarAsync();

                if (existing != null)
                {
                    var updateCmd = new SqlCommand(
                        "UPDATE Cart SET Quantity = Quantity + @Quantity WHERE UserId = @UserId AND ProductId = @ProductId", conn);
                    updateCmd.Parameters.AddWithValue("@Quantity", quantity);
                    updateCmd.Parameters.AddWithValue("@UserId", userId);
                    updateCmd.Parameters.AddWithValue("@ProductId", productId);
                    await updateCmd.ExecuteNonQueryAsync();
                }
                else
                {
                    var insertCmd = new SqlCommand(
                        "INSERT INTO Cart (UserId, ProductId, Quantity) VALUES (@UserId, @ProductId, @Quantity)", conn);
                    insertCmd.Parameters.AddWithValue("@UserId", userId);
                    insertCmd.Parameters.AddWithValue("@ProductId", productId);
                    insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                    await insertCmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<CartModel>> GetCartByUserAsync(int userId)
        {
            var list = new List<CartModel>();
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "SELECT c.CartId, c.UserId, c.ProductId, c.Quantity, " +
                    "p.Name AS ProductName, p.Price, p.ImagePath " +
                    "FROM Cart c " +
                    "INNER JOIN Products p ON c.ProductId = p.ProductId " +
                    "WHERE c.UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        list.Add(new CartModel
                        {
                            CartId = (int)r["CartId"],
                            UserId = (int)r["UserId"],
                            ProductId = (int)r["ProductId"],
                            Quantity = (int)r["Quantity"],
                            ProductName = r["ProductName"].ToString(),
                            Price = (decimal)r["Price"],
                            ImagePath = r["ImagePath"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public async Task RemoveFromCartAsync(int cartId)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM Cart WHERE CartId = @CartId", conn);
                cmd.Parameters.AddWithValue("@CartId", cartId);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM Cart WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateQuantityAsync(int cartId, int quantity)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "UPDATE Cart SET Quantity = @Quantity WHERE CartId = @CartId", conn);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@CartId", cartId);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}