using EcommerceWeb.BLL.Auth;
using EcommerceWeb.DAL;
using EcommerceWeb.DAL.User;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserModel = EcommerceWeb.Models.User.User;

namespace EcommerceWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthBLL _authBLL;

        public AuthController()
        {
            var db = new DbHelper();
            _authBLL = new AuthBLL(new UserDAL(db));
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _authBLL.LoginAsync(email, password);
            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserRole", user.Role);

            if (user.Role == "Seller")
                return RedirectToAction("Dashboard", "Product");
            else
                return RedirectToAction("Index", "Product");
        }

        // GET: /Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password, string role)
        {
            bool ok = await _authBLL.RegisterAsync(name, email, password, role);
            if (!ok)
            {
                ViewBag.Error = "Email already registered.";
                return View();
            }
            return RedirectToAction("Login");
        }

        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}