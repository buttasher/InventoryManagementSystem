using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;

namespace InventoryManagementSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly InventoryManagementSystemContext _context;
        private readonly NotificationService _notificationService;


        public ProductsController(InventoryManagementSystemContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var inventoryManagementSystemContext = _context.Products.Include(p => p.Category);
            return View(await inventoryManagementSystemContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Productcategories, "CategoryId", "CategoryId");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ItemName,ItemId,CategoryId,Brand,CostPrice,SellingPrice,Unit,Quantity,ManufactureDate,ExpiryDate,CreatedAt")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();

                // Check if the product is low in stock
                await CheckAndUpdateLowStock(product);

                await CheckAndUpdateExpiredItems(product);

                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Productcategories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Productcategories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ItemName,ItemId,CategoryId,Brand,CostPrice,SellingPrice,Unit,Quantity,ManufactureDate,ExpiryDate,CreatedAt")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    // Check if the product is low in stock
                    await CheckAndUpdateLowStock(product);

                    await CheckAndUpdateExpiredItems(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Productcategories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        private async Task CheckAndUpdateLowStock(Product product)
        {
            if (product.Quantity < 10) // If quantity is below 10, add/update LowStock
            {
                var existingLowStock = await _context.Lowstocks.FirstOrDefaultAsync(l => l.ProductId == product.ProductId);

                if (existingLowStock == null) // Insert if not already in LowStock
                {
                    var lowStockItem = new Lowstock
                    {
                        ProductId = product.ProductId,
                        ItemName = product.ItemName,
                        Category = product.CategoryId.ToString(), // Assuming CategoryId is foreign key
                        Brand = product.Brand,
                        Quantity = (int)product.Quantity,
                        Alert = 1,  // Assuming `Alert` is a short data type (1 = true)
                        CreatedAt = DateTime.Now
                    };
                    _context.Lowstocks.Add(lowStockItem);
                }
                else // Update existing LowStock entry
                {
                    existingLowStock.Quantity = (int)product.Quantity;
                    existingLowStock.Alert = 1;  // Ensure alert is still active
                    _context.Lowstocks.Update(existingLowStock);
                }

                await _context.SaveChangesAsync();
            }
            else // If quantity is 10 or more, remove from LowStock
            {
                var existingLowStock = await _context.Lowstocks.FirstOrDefaultAsync(l => l.ProductId == product.ProductId);
                if (existingLowStock != null)
                {
                    _context.Lowstocks.Remove(existingLowStock);
                    await _context.SaveChangesAsync();
                }
            }
        }

        private async Task CheckAndUpdateExpiredItems(Product product)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            if (product.ExpiryDate < today) // If product is expired
            {
                var existingExpiredItem = await _context.Expireditems.FirstOrDefaultAsync(e => e.ItemId == product.ProductId);

                if (existingExpiredItem == null) // Insert if not already in ExpiredItems
                {
                    var expiredItem = new Expireditem
                    {
                        ItemId = product.ProductId,
                        ItemName = product.ItemName,
                        Category = product.CategoryId.ToString(), // Assuming CategoryId is foreign key
                        ManufactureDate = (DateOnly)product.ManufactureDate,
                        ExpireDate = (DateOnly)product.ExpiryDate,
                        Alert = 1,  // Assuming `Alert` is a short data type (1 = true)
                        CreatedAt = DateTime.Now
                    };
                    _context.Expireditems.Add(expiredItem);
                }
                else // Update existing ExpiredItem entry
                {
                    existingExpiredItem.ExpireDate = (DateOnly)product.ExpiryDate;
                    existingExpiredItem.Alert = 1;  // Ensure alert is still active
                    _context.Expireditems.Update(existingExpiredItem);
                }

                await _context.SaveChangesAsync();
            }
            else // If product is not expired, remove it from ExpiredItems
            {
                var existingExpiredItem = await _context.Expireditems.FirstOrDefaultAsync(e => e.ItemId == product.ProductId);
                if (existingExpiredItem != null)
                {
                    _context.Expireditems.Remove(existingExpiredItem);
                    await _context.SaveChangesAsync();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProducts(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a valid CSV file.";
                return RedirectToAction(nameof(Index));
            }

            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                while (!stream.EndOfStream)
                {
                    var line = await stream.ReadLineAsync();
                    var values = line.Split(',');

                    if (values.Length >= 10) // Ensure enough columns
                    {
                        var product = new Product
                        {
                            ItemName = values[0],
                            ProductId = int.Parse(values[1]),
                            CategoryId = int.Parse(values[2]),
                            Brand = values[3],
                            CostPrice = (values[4]),
                            SellingPrice = (values[5]),
                            Unit = values[6],
                            Quantity = int.Parse(values[7]),
                            ManufactureDate = DateOnly.Parse(values[8]),
                            ExpiryDate = DateOnly.Parse(values[9]),
                            CreatedAt = DateTime.Now
                        };

                        _context.Products.Add(product);
                    }
                }

                await _context.SaveChangesAsync();

                int? userId = HttpContext.Session.GetInt32("UserId");
                string userRole = HttpContext.Session.GetString("UserRole");
                string message = "Excel File Successfully Uploaded";


                // ✅ Push Notification to Admin
                await _notificationService.SendNotification(message, userId.Value, userRole);
            }

            TempData["Success"] = "Products uploaded successfully!";
            return RedirectToAction(nameof(Index));
        }


    }
}
