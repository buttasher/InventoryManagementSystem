using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{
    public class LowStockController : Controller
    {
        private readonly InventoryManagementSystemContext _context;

        public LowStockController(InventoryManagementSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lowStockItems = await _context.Lowstocks.Where(l => l.Quantity < 10).ToListAsync();

            return View(lowStockItems);
        }
    }
}

