using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VVD_2210900012_DATN.Models;

namespace VVD_2210900012_DATN.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class SanPhamsController : Controller
    {
        private readonly BookstoreContext _context;

        public SanPhamsController(BookstoreContext context)
        {
            _context = context;
        }

        // =====================================================
        // DANH SÁCH + SEARCH
        // =====================================================
        public async Task<IActionResult> Index(string keyword, int? danhMucId)
        {
            var sanPhams = _context.SanPhams
                .Include(x => x.DanhMuc)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                sanPhams = sanPhams.Where(x => x.TenSach.Contains(keyword));
            }

            if (danhMucId != null)
            {
                sanPhams = sanPhams.Where(x => x.DanhMucId == danhMucId);
            }

            ViewBag.Keyword = keyword;

            ViewBag.DanhMucId = new SelectList(
                _context.DanhMucs,
                "Id",
                "TenDanhMuc",
                danhMucId
            );

            return View(await sanPhams.ToListAsync());
        }

        // =====================================================
        // CHI TIẾT
        // =====================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var sanPham = await _context.SanPhams
                .Include(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sanPham == null)
                return NotFound();

            return View(sanPham);
        }

        // =====================================================
        // CREATE GET
        // =====================================================
        public IActionResult Create()
        {
            ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "Id", "TenDanhMuc");

            ViewBag.VoucherId = new SelectList(
                _context.Vouchers.Where(x => x.TrangThai == true),
                "Id",
                "MaCode"
            );

            return View();
        }

        // =====================================================
        // CREATE POST (FIX CHUẨN ĐỒNG BỘ database)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sanPham, IFormFile uploadImage, int soLuongTon)
        {
            try
            {
                // ===== TẠO SLUG =====
                sanPham.Slug = sanPham.TenSach.Replace(" ", "-").ToLower();

                // ===== GIÁ SAU GIẢM =====
                if (sanPham.PhanTramGiam != null)
                {
                    sanPham.GiaSauGiam = sanPham.GiaGoc - (sanPham.GiaGoc * sanPham.PhanTramGiam / 100);
                }
                else
                {
                    sanPham.GiaSauGiam = sanPham.GiaGoc;
                }

                // ===== UPLOAD ẢNH =====
                if (uploadImage != null && uploadImage.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await uploadImage.CopyToAsync(stream);
                    }

                    sanPham.AnhBia = fileName;
                }

                sanPham.CreatedAt = DateTime.Now;
                sanPham.IsActive = true;

                // ===== SAVE SAN PHAM =====
                _context.Add(sanPham);
                await _context.SaveChangesAsync();

                // ===== DÒ TÌM KHÓA NGOẠI AN TOÀN TRÁNH LỖI RÀNG BUỘC =====
                var firstLoaiBia = await _context.LoaiBia.FirstOrDefaultAsync();
                var firstNgonNgu = await _context.NgonNgus.FirstOrDefaultAsync();

                if (firstLoaiBia == null || firstNgonNgu == null)
                {
                    throw new Exception("Vui lòng đảm bảo bạn đã nhập dữ liệu mẫu cho bảng Loại bìa và Ngôn ngữ trước!");
                }

                // ===== TỰ ĐỘNG THÊM BIẾN THỂ VỚI SỐ LƯỢNG TỒN =====
                var bienTheMacDinh = new BienTheSach
                {
                    SanPhamId = sanPham.Id,
                    LoaiBiaId = firstLoaiBia.MaLoaiBia, // Lấy mã thực tế có sẵn đầu tiên trong database
                    NgonNguId = firstNgonNgu.MaNgonNgu, // Lấy mã thực tế có sẵn đầu tiên trong database
                    SoLuongTon = soLuongTon,
                    GiaBan = sanPham.GiaSauGiam ?? sanPham.GiaGoc // Gán giá bán bắt buộc bằng giá tiền của sản phẩm
                };

                _context.Add(bienTheMacDinh);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));

                ViewBag.DanhMucId = new SelectList(_context.DanhMucs, "Id", "TenDanhMuc", sanPham.DanhMucId);
                ViewBag.VoucherId = new SelectList(_context.Vouchers.Where(x => x.TrangThai == true), "Id", "MaCode", sanPham.VoucherId);

                return View(sanPham);
            }
        }

        // =====================================================
        // EDIT GET
        // =====================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var sanPham = await _context.SanPhams.FindAsync(id);

            if (sanPham == null)
                return NotFound();

            ViewBag.DanhMucId = new SelectList(_context.DanhMucs, "Id", "TenDanhMuc", sanPham.DanhMucId);
            return View(sanPham);
        }

        // =====================================================
        // EDIT POST
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SanPham sanPham, IFormFile uploadImage)
        {
            var sp = await _context.SanPhams.FindAsync(id);

            if (sp == null)
                return NotFound();

            try
            {
                sp.TenSach = sanPham.TenSach;
                sp.DanhMucId = sanPham.DanhMucId;
                sp.TacGia = sanPham.TacGia;
                sp.NhaXuatBan = sanPham.NhaXuatBan;
                sp.NamXuatBan = sanPham.NamXuatBan;
                sp.SoTrang = sanPham.SoTrang;
                sp.Isbn = sanPham.Isbn;
                sp.GiaGoc = sanPham.GiaGoc;
                sp.PhanTramGiam = sanPham.PhanTramGiam;
                sp.MoTaNgan = sanPham.MoTaNgan;
                sp.MoTaChiTiet = sanPham.MoTaChiTiet;

                sp.Slug = sanPham.TenSach.Replace(" ", "-").ToLower();

                if (sp.PhanTramGiam != null)
                {
                    sp.GiaSauGiam = sp.GiaGoc - (sp.GiaGoc * sp.PhanTramGiam / 100);
                }
                else
                {
                    sp.GiaSauGiam = sp.GiaGoc;
                }

                if (uploadImage != null && uploadImage.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await uploadImage.CopyToAsync(stream);
                    }

                    sp.AnhBia = fileName;
                }

                sp.UpdatedAt = DateTime.Now;

                _context.Update(sp);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.DanhMucId = new SelectList(_context.DanhMucs, "Id", "TenDanhMuc", sanPham.DanhMucId);
                return View(sanPham);
            }
        }

        // =====================================================
        // DELETE
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);

            if (sanPham == null)
                return NotFound();

            var bienThes = _context.BienTheSaches
                .Where(x => x.SanPhamId == id)
                .ToList();

            bool daCoDon = false;

            foreach (var bt in bienThes)
            {
                if (_context.ChiTietDonHangs.Any(x => x.BienTheId == bt.Id))
                {
                    daCoDon = true;
                    break;
                }
            }

            if (daCoDon)
            {
                sanPham.IsActive = false;
                _context.Update(sanPham);
                await _context.SaveChangesAsync();

                TempData["Error"] = "Sản phẩm đã có đơn không thể xoá!";
                return RedirectToAction(nameof(Index));
            }

            _context.BienTheSaches.RemoveRange(bienThes);
            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // RESTORE
        // =====================================================
        public async Task<IActionResult> Restore(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);

            if (sanPham == null)
                return NotFound();

            sanPham.IsActive = true;
            _context.Update(sanPham);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã khôi phục sản phẩm!";
            return RedirectToAction(nameof(Index));
        }
    }
}