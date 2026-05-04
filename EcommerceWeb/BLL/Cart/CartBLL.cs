using System.Collections.Generic;
using EcommerceWeb.DAL;
using EcommerceWeb.DAL.Cart;
using CartModel = EcommerceWeb.Models.Cart.Cart;
using System.Threading.Tasks;

namespace EcommerceWeb.BLL.Cart
{
    public class CartBLL
    {
        private readonly CartDAL _cartDAL;

        public CartBLL(DbHelper db)
        {
            _cartDAL = new CartDAL(db);
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            await _cartDAL.AddToCartAsync(userId, productId, quantity);
        }

        public async Task<List<CartModel>> GetCartAsync(int userId)
        {
            return await _cartDAL.GetCartByUserAsync(userId);
        }

        public async Task RemoveFromCartAsync(int cartId)
        {
            await _cartDAL.RemoveFromCartAsync(cartId);
        }

        public async Task ClearCartAsync(int userId)
        {
            await _cartDAL.ClearCartAsync(userId);
        }

        public async Task UpdateQuantityAsync(int cartId, int quantity)
        {
            await _cartDAL.UpdateQuantityAsync(cartId, quantity);
        }
    }
}