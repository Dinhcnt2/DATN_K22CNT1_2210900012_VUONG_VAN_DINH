using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VVD_2210900012_DATN.Models;

namespace VVD_2210900012_DATN.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class NguoiDungsController : Controller
    {
        private readonly BookstoreContext _context;

        public NguoiDungsController(BookstoreContext context)
        {
            _context = context;
        }

        // ================= INDEX + SEARCH =================

        public async Task<IActionResult> Index(string keyword)
        {
            var query = _context.NguoiDungs
                .Where(x => x.IsActive == true)
                .AsQueryable();

            // ================= SEARCH =================

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x =>

                    (x.HoTen != null
                    && x.HoTen.Contains(keyword))

                    ||

                    (x.TenDangNhap != null
                    && x.TenDangNhap.Contains(keyword))

                    ||

                    (x.Email != null
                    && x.Email.Contains(keyword))

                    ||

                    (x.Sdt != null
                    && x.Sdt.Contains(keyword))
                );
            }

            // ================= VIEWBAG =================

            ViewBag.Keyword = keyword;

            // ================= LOAD LIST =================

            var list = await query.ToListAsync();

            return View(list);
        }

        // ================= DETAILS =================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(m =>
                    m.MaNguoiDung == id);

            if (nguoiDung == null)
                return NotFound();

            return View(nguoiDung);
        }

        // ================= CREATE =================

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(
            NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                nguoiDung.IsActive = true;

                _context.Add(nguoiDung);

                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index));
            }

            return View(nguoiDung);
        }

        // ================= EDIT =================

        public async Task<IActionResult> Edit(
            int? id)
        {
            if (id == null)
                return NotFound();

            var nguoiDung =
                await _context.NguoiDungs
                    .FindAsync(id);

            if (nguoiDung == null)
                return NotFound();

            return View(nguoiDung);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(
            int id,
            NguoiDung nguoiDung)
        {
            if (id != nguoiDung.MaNguoiDung)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nguoiDung);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoiDungExists(
                        nguoiDung.MaNguoiDung))
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

            return View(nguoiDung);
        }

        // ================= DELETE (GET) =================

        public async Task<IActionResult> Delete(
            int? id)
        {
            if (id == null)
                return NotFound();

            var nguoiDung =
                await _context.NguoiDungs
                    .FirstOrDefaultAsync(m =>
                        m.MaNguoiDung == id);

            if (nguoiDung == null)
                return NotFound();

            return View(nguoiDung);
        }

        // ================= DELETE (POST) =================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult>
            DeleteConfirmed(int id)
        {
            var nguoiDung =
                await _context.NguoiDungs
                    .FindAsync(id);

            if (nguoiDung != null)
            {
                // 🔥 XOÁ MỀM

                nguoiDung.IsActive = false;

                _context.NguoiDungs
                    .Update(nguoiDung);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Index));
        }

        // ================= CHECK =================

        private bool NguoiDungExists(int id)
        {
            return _context.NguoiDungs
                .Any(e =>
                    e.MaNguoiDung == id);
        }
    }
}