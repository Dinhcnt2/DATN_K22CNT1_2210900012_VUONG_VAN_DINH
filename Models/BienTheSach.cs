using System;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Models;

public partial class BienTheSach
{
    public int Id { get; set; }

    public int SanPhamId { get; set; }

    public int LoaiBiaId { get; set; }

    public int NgonNguId { get; set; }

    public int? SoLuongTon { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<GioHangChiTiet> GioHangChiTiets { get; set; } = new List<GioHangChiTiet>();

    public virtual LoaiBia LoaiBia { get; set; } = null!;

    public virtual NgonNgu NgonN { get; set; } = null!;

    public virtual SanPham SanPham { get; set; } = null!;
    
}

