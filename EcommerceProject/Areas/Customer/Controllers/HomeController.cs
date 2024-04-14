using Microsoft.AspNetCore.Mvc;
using EcommerceProject.Models;
using EcommerceProject.Repository;
using EcommerceProject.Utility;
using EcommerceProject.Database;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using EcommerceProject.Services;

namespace EcommerceProject.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ProductAddRepository _productAddRepository;
        private readonly EcommerceContext _context;
        private readonly CartRepository _cartRepository;
        private readonly EmailService _emailService;
        public HomeController(ProductAddRepository productAddRepository,EcommerceContext ecommerceContext
            ,CartRepository cartRepository,EmailService emailService)
        {
            _productAddRepository = productAddRepository;
            _context = ecommerceContext;
            _cartRepository = cartRepository;
            _emailService = emailService;

        }

        [HttpGet]
        public async Task<IActionResult> ShowAllRecords()
        {

            var data = await _productAddRepository.ShowAllProducts();

            string userId=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId != null ) {

                var count = await _cartRepository.GetAllCartsCount(userId);
                HttpContext.Session.SetInt32("CartCount", count);
                var cartData = await _cartRepository.GetAllCartsItems(userId);
                HttpContext.Session.Set("CartItemsList", cartData);

            }
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ShowDetails(int id)
        {
            var data= await _productAddRepository.showSingleProducts(id);
            return View(data);
        }
       
        [HttpPost]
        [ActionName("ShowDetails")]
        [Authorize]
        public async Task<IActionResult> ProductDetails(int id, int quantity)
        {
            List<CartItem> carts = new List<CartItem>();
            var product = await _productAddRepository.showSingleProducts(id);
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cartData = await _cartRepository.GetAllCartsItems(userId);
            HttpContext.Session.Set("CartItemsList", cartData);


            var cartFromDb= _context.CartItems.FirstOrDefault(c => c.UserId==userId&& c.ProductId==product.Id);

            if(cartFromDb!=null)
            {
                cartFromDb.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = product.Id,
                    Quantity = quantity,
                };
                    _context.CartItems.Add(cartItem); 
            };
            
            await _context.SaveChangesAsync();
            var count = await _cartRepository.GetAllCartsCount(userId);
            HttpContext.Session.SetInt32("CartCount", count);

            return RedirectToAction("ShowAllRecords"); 
        }

        [HttpPost]
        [Authorize]
        public async Task< IActionResult> RemoveFromCart(int id,int quantity)
        {
            string userId=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartItem=_context.CartItems.FirstOrDefault(c=>c.UserId==userId && c.ProductId==id );
            if(cartItem != null)
            {
                cartItem.Quantity-=quantity;
                if (cartItem.Quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
                _context.SaveChanges();
            }

          
            if (userId != null)
            {
                var cartData = await _cartRepository.GetAllCartsItems(userId);
                HttpContext.Session.Set("CartItemsList", cartData);

                var count = await _cartRepository.GetAllCartsCount(userId);
                HttpContext.Session.SetInt32("CartCount", count);




            }
            return RedirectToAction("AddtoCart");

        }

 
        [HttpGet]
        [Authorize]
        public IActionResult AddtoCart(bool IsSuccess=true)
        {
            string userId= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            List<CartItemModel> CartItemsData = HttpContext.Session.Get<List<CartItemModel>>("CartItemsList");

                    if ((CartItemsData?.Any() == true))
                    {
                        ViewBag.IsSuccess = IsSuccess;
                        return View(CartItemsData);
                    }
                    else
                    {
                        ViewBag.IsSuccess = false;
                        return View();
                    }
                
        }
    }
}
