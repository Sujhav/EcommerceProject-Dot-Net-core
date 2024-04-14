using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Models
{
    public class SpecialTagModel
    {
        public int id {  get; set; }

        [Required]
        [Display(Name ="Special Tag Name")]
        public string name { get; set; }
    }
}
