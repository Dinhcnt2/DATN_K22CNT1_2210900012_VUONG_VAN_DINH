using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VVD_2210900012_DATN.Models;

namespace VVD_2210900012_DATN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly BookstoreContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            BookstoreContext context)
        {
            _logger = logger;

            _context = context;
        }

        // ===== TRANG CHỦ =====

        public IActionResult Index()
        {
            // ===== LẤY SÁCH + TỒN KHO =====

            var sach = _context.SanPhams

                .Include(x => x.BienTheSaches)

                .Select(sp => new
                {
                    SanPham = sp,

                    TongTonKho =

                        sp.BienTheSaches

                        .Sum(bt =>
                            (int?)bt.SoLuongTon
                        ) ?? 0
                })

                // ===== SÁCH MỚI =====

                .OrderByDescending(x =>
                    x.SanPham.Id)

                .ToList();

            return View(sach);
        }

        // ===== PRIVACY =====

        public IActionResult Privacy()
        {
            return View();
        }

        // ===== ERROR =====

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]

        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId =
                        Activity.Current?.Id
                        ?? HttpContext.TraceIdentifier
                }
            );
        }
    }
}