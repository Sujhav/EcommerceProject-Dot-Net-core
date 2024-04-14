using Stripe;

namespace EcommerceProject.Utility
{
    public class OrderStatus
    {
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusCompleted = "Completed";

        public const string Payment = "Paid";
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "ApprovedforDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";
        public const string PaymentStatusRefunded = "Refunded";

    }
}
