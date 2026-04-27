using System;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Models;

public partial class ChiTietDonHang
{
    public int Id { get; set; }

    public int MaDonHang { get; set; }

    public int BienTheId { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal ThanhTien { get; set; }

    public virtual BienTheSach BienThe { get; set; } = null!;

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;
}
