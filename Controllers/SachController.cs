using Microsoft.AspNetCore.Mvc;
using VVD_2210900012_DATN.Models;
using System.Linq;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Controllers
{
    public class SachController : Controller
    {
        private readonly BookstoreContext _context;

        public SachController(BookstoreContext context)
        {
            _context = context;
        }

        // ===== 🔥 TRANG CHỦ =====

        public IActionResult Index()
        {
            var data = _context.ChiTietDonHangs

                .GroupBy(x => x.BienTheId)

                .Select(g => new
                {
                    BienTheId = g.Key,

                    LuotBan =
                        g.Sum(x => x.SoLuong)
                })

                .Join(
                    _context.BienTheSaches,

                    a => a.BienTheId,

                    b => b.Id,

                    (a, b) => new
                    {
                        a.LuotBan,

                        b.SanPhamId
                    }
                )

                .Join(
                    _context.SanPhams,

                    ab => ab.SanPhamId,

                    sp => sp.Id,

                    (ab, sp) => new
                    {
                        Id = sp.Id,

                        TenSach = sp.TenSach,

                        AnhBia = sp.AnhBia,

                        Gia =
                            sp.GiaSauGiam
                            ?? sp.GiaGoc,

                        LuotBan = ab.LuotBan,

                        IsActive = sp.IsActive
                    }
                )

                .Where(x => x.IsActive == true)

                .OrderByDescending(x =>
                    x.LuotBan)

                .Take(10)

                .ToList();

            // ===== NẾU CHƯA CÓ ĐƠN =====

            if (!data.Any())
            {
                data = _context.SanPhams

                    .Where(x =>
                        x.IsActive == true)

                    .Select(x => new
                    {
                        Id = x.Id,

                        TenSach = x.TenSach,

                        AnhBia = x.AnhBia,

                        Gia =
                            x.GiaSauGiam
                            ?? x.GiaGoc,

                        LuotBan = 0,

                        IsActive = x.IsActive
                    })

                    .Take(10)

                    .ToList();
            }

            return View(data);
        }

        // ===== 🔍 TÌM KIẾM =====

        public IActionResult TimKiem(
            string keyword)
        {
            ViewBag.Keyword = keyword;

            var data = _context.SanPhams

                .Where(x =>

                    x.IsActive == true

                    &&

                    (

                        string.IsNullOrEmpty(keyword)

                        ||

                        (x.TenSach ?? "")
                            .ToLower()
                            .Contains(
                                keyword.ToLower())

                        ||

                        (x.TacGia ?? "")
                            .ToLower()
                            .Contains(
                                keyword.ToLower())

                        ||

                        (

                            x.DanhMuc != null

                            ?

                            x.DanhMuc.TenDanhMuc

                            :

                            ""

                        )

                        .ToLower()

                        .Contains(
                            keyword.ToLower())
                    )
                )

                .Select(x => new
                {
                    x.Id,

                    x.TenSach,

                    x.AnhBia,

                    Gia =
                        x.GiaSauGiam
                        ?? x.GiaGoc,

                    TacGia = x.TacGia,

                    DanhMuc =

                        x.DanhMuc != null

                        ?

                        x.DanhMuc.TenDanhMuc

                        :

                        "Không có"
                })

                .ToList();

            return View(data);
        }

        // ===== CHI TIẾT =====

            public IActionResult ChiTiet(int id)
        {
            var sach = _context.SanPhams

                .FirstOrDefault(x =>
                    x.Id == id);

            if (sach == null)
                return NotFound();

            if (sach.IsActive == false)
            {
                ViewBag.NgungBan = true;
            }

            // ===== BIẾN THỂ =====

            var bienThe = _context.BienTheSaches

                .Where(x => x.SanPhamId == id)

                .OrderBy(x => x.Id)

                .FirstOrDefault();

            if (bienThe != null)
            {
                ViewBag.GiaBienThe = bienThe.GiaBan;

                ViewBag.SoLuongTon = bienThe.SoLuongTon;

                ViewBag.BienTheId = bienThe.Id;
            }
            else
            {
                ViewBag.GiaBienThe =
                    sach.GiaSauGiam
                    ?? sach.GiaGoc;

                ViewBag.SoLuongTon = 0;

                ViewBag.BienTheId = 0;
            }

            return View(sach);
        }



        // ===== MUA NGAY =====

        // ===== MUA NGAY =====

        public IActionResult MuaNgay(int id)
        {
            // 🔥 TÌM SÁCH

            var sach = _context.SanPhams

                .FirstOrDefault(x =>
                    x.Id == id);

            if (sach == null)
                return NotFound();

            // 🔥 NGỪNG BÁN

            if (sach.IsActive == false)
            {
                return Content(
                    "❌ Sản phẩm đã ngừng bán!");
            }

            // 🔥 LẤY BIẾN THỂ ĐẦU TIÊN

            var bienThe = _context.BienTheSaches

                .FirstOrDefault(x =>
                    x.SanPhamId == id);

            if (bienThe == null)
            {
                return Content(
                    "❌ Sách chưa có biến thể!");
            }

            // 🔥 CHUYỂN SANG GIỎ HÀNG THEO BIẾN THỂ

            return RedirectToAction(
                "MuaNgay",
                "GioHang",
                new
                {
                    id = bienThe.Id
                });
        }

        // ===== SEARCH AJAX =====

        [HttpGet]

        public IActionResult TimKiemAjax(
            string keyword)
        {
            // ===== RỖNG =====

            if (string.IsNullOrEmpty(keyword))
            {
                return Json(new
                {
                    success = false,

                    message =
                        "Vui lòng nhập từ khóa"
                });
            }

            keyword = keyword.ToLower();

            // ===== SEARCH =====

            var data = _context.SanPhams

                .Where(x =>

                    x.IsActive == true

                    &&

                    (

                        (x.TenSach ?? "")
                            .ToLower()
                            .Contains(keyword)

                        ||

                        (x.TacGia ?? "")
                            .ToLower()
                            .Contains(keyword)

                        ||

                        (

                            x.DanhMuc != null

                            ?

                            x.DanhMuc.TenDanhMuc

                            :

                            ""

                        )

                        .ToLower()

                        .Contains(keyword)
                    )
                )

                .Select(x => new
                {
                    id = x.Id,

                    ten = x.TenSach,

                    anh =
                        x.AnhBia
                        ?? "no-image.png",

                    gia =
                        x.GiaSauGiam
                        ?? x.GiaGoc,

                    isActive =
                        x.IsActive
                })

                .Take(5)

                .ToList();

            // ===== KHÔNG CÓ =====

            if (!data.Any())
            {
                return Json(new
                {
                    success = false,

                    message =
                        "Không tìm thấy sản phẩm"
                });
            }

            // ===== SUCCESS =====

            return Json(new
            {
                success = true,

                data = data
            });
        }
    }
}