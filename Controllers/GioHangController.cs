using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using VVD_2210900012_DATN.Models;
using System.Linq;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Controllers
{
    public class GioHangController : Controller
    {
        private readonly BookstoreContext _context;

        public GioHangController(BookstoreContext context)
        {
            _context = context;
        }

        // ===== CHECK LOGIN =====
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = HttpContext.Session.GetString("TenNguoiDung");

            if (string.IsNullOrEmpty(user))
            {
                context.Result =
                    new RedirectToActionResult(
                        "DangNhap",
                        "TaiKhoan",
                        null);

                return;
            }

            base.OnActionExecuting(context);
        }

        // ===== LẤY GIỎ HÀNG =====
        private List<GioHangItem> GetCart()
        {
            var session =
                HttpContext.Session.GetString("GioHang");

            if (!string.IsNullOrEmpty(session))
            {
                return JsonConvert.DeserializeObject<List<GioHangItem>>(session)
                       ?? new List<GioHangItem>();
            }

            return new List<GioHangItem>();
        }

        // ===== LƯU GIỎ =====
        private void SaveCart(List<GioHangItem> cart)
        {
            HttpContext.Session.SetString(
                "GioHang",
                JsonConvert.SerializeObject(cart));
        }

        // ===== GIỎ HÀNG =====
        public IActionResult Index()
        {
            var cart = GetCart();

            return View(cart);
        }

        // ===== MUA NGAY =====
        public IActionResult MuaNgay(int id)
        {
            var sach =
                _context.SanPhams
                .FirstOrDefault(x => x.Id == id);

            if (sach == null)
            {
                return NotFound();
            }

            var bienThe =
                _context.BienTheSaches
                .FirstOrDefault(x => x.SanPhamId == id);

            if (bienThe == null)
            {
                return Content("Không tìm thấy biến thể sách");
            }

            var cart = GetCart();

            var item =
                cart.FirstOrDefault(x => x.Id == bienThe.Id);

            decimal gia =
                sach.GiaSauGiam ?? sach.GiaGoc;

            if (item != null)
            {
                item.SoLuong++;
            }
            else
            {
                cart.Add(new GioHangItem
                {
                    Id = bienThe.Id,
                    TenSach = sach.TenSach,
                    Gia = gia,
                    SoLuong = 1,
                    Anh = sach.AnhBia ?? ""
                });
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        // ===== UPDATE AJAX =====
        [HttpPost]
        public IActionResult UpdateAjax(int id, int soluong)
        {
            var cart = GetCart();

            var item =
                cart.FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return Json(new
                {
                    itemTotal = 0,
                    cartTotal = 0
                });
            }

            item.SoLuong = soluong;

            SaveCart(cart);

            return Json(new
            {
                itemTotal =
                    item.Gia * item.SoLuong,

                cartTotal =
                    cart.Sum(x => x.Gia * x.SoLuong)
            });
        }

        // ===== XOÁ =====
        public IActionResult Xoa(int id)
        {
            var cart = GetCart();

            cart.RemoveAll(x => x.Id == id);

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        // ===== LOAD DANH SÁCH VOUCHER =====
        public IActionResult GetVoucherList()
        {
            var list =
                _context.Vouchers
                .Where(x => x.TrangThai == true)
                .ToList();

            return PartialView("_VoucherList", list);
        }

        // ===== CHỌN VOUCHER =====
        public IActionResult ChonVoucher(int id)
        {
            HttpContext.Session.SetInt32(
                "VoucherId",
                id);

            return Ok();
        }

        // ===== NHẬP CODE =====
        [HttpPost]
        public IActionResult NhapVoucher(string code)
        {
            var voucher =
                _context.Vouchers
                .FirstOrDefault(x =>
                    x.MaCode == code &&
                    x.TrangThai == true);

            if (voucher == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Voucher không tồn tại!"
                });
            }

            HttpContext.Session.SetInt32(
                "VoucherId",
                voucher.Id);

            return Json(new
            {
                success = true,
                message = "Áp dụng voucher thành công!"
            });
        }

        // ===== ĐẶT HÀNG =====
        [HttpPost]
        public IActionResult DatHang(
            string ten,
            string sdt,
            string diachi)
        {
            var cart = GetCart();

            if (!cart.Any())
            {
                return RedirectToAction("Index");
            }

            // ===== TÍNH TỔNG =====
            decimal tongTien =
                cart.Sum(x => x.Gia * x.SoLuong);

            // ===== LẤY VOUCHER =====
            int? voucherId =
                HttpContext.Session.GetInt32("VoucherId");

            Voucher? voucher = null;

            if (voucherId != null)
            {
                voucher =
                    _context.Vouchers
                    .FirstOrDefault(x => x.Id == voucherId);

                // ===== ÁP DỤNG GIẢM GIÁ =====
                if (voucher != null)
                {
                    decimal giamGia =
                        voucher.GiamGia ?? 0;

                    // ===== GIẢM PHẦN TRĂM =====
                    if (voucher.Loai == "PhanTram")
                    {
                        tongTien -=
                            tongTien * (giamGia / 100);
                    }
                    else
                    {
                        // ===== GIẢM TIỀN =====
                        tongTien -= giamGia;
                    }

                    // ===== CHỐNG ÂM =====
                    if (tongTien < 0)
                    {
                        tongTien = 0;
                    }

                    // ===== TRỪ LƯỢT =====
                    voucher.SoLuong -= 1;

                    _context.Vouchers.Update(voucher);
                }
            }

            // ===== USER =====
            var maNguoiDung =
                HttpContext.Session.GetInt32("MaNguoiDung");

            // ===== TẠO ĐƠN =====
            var don = new DonHang
            {
                MaDonHangCode =
                    "DH" + DateTime.Now.Ticks,

                HoTen = ten,

                SoDienThoai = sdt,

                DiaChi = diachi,

                TongTien = tongTien,

                TrangThai = "ChoXacNhan",

                TrangThaiThanhToan =
                    "ChuaThanhToan",

                NgayDat = DateTime.Now,

                VoucherId = voucher?.Id,

                MaNguoiDung = maNguoiDung
            };

            _context.DonHangs.Add(don);

            _context.SaveChanges();

            // ===== CHI TIẾT ĐƠN =====
            foreach (var item in cart)
            {
                var bienThe =
                    _context.BienTheSaches
                    .FirstOrDefault(x => x.Id == item.Id);

                if (bienThe != null)
                {
                    // ===== CHECK TỒN =====
                    if (bienThe.SoLuongTon < item.SoLuong)
                    {
                        TempData["Loi"] =
                            "Sản phẩm không đủ tồn kho";

                        return RedirectToAction("Index");
                    }

                    // ===== TRỪ KHO =====
                    bienThe.SoLuongTon -= item.SoLuong;

                    _context.ChiTietDonHangs.Add(
                        new ChiTietDonHang
                        {
                            MaDonHang = don.MaDonHang,

                            BienTheId = bienThe.Id,

                            SoLuong = item.SoLuong,

                            DonGia = item.Gia,

                            ThanhTien =
                                item.Gia * item.SoLuong
                        });
                }
            }

            _context.SaveChanges();

            // ===== XOÁ SESSION =====
            HttpContext.Session.Remove("GioHang");

            HttpContext.Session.Remove("VoucherId");

            // ===== CHUYỂN QR =====
            return RedirectToAction(
                "ThanhToan",
                "DonHang",
                new { id = don.MaDonHang });
        }
    }

    // ===== MODEL GIỎ =====
    public class GioHangItem
    {
        public int Id { get; set; }

        public string TenSach { get; set; } = "";

        public decimal Gia { get; set; }

        public int SoLuong { get; set; }

        public string Anh { get; set; } = "";
    }
}