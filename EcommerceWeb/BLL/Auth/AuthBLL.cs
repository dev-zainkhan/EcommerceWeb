using BC = BCrypt.Net.BCrypt;
using EcommerceWeb.DAL.User;
using System.Threading.Tasks;
using UserModel = EcommerceWeb.Models.User.User;

namespace EcommerceWeb.BLL.Auth
{
    public class AuthBLL
    {
        private readonly UserDAL _userDAL;
        public AuthBLL(UserDAL userDAL) { _userDAL = userDAL; }

        public async Task<bool> RegisterAsync(string name, string email, string password, string role)
        {
            if (await _userDAL.GetByEmailAsync(email) != null) return false;

            await _userDAL.InsertAsync(new UserModel
            {
                Name = name,
                Email = email,
                PasswordHash = BC.HashPassword(password),
                Role = role
            });
            return true;
        }

        public async Task<UserModel> LoginAsync(string email, string password)
        {
            var user = await _userDAL.GetByEmailAsync(email);
            if (user == null || !BC.Verify(password, user.PasswordHash)) return null;
            return user;
        }
    }
}