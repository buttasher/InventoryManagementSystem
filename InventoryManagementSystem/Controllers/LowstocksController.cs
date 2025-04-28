using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Controllers
{
    public class LowstocksController : Controller
    {
        private readonly InventoryManagementSystemContext _context;

        public LowstocksController(InventoryManagementSystemContext context)
        {
            _context = context;
        }

        // GET: Lowstocks
        public async Task<IActionResult> Index()
        {
            // First, fetch all Lowstock entries
            var lowStockItems = await _context.Lowstocks.ToListAsync();

            // Then check if any product quantity is now >= 10
            foreach (var item in lowStockItems.ToList()) // use .ToList() to avoid collection modification error
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
                if (product != null && product.Quantity >= 10)
                {
                    // Remove from Lowstock table
                    _context.Lowstocks.Remove(item);
                }
            }

            // Save changes after removing
            await _context.SaveChangesAsync();

            // Now, fetch updated low stock list
            var updatedLowStockItems = await _context.Lowstocks
                .Where(l => l.Quantity < 10)
                .ToListAsync();

            return View(updatedLowStockItems);
        }


        
    }
}
