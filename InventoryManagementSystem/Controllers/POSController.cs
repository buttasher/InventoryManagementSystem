using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    public class POSController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Staff")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
    }
}
