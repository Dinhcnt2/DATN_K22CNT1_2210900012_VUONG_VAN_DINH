using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        // ===== DANH SÁCH =====
        public async Task<IActionResult> Index()
        {
            var sanPhams = _context.SanPhams
                .Include(x => x.DanhMuc);

            return View(await sanPhams.ToListAsync());
        }

        // ===== CHI TIẾT =====
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sanPham = await _context.SanPhams
                .Include(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sanPham == null) return NotFound();

            return View(sanPham);
        }

        // ===== CREATE =====
        public IActionResult Create()
        {
            ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "Id", "TenDanhMuc");

            //  THÊM VOUCHER
            ViewBag.VoucherId = new SelectList(
                _context.Vouchers.Where(x => x.TrangThai == true),
                "Id",
                "MaCode"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sanPham, IFormFile uploadImage)
        {
            try
            {
                sanPham.Slug = sanPham.TenSach.Replace(" ", "-").ToLower();

                if (sanPham.PhanTramGiam != null)
                    sanPham.GiaSauGiam = sanPham.GiaGoc - (sanPham.GiaGoc * sanPham.PhanTramGiam / 100);
                else
                    sanPham.GiaSauGiam = sanPham.GiaGoc;

                if (uploadImage != null && uploadImage.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);

                    string path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images",
                        fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await uploadImage.CopyToAsync(stream);
                    }

                    sanPham.AnhBia = fileName;
                }

                sanPham.CreatedAt = DateTime.Now;
                sanPham.IsActive = true; // 🔥 mặc định bật

                _context.Add(sanPham);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.DanhMucId = new SelectList(_context.DanhMucs, "Id", "TenDanhMuc", sanPham.DanhMucId);
                return View(sanPham);
            }
        }

        // ===== EDIT =====
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sanPham = await _context.SanPhams.FindAsync(id);

            if (sanPham == null) return NotFound();

            ViewBag.DanhMucId = new SelectList(_context.DanhMucs, "Id", "TenDanhMuc", sanPham.DanhMucId);

            return View(sanPham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SanPham sanPham, IFormFile uploadImage)
        {
            var sp = await _context.SanPhams.FindAsync(id);

            if (sp == null) return NotFound();

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
                    sp.GiaSauGiam = sp.GiaGoc - (sp.GiaGoc * sp.PhanTramGiam / 100);
                else
                    sp.GiaSauGiam = sp.GiaGoc;

                if (uploadImage != null && uploadImage.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);

                    string path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images",
                        fileName);

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

        // ===== DELETE (XOÁ MỀM + XOÁ CỨNG) =====
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

                TempData["Error"] = "⚠️ Sản phẩm đã có đơn → chuyển sang ngừng bán!";
                return RedirectToAction(nameof(Index));
            }

            _context.BienTheSaches.RemoveRange(bienThes);
            _context.SanPhams.Remove(sanPham);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===== RESTORE =====
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