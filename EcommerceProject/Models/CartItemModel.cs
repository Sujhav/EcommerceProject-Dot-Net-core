using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EcommerceProject.Models
{
    public class CartItemModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [ValidateNever] 
        public string ImageUrl {  get; set; }

        [ValidateNever]
        public string ProductName {  get; set; }

        [ValidateNever]
        public Decimal Price {  get; set; }

        [ValidateNever]
        public string ProductColor {  get; set; }

        [ValidateNever]
        public string ProductType { get; set; }

    }
}
