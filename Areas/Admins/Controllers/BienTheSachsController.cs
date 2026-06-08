using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VVD_2210900012_DATN.Models;

namespace VVD_2210900012_DATN.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class BienTheSachsController : Controller
    {
        private readonly BookstoreContext _context;

        public BienTheSachsController(BookstoreContext context)
        {
            _context = context;
        }

        // =====================================================
        // ===== INDEX =====
        // =====================================================
        public async Task<IActionResult> Index()
        {
            var data = _context.BienTheSaches
                .Include(x => x.SanPham)
                .Include(x => x.LoaiBia)
                .Include(x => x.NgonN);

            return View(await data.ToListAsync());
        }

        // =====================================================
        // ===== DETAILS =====
        // =====================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _context.BienTheSaches
                .Include(x => x.SanPham)
                .Include(x => x.LoaiBia)
                .Include(x => x.NgonN)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
                return NotFound();

            return View(item);
        }

        // =====================================================
        // ===== CREATE GET =====
        // =====================================================
        public IActionResult Create()
        {
            LoadDropdown();
            return View();
        }

        // =====================================================
        // ===== CREATE POST =====
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BienTheSach model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdown(model);
                return View(model);
            }

            var exist = await _context.BienTheSaches
                .FirstOrDefaultAsync(x =>
                    x.SanPhamId == model.SanPhamId &&
                    x.LoaiBiaId == model.LoaiBiaId &&
                    x.NgonNguId == model.NgonNguId);

            if (exist != null)
            {
                ModelState.AddModelError("", "❌ Biến thể với loại bìa và ngôn ngữ này đã tồn tại!");
                LoadDropdown(model);
                return View(model);
            }

            if (model.GiaBan == null || model.GiaBan == 0)
            {
                var sanPham = await _context.SanPhams.FindAsync(model.SanPhamId);
                if (sanPham != null)
                {
                    model.GiaBan = sanPham.GiaSauGiam ?? sanPham.GiaGoc;
                }
                else
                {
                    model.GiaBan = 0;
                }
            }

            _context.BienTheSaches.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Thêm biến thể thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // ===== EDIT GET =====
        // =====================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _context.BienTheSaches.FindAsync(id);

            if (item == null)
                return NotFound();

            LoadDropdown(item);
            return View(item);
        }

        // =====================================================
        // ===== EDIT POST =====
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BienTheSach model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                LoadDropdown(model);
                return View(model);
            }

            try
            {
                if (model.GiaBan == null || model.GiaBan == 0)
                {
                    var sanPham = await _context.SanPhams.FindAsync(model.SanPhamId);
                    if (sanPham != null)
                    {
                        model.GiaBan = sanPham.GiaSauGiam ?? sanPham.GiaGoc;
                    }
                    else
                    {
                        model.GiaBan = 0;
                    }
                }

                _context.Update(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "✅ Cập nhật biến thể thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.BienTheSaches.Any(e => e.Id == model.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // ===== DELETE GET =====
        // =====================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _context.BienTheSaches
                .Include(x => x.SanPham)
                .Include(x => x.LoaiBia)
                .Include(x => x.NgonN)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
                return NotFound();

            return View(item);
        }

        // =====================================================
        // ===== DELETE POST =====
        // =====================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.BienTheSaches.FindAsync(id);

            if (item != null)
            {
                _context.BienTheSaches.Remove(item);
                await _context.SaveChangesAsync();

                TempData["Success"] = "🗑️ Đã xoá biến thể thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // ===== LOAD DROPDOWN =====
        // =====================================================
        // FIX CHUẨN ĐÉT: Thêm dấu ? vào sau BienTheSach để chấp nhận giá trị null mẫu
        private void LoadDropdown(BienTheSach? model = null)
        {
            ViewBag.SanPhamId = new SelectList(
                _context.SanPhams.ToList(),
                "Id",
                "TenSach",
                model?.SanPhamId
            );

            ViewBag.LoaiBiaId = new SelectList(
                _context.LoaiBia.ToList(),
                "MaLoaiBia",
                "TenLoaiBia",
                model?.LoaiBiaId
            );

            ViewBag.NgonNguId = new SelectList(
                _context.NgonNgus.ToList(),
                "MaNgonNgu",
                "TenNgonNgu",
                model?.NgonNguId
            );
        }
    }
}