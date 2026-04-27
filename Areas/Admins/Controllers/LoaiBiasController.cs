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
    public class LoaiBiasController : Controller
    {
        private readonly BookstoreContext _context;

        public LoaiBiasController(BookstoreContext context)
        {
            _context = context;
        }

        // GET: Admins/LoaiBias
        public async Task<IActionResult> Index()
        {
            return View(await _context.LoaiBia.ToListAsync());
        }

        // GET: Admins/LoaiBias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiBia = await _context.LoaiBia
                .FirstOrDefaultAsync(m => m.MaLoaiBia == id);
            if (loaiBia == null)
            {
                return NotFound();
            }

            return View(loaiBia);
        }

        // GET: Admins/LoaiBias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/LoaiBias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLoaiBia,TenLoaiBia")] LoaiBia loaiBia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaiBia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiBia);
        }

        // GET: Admins/LoaiBias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiBia = await _context.LoaiBia.FindAsync(id);
            if (loaiBia == null)
            {
                return NotFound();
            }
            return View(loaiBia);
        }

        // POST: Admins/LoaiBias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLoaiBia,TenLoaiBia")] LoaiBia loaiBia)
        {
            if (id != loaiBia.MaLoaiBia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiBia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiBiaExists(loaiBia.MaLoaiBia))
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
            return View(loaiBia);
        }

        // GET: Admins/LoaiBias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiBia = await _context.LoaiBia
                .FirstOrDefaultAsync(m => m.MaLoaiBia == id);
            if (loaiBia == null)
            {
                return NotFound();
            }

            return View(loaiBia);
        }

        // POST: Admins/LoaiBias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiBia = await _context.LoaiBia.FindAsync(id);
            if (loaiBia != null)
            {
                _context.LoaiBia.Remove(loaiBia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaiBiaExists(int id)
        {
            return _context.LoaiBia.Any(e => e.MaLoaiBia == id);
        }
    }
}
