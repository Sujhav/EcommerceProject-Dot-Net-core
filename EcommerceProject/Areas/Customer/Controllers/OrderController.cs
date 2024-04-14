using Microsoft.AspNetCore.Mvc;
using EcommerceProject.Models;
using EcommerceProject.Repository;
using EcommerceProject.Utility;
using EcommerceProject.Database;
using Stripe.Checkout;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Stripe.Climate;


namespace EcommerceProject.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly OrderRepository _orderRepository;
        private readonly EcommerceContext _ecommerceContext;
        
       

        public OrderController(OrderRepository orderRepository,EcommerceContext ecommerceContext
            )
        {
            _orderRepository = orderRepository;
            _ecommerceContext = ecommerceContext;
           
            
        }
       
        

        [HttpGet]   
        public IActionResult CheckOut(bool isSuccess=false)
        {
            ViewBag.IsSuccess = isSuccess;
            return View();
        }

        [HttpPost]
        public async Task< IActionResult> CheckOut(OrderModel model)
        {
            List<CartItemModel> CartItemsData = HttpContext.Session.Get<List<CartItemModel>>("CartItemsList");
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           
            model.OrderNo = _orderRepository.GenerateOrderNumber();
             var order = await _orderRepository.AddOrder(model,userId);
            int orderId=order.Id;

            if (CartItemsData != null)
            {
                foreach (var product in CartItemsData)
                {
                    var data = new OrderDetail
                    {
                        ProductId = product.ProductId,
                        OrderId = orderId,
                        count=product.Quantity,

                    };
                    await _ecommerceContext.AddAsync(data); 

                }
                await _ecommerceContext.SaveChangesAsync();
            }

                //stripeSetings
                var domain = "http://localhost:26966/";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },
                    LineItems = new List<SessionLineItemOptions>()
                   ,
                    Mode = "payment",
                    SuccessUrl = $"{domain}Customer/Order/OrderConfirmation?Id={orderId}",
                    CancelUrl = $"{domain}Customer/Home/AddtoCart",
                };
                foreach (var item in CartItemsData)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName,
                            },
                        },
                        Quantity = item.Quantity,
                    };
                    options.LineItems.Add(sessionLineItem);
                 }

                var service = new SessionService();
                Session session = service.Create(options);
                


            _orderRepository.UpdateStripePaymentId(orderId, session.Id);
            await _ecommerceContext.SaveChangesAsync();
            Response.Headers.Add("Location", session.Url);
               
                return new StatusCodeResult(303);

        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = _ecommerceContext.Orders.FirstOrDefault(o => o.Id == id);
            var service = new SessionService();
            Session session = service.Get(order.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                order.PaymentIntentID = session.PaymentIntentId;
                _orderRepository.UpdateStatus(id, Utility.OrderStatus.StatusApproved, Utility.OrderStatus.PaymentStatusApproved);
                await _ecommerceContext.SaveChangesAsync(); 
            }

            HttpContext.Session.Set("CartCount", 0);
            HttpContext.Session.Set("CartItemsList", new List<CartItemModel>());

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartItemsToDelete = await _ecommerceContext.CartItems.Where(c => c.UserId == userId).ToListAsync(); 

            _ecommerceContext.CartItems.RemoveRange(cartItemsToDelete);
            await _ecommerceContext.SaveChangesAsync(); 
            return View(id);
        }

    }
}
