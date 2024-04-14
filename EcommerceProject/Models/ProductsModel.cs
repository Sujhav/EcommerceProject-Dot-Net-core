using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Models
{
    public class ProductsModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Required]
        [Display(Name = "Product Price")]
        public decimal Price { get; set; }

        [ValidateNever]
     
        [Display(Name = " Choose a Image")]
        public IFormFile Image {  get; set; }
        public string ProductColor {  get; set; }
        [Required]
        [Display(Name = "Available")]
        public bool IsAvailable {  get; set; }

        [Required]
        [Display(Name = "Product Type")]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Special Tag")]
        public int SpecialTagId {  get; set; }

        [ValidateNever]
        public string ImageUrl {  get; set; }

        public string? ProductType {  get; set; }
        public string? SpecialTag {  get; set; }

        public int Quantity {  get; set; }

       


    }
}
