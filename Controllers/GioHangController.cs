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
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", null);
                return;
            }

            base.OnActionExecuting(context);
        }

        // ===== LẤY GIỎ =====
        private List<GioHangItem> GetCart()
        {
            var session = HttpContext.Session.GetString("GioHang");

            if (!string.IsNullOrEmpty(session))
            {
                return JsonConvert.DeserializeObject<List<GioHangItem>>(session)
                       ?? new List<GioHangItem>();
            }

            return new List<GioHangItem>();
        }

        private void SaveCart(List<GioHangItem> cart)
        {
            HttpContext.Session.SetString("GioHang", JsonConvert.SerializeObject(cart));
        }

        // ===== MUA NGAY =====
        public IActionResult MuaNgay(int id)
        {
            var sach = _context.SanPhams.FirstOrDefault(x => x.Id == id);// Lấy sản phẩm

            if (sach == null || sach.IsActive == false) // check sản phẩm ngừng bán
            {
                return Content("Sản phẩm đã ngừng bán");
            }

            var bienThe = _context.BienTheSaches// lấy biến thể
                .FirstOrDefault(x => x.SanPhamId == id);

            if (bienThe == null)// tạo biến thể
            {
                var loaiBia = _context.LoaiBia.First();
                var ngonNgu = _context.NgonNgus.First();

                bienThe = new BienTheSach
                {
                    SanPhamId = id,
                    LoaiBiaId = loaiBia.MaLoaiBia,
                    NgonNguId = ngonNgu.MaNgonNgu,
                    SoLuongTon = 100
                };

                _context.BienTheSaches.Add(bienThe);
                _context.SaveChanges();
            }

            var cart = GetCart();// lấy giỏ hàng
            var item = cart.FirstOrDefault(x => x.Id == bienThe.Id); //check tồn tại

            decimal gia = sach.GiaSauGiam ?? sach.GiaGoc;//tính giá

            if (item != null)// tính số lượng
                item.SoLuong++;
            else
            {
                cart.Add(new GioHangItem
                {
                    Id = bienThe.Id,
                    TenSach = sach.TenSach ?? "",
                    Gia = gia,
                    SoLuong = 1,
                    Anh = sach.AnhBia ?? ""
                });
            }
            SaveCart(cart);
            return RedirectToAction("Index");// chuyển trang index
        }

        // ===== GIỎ =====
        public IActionResult Index()
        {
            var cart = GetCart();

            var invalidItems = new List<GioHangItem>();

            foreach (var item in cart)
            {
                var bienThe = _context.BienTheSaches.FirstOrDefault(x => x.Id == item.Id);

                if (bienThe == null)
                {
                    invalidItems.Add(item);
                    continue;
                }

                var sp = _context.SanPhams.FirstOrDefault(x => x.Id == bienThe.SanPhamId);

                if (sp == null || sp.IsActive == false) // sản phẩm bị xoá mềm
                {
                    invalidItems.Add(item);
                }
            }

            foreach (var i in invalidItems)
            {
                cart.Remove(i);
            }

            SaveCart(cart);

            return View(cart);
        }

        // ===== UPDATE =====
        [HttpPost]
        public IActionResult UpdateAjax(int id, int soluong)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.Id == id);

            if (item == null)
                return Json(new { itemTotal = 0, cartTotal = 0 });

            item.SoLuong = soluong;
            SaveCart(cart);

            return Json(new
            {
                itemTotal = item.Gia * item.SoLuong,
                cartTotal = cart.Sum(x => x.Gia * x.SoLuong)
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

        // ===== LOAD DANH SÁCH VOUCHER 
        public IActionResult GetVoucherList()
        {
            var list = _context.Vouchers
                .Where(x => x.TrangThai == true) // chỉ lấy voucher đang hoạt động
                .ToList();

            return PartialView("_VoucherList", list);
        }

        // ===== CHỌN VOUCHER =====
        public IActionResult ChonVoucher(int id)
        {
            HttpContext.Session.SetInt32("VoucherId", id);
            return Ok(); // AJAX gọi nên trả OK
        }

        // ===== NHẬP CODE =====
        [HttpPost]
        public IActionResult NhapVoucher(string code)
        {
            var voucher = _context.Vouchers
                .FirstOrDefault(x => x.MaCode == code && x.TrangThai == true);

            if (voucher == null)
                return Json(new { success = false, message = "Voucher không tồn tại!" });

            if (voucher.NgayKetThuc < DateTime.Now)
                return Json(new { success = false, message = "Voucher đã hết hạn!" });

            if (voucher.NgayBatDau > DateTime.Now)
                return Json(new { success = false, message = "Voucher chưa bắt đầu!" });

            if (voucher.SoLuong <= 0)
                return Json(new { success = false, message = "Voucher đã hết lượt!" });

            HttpContext.Session.SetInt32("VoucherId", voucher.Id);

            return Json(new { success = true, message = "Áp dụng thành công!" });
        }

        // ===== ĐẶT HÀNG =====
        [HttpPost]
        public IActionResult DatHang(string ten, string sdt, string diachi)
        {
            var cart = GetCart();

            if (!cart.Any())
                return RedirectToAction("Index");

            decimal tongTien = cart.Sum(x => x.Gia * x.SoLuong);

            int? voucherId = HttpContext.Session.GetInt32("VoucherId");

            Voucher? voucher = null;

            if (voucherId != null)
            {
                var v = _context.Vouchers.Find(voucherId);

                if (v != null &&
                    v.TrangThai == true &&
                    v.NgayKetThuc >= DateTime.Now &&
                    v.NgayBatDau <= DateTime.Now &&
                    v.SoLuong > 0)
                {
                    voucher = v;
                }
                else
                {
                    HttpContext.Session.Remove("VoucherId"); // xoá nếu không hợp lệ
                }
            }

            if (voucher != null)
            {
                decimal giamGia = voucher.GiamGia ?? 0;

                if (voucher.Loai == "PhanTram")
                    tongTien -= tongTien * (giamGia / 100);
                else
                    tongTien -= giamGia;

                if (tongTien < 0) tongTien = 0;

                voucher.SoLuong -= 1; // trừ lượt
                _context.Vouchers.Update(voucher);
            }

            var maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");

            var don = new DonHang
            {
                MaDonHangCode = "DH" + DateTime.Now.Ticks,

                HoTen = ten,

                SoDienThoai = sdt,

                DiaChi = diachi,

                TongTien = tongTien,

                TrangThai = "ChoXacNhan",

                TrangThaiThanhToan = "ChuaThanhToan",

                NgayDat = DateTime.Now,

                VoucherId = voucher?.Id,

                // ===== FIX LỊCH SỬ MUA HÀNG =====
                MaNguoiDung = maNguoiDung
            };


            _context.DonHangs.Add(don);
            _context.SaveChanges();

            foreach (var item in cart)
            {
                var bienThe = _context.BienTheSaches.FirstOrDefault(x => x.Id == item.Id);

                if (bienThe != null)
                {
                    bienThe.SoLuongTon -= item.SoLuong;

                    _context.ChiTietDonHangs.Add(new ChiTietDonHang
                    {
                        MaDonHang = don.MaDonHang,
                        BienTheId = bienThe.Id,
                        SoLuong = item.SoLuong,
                        DonGia = item.Gia,
                        ThanhTien = item.Gia * item.SoLuong
                    });
                }
            }
            _context.SaveChanges();

            HttpContext.Session.Remove("GioHang");
            HttpContext.Session.Remove("VoucherId");

            return RedirectToAction("ThanhToan", "DonHang", new { id = don.MaDonHang });
        }
    }

    public class GioHangItem
    {
        public int Id { get; set; }
        public string TenSach { get; set; } = "";
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }
        public string Anh { get; set; } = "";
    }
}
