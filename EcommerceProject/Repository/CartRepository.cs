using EcommerceProject.Database;
using EcommerceProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace EcommerceProject.Repository
{
    public class CartRepository
    {
        private readonly EcommerceContext _context;
        public CartRepository(EcommerceContext ecommerceContext)
        {
            _context = ecommerceContext;
        }


        public async Task<int> GetAllCartsCount(string userId)
        {
            var count = await _context.CartItems.Include(c=>c.Products)
                .Where(x=>x.UserId == userId)
                .CountAsync();


            return count;
        }
        public async Task<List<CartItemModel>> GetAllCartsItems(string userId)
        {
            
            var data = await _context.CartItems.Include(c => c.Products)
                .Where(x => x.UserId == userId)
                .Select(c=>new CartItemModel
                {
                    ProductColor=c.Products.ProductColor,
                    ProductType=c.Products.ProductType.ProductType,
                    ProductId = c.Products.Id,
                    ProductName = c.Products.ProductName,
                    ImageUrl = c.Products.ImageUrl,
                    Price = c.Products.Price,
                    Quantity=c.Quantity
                    
                } ).ToListAsync();
            return data;
            
        }



    }
}
