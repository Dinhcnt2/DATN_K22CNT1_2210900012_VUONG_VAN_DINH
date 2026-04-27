using Microsoft.AspNetCore.Mvc;
using VVD_2210900012_DATN.Models;
using System.Linq;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Controllers
{
    public class SachController : Controller
    {
        private readonly BookstoreContext _context;

        public SachController(BookstoreContext context)
        {
            _context = context;
        }

        // ===== 🔥 SÁCH BÁN CHẠY =====
        public IActionResult Index()
        {
            var data = _context.ChiTietDonHangs
                .GroupBy(x => x.BienTheId)
                .Select(g => new
                {
                    BienTheId = g.Key,
                    LuotBan = g.Sum(x => x.SoLuong)
                })
                .Join(_context.BienTheSaches,
                    a => a.BienTheId,
                    b => b.Id,
                    (a, b) => new { a.LuotBan, b.SanPhamId })
                .Join(_context.SanPhams,
                    ab => ab.SanPhamId,
                    sp => sp.Id,
                    (ab, sp) => new
                    {
                        Id = sp.Id,
                        TenSach = sp.TenSach,
                        AnhBia = sp.AnhBia,
                        Gia = sp.GiaSauGiam ?? sp.GiaGoc,
                        LuotBan = ab.LuotBan,
                        IsActive = sp.IsActive
                    })
                // 🔥 THÊM FILTER Ở ĐÂY (KHÔNG PHÁ CODE CŨ)
                .Where(x => x.IsActive == true)
                .OrderByDescending(x => x.LuotBan)
                .Take(10)
                .ToList();

            // nếu chưa có đơn → vẫn hiển thị
            if (!data.Any())
            {
                data = _context.SanPhams
                    .Select(x => new
                    {
                        Id = x.Id,
                        TenSach = x.TenSach,
                        AnhBia = x.AnhBia,
                        Gia = x.GiaSauGiam ?? x.GiaGoc,
                        LuotBan = 0,
                        IsActive = x.IsActive
                    })
                    // 🔥 THÊM FILTER Ở ĐÂY
                    .Where(x => x.IsActive == true)
                    .Take(10)
                    .ToList();
            }

            return View(data);
        }

        // ===== CHI TIẾT =====
        public IActionResult ChiTiet(int id)
        {
            var sach = _context.SanPhams
                .FirstOrDefault(x => x.Id == id);

            if (sach == null)
                return NotFound();

            if (sach.IsActive == false)
            {
                ViewBag.NgungBan = true;
            }

            return View(sach);
        }

        // ===== MUA NGAY =====
        public IActionResult MuaNgay(int id)
        {
            var sach = _context.SanPhams
                .FirstOrDefault(x => x.Id == id);

            if (sach == null)
                return NotFound();

            if (sach.IsActive == false)
            {
                return Content("❌ Sản phẩm đã ngừng bán!");
            }

            return RedirectToAction("MuaNgay", "GioHang", new { id = id });
        }

        // ===== SEARCH AJAX =====
        [HttpGet]
        public IActionResult TimKiemAjax(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return Json(new List<object>());

            keyword = keyword.ToLower();

            var data = _context.SanPhams
                .Where(x => x.TenSach.ToLower().Contains(keyword))
                .Select(x => new
                {
                    id = x.Id,
                    ten = x.TenSach,
                    anh = x.AnhBia ?? "no-image.png",
                    gia = x.GiaSauGiam ?? x.GiaGoc,
                    isActive = x.IsActive
                })
                .Take(5)
                .ToList();

            return Json(data);
        }
    }
}