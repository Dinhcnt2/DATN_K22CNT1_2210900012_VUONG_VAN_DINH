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
    public class DanhGiasController : Controller
    {
        private readonly BookstoreContext _context;

        public DanhGiasController(BookstoreContext context)
        {
            _context = context;
        }

        // GET: Admins/DanhGias
        public async Task<IActionResult> Index()
        {
            var bookstoreContext = _context.DanhGia.Include(d => d.MaNguoiDungNavigation).Include(d => d.SanPham);
            return View(await bookstoreContext.ToListAsync());
        }

        // GET: Admins/DanhGias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGia = await _context.DanhGia
                .Include(d => d.MaNguoiDungNavigation)
                .Include(d => d.SanPham)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (danhGia == null)
            {
                return NotFound();
            }

            return View(danhGia);
        }

        // GET: Admins/DanhGias/Create
        public IActionResult Create()
        {
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung");
            ViewData["SanPhamId"] = new SelectList(_context.SanPhams, "Id", "Id");
            return View();
        }

        // POST: Admins/DanhGias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SanPhamId,MaNguoiDung,SoSao,NoiDung,NgayDanhGia")] DanhGia danhGia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(danhGia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", danhGia.MaNguoiDung);
            ViewData["SanPhamId"] = new SelectList(_context.SanPhams, "Id", "Id", danhGia.SanPhamId);
            return View(danhGia);
        }

        // GET: Admins/DanhGias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGia = await _context.DanhGia.FindAsync(id);
            if (danhGia == null)
            {
                return NotFound();
            }
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", danhGia.MaNguoiDung);
            ViewData["SanPhamId"] = new SelectList(_context.SanPhams, "Id", "Id", danhGia.SanPhamId);
            return View(danhGia);
        }

        // POST: Admins/DanhGias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SanPhamId,MaNguoiDung,SoSao,NoiDung,NgayDanhGia")] DanhGia danhGia)
        {
            if (id != danhGia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhGia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhGiaExists(danhGia.Id))
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
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", danhGia.MaNguoiDung);
            ViewData["SanPhamId"] = new SelectList(_context.SanPhams, "Id", "Id", danhGia.SanPhamId);
            return View(danhGia);
        }

        // GET: Admins/DanhGias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGia = await _context.DanhGia
                .Include(d => d.MaNguoiDungNavigation)
                .Include(d => d.SanPham)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (danhGia == null)
            {
                return NotFound();
            }

            return View(danhGia);
        }

        // POST: Admins/DanhGias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var danhGia = await _context.DanhGia.FindAsync(id);
            if (danhGia != null)
            {
                _context.DanhGia.Remove(danhGia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DanhGiaExists(int id)
        {
            return _context.DanhGia.Any(e => e.Id == id);
        }
    }
}
