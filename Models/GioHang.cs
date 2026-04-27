using System;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Models;

public partial class GioHang
{
    public int MaGioHang { get; set; }

    public string? SessionId { get; set; }

    public int? MaNguoiDung { get; set; }

    public decimal? TongTien { get; set; }

    public string? TrangThai { get; set; }

    public string? GhiChu { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual ICollection<GioHangChiTiet> GioHangChiTiets { get; set; } = new List<GioHangChiTiet>();

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}
