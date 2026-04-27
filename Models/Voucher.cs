using System;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Models;

public partial class Voucher
{
    public int Id { get; set; }

    public string? MaCode { get; set; }

    public decimal? GiamGia { get; set; }

    public string? Loai { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    public int? SoLuong { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
