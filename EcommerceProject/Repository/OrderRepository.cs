using EcommerceProject.Database;
using EcommerceProject.Models;
using EcommerceProject.Utility;
using Microsoft.EntityFrameworkCore;
namespace EcommerceProject.Repository
{
    public class OrderRepository
    {
        private readonly EcommerceContext _context;
        public OrderRepository(EcommerceContext ecommerceContext)
        {
            _context = ecommerceContext;
        }

        public string GenerateOrderNumber()
        {
           
            string prefix = "ORD"; 
            string timestamp = DateTime.Now.ToString("yyMMddHHmmss"); 
            string uniqueId = Guid.NewGuid().ToString().Substring(0, 6); 
            return $"{prefix}-{timestamp}-{uniqueId}";
        }


        public async Task<Order> AddOrder(OrderModel model, string id)
        {

            var data = new Order
            {
                ApplicationUserId = id,
                Name = model.Name,
                PhoneNo = model.PhoneNo,
                Email = model.Email,
                Address = model.Address,
                OrderDate = model.OrderDate,
                OrderNo = model.OrderNo,
                PaymentStatus = Utility.OrderStatus.PaymentStatusPending,
                OrderStatus = Utility.OrderStatus.StatusPending,
            };
            await _context.AddAsync(data);
            await _context.SaveChangesAsync();
            return data;
        }
        public async Task<List<OrderModel>> GetAllOrder()
        {
            var data = await _context.Orders.Select(x => new OrderModel
            {
                Id=x.Id,
                Name = x.Name,
                PhoneNo = x.PhoneNo,
                Email = x.Email,
                Address = x.Address,
                OrderDate = x.OrderDate,
                OrderNo = x.OrderNo,


            }).ToListAsync();
            return data;
        }

        public async Task<List<Order>> GetOrderOfSingleUser(string Id)
        {

            var data = await _context.Orders.Where(c => c.ApplicationUserId == Id).ToListAsync();
                
            return data;
        }
        public async Task<List<Order>> GetOrderDetails(string Id)
        {

            var data = await _context.Orders.Where(c => c.ApplicationUserId == Id).ToListAsync();

            return data;
        }



        public void UpdateStatus(int id, string orderStatus, string? PaymentStatus = null)
        {
            var orderfromdb = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (orderfromdb != null)
            {
                orderfromdb.OrderStatus = orderStatus;
                if (PaymentStatus != null)
                {
                    orderfromdb.PaymentStatus = PaymentStatus;
                }
            }

        }


            public void UpdateStripePaymentId(int id, string sessionId)
            {
                var orderfromdb = _context.Orders.FirstOrDefault(o => o.Id == id);
            if(orderfromdb != null)
            {
                orderfromdb.SessionId = sessionId;

                }
           


            }
        
    }
    
}
