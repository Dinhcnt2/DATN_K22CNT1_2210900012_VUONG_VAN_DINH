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
    public class DonHangsController : Controller
    {
        private readonly BookstoreContext _context;

        public DonHangsController(BookstoreContext context)
        {
            _context = context;
        }

        // ================= DANH SÁCH =================
        public async Task<IActionResult> Index()
        {
            var data = _context.DonHangs
                .Include(x => x.MaNguoiDungNavigation)
                .Include(x => x.Voucher);

            return View(await data.ToListAsync());
        }

        // ================= CHI TIẾT =================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var don = await _context.DonHangs
                .Include(x => x.MaNguoiDungNavigation)
                .Include(x => x.Voucher)
                .FirstOrDefaultAsync(x => x.MaDonHang == id);

            if (don == null) return NotFound();

            return View(don);
        }

        // ================= CREATE =================
        public IActionResult Create()
        {
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung");
            ViewData["VoucherId"] = new SelectList(_context.Vouchers, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                donHang.TrangThai = "ChoXacNhan";
                donHang.TrangThaiThanhToan = "ChuaThanhToan";
                donHang.NgayDat = DateTime.Now;

                _context.Add(donHang);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(donHang);
        }

        // ================= EDIT =================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var don = await _context.DonHangs.FindAsync(id);
            if (don == null) return NotFound();

            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", don.MaNguoiDung);
            ViewData["VoucherId"] = new SelectList(_context.Vouchers, "Id", "Id", don.VoucherId);

            return View(don);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DonHang donHang)
        {
            if (id != donHang.MaDonHang) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var oldDon = await _context.DonHangs.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.MaDonHang == id);

                    if (oldDon == null) return NotFound();

                    donHang.TrangThai ??= oldDon.TrangThai;
                    donHang.TrangThaiThanhToan ??= oldDon.TrangThaiThanhToan;

                    _context.Update(donHang);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(donHang);
        }

        // ================= DELETE (CHUYỂN THÀNH HUỶ) =================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var don = await _context.DonHangs
                .FirstOrDefaultAsync(x => x.MaDonHang == id);

            if (don == null) return NotFound();

            return View(don);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var don = await _context.DonHangs.FindAsync(id);

            if (don == null)
                return NotFound();

            // 🔥 KHÔNG XOÁ → CHUYỂN TRẠNG THÁI
            if (don.TrangThai == "HoanThanh")
            {
                TempData["Error"] = "Đơn đã hoàn thành, không thể huỷ!";
                return RedirectToAction(nameof(Index));
            }

            don.TrangThai = "DaHuy";

            _context.Update(don);
            await _context.SaveChangesAsync();

            TempData["msg"] = "Đơn hàng đã được chuyển sang HUỶ!";

            return RedirectToAction(nameof(Index));
        }

        // ================= 🔥 HUỶ ĐƠN (DÙNG CHO BUTTON MỚI) =================
        [HttpPost]
        public async Task<IActionResult> HuyDon(int id)
        {
            var don = await _context.DonHangs.FindAsync(id);

            if (don == null)
                return NotFound();

            if (don.TrangThai == "HoanThanh")
            {
                TempData["Error"] = "Đơn đã hoàn thành, không thể huỷ!";
                return RedirectToAction(nameof(Index));
            }

            don.TrangThai = "DaHuy";

            _context.Update(don);
            await _context.SaveChangesAsync();

            TempData["msg"] = "Đã huỷ đơn hàng!";
            return RedirectToAction(nameof(Index));
        }

        // ================= 🔥 UPDATE THANH TOÁN =================
        public async Task<IActionResult> CapNhatThanhToan(int id, string status)
        {
            var don = await _context.DonHangs.FindAsync(id);
            if (don == null) return NotFound();

            don.TrangThaiThanhToan = status;

            if (status == "DaThanhToan" && don.TrangThai == "ChoXacNhan")
            {
                don.TrangThai = "DangXuLy";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= 🔥 UPDATE TRẠNG THÁI =================
        public async Task<IActionResult> CapNhatTrangThai(int id, string status)
        {
            var don = await _context.DonHangs.FindAsync(id);
            if (don == null) return NotFound();

            if (status == "HoanThanh" && don.TrangThaiThanhToan != "DaThanhToan")
            {
                TempData["Error"] = "Chưa thanh toán không thể hoàn thành!";
                return RedirectToAction(nameof(Index));
            }

            don.TrangThai = status;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DonHangExists(int id)
        {
            return _context.DonHangs.Any(e => e.MaDonHang == id);
        }
    }
}