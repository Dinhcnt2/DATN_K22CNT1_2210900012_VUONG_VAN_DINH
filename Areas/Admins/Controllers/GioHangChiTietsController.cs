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
    public class GioHangChiTietsController : Controller
    {
        private readonly BookstoreContext _context;

        public GioHangChiTietsController(BookstoreContext context)
        {
            _context = context;
        }

        // GET: Admins/GioHangChiTiets
        public async Task<IActionResult> Index()
        {
            var bookstoreContext = _context.GioHangChiTiets.Include(g => g.BienThe).Include(g => g.GioHang);
            return View(await bookstoreContext.ToListAsync());
        }

        // GET: Admins/GioHangChiTiets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gioHangChiTiet = await _context.GioHangChiTiets
                .Include(g => g.BienThe)
                .Include(g => g.GioHang)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gioHangChiTiet == null)
            {
                return NotFound();
            }

            return View(gioHangChiTiet);
        }

        // GET: Admins/GioHangChiTiets/Create
        public IActionResult Create()
        {
            ViewData["BienTheId"] = new SelectList(_context.BienTheSaches, "Id", "Id");
            ViewData["GioHangId"] = new SelectList(_context.GioHangs, "MaGioHang", "MaGioHang");
            return View();
        }

        // POST: Admins/GioHangChiTiets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GioHangId,BienTheId,SoLuong,DonGia,ThanhTien")] GioHangChiTiet gioHangChiTiet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gioHangChiTiet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BienTheId"] = new SelectList(_context.BienTheSaches, "Id", "Id", gioHangChiTiet.BienTheId);
            ViewData["GioHangId"] = new SelectList(_context.GioHangs, "MaGioHang", "MaGioHang", gioHangChiTiet.GioHangId);
            return View(gioHangChiTiet);
        }

        // GET: Admins/GioHangChiTiets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gioHangChiTiet = await _context.GioHangChiTiets.FindAsync(id);
            if (gioHangChiTiet == null)
            {
                return NotFound();
            }
            ViewData["BienTheId"] = new SelectList(_context.BienTheSaches, "Id", "Id", gioHangChiTiet.BienTheId);
            ViewData["GioHangId"] = new SelectList(_context.GioHangs, "MaGioHang", "MaGioHang", gioHangChiTiet.GioHangId);
            return View(gioHangChiTiet);
        }

        // POST: Admins/GioHangChiTiets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GioHangId,BienTheId,SoLuong,DonGia,ThanhTien")] GioHangChiTiet gioHangChiTiet)
        {
            if (id != gioHangChiTiet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gioHangChiTiet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GioHangChiTietExists(gioHangChiTiet.Id))
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
            ViewData["BienTheId"] = new SelectList(_context.BienTheSaches, "Id", "Id", gioHangChiTiet.BienTheId);
            ViewData["GioHangId"] = new SelectList(_context.GioHangs, "MaGioHang", "MaGioHang", gioHangChiTiet.GioHangId);
            return View(gioHangChiTiet);
        }

        // GET: Admins/GioHangChiTiets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gioHangChiTiet = await _context.GioHangChiTiets
                .Include(g => g.BienThe)
                .Include(g => g.GioHang)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gioHangChiTiet == null)
            {
                return NotFound();
            }

            return View(gioHangChiTiet);
        }

        // POST: Admins/GioHangChiTiets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gioHangChiTiet = await _context.GioHangChiTiets.FindAsync(id);
            if (gioHangChiTiet != null)
            {
                _context.GioHangChiTiets.Remove(gioHangChiTiet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GioHangChiTietExists(int id)
        {
            return _context.GioHangChiTiets.Any(e => e.Id == id);
        }
    }
}
