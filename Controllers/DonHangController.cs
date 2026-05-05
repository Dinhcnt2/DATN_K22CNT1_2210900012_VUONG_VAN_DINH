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
            var don = _context.DonHangs
                .FirstOrDefault(x => x.MaDonHang == id);

            if (don == null)
                return NotFound();

            // ===== FIX NULL =====
            decimal tien = don.TongTien ?? 0;

            string maDon = string.IsNullOrEmpty(don.MaDonHangCode)
                ? "DH" + don.MaDonHang
                : don.MaDonHangCode;

            // ===== THÔNG TIN BANK =====
            string stk = "0976067728";
            string ten = "VUONG VAN DINH";
            string bank = "MB";

            // ===== QR =====
            string qrUrl =
                $"https://img.vietqr.io/image/{bank}-{stk}-compact.png?amount={tien}&addInfo={maDon}&accountName={ten}";

            ViewBag.QR = qrUrl;
            ViewBag.MaDon = maDon;
            ViewBag.TongTien = tien;

            // ===== FIX VIEW =====
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
                        don.MaDonHangCode ?? ("DH" + don.MaDonHang);

                    ViewBag.TongTien =
                        don.TongTien ?? 0;
                }
            }

            return View();
        }

        // ===== LỊCH SỬ ĐƠN HÀNG ĐÃ MUA =====
        public IActionResult DonHangDaMua(string status)
        {
            // ===== CHECK LOGIN =====
            var maNguoiDung =
                HttpContext.Session.GetInt32("MaNguoiDung");

            if (maNguoiDung == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // ===== LẤY ĐƠN HÀNG =====
            var query = _context.DonHangs
                .Include(x => x.ChiTietDonHangs)
                .ThenInclude(ct => ct.BienThe)
                .ThenInclude(bt => bt.SanPham)
                .Where(x => x.MaNguoiDung == maNguoiDung);

            // ===== FILTER TRẠNG THÁI =====
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.TrangThai == status);
            }

            // ===== ORDER =====
            var data = query
                .OrderByDescending(x => x.NgayDat)
                .ToList();
            return View(data);
        }
    }
}
