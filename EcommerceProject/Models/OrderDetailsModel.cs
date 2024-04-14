using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Models
{
    public class OrderDetailsModel
    {
        public int Id { get; set; }

        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [Display(Name = "Order")]
        public int OrderId { get; set; }
        public int count { get; set; }
    }
}
