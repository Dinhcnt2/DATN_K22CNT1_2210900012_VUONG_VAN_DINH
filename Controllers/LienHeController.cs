using Microsoft.AspNetCore.Mvc;
using VVD_2210900012_DATN.Models;
using System;

namespace VVD_2210900012_DATN.Controllers
{
    public class LienHeController : Controller
    {
        private readonly BookstoreContext _context;

        public LienHeController(BookstoreContext context)
        {
            _context = context;
        }

        // ===== VIEW =====
        public IActionResult Index()
        {
            return View();
        }

        // ===== GỬI =====
        [HttpPost]
        public IActionResult Gui(string hoTen, string email, string tieuDe, string noiDung)
        {
            if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(noiDung))
            {
                TempData["msg"] = "⚠️ Nhập thiếu thông tin!";
                return RedirectToAction("Index");
            }

            var lh = new LienHe
            {
                HoTen = hoTen,
                Email = email,
                TieuDe = tieuDe,
                NoiDung = noiDung,
                NgayGui = DateTime.Now
            };

            _context.LienHes.Add(lh);
            _context.SaveChanges();

            TempData["msg"] = "✅ Gửi thành công!";
            return RedirectToAction("Index");
        }
    }
}