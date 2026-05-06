using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VVD_2210900012_DATN.Models;
using System.Linq;

namespace VVD_2210900012_DATN.Controllers
{
    public class DonHangController : Controller
    {
        private readonly BookstoreContext _context;

        public DonHangController(BookstoreContext context)
        {
            _context = context;
        }

        // ===== TRANG THANH TOÁN QR =====
        public IActionResult ThanhToan(int id)
        {
            // ===== LẤY ĐƠN =====
            var don = _context.DonHangs
                .Include(x => x.Voucher)
                .FirstOrDefault(x => x.MaDonHang == id);

            if (don == null)
            {
                return NotFound();
            }

            // ===== TỔNG TIỀN =====
            decimal tien = don.TongTien ?? 0;

            // ===== FIX VOUCHER CŨ =====
            // Nếu DB chưa trừ voucher thì trừ lại
            if (don.Voucher != null)
            {
                decimal tongGoc = 0;

                // ===== TÍNH LẠI TỔNG GỐC =====
                var chiTiet = _context.ChiTietDonHangs
                    .Where(x => x.MaDonHang == don.MaDonHang)
                    .ToList();

                tongGoc = chiTiet.Sum(x => x.ThanhTien);

                decimal giamGia =
                    don.Voucher.GiamGia ?? 0;

                // ===== GIẢM PHẦN TRĂM =====
                if (!string.IsNullOrEmpty(don.Voucher.Loai) &&
                (
                    don.Voucher.Loai.ToLower().Contains("phan") ||
                    don.Voucher.Loai.ToLower().Contains("%")
                ))
                {
                    tien =
                        tongGoc -
                        (tongGoc * giamGia / 100);
                }
                else
                {
                    // ===== GIẢM TIỀN =====
                    tien =
                        tongGoc - giamGia;
                }

                // ===== CHỐNG ÂM =====
                if (tien < 0)
                {
                    tien = 0;
                }
            }

            // ===== MÃ ĐƠN =====
            string maDon =
                string.IsNullOrEmpty(don.MaDonHangCode)
                ? "DH" + don.MaDonHang
                : don.MaDonHangCode;

            // ===== THÔNG TIN BANK =====
            string stk = "0976067728";

            string ten = "VUONG VAN DINH";

            string bank = "MB";

            // ===== QR =====
            string qrUrl =
                $"https://img.vietqr.io/image/{bank}-{stk}-compact.png?amount={tien}&addInfo={maDon}&accountName={ten}";

            // ===== VIEWBAG =====
            ViewBag.QR = qrUrl;

            ViewBag.MaDon = maDon;

            ViewBag.TongTien = tien;

            // ===== VIEW =====
            return View("~/Views/GioHang/ThanhToan.cshtml");
        }

        // ===== THANH CÔNG =====
        public IActionResult ThanhCong(int? id)
        {
            if (id != null)
            {
                var don = _context.DonHangs
                    .FirstOrDefault(x => x.MaDonHang == id);

                if (don != null)
                {
                    ViewBag.MaDon =
                        don.MaDonHangCode ??
                        ("DH" + don.MaDonHang);

                    ViewBag.TongTien =
                        don.TongTien ?? 0;
                }
            }

            return View();
        }

        // ===== ĐƠN HÀNG ĐÃ MUA =====
        public IActionResult DonHangDaMua(string status)
        {
            // ===== CHECK LOGIN =====
            var maNguoiDung =
                HttpContext.Session.GetInt32("MaNguoiDung");

            if (maNguoiDung == null)
            {
                return RedirectToAction(
                    "DangNhap",
                    "TaiKhoan");
            }

            // ===== QUERY =====
            var query = _context.DonHangs
                .Include(x => x.ChiTietDonHangs)
                .ThenInclude(ct => ct.BienThe)
                .ThenInclude(bt => bt.SanPham)
                .Where(x => x.MaNguoiDung == maNguoiDung);

            // ===== FILTER =====
            if (!string.IsNullOrEmpty(status))
            {
                query =
                    query.Where(x => x.TrangThai == status);
            }

            // ===== ORDER =====
            var data = query
                .OrderByDescending(x => x.NgayDat)
                .ToList();

            return View(data);
        }

        // ===== CHI TIẾT ĐƠN =====
        public IActionResult ChiTiet(int id)
        {
            // ===== CHECK LOGIN =====
            var maNguoiDung =
                HttpContext.Session.GetInt32("MaNguoiDung");

            if (maNguoiDung == null)
            {
                return RedirectToAction(
                    "DangNhap",
                    "TaiKhoan");
            }

            // ===== LẤY ĐƠN =====
            var donHang = _context.DonHangs
                .Include(x => x.ChiTietDonHangs)
                    .ThenInclude(ct => ct.BienThe)
                        .ThenInclude(bt => bt.SanPham)
                .FirstOrDefault(x =>
                    x.MaDonHang == id &&
                    x.MaNguoiDung == maNguoiDung);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }
    }
}