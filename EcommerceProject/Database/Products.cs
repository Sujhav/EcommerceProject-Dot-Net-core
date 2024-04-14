using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceProject.Database
{
    public class Products
    {
        public int Id { get; set; }
        
        public string ProductName { get; set; }
       
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
       
        public string ProductColor { get; set; }
       
        public bool IsAvailable { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public ProductTypes ProductType { get; set; }

        
        public int SpecialTagId {  get; set; }

        [ForeignKey("SpecialTagId")]
        public SpecialTag SpecialTag { get; set; }
    }
}
