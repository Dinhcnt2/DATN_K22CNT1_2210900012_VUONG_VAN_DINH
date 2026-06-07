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

            var role =
                HttpContext.Session
                    .GetString("VaiTro");

            if (string.IsNullOrEmpty(role)
                || role != "admin")
            {
                return RedirectToAction(
                    "DangNhap",
                    "TaiKhoan",
                    new { area = "" });
            }

            // ================= DASHBOARD =================

            // 📚 TỔNG SÁCH

            ViewBag.TongSach =
                _context.SanPhams.Count();

            // 📦 TỔNG ĐƠN

            ViewBag.TongDon =
                _context.DonHangs.Count();

            // 👤 TỔNG USER

            ViewBag.TongUser =
                _context.NguoiDungs.Count();

            // 💰 DOANH THU

            ViewBag.DoanhThu =
                _context.DonHangs

                .Where(x =>
                    x.TrangThaiThanhToan
                        == "DaThanhToan")

                .Sum(x =>
                    (decimal?)x.TongTien)

                ?? 0;

            // 📋 ĐƠN GẦN ĐÂY

            var donGanDay =
                _context.DonHangs

                .OrderByDescending(x =>
                    x.NgayDat)

                .Take(5)

                .ToList();

            // ⭐ ĐÁNH GIÁ GẦN ĐÂY

            ViewBag.DanhGiaGanDay =
                _context.DanhGia

                .Include(x =>
                    x.MaNguoiDungNavigation)

                .Include(x =>
                    x.SanPham)

                .OrderByDescending(x =>
                    x.NgayDanhGia)

                .Take(5)

                .ToList();

            // 🔥 SÁCH BÁN CHẠY

            ViewBag.SachBanChay =
                _context.ChiTietDonHangs

                .GroupBy(x =>
                    x.BienTheId)

                .Select(g => new
                {
                    BienTheId = g.Key,

                    SoLuongBan =
                        g.Sum(x => x.SoLuong)
                })

                .Join(
                    _context.BienTheSaches,

                    a => a.BienTheId,

                    b => b.Id,

                    (a, b) => new
                    {
                        a.SoLuongBan,

                        b.SanPhamId
                    }
                )

                .Join(
                    _context.SanPhams,

                    ab => ab.SanPhamId,

                    sp => sp.Id,

                    (ab, sp) => new
                    {
                        TenSach = sp.TenSach,

                        AnhBia = sp.AnhBia,

                        SoLuongBan = ab.SoLuongBan
                    }
                )

                .OrderByDescending(x =>
                    x.SoLuongBan)

                .Take(5)

                .ToList();

            // ================= RETURN =================

            return View(donGanDay);
        }
    }
}