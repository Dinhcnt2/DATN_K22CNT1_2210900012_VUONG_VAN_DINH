using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VVD_2210900012_DATN.Models;

namespace VVD_2210900012_DATN.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class VouchersController : Controller
    {
        private readonly BookstoreContext _context;

        public VouchersController(BookstoreContext context)
        {
            _context = context;
        }

        // GET: Admins/Vouchers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Vouchers.ToListAsync());
        }

        // GET: Admins/Vouchers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (voucher == null) return NotFound();

            return View(voucher);
        }

        // GET: Admins/Vouchers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/Vouchers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaCode,GiamGia,Loai,NgayBatDau,NgayKetThuc,SoLuong,TrangThai")] Voucher voucher)
        {
            // 🔥 CHECK TRÙNG MÃ
            if (_context.Vouchers.Any(x => x.MaCode == voucher.MaCode))
            {
                ModelState.AddModelError("", "❌ Mã voucher đã tồn tại!");
                return View(voucher);
            }

            if (ModelState.IsValid)
            {
                _context.Add(voucher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(voucher);
        }

        // GET: Admins/Vouchers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null) return NotFound();

            return View(voucher);
        }

        // POST: Admins/Vouchers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaCode,GiamGia,Loai,NgayBatDau,NgayKetThuc,SoLuong,TrangThai")] Voucher voucher)
        {
            if (id != voucher.Id) return NotFound();

            // 🔥 CHECK TRÙNG (TRỪ CHÍNH NÓ)
            if (_context.Vouchers.Any(x => x.MaCode == voucher.MaCode && x.Id != voucher.Id))
            {
                ModelState.AddModelError("", "❌ Mã voucher đã tồn tại!");
                return View(voucher);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voucher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoucherExists(voucher.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }
            return View(voucher);
        }

        // GET: Admins/Vouchers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (voucher == null) return NotFound();

            return View(voucher);
        }

        // POST: Admins/Vouchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);

            if (voucher != null)
            {
                // 🔥 KHÔNG XOÁ → CHỈ TẮT
                voucher.TrangThai = false;

                _context.Update(voucher);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VoucherExists(int id)
        {
            return _context.Vouchers.Any(e => e.Id == id);
        }
    }
}
