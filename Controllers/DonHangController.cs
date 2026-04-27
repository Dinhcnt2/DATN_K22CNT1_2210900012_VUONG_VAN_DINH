using Microsoft.AspNetCore.Mvc;
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
            string qrUrl = $"https://img.vietqr.io/image/{bank}-{stk}-compact.png?amount={tien}&addInfo={maDon}&accountName={ten}";

            ViewBag.QR = qrUrl;
            ViewBag.MaDon = maDon;
            ViewBag.TongTien = tien;

            //FIX VIEW 
            return View("~/Views/GioHang/ThanhToan.cshtml");
        }
        public IActionResult ThanhCong(int? id)
        {
            if (id != null)
            {
                var don = _context.DonHangs.FirstOrDefault(x => x.MaDonHang == id);

                if (don != null)
                {
                    ViewBag.MaDon = don.MaDonHangCode ?? ("DH" + don.MaDonHang);
                    ViewBag.TongTien = don.TongTien ?? 0;
                }
            }

            return View();
        }


    }
}
