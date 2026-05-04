using EcommerceWeb.BLL.Cart;
using EcommerceWeb.BLL.Order;
using EcommerceWeb.DAL;
using EcommerceWeb.DAL.Order;
using EcommerceWeb.DAL.Product;
using System.Threading.Tasks;
using EcommerceWeb.Models.Shipping;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EcommerceWeb.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderBLL _orderBLL;
        private readonly CartBLL _cartBLL;
        private readonly DbHelper _db;

        public OrderController()
        {
            _db = new DbHelper();
            _orderBLL = new OrderBLL(_db, new OrderDAL(_db), new ProductDAL(_db));
            _cartBLL = new CartBLL(_db);
        }

        // GET: /Order/Checkout
        public async Task<IActionResult> Checkout()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            int userId = HttpContext.Session.GetInt32("UserId").Value;
            var cart = await _cartBLL.GetCartAsync(userId);

            if (cart.Count == 0)
                return RedirectToAction("Index", "Cart");

            return View(cart);
        }

        // POST: /Order/PlaceOrder
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(ShippingAddress address)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            int buyerId = HttpContext.Session.GetInt32("UserId").Value;
            var cart = await _cartBLL.GetCartAsync(buyerId);
            if (cart.Count == 0)
                return RedirectToAction("Index", "Cart");

            var items = new List<Tuple<int, int>>();
            foreach (var item in cart)
                items.Add(Tuple.Create(item.ProductId, item.Quantity));

            var result = await _orderBLL.PlaceOrderAsync(buyerId, items, address);
            if (result.Item1)
                await _cartBLL.ClearCartAsync(buyerId);

            TempData["Message"] = result.Item2;
            TempData["Success"] = result.Item1.ToString();

            return RedirectToAction("Index", "Product");
        }
    }
}