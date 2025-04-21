using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Configuration;
using MySqlX.XDevAPI.Common;

namespace InventoryManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly InventoryManagementSystemContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(InventoryManagementSystemContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Step 1: Admin Dashboard
        public async Task<IActionResult> Index()

        {
            decimal totalSalesToday = 0;
            int totalItemSold = 0;
            int expiredItems = 0;
            int lowStockItems = 0;

            // Retrieve the connection string from appsettings.json
            string connectionString = _configuration.GetConnectionString("dbconn");



            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SUM(TotalAmount) FROM inventorymanagementsystem.transactions WHERE CAST(TransactionDate AS DATE) = CAST(GETDATE() AS DATE);";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();

                var result = cmd.ExecuteScalar();
                // Check if the result is DBNull, and if so, set the total sales to 0.
                totalSalesToday = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SUM(ProductID) FROM inventorymanagementsystem.transactiondetails JOIN inventorymanagementsystem.transactions ON transactions.TransactionID = transactiondetails.TransactionID WHERE CAST(transactionDate AS DATE) = CAST(GETDATE() AS DATE); ";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();

                var result = cmd.ExecuteScalar();
                // Check if the result is DBNull, and if so, set the total sales to 0.
                totalItemSold = (result != DBNull.Value) ? Convert.ToInt32(result) : 0;

            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM inventorymanagementsystem.expireditems";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();

                var result = cmd.ExecuteScalar();
                // Check if the result is DBNull, and if so, set the total sales to 0.
                expiredItems = result != DBNull.Value ? Convert.ToInt32(result): 0;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM inventorymanagementsystem.lowstock";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();

                var result = cmd.ExecuteScalar();
                // Check if the result is DBNull, and if so, set the total sales to 0.
                lowStockItems = result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }

            ViewBag.TotalSalesToday = totalSalesToday;
            ViewBag.TotalItemSold = totalItemSold;
            ViewBag.ExpiredItems = expiredItems;
            ViewBag.LowStockItems = lowStockItems;

            var expiredItemList = await _context.Expireditems
            .Where(e => e.ExpireDate < DateOnly.FromDateTime(DateTime.Now))
            .OrderByDescending(e => e.ExpireDate)
            .Take(5) // Just top 5, or remove Take() to show all
            .ToListAsync();

            ViewBag.ExpiredItemList = expiredItemList;


            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
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

        // Step 2: Show All Staff
        public async Task<IActionResult> ManageUsers()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var staff = await _context.Users.Where(u => u.Role == "Staff").ToListAsync();
            return View(staff);
        }

        // Step 3: Create Staff Account Form
        public IActionResult CreateStaff()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // Step 4: Save Staff to Database
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
                Password = password, // Encrypt this in the future
                PhoneNumber = phoneNumber,
                Role = "Staff",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(staff);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageUsers");
        }
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Email,FullName,Password,PhoneNumber,CreateAt")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExistes(user.UserId))
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
            return View(user);
        }
        private bool UserExistes(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        

    }
}
