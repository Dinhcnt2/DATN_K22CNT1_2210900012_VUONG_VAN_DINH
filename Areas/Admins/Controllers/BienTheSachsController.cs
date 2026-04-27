using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        // ===== INDEX =====
        public async Task<IActionResult> Index()
        {
            var data = _context.BienTheSaches
                .Include(x => x.SanPham)
                .Include(x => x.LoaiBia)
                .Include(x => x.NgonN);

            return View(await data.ToListAsync());
        }

        // ===== DETAILS =====
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.BienTheSaches
                .Include(x => x.SanPham)
                .Include(x => x.LoaiBia)
                .Include(x => x.NgonN)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null) return NotFound();

            return View(item);
        }

        // ===== CREATE GET =====
        public IActionResult Create()
        {
            LoadDropdown();
            return View();
        }

        // ===== CREATE POST =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BienTheSach model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdown(model);
                return View(model);
            }

            var exist = await _context.BienTheSaches.FirstOrDefaultAsync(x =>
                x.SanPhamId == model.SanPhamId &&
                x.LoaiBiaId == model.LoaiBiaId &&
                x.NgonNguId == model.NgonNguId
            );

            if (exist != null)
            {
                ModelState.AddModelError("", "❌ Biến thể đã tồn tại!");
                LoadDropdown(model);
                return View(model);
            }

            _context.BienTheSaches.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===== EDIT GET =====
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.BienTheSaches.FindAsync(id);
            if (item == null) return NotFound();

            LoadDropdown(item);
            return View(item);
        }

        // ===== EDIT POST =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BienTheSach model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                LoadDropdown(model);
                return View(model);
            }

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.BienTheSaches.Any(e => e.Id == model.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ===== DELETE GET =====
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.BienTheSaches
                .Include(x => x.SanPham)
                .Include(x => x.LoaiBia)
                .Include(x => x.NgonN)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null) return NotFound();

            return View(item);
        }

        // ===== DELETE POST =====
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.BienTheSaches.FindAsync(id);

            if (item != null)
            {
                _context.BienTheSaches.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ===== LOAD DROPDOWN =====
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