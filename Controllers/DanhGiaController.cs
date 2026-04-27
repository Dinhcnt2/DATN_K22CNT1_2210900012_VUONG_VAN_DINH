using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VVD_2210900012_DATN.Models;
using System.Linq;

namespace VVD_2210900012_DATN.Controllers
{
    public class DanhGiaController : Controller
    {
        private readonly BookstoreContext _context;

        // 🔥 THÊM CONSTRUCTOR (MÀY ĐANG THIẾU)
        public DanhGiaController(BookstoreContext context)
        {
            _context = context;
        }

        // ===== LOAD LIST =====
        public IActionResult List(int sanPhamId)
        {
            var list = _context.DanhGia
                .Include(x => x.MaNguoiDungNavigation)
                .Where(x => x.SanPhamId == sanPhamId)
                .OrderByDescending(x => x.NgayDanhGia)
                .ToList();

            return PartialView("_DanhGiaList", list);
        }

        // ===== CREATE =====
        [HttpPost]
        public IActionResult Create(int sanPhamId, int soSao, string noiDung)
        {
            var tenNguoiDung = HttpContext.Session.GetString("TenNguoiDung");

            if (string.IsNullOrEmpty(tenNguoiDung))
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập!" });
            }

            var user = _context.NguoiDungs
                .FirstOrDefault(x => x.TenDangNhap == tenNguoiDung);

            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy user!" });
            }

            var dg = new DanhGia
            {
                SanPhamId = sanPhamId,
                MaNguoiDung = user.MaNguoiDung,
                SoSao = soSao,
                NoiDung = noiDung,
                NgayDanhGia = DateTime.Now
            };

            _context.DanhGia.Add(dg);
            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}