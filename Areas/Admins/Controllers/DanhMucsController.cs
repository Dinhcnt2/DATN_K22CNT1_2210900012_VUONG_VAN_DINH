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
    public class DanhMucsController : Controller
    {
        private readonly BookstoreContext _context;

        public DanhMucsController(BookstoreContext context)
        {
            _context = context;
        }

        // GET: Admins/DanhMucs
        public async Task<IActionResult> Index()
        {
            var bookstoreContext = _context.DanhMucs.Include(d => d.CreatedByNavigation).Include(d => d.UpdatedByNavigation);
            return View(await bookstoreContext.ToListAsync());
        }

        // GET: Admins/DanhMucs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.DanhMucs
                .Include(d => d.CreatedByNavigation)
                .Include(d => d.UpdatedByNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (danhMuc == null)
            {
                return NotFound();
            }

            return View(danhMuc);
        }

        // GET: Admins/DanhMucs/Create
        public IActionResult Create()
        {
            ViewData["CreatedBy"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung");
            ViewData["UpdatedBy"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung");
            return View();
        }

        // POST: Admins/DanhMucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenDanhMuc,Slug,CreatedBy,UpdatedBy")] DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(danhMuc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedBy"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", danhMuc.CreatedBy);
            ViewData["UpdatedBy"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", danhMuc.UpdatedBy);
            return View(danhMuc);
        }

        // GET: Admins/DanhMucs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            ViewData["CreatedBy"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", danhMuc.CreatedBy);
            ViewData["UpdatedBy"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", danhMuc.UpdatedBy);
            return View(danhMuc);
        }

        // POST: Admins/DanhMucs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenDanhMuc,Slug,CreatedBy,UpdatedBy")] DanhMuc danhMuc)
        {
            if (id != danhMuc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhMuc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhMucExists(danhMuc.Id))
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
            ViewData["CreatedBy"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", danhMuc.CreatedBy);
            ViewData["UpdatedBy"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", danhMuc.UpdatedBy);
            return View(danhMuc);
        }

        // GET: Admins/DanhMucs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.DanhMucs
                .Include(d => d.CreatedByNavigation)
                .Include(d => d.UpdatedByNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (danhMuc == null)
            {
                return NotFound();
            }

            return View(danhMuc);
        }

        // POST: Admins/DanhMucs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc != null)
            {
                _context.DanhMucs.Remove(danhMuc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DanhMucExists(int id)
        {
            return _context.DanhMucs.Any(e => e.Id == id);
        }
    }
}
