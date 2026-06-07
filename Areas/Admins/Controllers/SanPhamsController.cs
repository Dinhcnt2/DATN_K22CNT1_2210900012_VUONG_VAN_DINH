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

        // =====================================================
        // DANH SÁCH + SEARCH
        // =====================================================

        public async Task<IActionResult> Index(
            string keyword,
            int? danhMucId)
        {
            var sanPhams = _context.SanPhams
                .Include(x => x.DanhMuc)
                .AsQueryable();

            // ===== SEARCH TÊN SÁCH =====

            if (!string.IsNullOrEmpty(keyword))
            {
                sanPhams = sanPhams.Where(x =>
                    x.TenSach.Contains(keyword));
            }

            // ===== FILTER DANH MỤC =====

            if (danhMucId != null)
            {
                sanPhams = sanPhams.Where(x =>
                    x.DanhMucId == danhMucId);
            }

            // ===== VIEWBAG =====

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
            ViewData["DanhMucId"] = new SelectList(
                _context.DanhMucs,
                "Id",
                "TenDanhMuc"
            );

            // ===== VOUCHER =====

            ViewBag.VoucherId = new SelectList(
                _context.Vouchers
                    .Where(x => x.TrangThai == true),
                "Id",
                "MaCode"
            );

            return View();
        }

        // =====================================================
        // CREATE POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            SanPham sanPham,
            IFormFile uploadImage)
        {
            try
            {
                // ===== TẠO SLUG =====

                sanPham.Slug = sanPham.TenSach
                    .Replace(" ", "-")
                    .ToLower();

                // ===== GIÁ SAU GIẢM =====

                if (sanPham.PhanTramGiam != null)
                {
                    sanPham.GiaSauGiam =
                        sanPham.GiaGoc
                        - (sanPham.GiaGoc
                        * sanPham.PhanTramGiam / 100);
                }
                else
                {
                    sanPham.GiaSauGiam =
                        sanPham.GiaGoc;
                }

                // ===== UPLOAD ẢNH =====

                if (uploadImage != null
                    && uploadImage.Length > 0)
                {
                    string fileName =
                        Guid.NewGuid().ToString()
                        + Path.GetExtension(
                            uploadImage.FileName);

                    string path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images",
                        fileName
                    );

                    using (var stream =
                        new FileStream(
                            path,
                            FileMode.Create))
                    {
                        await uploadImage
                            .CopyToAsync(stream);
                    }

                    sanPham.AnhBia = fileName;
                }

                // ===== CREATED =====

                sanPham.CreatedAt = DateTime.Now;

                sanPham.IsActive = true;

                // ===== SAVE =====

                _context.Add(sanPham);

                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index));
            }
            catch
            {
                ViewBag.DanhMucId =
                    new SelectList(
                        _context.DanhMucs,
                        "Id",
                        "TenDanhMuc",
                        sanPham.DanhMucId
                    );

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

            var sanPham =
                await _context.SanPhams
                    .FindAsync(id);

            if (sanPham == null)
                return NotFound();

            ViewBag.DanhMucId =
                new SelectList(
                    _context.DanhMucs,
                    "Id",
                    "TenDanhMuc",
                    sanPham.DanhMucId
                );

            return View(sanPham);
        }

        // =====================================================
        // EDIT POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            SanPham sanPham,
            IFormFile uploadImage)
        {
            var sp =
                await _context.SanPhams
                    .FindAsync(id);

            if (sp == null)
                return NotFound();

            try
            {
                // ===== UPDATE =====

                sp.TenSach = sanPham.TenSach;

                sp.DanhMucId =
                    sanPham.DanhMucId;

                sp.TacGia =
                    sanPham.TacGia;

                sp.NhaXuatBan =
                    sanPham.NhaXuatBan;

                sp.NamXuatBan =
                    sanPham.NamXuatBan;

                sp.SoTrang =
                    sanPham.SoTrang;

                sp.Isbn =
                    sanPham.Isbn;

                sp.GiaGoc =
                    sanPham.GiaGoc;

                sp.PhanTramGiam =
                    sanPham.PhanTramGiam;

                sp.MoTaNgan =
                    sanPham.MoTaNgan;

                sp.MoTaChiTiet =
                    sanPham.MoTaChiTiet;

                // ===== SLUG =====

                sp.Slug = sanPham.TenSach
                    .Replace(" ", "-")
                    .ToLower();

                // ===== GIÁ SAU GIẢM =====

                if (sp.PhanTramGiam != null)
                {
                    sp.GiaSauGiam =
                        sp.GiaGoc
                        - (sp.GiaGoc
                        * sp.PhanTramGiam / 100);
                }
                else
                {
                    sp.GiaSauGiam =
                        sp.GiaGoc;
                }

                // ===== UPLOAD ẢNH =====

                if (uploadImage != null
                    && uploadImage.Length > 0)
                {
                    string fileName =
                        Guid.NewGuid().ToString()
                        + Path.GetExtension(
                            uploadImage.FileName);

                    string path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images",
                        fileName
                    );

                    using (var stream =
                        new FileStream(
                            path,
                            FileMode.Create))
                    {
                        await uploadImage
                            .CopyToAsync(stream);
                    }

                    sp.AnhBia = fileName;
                }

                // ===== UPDATED =====

                sp.UpdatedAt = DateTime.Now;

                // ===== SAVE =====

                _context.Update(sp);

                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index));
            }
            catch
            {
                ViewBag.DanhMucId =
                    new SelectList(
                        _context.DanhMucs,
                        "Id",
                        "TenDanhMuc",
                        sanPham.DanhMucId
                    );

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
            var sanPham =
                await _context.SanPhams
                    .FindAsync(id);

            if (sanPham == null)
                return NotFound();

            var bienThes =
                _context.BienTheSaches
                    .Where(x =>
                        x.SanPhamId == id)
                    .ToList();

            bool daCoDon = false;

            foreach (var bt in bienThes)
            {
                if (_context.ChiTietDonHangs
                    .Any(x =>
                        x.BienTheId == bt.Id))
                {
                    daCoDon = true;
                    break;
                }
            }

            // ===== ĐÃ CÓ ĐƠN =====

            if (daCoDon)
            {
                sanPham.IsActive = false;

                _context.Update(sanPham);

                await _context.SaveChangesAsync();

                TempData["Error"] =
                    " Sản phẩm đã có đơn không thể xoá!";

                return RedirectToAction(
                    nameof(Index));
            }

            // ===== XOÁ =====

            _context.BienTheSaches
                .RemoveRange(bienThes);

            _context.SanPhams
                .Remove(sanPham);

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Index));
        }

        // =====================================================
        // RESTORE
        // =====================================================

        public async Task<IActionResult> Restore(int id)
        {
            var sanPham =
                await _context.SanPhams
                    .FindAsync(id);

            if (sanPham == null)
                return NotFound();

            sanPham.IsActive = true;

            _context.Update(sanPham);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "✅ Đã khôi phục sản phẩm!";

            return RedirectToAction(
                nameof(Index));
        }
    }
}