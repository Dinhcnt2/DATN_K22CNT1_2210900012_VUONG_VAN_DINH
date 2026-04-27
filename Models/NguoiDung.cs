using System;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Models;

public partial class NguoiDung
{
    public int MaNguoiDung { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? HoTen { get; set; }

    public string? Email { get; set; }

    public string? Sdt { get; set; }

    public string? VaiTro { get; set; }

    public DateTime? NgayTao { get; set; }

    // 🔥 FIX LỖI QUÊN MẬT KHẨU (THÊM 2 FIELD)
    public string? ResetCode { get; set; }

    public DateTime? ResetTime { get; set; }

    // ================= NAVIGATION =================

    public virtual ICollection<BaiViet> BaiViets { get; set; } = new List<BaiViet>();

    public virtual ICollection<DanhGia> DanhGia { get; set; } = new List<DanhGia>();

    public virtual ICollection<DanhMuc> DanhMucCreatedByNavigations { get; set; } = new List<DanhMuc>();

    public virtual ICollection<DanhMuc> DanhMucUpdatedByNavigations { get; set; } = new List<DanhMuc>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual ICollection<SanPham> SanPhamCreatedByNavigations { get; set; } = new List<SanPham>();

    public virtual ICollection<SanPham> SanPhamUpdatedByNavigations { get; set; } = new List<SanPham>();
    public bool IsActive { get; set; } = true;
}
