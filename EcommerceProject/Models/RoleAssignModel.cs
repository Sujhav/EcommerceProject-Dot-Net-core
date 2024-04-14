using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcommerceProject.Models
{
    public class RoleAssignModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public IEnumerable<SelectListItem>? RoleList { get; set; }
    }
}
