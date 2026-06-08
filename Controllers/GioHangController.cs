using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using VVD_2210900012_DATN.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;

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

        public override void OnActionExecuting(
            ActionExecutingContext context)
        {
            var user =
                HttpContext.Session
                    .GetString("TenNguoiDung");

            // 🔥 CHƯA LOGIN

            if (string.IsNullOrEmpty(user))
            {
                context.Result =

                    new RedirectToActionResult(
                        "DangNhap",
                        "TaiKhoan",
                        null
                    );

                return;
            }

            base.OnActionExecuting(context);
        }

        // ===== LẤY GIỎ HÀNG =====

        private List<GioHangItem> GetCart()
        {
            var session =

                HttpContext.Session
                    .GetString("GioHang");

            // 🔥 ĐÃ CÓ SESSION

            if (!string.IsNullOrEmpty(session))
            {
                return JsonConvert
                    .DeserializeObject<List<GioHangItem>>(session)

                    ?? new List<GioHangItem>();
            }

            // 🔥 CHƯA CÓ

            return new List<GioHangItem>();
        }

        // ===== LƯU GIỎ =====

        private void SaveCart(
            List<GioHangItem> cart)
        {
            HttpContext.Session.SetString(

                "GioHang",

                JsonConvert.SerializeObject(cart)
            );
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
            // 🔥 TÌM BIẾN THỂ


            var bienThe =
                _context.BienTheSaches
                .FirstOrDefault(x => x.Id == id);

            // 🔥 KHÔNG TỒN TẠI

            if (bienThe == null)
            {
                return Content(
                    "❌ Không tìm thấy biến thể sách");
            }

            // 🔥 TÌM SÁCH

            var sach =
                _context.SanPhams
                .FirstOrDefault(x =>
                    x.Id == bienThe.SanPhamId);

            if (sach == null)
            {
                return NotFound();
            }

            // 🔥 CHECK NGỪNG BÁN

            if (sach.IsActive == false)
            {
                return Content(
                    "❌ Sản phẩm đã ngừng bán!");
            }

            // 🔥 HẾT HÀNG

            if (bienThe.SoLuongTon <= 0)
            {
                return Content(
                    "❌ Sản phẩm đã hết hàng");
            }

            // 🔥 LẤY GIỎ

            var cart = GetCart();

            // 🔥 TÌM ITEM

            var item =
                cart.FirstOrDefault(x =>
                    x.Id == bienThe.Id);

            // 🔥 GIÁ BÁN

            decimal gia =
                bienThe.GiaBan.HasValue
                && bienThe.GiaBan.Value > 0

                ?

                bienThe.GiaBan.Value

                :

                (sach.GiaSauGiam
                    ?? sach.GiaGoc);

            // 🔥 ĐÃ CÓ → TĂNG

            if (item != null)
            {
                if (item.SoLuong + 1 >
                    bienThe.SoLuongTon)
                {
                    return Content(
                        "❌ Vượt quá tồn kho");
                }

                item.SoLuong++;
            }

            // 🔥 CHƯA CÓ → ADD

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

            // 🔥 LƯU

            SaveCart(cart);

            return RedirectToAction("Index");


        }


        // ===== UPDATE AJAX =====

        [HttpPost]

        public IActionResult UpdateAjax(
            int id,
            int soluong)
        {
            var cart = GetCart();

            var item =
                cart.FirstOrDefault(x =>
                    x.Id == id);

            // 🔥 KHÔNG TÌM THẤY

            if (item == null)
            {
                return Json(new
                {
                    success = false,

                    message =
                        "Không tìm thấy sản phẩm",

                    itemTotal = 0,

                    cartTotal = 0
                });
            }

            // 🔥 CHECK <=0

            if (soluong <= 0)
            {
                return Json(new
                {
                    success = false,

                    message =
                        "Số lượng không hợp lệ"
                });
            }

            // 🔥 CHECK TỒN

            var bienThe =
                _context.BienTheSaches
                .FirstOrDefault(x =>
                    x.Id == id);

            if (bienThe != null
                && soluong > bienThe.SoLuongTon)
            {
                return Json(new
                {
                    success = false,

                    message =
                        "Vượt quá số lượng tồn kho"
                });
            }

            // 🔥 UPDATE

            item.SoLuong = soluong;

            SaveCart(cart);

            return Json(new
            {
                success = true,

                itemTotal =
                    item.Gia * item.SoLuong,

                cartTotal =
                    cart.Sum(x =>
                        x.Gia * x.SoLuong)
            });
        }

        // ===== XOÁ =====

        public IActionResult Xoa(int id)
        {
            var cart = GetCart();

            cart.RemoveAll(x =>
                x.Id == id);

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        // ===== LOAD VOUCHER =====

        public IActionResult GetVoucherList()
        {
            var list =
                _context.Vouchers

                .Where(x =>
                    x.TrangThai == true)

                .ToList();

            return PartialView(
                "_VoucherList",
                list);
        }

        // ===== CHỌN VOUCHER =====

        public IActionResult ChonVoucher(int id)
        {
            // 🛡️ CHẶN KHI CLICK CHỌN VOUCHER TỪ DANH SÁCH (Dưới 50k không cho chọn)
            var cart = GetCart();
            decimal tongTienTruocGiam = cart.Sum(x => x.Gia * x.SoLuong);

            if (tongTienTruocGiam < 50000)
            {
                return BadRequest("Đơn hàng tối thiểu từ 50.000đ trở lên mới được áp dụng Voucher!");
            }

            HttpContext.Session.SetInt32(
                "VoucherId",
                id);

            return Ok();
        }

        // ===== NHẬP CODE =====

        [HttpPost]

        public IActionResult NhapVoucher(
            string code)
        {
            // 🛡️ CHẶN KHI GÕ CODE BẰNG TAY (Dưới 50k không cho áp dụng)
            var cart = GetCart();
            decimal tongTienTruocGiam = cart.Sum(x => x.Gia * x.SoLuong);

            if (tongTienTruocGiam < 50000)
            {
                return Json(new
                {
                    success = false,
                    message = "Đơn hàng tối thiểu từ 50.000đ trở lên mới được áp dụng Voucher!"
                });
            }

            var voucher =
                _context.Vouchers

                .FirstOrDefault(x =>

                    x.MaCode == code

                    &&

                    x.TrangThai == true
                );

            // 🔥 KHÔNG TỒN TẠI

            if (voucher == null)
            {
                return Json(new
                {
                    success = false,

                    message =
                        "Voucher không tồn tại!"
                });
            }

            // 🔥 HẾT LƯỢT

            if (voucher.SoLuong <= 0)
            {
                return Json(new
                {
                    success = false,

                    message =
                        "Voucher đã hết lượt sử dụng!"
                });
            }

            HttpContext.Session.SetInt32(
                "VoucherId",
                voucher.Id);

            return Json(new
            {
                success = true,

                message =
                    "Áp dụng voucher thành công!"
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

            // 🔥 GIỎ RỖNG

            if (!cart.Any())
            {
                TempData["Loi"] =
                    "Giỏ hàng đang trống";

                return RedirectToAction(
                    "Index");
            }

            // ===== TÍNH TỔNG =====

            decimal tongTien =
                cart.Sum(x =>
                    x.Gia * x.SoLuong);

            // ===== LẤY VOUCHER =====

            int? voucherId =
                HttpContext.Session
                    .GetInt32("VoucherId");

            Voucher? voucher = null;

            if (voucherId != null)
            {
                // 🛡️ CHẶN CUỐI: Nếu tổng tiền sản phẩm thực tế lúc đặt hàng dưới 50k (do người dùng sửa giảm số lượng trước đó)
                if (tongTien < 50000)
                {
                    HttpContext.Session.Remove("VoucherId"); // Xóa voucher khỏi phiên làm việc
                    voucherId = null; // Huỷ bỏ giảm giá đơn hàng này
                }
                else
                {
                    voucher =
                        _context.Vouchers
                        .FirstOrDefault(x =>
                            x.Id == voucherId);

                    // ===== GIẢM GIÁ =====

                    if (voucher != null)
                    {
                        decimal giamGia =
                            voucher.GiamGia ?? 0;

                        // 🔥 %

                        if (voucher.Loai
                            == "PhanTram")
                        {
                            tongTien -=
                                tongTien
                                * (giamGia / 100);
                        }

                        // 🔥 TIỀN

                        else
                        {
                            tongTien -=
                                giamGia;
                        }

                        // 🔥 CHỐNG ÂM

                        if (tongTien < 0)
                        {
                            tongTien = 0;
                        }

                        // 🔥 TRỪ LƯỢT

                        voucher.SoLuong -= 1;

                        _context.Vouchers
                            .Update(voucher);
                    }
                }
            }

            // ===== USER =====

            var maNguoiDung =
                HttpContext.Session
                    .GetInt32("MaNguoiDung");

            // ===== TẠO ĐƠN =====

            var don = new DonHang
            {
                MaDonHangCode =
                    "DH" + DateTime.Now.Ticks,

                HoTen = ten,

                SoDienThoai = sdt,

                DiaChi = diachi,

                TongTien = tongTien,

                TrangThai =
                    "ChoXacNhan",

                TrangThaiThanhToan =
                    "ChuaThanhToan",

                NgayDat = DateTime.Now,

                VoucherId = voucher?.Id,

                MaNguoiDung = maNguoiDung
            };

            _context.DonHangs.Add(don);

            _context.SaveChanges();

            // ===== CHI TIẾT =====

            foreach (var item in cart)
            {
                var bienThe =
                    _context.BienTheSaches
                    .FirstOrDefault(x =>
                        x.Id == item.Id);

                if (bienThe != null)
                {
                    //  KHÔNG ĐỦ KHO

                    if (bienThe.SoLuongTon
                        < item.SoLuong)
                    {
                        TempData["Loi"] =
                            "Sản phẩm không đủ tồn kho";

                        return RedirectToAction(
                            "Index");
                    }

                    // 🔥 TRỪ KHO

                    bienThe.SoLuongTon -=
                        item.SoLuong;

                    _context.ChiTietDonHangs.Add(

                        new ChiTietDonHang
                        {
                            MaDonHang =
                                don.MaDonHang,

                            BienTheId =
                                bienThe.Id,

                            SoLuong =
                                item.SoLuong,

                            DonGia =
                                item.Gia,

                            ThanhTien =
                                item.Gia
                                * item.SoLuong
                        });
                }
            }

            _context.SaveChanges();

            // ===== XOÁ SESSION =====

            HttpContext.Session.Remove(
                "GioHang");

            HttpContext.Session.Remove(
                "VoucherId");

            // ===== QR =====

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

        public string TenSach { get; set; }
            = "";

        public decimal Gia { get; set; }

        public int SoLuong { get; set; }

        public string Anh { get; set; }
            = "";
    }
}