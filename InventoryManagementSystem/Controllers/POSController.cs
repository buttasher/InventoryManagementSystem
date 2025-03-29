using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagementSystem.Controllers
{
    public class POSController : Controller
    {
        private readonly InventoryManagementSystemContext _context;

        public POSController(InventoryManagementSystemContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Staff")
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch all product IDs and pass them to the view
            ViewBag.ProductId = new SelectList(_context.Products, "ProductId", "ProductId");
            ViewBag.TransactionId = new SelectList(_context.Transactions, "TransactionId", "TransactionId");
            return View(); 
        }

        [HttpGet]
        public IActionResult GetProductDetails(int id)
        {
            var product = _context.Products
                .Where(p => p.ProductId == id)
                .Select(p => new
                {
                    p.ProductId,
                    p.ItemName,
                    p.Category,
                    p.Brand,
                    p.Price,
                    p.Unit,
                    p.Quantity,
                    // Include any other fields you need
                })
                .FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }
            return Json(product);
        }
        [HttpGet]
        public IActionResult GetNewTransactionId()
        {
            // For example, return the max transaction id + 1 if transactions exist, else 1.
            int newTransactionId = 1;
            if (_context.Transactions.Any())
            {
                newTransactionId = _context.Transactions.Max(t => t.TransactionId) + 1;
            }
            return Json(new { transactionId = newTransactionId });
        }

        [HttpPost]
        [Route("Checkout")]
        public IActionResult Checkout([FromBody] CheckoutViewModel checkoutData)
        {
            try
            {
                if (checkoutData == null || checkoutData.Products == null || !checkoutData.Products.Any())
                {
                    return BadRequest("Invalid checkout data.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    var newTransaction = new Transaction
                    {
                        UserId = checkoutData.UserId,
                        CustomerId = checkoutData.CustomerId,
                        PaymentMethodId = checkoutData.PaymentMethodId,
                        TransactionDate = checkoutData.TransactionDate,
                        Subtotal = checkoutData.Subtotal,
                        Tax = checkoutData.Tax,
                        TotalAmount = checkoutData.TotalAmount
                    };

                    _context.Transactions.Add(newTransaction);
                    _context.SaveChanges();

                    foreach (var product in checkoutData.Products)
                    {
                        var transactionDetail = new Transactiondetail
                        {
                            TransactionId = newTransaction.TransactionId,
                            ProductId = product.ProductId,
                            Quantity = product.Quantity,
                            Price = product.Price,
                            Subtotal = product.Quantity * product.Price
                        };

                        _context.Transactiondetails.Add(transactionDetail);
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    return Ok(new { message = "Transaction successful", transactionId = newTransaction.TransactionId });
                }
            }
            catch (Exception ex)
            {
                // Log full error including inner exception
                return StatusCode(500, "Error processing transaction: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message);
            }
        }






    }
}
