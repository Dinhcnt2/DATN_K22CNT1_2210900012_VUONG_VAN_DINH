using Microsoft.AspNetCore.Mvc;
using VVD_2210900012_DATN.Models;
using System.Linq;

namespace VVD_2210900012_DATN.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly BookstoreContext _context;

        public TaiKhoanController(BookstoreContext context)
        {
            _context = context;
        }

        // ================= ĐĂNG NHẬP =================

        public IActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangNhap(string TenDangNhap, string MatKhau)
        {
            if (string.IsNullOrEmpty(TenDangNhap) || string.IsNullOrEmpty(MatKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            var user = _context.NguoiDungs
                .FirstOrDefault(x => x.TenDangNhap == TenDangNhap && x.MatKhau == MatKhau);

            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            HttpContext.Session.SetString("TenNguoiDung", user.TenDangNhap ?? "");
            HttpContext.Session.SetString("VaiTro", user.VaiTro ?? "khachhang");
            HttpContext.Session.SetInt32("UserId", user.MaNguoiDung);

            if (user.VaiTro == "admin")
            {
                return RedirectToAction("Index", "Home", new { area = "Admins" });
            }

            return RedirectToAction("Index", "Home");
        }

        // ================= ĐĂNG KÝ =================

        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(NguoiDung model)
        {
            if (model == null)
            {
                ViewBag.Error = "Dữ liệu không hợp lệ";
                return View();
            }

            if (string.IsNullOrEmpty(model.TenDangNhap) || string.IsNullOrEmpty(model.MatKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return View(model);
            }

            // 🔥 CHECK USERNAME TRÙNG (CODE CŨ)
            var check = _context.NguoiDungs
                .FirstOrDefault(x => x.TenDangNhap == model.TenDangNhap);

            if (check != null)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại";
                return View(model);
            }

            // ================= THÊM VALIDATE =================

            // 🔥 CHECK EMAIL PHẢI @gmail.com
            if (string.IsNullOrEmpty(model.Email) || !model.Email.EndsWith("@gmail.com"))
            {
                ViewBag.Error = "Email phải có dạng @gmail.com";
                return View(model);
            }

            // 🔥 CHECK EMAIL TRÙNG
            if (_context.NguoiDungs.Any(x => x.Email == model.Email))
            {
                ViewBag.Error = "Email đã tồn tại";
                return View(model);
            }

            // 🔥 CHECK SĐT TRÙNG
            if (_context.NguoiDungs.Any(x => x.Sdt == model.Sdt))
            {
                ViewBag.Error = "Số điện thoại đã tồn tại";
                return View(model);
            }

            // =================================================

            model.VaiTro = "khachhang";
            model.NgayTao = DateTime.Now;

            model.HoTen ??= "";
            model.Email ??= "";
            model.Sdt ??= "";

            _context.NguoiDungs.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Đăng ký thành công!";
            return RedirectToAction("DangNhap");
        }

        // ================= ĐĂNG XUẤT =================

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap");
        }

        // ================= QUÊN MẬT KHẨU =================

        public IActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        public IActionResult QuenMatKhau(string TenDangNhap)
        {
            var user = _context.NguoiDungs
                .FirstOrDefault(x => x.TenDangNhap == TenDangNhap);

            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản";
                return View();
            }

            var random = new Random();
            string code = random.Next(100000, 999999).ToString();

            user.ResetCode = code;
            user.ResetTime = DateTime.Now.AddMinutes(5);

            _context.SaveChanges();

            TempData["Code"] = code;

            return RedirectToAction("XacNhanMa", new { user = TenDangNhap });
        }

        // ================= XÁC NHẬN MÃ =================

        public IActionResult XacNhanMa(string user)
        {
            ViewBag.User = user;
            ViewBag.Code = TempData["Code"];
            return View();
        }

        [HttpPost]
        public IActionResult XacNhanMa(string TenDangNhap, string code)
        {
            var user = _context.NguoiDungs
                .FirstOrDefault(x => x.TenDangNhap == TenDangNhap);

            if (user == null)
                return RedirectToAction("QuenMatKhau");

            if (user.ResetCode != code || user.ResetTime < DateTime.Now)
            {
                ViewBag.Error = "Mã sai hoặc hết hạn";
                ViewBag.User = TenDangNhap;
                return View();
            }

            return RedirectToAction("DoiMatKhau", new { user = TenDangNhap });
        }

        // ================= ĐỔI MẬT KHẨU =================

        public IActionResult DoiMatKhau(string user)
        {
            ViewBag.User = user;
            return View();
        }

        [HttpPost]
        public IActionResult DoiMatKhau(string TenDangNhap, string MatKhauMoi)
        {
            var user = _context.NguoiDungs
                .FirstOrDefault(x => x.TenDangNhap == TenDangNhap);

            if (user == null)
                return RedirectToAction("QuenMatKhau");

            user.MatKhau = MatKhauMoi;
            user.ResetCode = null;

            _context.SaveChanges();

            return RedirectToAction("DangNhap");
        }
    }
}