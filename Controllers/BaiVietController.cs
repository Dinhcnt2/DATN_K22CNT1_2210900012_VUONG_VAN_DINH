using Microsoft.AspNetCore.Mvc;
using VVD_2210900012_DATN.Models;
using System.Linq;

namespace VVD_2210900012_DATN.Controllers
{
    public class BaiVietController : Controller
    {
        private readonly BookstoreContext _context;

        public BaiVietController(BookstoreContext context)
        {
            _context = context;
        }

        // ===== USER XEM DANH SÁCH =====
        public IActionResult Index()
        {
            var list = _context.BaiViets
                .Where(x => x.IsPublished == true) // 🔥 đúng DB
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(list);
        }

        // ===== CHI TIẾT =====
        public IActionResult Details(int id)
        {
            var bv = _context.BaiViets
                .FirstOrDefault(x => x.Id == id);

            if (bv == null)
                return NotFound();

            return View(bv);
        }
    }
}