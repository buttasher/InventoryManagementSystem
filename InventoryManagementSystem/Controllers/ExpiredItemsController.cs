using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Controllers
{
    public class ExpiredItemsController : Controller
    {
        private readonly InventoryManagementSystemContext _context;

        public ExpiredItemsController(InventoryManagementSystemContext context)
        {
            _context = context;
        }

        // GET: ExpiredItems Warning
        public async Task<IActionResult> Index()
        {
            // Convert DateTime.UtcNow to DateOnly for comparison
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

            var expiredItems = await _context.Expireditems
                .Where(e => e.ExpireDate < today)  // Corrected type comparison
                .ToListAsync();

            return View(expiredItems);
        }
    }
}
