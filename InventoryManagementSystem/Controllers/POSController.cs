using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Security.Claims;

namespace InventoryManagementSystem.Controllers
{
    public class POSController : Controller
    {
        private readonly InventoryManagementSystemContext _context;
        private readonly NotificationService _notificationService;
        public POSController(InventoryManagementSystemContext context , NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            int? returnItemId = HttpContext.Session.GetInt32("ReturnItemId");

            if (HttpContext.Session.GetString("UserRole") != "Staff")
            {
                return RedirectToAction("Login", "Account");
            }


            ViewBag.UserId = userId;
            ViewBag.CustomerId = customerId;
            ViewBag.ReturnItemId = returnItemId;

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
                    p.SellingPrice,
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
                        TransactionDate = DateTime.Now,
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

                        var productInDb = _context.Products.FirstOrDefault(p => p.ProductId == product.ProductId);
                        if (productInDb != null)
                        {
                            productInDb.Quantity -= product.Quantity;

                            // Ensure quantity doesn't go below zero
                            if (productInDb.Quantity < 0)
                            {
                                productInDb.Quantity = 0;
                            }

                            _context.Products.Update(productInDb);
                        }
                    }
            
                    _context.SaveChanges();
                    transaction.Commit();

                    // ✅ ADD YOUR NOTIFICATION HERE
                    string userRole = HttpContext.Session.GetString("UserRole");
                    int? userId = HttpContext.Session.GetInt32("UserId");

                    string Message = "Transaction #" + newTransaction.TransactionId + " completed successfully.";
                    string Role = userRole ?? "Unknown"; // Optional: Staff or Admin
                    int UserId = userId ?? 0;         // Optional: current logged in user

                    _ = _notificationService.SendNotification(Message, UserId, Role);

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
