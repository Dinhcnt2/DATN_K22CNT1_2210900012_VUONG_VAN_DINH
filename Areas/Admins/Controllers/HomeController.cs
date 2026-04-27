using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VVD_2210900012_DATN.Models;
using System.Linq;

namespace VVD_2210900012_DATN.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class HomeController : Controller
    {
        private readonly BookstoreContext _context;

        public HomeController(BookstoreContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // 🔒 CHECK ADMIN
            var role = HttpContext.Session.GetString("VaiTro");

            if (string.IsNullOrEmpty(role) || role != "admin")
            {
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            // ===== DASHBOARD DATA =====

            // 📚 Tổng sách
            ViewBag.TongSach = _context.SanPhams.Count();

            // 📦 Tổng đơn
            ViewBag.TongDon = _context.DonHangs.Count();

            // 👤 Tổng user
            ViewBag.TongUser = _context.NguoiDungs.Count();

            // 💰 Doanh thu (chỉ tính đã thanh toán)
            ViewBag.DoanhThu = _context.DonHangs
                .Where(x => x.TrangThaiThanhToan == "DaThanhToan")
                .Sum(x => (decimal?)x.TongTien) ?? 0;

            // 📋 Đơn gần đây (GIỮ NGUYÊN)
            var donGanDay = _context.DonHangs
                .OrderByDescending(x => x.NgayDat)
                .Take(5)
                .ToList();

            // 🔥 THÊM ĐÁNH GIÁ GẦN ĐÂY (KHÔNG ẢNH HƯỞNG CODE CŨ)
            ViewBag.DanhGiaGanDay = _context.DanhGia
                .Include(x => x.MaNguoiDungNavigation)
                .Include(x => x.SanPham)
                .OrderByDescending(x => x.NgayDanhGia)
                .Take(5)
                .ToList();

            return View(donGanDay); // ❗ GIỮ NGUYÊN
        }
    }
}