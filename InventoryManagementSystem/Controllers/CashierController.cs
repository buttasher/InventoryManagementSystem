using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    public class CashierController : Controller
    {
        // GET: CashierController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CashierController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CashierController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CashierController/Create
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

        // GET: CashierController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CashierController/Edit/5
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

        // GET: CashierController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CashierController/Delete/5
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
