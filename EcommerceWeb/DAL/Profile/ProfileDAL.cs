using System;
using Microsoft.Data.SqlClient;
using UserModel = EcommerceWeb.Models.User.User;
using System.Threading.Tasks;

namespace EcommerceWeb.DAL.Profile
{
    public class ProfileDAL
    {
        private readonly DbHelper _db;
        public ProfileDAL(DbHelper db) { _db = db; }

        public async Task<UserModel> GetByIdAsync(int userId)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM Users WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync())
                    {
                        return new UserModel
                        {
                            UserId = (int)r["UserId"],
                            Name = r["Name"].ToString(),
                            Email = r["Email"].ToString(),
                            PasswordHash = r["PasswordHash"].ToString(),
                            Role = r["Role"].ToString(),
                            ProfileImage = r["ProfileImage"].ToString()
                        };
                    }
                    return null;
                }
            }
        }

        public async Task UpdateProfileAsync(int userId, string name, string email)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "UPDATE Users SET Name = @Name, Email = @Email WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@UserId", userId);
                await cmd.ExecuteNonQueryAsync();
            }
        }
        public async Task UpdatePasswordAsync(int userId, string passwordHash)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "UPDATE Users SET PasswordHash = @Hash WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@Hash", passwordHash);
                cmd.Parameters.AddWithValue("@UserId", userId);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateProfileImageAsync(int userId, string imagePath)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "UPDATE Users SET ProfileImage = @Image WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@Image", imagePath);
                cmd.Parameters.AddWithValue("@UserId", userId);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}