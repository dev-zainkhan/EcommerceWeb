using Microsoft.Data.SqlClient;
using EcommerceWeb.Models.Shipping;

namespace EcommerceWeb.DAL.Shipping
{
    public class ShippingDAL
    {
        private readonly DbHelper _db;
        public ShippingDAL(DbHelper db) { _db = db; }

        public void Insert(ShippingAddress address, SqlConnection conn, SqlTransaction tx)
        {
            var cmd = new SqlCommand(
                "INSERT INTO ShippingAddress (OrderId, FullName, PhoneNumber, Country, Province, City, AddressLine) " +
                "VALUES (@OrderId, @FullName, @Phone, @Country, @Province, @City, @AddressLine)", conn, tx);
            cmd.Parameters.AddWithValue("@OrderId", address.OrderId);
            cmd.Parameters.AddWithValue("@FullName", address.FullName);
            cmd.Parameters.AddWithValue("@Phone", address.PhoneNumber);
            cmd.Parameters.AddWithValue("@Country", address.Country);
            cmd.Parameters.AddWithValue("@Province", address.Province);
            cmd.Parameters.AddWithValue("@City", address.City);
            cmd.Parameters.AddWithValue("@AddressLine", address.AddressLine);
            cmd.ExecuteNonQuery();
        }
    }
}