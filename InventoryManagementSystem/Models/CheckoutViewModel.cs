namespace InventoryManagementSystem.Models
{
    public class CheckoutViewModel
    {
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public int PaymentMethodId { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public List<TransactionProduct> Products { get; set; } = new List<TransactionProduct>();

    }

    public class TransactionProduct
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
 
}
