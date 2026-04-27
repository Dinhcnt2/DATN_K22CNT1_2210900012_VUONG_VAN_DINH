using System;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Models;

public partial class DonHang
{
    public int MaDonHang { get; set; }

    public string? MaDonHangCode { get; set; }

    public string? HoTen { get; set; }

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    public decimal? TongTien { get; set; }

    public decimal? PhiVanChuyen { get; set; }

    public string? PhuongThucThanhToan { get; set; }

    public string? TrangThaiThanhToan { get; set; }

    public string? TrangThai { get; set; }

    public string? GhiChu { get; set; }

    public DateTime? NgayDat { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public int? MaNguoiDung { get; set; }

    public int? VoucherId { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
