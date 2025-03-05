using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AdminController
        public ActionResult CreateUser()
        {
            return View();
        }

        // GET: AdminController
        public ActionResult AddNewItem()
        {
            return View();
        }

        // GET: AdminController
        public ActionResult ProductList()
        {
            return View();
        }

        // GET: AdminController
        public ActionResult LowStock()
        {
            return View();
        }

        // GET: AdminController
        public ActionResult ExpiredItem()
        {
            return View();
        }

        // GET: AdminController
        public ActionResult UserList()
        {
            return View();
        }

        // GET: AdminController
        public ActionResult GeneralSettings()
        {
            return View();
        }


        // GET: AdminController
        public ActionResult PaymentSettings()
        {
            return View();
        }

        // GET: AdminController
        public ActionResult GroupPermissions()
        {
            return View();
        }

        // GET: AdminController
        public ActionResult TaxRates()
        {
            return View();
        }

     
        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
