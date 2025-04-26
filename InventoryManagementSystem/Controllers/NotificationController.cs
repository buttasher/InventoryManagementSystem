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
    public class NotificationController : Controller
    {
        private readonly InventoryManagementSystemContext _context;

        public NotificationController(InventoryManagementSystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetNotifications()
        {
            var notifications = _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new
                {
                    n.Id,
                    n.Message,
                    n.CreatedAt
                })
                .ToList();

            return Json(notifications);
        }

        // ✅ Corrected to match your JS AJAX URL
        [HttpPost]
        [Route("Notification/ClearAll")] // ✅ Force the route to match your JS
        public async Task<IActionResult> ClearAll()
        {
            var allNotifications = _context.Notifications.ToList();

            if (allNotifications.Any())
            {
                _context.Notifications.RemoveRange(allNotifications);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, message = "All notifications cleared successfully." });
        }


        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            return View(await _context.Notifications.ToListAsync());
        }

        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }


     
    }
}
