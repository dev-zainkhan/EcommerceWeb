using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using EcommerceWeb.DAL;
using EcommerceWeb.DAL.Order;
using EcommerceWeb.DAL.Product;
using System.Threading.Tasks;
using EcommerceWeb.DAL.Shipping;
using EcommerceWeb.Models.Shipping;
using OrderModel = EcommerceWeb.Models.Order.Order;
using OrderDetailModel = EcommerceWeb.Models.Order.OrderDetail;

namespace EcommerceWeb.BLL.Order
{
    public class OrderBLL
    {
        private readonly DbHelper _db;
        private readonly OrderDAL _orderDAL;
        private readonly ProductDAL _productDAL;
        private readonly ShippingDAL _shippingDAL;

        public OrderBLL(DbHelper db, OrderDAL orderDAL, ProductDAL productDAL)
        {
            _db = db;
            _orderDAL = orderDAL;
            _productDAL = productDAL;
            _shippingDAL = new ShippingDAL(db);
        }

        public async Task<Tuple<bool, string>> PlaceOrderAsync(int buyerId, List<Tuple<int, int>> items, ShippingAddress address)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        decimal total = 0;
                        var details = new List<OrderDetailModel>();

                        foreach (var item in items)
                        {
                            int productId = item.Item1;
                            int qty = item.Item2;

                            var p = await _productDAL.GetByIdAsync(productId);
                            if (p == null) return Tuple.Create(false, "Product not found.");
                            if (p.Stock < qty) return Tuple.Create(false, "Not enough stock for: " + p.Name);

                            total += p.Price * qty;
                            details.Add(new OrderDetailModel
                            {
                                ProductId = productId,
                                Quantity = qty,
                                Price = p.Price
                            });
                            _productDAL.UpdateStock(productId, p.Stock - qty, conn, tx);
                        }

                        int orderId = await _orderDAL.InsertOrderAsync(
                            new OrderModel { BuyerId = buyerId, TotalAmount = total }, conn, tx);

                        foreach (var d in details)
                        {
                            d.OrderId = orderId;
                            await _orderDAL.InsertOrderDetailAsync(d, conn, tx);
                        }

                        address.OrderId = orderId;
                        _shippingDAL.Insert(address, conn, tx);

                        tx.Commit();
                        return Tuple.Create(true, "Order placed successfully! Total: Rs. " + total.ToString("N0"));
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        return Tuple.Create(false, "Order failed: " + ex.Message);
                    }
                }
            }
        }
    }
}