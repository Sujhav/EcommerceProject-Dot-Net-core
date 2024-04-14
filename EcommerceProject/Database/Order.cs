 using EcommerceProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceProject.Database
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }

        public string? TrackingNumber {  get; set; }
        public string? SessionId { get; set; }
        public string? PaymentIntentID { get; set; }

        public string? OrderStatus {  get; set; }
        public string? PaymentStatus {  get; set; }  
        public DateTime? ShippedDate { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }

        public string? ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUsers applicationUser { get; set; }

    }
}
