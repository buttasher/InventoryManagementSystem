using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;

namespace InventoryManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly InventoryManagementSystemContext _context;
        private readonly IConfiguration _configuration;
        private readonly NotificationService _notificationService;

        public AdminController(InventoryManagementSystemContext context, IConfiguration configuration, NotificationService notificationService)
        {
            _context = context;
            _configuration = configuration;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            decimal totalSalesToday = 0;
            int totalItemSold = 0;
            int expiredItems = 0;
            int lowStockItems = 0;

            string connectionString = _configuration.GetConnectionString("dbconn");

            // Retrieve session values
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var userRole = HttpContext.Session.GetString("UserRole");

            // Fetch latest notifications for the user
            var notifications = _context.Notifications
                .Where(n => n.UserId == userId && n.Role == userRole)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToList();

            ViewBag.Notifications = notifications;
            ViewBag.NotificationCount = notifications.Count(n => !(bool)n.IsRead);

            // Total Sales Today
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT SUM(TotalAmount) FROM inventorymanagementsystem.transactions 
                                 WHERE CAST(TransactionDate AS DATE) = CAST(GETDATE() AS DATE);";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                totalSalesToday = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }

            // Total Items Sold Today
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT SUM(ProductID) FROM inventorymanagementsystem.transactiondetails 
                                 JOIN inventorymanagementsystem.transactions 
                                 ON transactions.TransactionID = transactiondetails.TransactionID 
                                 WHERE CAST(TransactionDate AS DATE) = CAST(GETDATE() AS DATE);";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                totalItemSold = result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }

            // Expired Items Count
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM inventorymanagementsystem.expireditems";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                expiredItems = result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }

            // Low Stock Items Count
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM inventorymanagementsystem.lowstock";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                lowStockItems = result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }

            ViewBag.TotalSalesToday = totalSalesToday;
            ViewBag.TotalItemSold = totalItemSold;
            ViewBag.ExpiredItems = expiredItems;
            ViewBag.LowStockItems = lowStockItems;

            // Top 5 Expired Items
            var expiredItemList = await _context.Expireditems
                .Where(e => e.ExpireDate < DateOnly.FromDateTime(DateTime.Now))
                .OrderByDescending(e => e.ExpireDate)
                .Take(5)
                .ToListAsync();

            ViewBag.ExpiredItemList = expiredItemList;

            return View();
        }

        public IActionResult MarkNotificationAsRead(int notificationId)
        {
            var notification = _context.Notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetMonthlySales(int year = 0)
        {
            string connectionString = _configuration.GetConnectionString("dbconn");
            List<decimal> monthlySales = new List<decimal>();
            decimal yearlyTotal = 0;

            if (year == 0)
            {
                year = DateTime.Now.Year;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                for (int month = 1; month <= 12; month++)
                {
                    string query = @"
                        SELECT ISNULL(SUM(TotalAmount), 0) 
                        FROM inventorymanagementsystem.transactions 
                        WHERE MONTH(TransactionDate) = @Month AND YEAR(TransactionDate) = @Year";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    var result = cmd.ExecuteScalar();
                    decimal monthTotal = result != DBNull.Value ? Convert.ToDecimal(result) : 0;

                    monthlySales.Add(monthTotal);
                    yearlyTotal += monthTotal;
                }
            }

            var monthlySalesPercentage = monthlySales.Select(s => yearlyTotal > 0 ? Math.Round((s / yearlyTotal) * 100, 2) : 0).ToList();

            return Json(new
            {
                labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" },
                data = monthlySales,
                percentages = monthlySalesPercentage
            });
        }

        public async Task<IActionResult> ManageUsers()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var staff = await _context.Users.Where(u => u.Role == "Staff").ToListAsync();
            return View(staff);
        }

        public IActionResult CreateStaff()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff(string email, string fullName, string password, int phoneNumber)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var staff = new User
            {
                Email = email,
                FullName = fullName,
                Password = password,
                PhoneNumber = phoneNumber,
                Role = "Staff",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(staff);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageUsers");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Email,FullName,Password,PhoneNumber,CreatedAt")] User user)
        {
            if (id != user.UserId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.UserId == user.UserId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
                return NotFound();

            return View(user);
        }
    }
}
