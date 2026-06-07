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
    public class GioHangChiTietsController : Controller
    {
        private readonly BookstoreContext _context;

        public GioHangChiTietsController(BookstoreContext context)
        {
            _context = context;
        }

        // =====================================================
        // DANH SÁCH CHI TIẾT GIỎ HÀNG
        // =====================================================

        public async Task<IActionResult> Index()
        {
            var data = _context.GioHangChiTiets

                // ===== GIỎ HÀNG =====

                .Include(x => x.GioHang)

                    .ThenInclude(x =>
                        x.MaNguoiDungNavigation)

                // ===== BIẾN THỂ =====

                .Include(x => x.BienThe)

                    .ThenInclude(x =>
                        x.SanPham)

                // ===== MỚI NHẤT =====

                .OrderByDescending(x =>
                    x.GioHang.NgayTao);

            return View(await data.ToListAsync());
        }

        // =====================================================
        // CHI TIẾT
        // =====================================================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gioHangChiTiet = await _context.GioHangChiTiets

                .Include(x => x.GioHang)

                    .ThenInclude(x =>
                        x.MaNguoiDungNavigation)

                .Include(x => x.BienThe)

                    .ThenInclude(x =>
                        x.SanPham)

                .FirstOrDefaultAsync(x =>
                    x.Id == id);

            if (gioHangChiTiet == null)
            {
                return NotFound();
            }

            return View(gioHangChiTiet);
        }

        // =====================================================
        // CREATE
        // =====================================================

        public IActionResult Create()
        {
            // ===== DROPDOWN SÁCH =====

            ViewData["BienTheId"] =
                new SelectList(

                    _context.BienTheSaches
                        .Include(x => x.SanPham),

                    "Id",

                    "SanPham.TenSach"
                );

            // ===== DROPDOWN GIỎ =====

            ViewData["GioHangId"] =
                new SelectList(

                    _context.GioHangs,

                    "MaGioHang",

                    "MaGioHang"
                );

            return View();
        }

        // =====================================================
        // POST CREATE
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(
            [Bind("Id,GioHangId,BienTheId,SoLuong,DonGia,ThanhTien")]
            GioHangChiTiet gioHangChiTiet)
        {
            if (ModelState.IsValid)
            {
                // ===== TÍNH THÀNH TIỀN =====

                gioHangChiTiet.ThanhTien =
                    gioHangChiTiet.SoLuong
                    *
                    gioHangChiTiet.DonGia;

                _context.Add(gioHangChiTiet);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // ===== LOAD LẠI DROPDOWN =====

            ViewData["BienTheId"] =
                new SelectList(

                    _context.BienTheSaches
                        .Include(x => x.SanPham),

                    "Id",

                    "SanPham.TenSach",

                    gioHangChiTiet.BienTheId
                );

            ViewData["GioHangId"] =
                new SelectList(

                    _context.GioHangs,

                    "MaGioHang",

                    "MaGioHang",

                    gioHangChiTiet.GioHangId
                );

            return View(gioHangChiTiet);
        }

        // =====================================================
        // EDIT
        // =====================================================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gioHangChiTiet =
                await _context.GioHangChiTiets
                    .FindAsync(id);

            if (gioHangChiTiet == null)
            {
                return NotFound();
            }

            // ===== DROPDOWN SÁCH =====

            ViewData["BienTheId"] =
                new SelectList(

                    _context.BienTheSaches
                        .Include(x => x.SanPham),

                    "Id",

                    "SanPham.TenSach",

                    gioHangChiTiet.BienTheId
                );

            // ===== DROPDOWN GIỎ =====

            ViewData["GioHangId"] =
                new SelectList(

                    _context.GioHangs,

                    "MaGioHang",

                    "MaGioHang",

                    gioHangChiTiet.GioHangId
                );

            return View(gioHangChiTiet);
        }

        // =====================================================
        // POST EDIT
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(
            int id,

            [Bind("Id,GioHangId,BienTheId,SoLuong,DonGia,ThanhTien")]
            GioHangChiTiet gioHangChiTiet)
        {
            if (id != gioHangChiTiet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // ===== UPDATE THÀNH TIỀN =====

                    gioHangChiTiet.ThanhTien =
                        gioHangChiTiet.SoLuong
                        *
                        gioHangChiTiet.DonGia;

                    _context.Update(gioHangChiTiet);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GioHangChiTietExists(gioHangChiTiet.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // ===== LOAD LẠI DROPDOWN =====

            ViewData["BienTheId"] =
                new SelectList(

                    _context.BienTheSaches
                        .Include(x => x.SanPham),

                    "Id",

                    "SanPham.TenSach",

                    gioHangChiTiet.BienTheId
                );

            ViewData["GioHangId"] =
                new SelectList(

                    _context.GioHangs,

                    "MaGioHang",

                    "MaGioHang",

                    gioHangChiTiet.GioHangId
                );

            return View(gioHangChiTiet);
        }

        // =====================================================
        // DELETE
        // =====================================================

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gioHangChiTiet = await _context.GioHangChiTiets

                .Include(x => x.GioHang)

                    .ThenInclude(x =>
                        x.MaNguoiDungNavigation)

                .Include(x => x.BienThe)

                    .ThenInclude(x =>
                        x.SanPham)

                .FirstOrDefaultAsync(x =>
                    x.Id == id);

            if (gioHangChiTiet == null)
            {
                return NotFound();
            }

            return View(gioHangChiTiet);
        }

        // =====================================================
        // POST DELETE
        // =====================================================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gioHangChiTiet =
                await _context.GioHangChiTiets
                    .FindAsync(id);

            if (gioHangChiTiet != null)
            {
                _context.GioHangChiTiets
                    .Remove(gioHangChiTiet);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // CHECK EXISTS
        // =====================================================

        private bool GioHangChiTietExists(int id)
        {
            return _context.GioHangChiTiets
                .Any(e => e.Id == id);
        }
    }
}