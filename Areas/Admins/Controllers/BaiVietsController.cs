using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VVD_2210900012_DATN.Models;

namespace VVD_2210900012_DATN.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class BaiVietsController : Controller
    {
        private readonly BookstoreContext _context;

        public BaiVietsController(BookstoreContext context)
        {
            _context = context;
        }

        // ================= INDEX + SEARCH + FILTER =================

        public async Task<IActionResult> Index(
            string keyword,
            bool? status)
        {
            var data = _context.BaiViets
                .Include(b => b.CreatedByNavigation)
                .AsQueryable();

            // ================= SEARCH =================

            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(x =>

                    (x.TieuDe != null
                    && x.TieuDe.Contains(keyword))

                    ||

                    (x.Slug != null
                    && x.Slug.Contains(keyword))
                );
            }

            // ================= FILTER =================

            if (status != null)
            {
                data = data.Where(x =>
                    x.IsPublished == status);
            }

            // ================= VIEWBAG =================

            ViewBag.Keyword = keyword;

            ViewBag.Status = status;

            return View(await data.ToListAsync());
        }

        // ================= DETAILS =================

        public async Task<IActionResult> Details(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baiViet = await _context.BaiViets

                .Include(b =>
                    b.CreatedByNavigation)

                .FirstOrDefaultAsync(m =>
                    m.Id == id);

            if (baiViet == null)
            {
                return NotFound();
            }

            return View(baiViet);
        }

        // ================= CREATE =================

        public IActionResult Create()
        {
            ViewData["CreatedBy"] =
                new SelectList(
                    _context.NguoiDungs,
                    "MaNguoiDung",
                    "MaNguoiDung"
                );

            return View();
        }

        // ================= CREATE POST =================

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(

            [Bind(
                "Id,TieuDe,Slug,HinhAnh,NoiDung,IsPublished,CreatedAt,CreatedBy"
            )]

            BaiViet baiViet)
        {
            if (ModelState.IsValid)
            {
                // ================= AUTO DATE =================

                baiViet.CreatedAt =
                    DateTime.Now;

                _context.Add(baiViet);

                await _context.SaveChangesAsync();

                TempData["msg"] =
                    "Thêm bài viết thành công!";

                return RedirectToAction(
                    nameof(Index));
            }

            ViewData["CreatedBy"] =
                new SelectList(
                    _context.NguoiDungs,
                    "MaNguoiDung",
                    "MaNguoiDung",
                    baiViet.CreatedBy
                );

            return View(baiViet);
        }

        // ================= EDIT =================

        public async Task<IActionResult> Edit(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baiViet =
                await _context.BaiViets
                    .FindAsync(id);

            if (baiViet == null)
            {
                return NotFound();
            }

            ViewData["CreatedBy"] =
                new SelectList(
                    _context.NguoiDungs,
                    "MaNguoiDung",
                    "MaNguoiDung",
                    baiViet.CreatedBy
                );

            return View(baiViet);
        }

        // ================= EDIT POST =================

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(

            int id,

            [Bind(
                "Id,TieuDe,Slug,HinhAnh,NoiDung,IsPublished,CreatedAt,CreatedBy"
            )]

            BaiViet baiViet)
        {
            if (id != baiViet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(baiViet);

                    await _context.SaveChangesAsync();

                    TempData["msg"] =
                        "Cập nhật bài viết thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BaiVietExists(
                        baiViet.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(
                    nameof(Index));
            }

            ViewData["CreatedBy"] =
                new SelectList(
                    _context.NguoiDungs,
                    "MaNguoiDung",
                    "MaNguoiDung",
                    baiViet.CreatedBy
                );

            return View(baiViet);
        }

        // ================= DELETE =================

        public async Task<IActionResult> Delete(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baiViet = await _context.BaiViets

                .Include(b =>
                    b.CreatedByNavigation)

                .FirstOrDefaultAsync(m =>
                    m.Id == id);

            if (baiViet == null)
            {
                return NotFound();
            }

            return View(baiViet);
        }

        // ================= DELETE POST =================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult>
            DeleteConfirmed(int id)
        {
            var baiViet =
                await _context.BaiViets
                    .FindAsync(id);

            if (baiViet != null)
            {
                // 🔥 XOÁ MỀM

                baiViet.IsPublished = false;

                _context.BaiViets
                    .Update(baiViet);
            }

            await _context.SaveChangesAsync();

            TempData["msg"] =
                "Đã ẩn bài viết!";

            return RedirectToAction(
                nameof(Index));
        }

        // ================= CHECK =================

        private bool BaiVietExists(int id)
        {
            return _context.BaiViets
                .Any(e => e.Id == id);
        }
    }
}