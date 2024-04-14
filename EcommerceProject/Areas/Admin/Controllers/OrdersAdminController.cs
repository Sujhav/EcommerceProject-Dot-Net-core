using EcommerceProject.Database;
using EcommerceProject.Models;
using EcommerceProject.Repository;
using EcommerceProject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;

namespace EcommerceProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrdersAdminController : Controller
    {
        private readonly OrderRepository _orderRepository;
        private readonly EcommerceContext _context;

        [BindProperty]
        public OrderDetailsVM OrderDetailsVM { get; set; }
        public OrdersAdminController(OrderRepository orderRepository,EcommerceContext ecommerceContext)
        {
            _orderRepository = orderRepository;
            _context = ecommerceContext;

        }
        public ActionResult Index()
        {
            return View();
        }

    
        [HttpGet]
        public async Task<IActionResult> GetOrderList(string status)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var check=await _orderRepository.GetOrderOfSingleUser(userId);

            if(!User.IsInRole("Admin")&& check.Count==0)
            {
                ViewBag.Empty = true;
            }

            IEnumerable<Order> OrderList;
            if (status != null && status != "All")
            {
                if (User.IsInRole("Admin"))
                {
                    OrderList = await _context.Orders.Where(x => x.OrderStatus == status).ToListAsync();
                }
                else
                {
                    OrderList = await _context.Orders.Where(x => x.OrderStatus == status && x.ApplicationUserId == userId).ToListAsync();
                }
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    OrderList = await _context.Orders.ToListAsync();
                }
                else
                {
                    OrderList = await _context.Orders.Where(x =>  x.ApplicationUserId == userId).ToListAsync();
                }
            }
           
            return View(OrderList);
        }
        public IActionResult OrderDetails(int id)
        {
            OrderDetailsVM = new OrderDetailsVM();
            OrderDetailsVM.Order= _context.Orders.FirstOrDefault(x => x.Id==id);
            OrderDetailsVM.OrderDetail= _context.OrderDetails.Include(x=>x.products).Where(x=>x.OrderId==id).ToList();
            return View(OrderDetailsVM);
        }

        
        public IActionResult Inprocess(int id)
        {
            var order = _context.Orders.FirstOrDefault(x=>x.Id== id);
            if(order!=null)
            {
                order.OrderStatus = Utility.OrderStatus.StatusInProcess;
                _context.SaveChanges();
            }
            return RedirectToAction("OrderDetails",new { id =id});
        }
        public IActionResult Shipped(OrderDetailsVM model)
        {
             

            var order = _context.Orders.FirstOrDefault(x => x.Id == model.Order.Id);
            if (order != null)
            {
                order.OrderStatus = Utility.OrderStatus.StatusShipped;
                order.TrackingNumber = model.Order.TrackingNumber;
                order.ShippedDate=DateTime.Now;
              

                _context.SaveChanges();
            }
            return RedirectToAction("OrderDetails", new { id = model.Order.Id });
        }
        public IActionResult Completed(OrderDetailsVM model)
        {
            var order = _context.Orders.FirstOrDefault(x => x.Id == model.Order.Id);
            if (order != null)
            {
                order.OrderStatus = Utility.OrderStatus.StatusCompleted;
                _context.SaveChanges();
            }
            return RedirectToAction("OrderDetails", new { id = model.Order.Id });
        }

        
        public async Task< IActionResult> CancelOrder(int id)
        {
            var order =  _context.Orders.FirstOrDefault(x => x.Id == id);
            if (order != null)
            {
                if (order.PaymentStatus == Utility.OrderStatus.PaymentStatusApproved)
                {
                    var options = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = order.PaymentIntentID,
                    };
                    var service = new RefundService();
                    Refund refund = service.Create(options);
                    order.OrderStatus = Utility.OrderStatus.StatusCancelled;
                    order.PaymentStatus = Utility.OrderStatus.PaymentStatusRefunded;
              

                }
                
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Refund Successsfully" });
            }
            return Json(new { success = false, message="Refund Failed" });
        }
    }

}

