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
    public class ReturnItemsController : Controller
    {
        private readonly InventoryManagementSystemContext _context;

        public ReturnItemsController(InventoryManagementSystemContext context)
        {
            _context = context;
        }

        // GET: ReturnItems
        public async Task<IActionResult> Index()
        {
            var inventoryManagementSystemContext = _context.ReturnItems.Include(r => r.Product).Include(r => r.Transaction);
            return View(await inventoryManagementSystemContext.ToListAsync());
        }

        // GET: ReturnItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var returnItem = await _context.ReturnItems
                .Include(r => r.Product)
                .Include(r => r.Transaction)
                .FirstOrDefaultAsync(m => m.ReturnItemsId == id);
            if (returnItem == null)
            {
                return NotFound();
            }

            return View(returnItem);
        }

        // GET: ReturnItems/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "TransactionId", "TransactionId");
            return View();
        }

        // POST: ReturnItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReturnItemsId,TransactionId,ProductId,ReturnQuantity,ReturnReason,ReturnDate,Status")] ReturnItem returnItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(returnItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", returnItem.ProductId);
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "TransactionId", "TransactionId", returnItem.TransactionId);
            return View(returnItem);
        }

        // GET: ReturnItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var returnItem = await _context.ReturnItems.FindAsync(id);
            if (returnItem == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", returnItem.ProductId);
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "TransactionId", "TransactionId", returnItem.TransactionId);
            return View(returnItem);
        }

        // POST: ReturnItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReturnItemsId,TransactionId,ProductId,ReturnQuantity,ReturnReason,ReturnDate,Status")] ReturnItem returnItem)
        {
            if (id != returnItem.ReturnItemsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(returnItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReturnItemExists(returnItem.ReturnItemsId))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", returnItem.ProductId);
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "TransactionId", "TransactionId", returnItem.TransactionId);
            return View(returnItem);
        }

        // GET: ReturnItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var returnItem = await _context.ReturnItems
                .Include(r => r.Product)
                .Include(r => r.Transaction)
                .FirstOrDefaultAsync(m => m.ReturnItemsId == id);
            if (returnItem == null)
            {
                return NotFound();
            }

            return View(returnItem);
        }

        // POST: ReturnItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var returnItem = await _context.ReturnItems.FindAsync(id);
            if (returnItem != null)
            {
                _context.ReturnItems.Remove(returnItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReturnItemExists(int id)
        {
            return _context.ReturnItems.Any(e => e.ReturnItemsId == id);
        }
    }
}
