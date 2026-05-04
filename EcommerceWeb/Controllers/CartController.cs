using EcommerceWeb.BLL.Cart;
using EcommerceWeb.DAL;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWeb.Controllers
{
    public class CartController : Controller
    {
        private readonly CartBLL _cartBLL;

        public CartController()
        {
            _cartBLL = new CartBLL(new DbHelper());
        }

        // GET: /Cart/Index
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            int userId = HttpContext.Session.GetInt32("UserId").Value;
            var cart = await _cartBLL.GetCartAsync(userId);
            return View(cart);
        }

        // POST: /Cart/Add
        // POST: /Cart/Add
        [HttpPost]
        public async Task<IActionResult> Add(int productId, int quantity)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            int userId = HttpContext.Session.GetInt32("UserId").Value;

            // Check stock
            var db = new DbHelper();
            var productDAL = new DAL.Product.ProductDAL(db);
            var product = await productDAL.GetByIdAsync(productId);
            if (product == null)
            {
                TempData["CartError"] = "Product not found.";
                return RedirectToAction("Index", "Product");
            }

            if (product.Stock < quantity)
            {
                TempData["CartError"] = $"Only {product.Stock} items available for {product.Name}.";
                return RedirectToAction("Index", "Product");
            }

            // Check existing cart quantity
            var cart = await _cartBLL.GetCartAsync(userId);
            var existing = cart.Find(c => c.ProductId == productId);
            if (existing != null && (existing.Quantity + quantity) > product.Stock)
            {
                TempData["CartError"] = $"Cannot add more. Only {product.Stock} items available.";
                return RedirectToAction("Index", "Product");
            }

            await _cartBLL.AddToCartAsync(userId, productId, quantity);
            TempData["CartSuccess"] = $"{product.Name} added to cart!";
            return RedirectToAction("Index", "Product");
        }

        // POST: /Cart/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int cartId, string returnUrl = "cart")
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            await _cartBLL.RemoveFromCartAsync(cartId);
            if (returnUrl == "shop")
                return RedirectToAction("Index", "Product");

            return RedirectToAction("Index");
        }

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartId, int quantity)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            if (quantity < 1) quantity = 1;
            await _cartBLL.UpdateQuantityAsync(cartId, quantity);
            return RedirectToAction("Index");
        }
    }
}