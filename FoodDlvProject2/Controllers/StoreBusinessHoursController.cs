using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoodDlvProject2.EFModels;

namespace FoodDlvProject2.Controllers
{
    public class StoreBusinessHoursController : Controller
    {
        private readonly AppDbContext _context;

        public StoreBusinessHoursController(AppDbContext context)
        {
            _context = context;
        }

        // GET: StoreBusinessHours
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.StoreBusinessHours.Include(s => s.Store);
            return View(await appDbContext.ToListAsync());
        }

        // GET: StoreBusinessHours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StoreBusinessHours == null)
            {
                return NotFound();
            }

            var storeBusinessHour = await _context.StoreBusinessHours
                .Include(s => s.Store)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storeBusinessHour == null)
            {
                return NotFound();
            }

            return View(storeBusinessHour);
        }

        // GET: StoreBusinessHours/Create
        public IActionResult Create()
        {

            ViewData["OpeningDays"] = new SelectList(new Dictionary<int, string>()
    {
        { 0, "星期天" },
        { 1, "星期一" },
        { 2, "星期二" },
        { 3, "星期三" },
        { 4, "星期四" },
        { 5, "星期五" },
        { 6, "星期六" }
    }, "Key", "Value");

            


            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "StoreName");
            return View();
        }

        // POST: StoreBusinessHours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StoreId,OpeningTime,ClosingTime,OpeningDays")] StoreBusinessHour storeBusinessHour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storeBusinessHour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "StoreName", storeBusinessHour.StoreId);
            return View(storeBusinessHour);
        }

        // GET: StoreBusinessHours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StoreBusinessHours == null)
            {
                return NotFound();
            }

            var storeBusinessHour = await _context.StoreBusinessHours.FindAsync(id);
            if (storeBusinessHour == null)
            {
                return NotFound();
            }
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "StoreName", storeBusinessHour.StoreId);
            return View(storeBusinessHour);
        }

        // POST: StoreBusinessHours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StoreId,OpeningTime,ClosingTime,OpeningDays")] StoreBusinessHour storeBusinessHour)
        {
            if (id != storeBusinessHour.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storeBusinessHour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreBusinessHourExists(storeBusinessHour.Id))
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
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "StoreName", storeBusinessHour.StoreId);
            return View(storeBusinessHour);
        }

        // GET: StoreBusinessHours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StoreBusinessHours == null)
            {
                return NotFound();
            }

            var storeBusinessHour = await _context.StoreBusinessHours
                .Include(s => s.Store)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storeBusinessHour == null)
            {
                return NotFound();
            }

            return View(storeBusinessHour);
        }

        // POST: StoreBusinessHours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StoreBusinessHours == null)
            {
                return Problem("Entity set 'AppDbContext.StoreBusinessHours'  is null.");
            }
            var storeBusinessHour = await _context.StoreBusinessHours.FindAsync(id);
            if (storeBusinessHour != null)
            {
                _context.StoreBusinessHours.Remove(storeBusinessHour);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreBusinessHourExists(int id)
        {
          return _context.StoreBusinessHours.Any(e => e.Id == id);
        }
    }
}
