using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VVD_2210900012_DATN.Models;

namespace VVD_2210900012_DATN.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class NgonNgusController : Controller
    {
        private readonly BookstoreContext _context;

        public NgonNgusController(BookstoreContext context)
        {
            _context = context;
        }

        // GET: Admins/NgonNgus
        public async Task<IActionResult> Index()
        {
            return View(await _context.NgonNgus.ToListAsync());
        }

        // GET: Admins/NgonNgus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ngonNgu = await _context.NgonNgus
                .FirstOrDefaultAsync(m => m.MaNgonNgu == id);
            if (ngonNgu == null)
            {
                return NotFound();
            }

            return View(ngonNgu);
        }

        // GET: Admins/NgonNgus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/NgonNgus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNgonNgu,TenNgonNgu")] NgonNgu ngonNgu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ngonNgu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ngonNgu);
        }

        // GET: Admins/NgonNgus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ngonNgu = await _context.NgonNgus.FindAsync(id);
            if (ngonNgu == null)
            {
                return NotFound();
            }
            return View(ngonNgu);
        }

        // POST: Admins/NgonNgus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNgonNgu,TenNgonNgu")] NgonNgu ngonNgu)
        {
            if (id != ngonNgu.MaNgonNgu)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ngonNgu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NgonNguExists(ngonNgu.MaNgonNgu))
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
            return View(ngonNgu);
        }

        // GET: Admins/NgonNgus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ngonNgu = await _context.NgonNgus
                .FirstOrDefaultAsync(m => m.MaNgonNgu == id);
            if (ngonNgu == null)
            {
                return NotFound();
            }

            return View(ngonNgu);
        }

        // POST: Admins/NgonNgus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ngonNgu = await _context.NgonNgus.FindAsync(id);
            if (ngonNgu != null)
            {
                _context.NgonNgus.Remove(ngonNgu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NgonNguExists(int id)
        {
            return _context.NgonNgus.Any(e => e.MaNgonNgu == id);
        }
    }
}
