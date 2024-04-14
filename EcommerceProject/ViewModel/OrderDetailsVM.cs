using EcommerceProject.Database;

namespace EcommerceProject.ViewModel
{
    public class OrderDetailsVM
    {
        public Order? Order { get; set; }

        public IEnumerable<OrderDetail>? OrderDetail { get; set; }
    }
}
