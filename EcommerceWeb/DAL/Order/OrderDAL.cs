using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using OrderModel = EcommerceWeb.Models.Order.Order;
using OrderDetailModel = EcommerceWeb.Models.Order.OrderDetail;

namespace EcommerceWeb.DAL.Order
{
    public class OrderDAL
    {
        private readonly DbHelper _db;
        public OrderDAL(DbHelper db) { _db = db; }

        public async Task<int> InsertOrderAsync(OrderModel order, SqlConnection conn, SqlTransaction tx)
        {
            var cmd = new SqlCommand(
                "INSERT INTO Orders (BuyerId, TotalAmount, Status) " +
                "OUTPUT INSERTED.OrderId VALUES (@BuyerId, @Total, 'Pending')", conn, tx);
            cmd.Parameters.AddWithValue("@BuyerId", order.BuyerId);
            cmd.Parameters.AddWithValue("@Total", order.TotalAmount);
            return (int)await cmd.ExecuteScalarAsync();
        }

        public async Task InsertOrderDetailAsync(OrderDetailModel d, SqlConnection conn, SqlTransaction tx)
        {
            var cmd = new SqlCommand(
                "INSERT INTO OrderDetails (OrderId, ProductId, Quantity, Price) " +
                "VALUES (@OrderId, @ProductId, @Qty, @Price)", conn, tx);
            cmd.Parameters.AddWithValue("@OrderId", d.OrderId);
            cmd.Parameters.AddWithValue("@ProductId", d.ProductId);
            cmd.Parameters.AddWithValue("@Qty", d.Quantity);
            cmd.Parameters.AddWithValue("@Price", d.Price);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<OrderModel>> GetByBuyerAsync(int buyerId)
        {
            var list = new List<OrderModel>();
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "SELECT * FROM Orders WHERE BuyerId = @BuyerId ORDER BY OrderDate DESC", conn);
                cmd.Parameters.AddWithValue("@BuyerId", buyerId);
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                        list.Add(new OrderModel
                        {
                            OrderId = (int)r["OrderId"],
                            BuyerId = (int)r["BuyerId"],
                            OrderDate = (DateTime)r["OrderDate"],
                            TotalAmount = (decimal)r["TotalAmount"],
                            Status = r["Status"].ToString()
                        });
                }
            }
            return list;
        }
    }
}