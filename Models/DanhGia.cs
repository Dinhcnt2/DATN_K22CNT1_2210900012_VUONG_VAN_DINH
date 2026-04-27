using System;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Models;

public partial class DanhGia
{
    public int Id { get; set; }

    public int SanPhamId { get; set; }

    public int MaNguoiDung { get; set; }

    public int? SoSao { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? NgayDanhGia { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    public virtual SanPham SanPham { get; set; } = null!;
}
