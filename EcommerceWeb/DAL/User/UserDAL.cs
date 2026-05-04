using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using UserModel = EcommerceWeb.Models.User.User;

namespace EcommerceWeb.DAL.User
{
    public class UserDAL
    {
        private readonly DbHelper _db;
        public UserDAL(DbHelper db) { _db = db; }

        public async Task<UserModel> GetByEmailAsync(string email)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM Users WHERE Email = @Email", conn);
                cmd.Parameters.AddWithValue("@Email", email);
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    return await r.ReadAsync() ? MapUser(r) : null;
                }
            }
        }

        public async Task InsertAsync(UserModel user)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "INSERT INTO Users (Name, Email, PasswordHash, Role) VALUES (@Name, @Email, @Hash, @Role)", conn);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Hash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                await cmd.ExecuteNonQueryAsync();
            }
        }
        private UserModel MapUser(SqlDataReader r) => new UserModel
        {
            UserId = (int)r["UserId"],
            Name = r["Name"].ToString(),
            Email = r["Email"].ToString(),
            PasswordHash = r["PasswordHash"].ToString(),
            Role = r["Role"].ToString(),
            CreatedAt = (DateTime)r["CreatedAt"]
        };
    }
}