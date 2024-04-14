using EcommerceProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceProject.Database
{
    public class CartItem
    {
        public int Id { get; set; }


        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUsers applicationUsers { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Products Products { get; set; }
        public int Quantity { get; set; }

        
    }
}
