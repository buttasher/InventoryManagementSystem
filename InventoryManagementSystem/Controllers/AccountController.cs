using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly InventoryManagementSystemContext _context;

        public AccountController(InventoryManagementSystemContext context)
        {
            _context = context;
        }

        // GET: Login Page
        public IActionResult Index()
        {
            return View();
        }

        // POST: Handle Login
        // POST: Handle Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Store role in session
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("FullName", user.FullName);

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (user.Role == "Staff")
                {
                    return RedirectToAction("Index", "POS");
                }
            }

            // ❌ If credentials are incorrect, show error and return to Index view
            ViewBag.Error = "Invalid credentials!";
            return View("Index"); // ✅ Ensure it goes back to Index.cshtml
        }


        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
