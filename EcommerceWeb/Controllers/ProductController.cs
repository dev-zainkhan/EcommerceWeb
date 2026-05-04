using EcommerceWeb.BLL.Product;
using EcommerceWeb.DAL;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ProductModel = EcommerceWeb.Models.Product.Product;

namespace EcommerceWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductBLL _productBLL;
        private readonly DbHelper _db;
        private readonly S3Helper _s3Helper;

        public ProductController()
        {
            _db = new DbHelper();
            _productBLL = new ProductBLL(_db);
            _s3Helper = new S3Helper();
        }

        // GET: /Product/Index — Buyer sees all products
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            int userId = HttpContext.Session.GetInt32("UserId").Value;
            var products = await _productBLL.GetAllProductsAsync(); var images = _productBLL.GetAllProductImages();
            var cartBLL = new EcommerceWeb.BLL.Cart.CartBLL(_db);
            var cartItems = await cartBLL.GetCartAsync(userId);
            ViewBag.ProductImages = images;
            ViewBag.CartItems = cartItems;
            return View(products);
        }

        // GET: /Product/Dashboard — Seller sees their products
        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            int sellerId = HttpContext.Session.GetInt32("UserId").Value;
            var products = await _productBLL.GetSellerProductsAsync(sellerId);
            var images = _productBLL.GetAllProductImages();
            ViewBag.ProductImages = images;
            return View(products);
        }

        // GET: /Product/Add
        public IActionResult Add()
        {
            if (HttpContext.Session.GetString("UserRole") != "Seller")
                return RedirectToAction("Index");
            return View();
        }

        // POST: /Product/Add
        [HttpPost]
        public async Task<IActionResult> Add(ProductModel product, List<IFormFile> imageFiles)
        {
            if (HttpContext.Session.GetString("UserRole") != "Seller")
                return RedirectToAction("Index");

            if (imageFiles == null || imageFiles.Count < 3)
            {
                ViewBag.Error = "Please upload at least 3 images.";
                return View();
            }

            try
            {
                int sellerId = HttpContext.Session.GetInt32("UserId").Value;
                product.SellerId = sellerId;

                bool success = await _productBLL.AddProductAsync(product, imageFiles, _s3Helper);
                if (success)
                    return RedirectToAction("Dashboard");
                else
                {
                    ViewBag.Error = "Failed to add product. Please try again.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
    }
}