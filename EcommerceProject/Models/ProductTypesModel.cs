using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Models
{
    public class ProductTypesModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name ="ProductType")]
        public string ProductType { get; set; }
    }
}
