using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
