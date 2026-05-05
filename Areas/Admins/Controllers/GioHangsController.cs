using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // dùng cho dropdown
using Microsoft.EntityFrameworkCore;
using VVD_2210900012_DATN.Models;

namespace VVD_2210900012_DATN.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class GioHangsController : Controller
    {
        private readonly BookstoreContext _context;

        public GioHangsController(BookstoreContext context)
        {
            _context = context;
        }

        // ===== DANH SÁCH =====
        public async Task<IActionResult> Index()
        {
            var data = _context.GioHangs
                .Include(g => g.MaNguoiDungNavigation)
                .OrderByDescending(x => x.NgayTao);

            return View(await data.ToListAsync());
        }

        // ===== CHI TIẾT =====
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var gioHang = await _context.GioHangs
                .Include(g => g.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(x => x.MaGioHang == id);

            if (gioHang == null) return NotFound();

            return View(gioHang);
        }

        // ===== CREATE =====
        public IActionResult Create()
        {
            ViewBag.MaNguoiDung = _context.NguoiDungs
                .Select(x => new SelectListItem
                {
                    Value = x.MaNguoiDung.ToString(),
                    Text = x.HoTen
                }).ToList(); // 🔥 load dropdown user (tránh null)

            return View();
        }

        // ===== POST CREATE =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GioHang gioHang)
        {
            if (ModelState.IsValid)
            {
                gioHang.NgayTao = DateTime.Now; // tự set ngày tạo

                _context.Add(gioHang);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            //  nếu lỗi thì load lại dropdown 
            ViewBag.MaNguoiDung = _context.NguoiDungs
                .Select(x => new SelectListItem
                {
                    Value = x.MaNguoiDung.ToString(),
                    Text = x.HoTen
                }).ToList();

            return View(gioHang);
        }

        // ===== EDIT =====
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var gioHang = await _context.GioHangs.FindAsync(id);
            if (gioHang == null) return NotFound();

            ViewBag.MaNguoiDung = _context.NguoiDungs
                .Select(x => new SelectListItem
                {
                    Value = x.MaNguoiDung.ToString(),
                    Text = x.HoTen
                }).ToList(); // load dropdown
            return View(gioHang);
        }

        // ===== POST EDIT =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GioHang gioHang)
        {
            if (id != gioHang.MaGioHang)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gioHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.GioHangs.Any(x => x.MaGioHang == id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            //  nếu lỗi thì load lại dropdown
            ViewBag.MaNguoiDung = _context.NguoiDungs
                .Select(x => new SelectListItem
                {
                    Value = x.MaNguoiDung.ToString(),
                    Text = x.HoTen
                }).ToList();

            return View(gioHang);
        }

        // ===== XOÁ =====
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var gioHang = await _context.GioHangs
                .Include(g => g.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(x => x.MaGioHang == id);

            if (gioHang == null) return NotFound();

            return View(gioHang);
        }

        // ===== POST XOÁ =====
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gioHang = await _context.GioHangs.FindAsync(id);

            if (gioHang != null)
            {
                _context.GioHangs.Remove(gioHang);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
