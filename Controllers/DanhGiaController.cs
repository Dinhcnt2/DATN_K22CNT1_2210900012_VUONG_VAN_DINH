using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VVD_2210900012_DATN.Models;
using System.Linq;

namespace VVD_2210900012_DATN.Controllers
{
    public class DanhGiaController : Controller
    {
        private readonly BookstoreContext _context;

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public DanhGiaController(BookstoreContext context)
        {
            _context = context;
        }

        // =====================================================
        // LOAD DANH SÁCH ĐÁNH GIÁ
        // =====================================================

        public IActionResult List(int sanPhamId)
        {
            var list = _context.DanhGia

                .Include(x => x.MaNguoiDungNavigation)

                .Where(x => x.SanPhamId == sanPhamId)

                .OrderByDescending(x => x.NgayDanhGia)

                .ToList();

            return PartialView(
                "_DanhGiaList",
                list
            );
        }

        // =====================================================
        // FORM ĐÁNH GIÁ
        // =====================================================

        [HttpGet]
        public IActionResult Create(int sanPhamId)
        {
            // ===== CHECK LOGIN =====

            var tenNguoiDung =
                HttpContext.Session
                    .GetString("TenNguoiDung");

            if (string.IsNullOrEmpty(tenNguoiDung))
            {
                return RedirectToAction(
                    "DangNhap",
                    "TaiKhoan"
                );
            }

            // ===== USER =====

            var user = _context.NguoiDungs

                .FirstOrDefault(x =>
                    x.TenDangNhap
                        == tenNguoiDung);

            if (user == null)
            {
                return RedirectToAction(
                    "DangNhap",
                    "TaiKhoan"
                );
            }

            // ===== SẢN PHẨM =====

            var sanPham = _context.SanPhams

                .FirstOrDefault(x =>
                    x.Id == sanPhamId);

            if (sanPham == null)
            {
                return NotFound();
            }

            // =================================================
            // CHECK ĐÃ MUA
            // =================================================

            var daMua = _context.ChiTietDonHangs

                .Include(x => x.MaDonHangNavigation)

                .Include(x => x.BienThe)

                .Any(x =>

                    x.MaDonHangNavigation != null

                    &&

                    x.MaDonHangNavigation.MaNguoiDung
                        == user.MaNguoiDung

                    &&

                    x.MaDonHangNavigation.TrangThai
                        != "DaHuy"

                    &&

                    x.BienThe != null

                    &&

                    x.BienThe.SanPhamId == sanPhamId
                );

            // ===== CHƯA MUA =====

            if (!daMua)
            {
                TempData["Error"] =
                    "Bạn phải mua sản phẩm mới được đánh giá!";

                return RedirectToAction(
                    "DonHangDaMua",
                    "DonHang"
                );
            }

            // =================================================
            // CHECK ĐÃ ĐÁNH GIÁ
            // =================================================

            var daDanhGia = _context.DanhGia

                .Any(x =>

                    x.SanPhamId
                        == sanPhamId

                    &&

                    x.MaNguoiDung
                        == user.MaNguoiDung
                );

            if (daDanhGia)
            {
                TempData["Error"] =
                    "Bạn đã đánh giá sản phẩm này rồi!";

                return RedirectToAction(
                    "DonHangDaMua",
                    "DonHang"
                );
            }

            // ===== VIEWBAG =====

            ViewBag.SanPhamId =
                sanPhamId;

            ViewBag.TenSach =
                sanPham.TenSach;

            ViewBag.AnhBia =
                sanPham.AnhBia;

            return View();
        }

        // =====================================================
        // CREATE ĐÁNH GIÁ
        // =====================================================

        [HttpPost]
        public IActionResult Create(
            int sanPhamId,
            int soSao,
            string noiDung)
        {
            // ===== LOGIN =====

            var tenNguoiDung =
                HttpContext.Session
                    .GetString("TenNguoiDung");

            if (string.IsNullOrEmpty(tenNguoiDung))
            {
                TempData["Error"] =
                    "Vui lòng đăng nhập!";

                return RedirectToAction(
                    "DangNhap",
                    "TaiKhoan"
                );
            }

            // ===== USER =====

            var user = _context.NguoiDungs

                .FirstOrDefault(x =>
                    x.TenDangNhap
                        == tenNguoiDung);

            if (user == null)
            {
                TempData["Error"] =
                    "Không tìm thấy tài khoản!";

                return RedirectToAction(
                    "DangNhap",
                    "TaiKhoan"
                );
            }

            // =================================================
            // CHECK ĐÃ MUA
            // =================================================

            var daMua = _context.ChiTietDonHangs

                .Include(x => x.MaDonHangNavigation)

                .Include(x => x.BienThe)

                .Any(x =>

                    x.MaDonHangNavigation != null

                    &&

                    x.MaDonHangNavigation.MaNguoiDung
                        == user.MaNguoiDung

                    &&

                    x.MaDonHangNavigation.TrangThai
                        != "DaHuy"

                    &&

                    x.BienThe != null

                    &&

                    x.BienThe.SanPhamId == sanPhamId
                );

            if (!daMua)
            {
                TempData["Error"] =
                    "Bạn phải mua sản phẩm mới được đánh giá!";

                return RedirectToAction(
                    "DonHangDaMua",
                    "DonHang"
                );
            }

            // =================================================
            // CHECK ĐÃ ĐÁNH GIÁ
            // =================================================

            var daDanhGia = _context.DanhGia

                .Any(x =>

                    x.SanPhamId
                        == sanPhamId

                    &&

                    x.MaNguoiDung
                        == user.MaNguoiDung
                );

            if (daDanhGia)
            {
                TempData["Error"] =
                    "Bạn đã đánh giá sản phẩm này rồi!";

                return RedirectToAction(
                    "DonHangDaMua",
                    "DonHang"
                );
            }

            // ===== VALIDATE =====

            if (soSao <= 0)
            {
                TempData["Error"] =
                    "Vui lòng chọn số sao!";

                return RedirectToAction(
                    "Create",
                    new { sanPhamId = sanPhamId });
            }

            if (string.IsNullOrEmpty(noiDung))
            {
                TempData["Error"] =
                    "Vui lòng nhập nội dung đánh giá!";

                return RedirectToAction(
                    "Create",
                    new { sanPhamId = sanPhamId });
            }

            // =================================================
            // TẠO ĐÁNH GIÁ
            // =================================================

            var dg = new DanhGia
            {
                SanPhamId = sanPhamId,

                MaNguoiDung =
                    user.MaNguoiDung,

                SoSao = soSao,

                NoiDung = noiDung,

                NgayDanhGia =
                    DateTime.Now
            };

            // ===== SAVE =====

            _context.DanhGia.Add(dg);

            _context.SaveChanges();

            TempData["Success"] =
                "Đánh giá thành công!";

            // ===== REDIRECT =====

            return RedirectToAction(
                "ChiTiet",
                "Sach",
                new { id = sanPhamId });
        }
    }
}