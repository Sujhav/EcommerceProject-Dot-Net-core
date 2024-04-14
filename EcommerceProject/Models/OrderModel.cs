using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Models
{
    public class OrderModel
    {
        public int Id { get; set; }

        [Display(Name="Order No,")]
        public string OrderNo { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Phone No.")]
        [Required]
        public string PhoneNo { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }
        public string? TrackingNumber { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }

    }
}
