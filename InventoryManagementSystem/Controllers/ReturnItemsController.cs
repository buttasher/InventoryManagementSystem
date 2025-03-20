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
    public class ReturnitemsController : Controller
    {
        private readonly InventoryManagementSystemContext _context;

        public ReturnitemsController(InventoryManagementSystemContext context)
        {
            _context = context;
        }

        // GET: Returnitems
        public async Task<IActionResult> Index()
        {
            var inventoryManagementSystemContext = _context.Returnitems.Include(r => r.Product).Include(r => r.Transaction);
            return View(await inventoryManagementSystemContext.ToListAsync());
        }

        // GET: Returnitems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var returnitem = await _context.Returnitems
                .Include(r => r.Product)
                .Include(r => r.Transaction)
                .FirstOrDefaultAsync(m => m.ReturnItemsId == id);
            if (returnitem == null)
            {
                return NotFound();
            }

            return View(returnitem);
        }

        // GET: Returnitems/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "TransactionId", "TransactionId");
            return View();
        }

        // POST: Returnitems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReturnItemsId,TransactionId,ProductId,ReturnQuantity,ReturnReason,ReturnDate,Status")] Returnitem returnitem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(returnitem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", returnitem.ProductId);
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "TransactionId", "TransactionId", returnitem.TransactionId);
            return View(returnitem);
        }

        // GET: Returnitems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var returnitem = await _context.Returnitems.FindAsync(id);
            if (returnitem == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", returnitem.ProductId);
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "TransactionId", "TransactionId", returnitem.TransactionId);
            return View(returnitem);
        }

        // POST: Returnitems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReturnItemsId,TransactionId,ProductId,ReturnQuantity,ReturnReason,ReturnDate,Status")] Returnitem returnitem)
        {
            if (id != returnitem.ReturnItemsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(returnitem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReturnitemExists(returnitem.ReturnItemsId))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", returnitem.ProductId);
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "TransactionId", "TransactionId", returnitem.TransactionId);
            return View(returnitem);
        }

        // GET: Returnitems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var returnitem = await _context.Returnitems
                .Include(r => r.Product)
                .Include(r => r.Transaction)
                .FirstOrDefaultAsync(m => m.ReturnItemsId == id);
            if (returnitem == null)
            {
                return NotFound();
            }

            return View(returnitem);
        }

        // POST: Returnitems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var returnitem = await _context.Returnitems.FindAsync(id);
            if (returnitem != null)
            {
                _context.Returnitems.Remove(returnitem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReturnitemExists(int id)
        {
            return _context.Returnitems.Any(e => e.ReturnItemsId == id);
        }
    }
}
