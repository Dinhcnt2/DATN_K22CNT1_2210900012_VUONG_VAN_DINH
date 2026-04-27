using System;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Models;

public partial class DanhMuc
{
    public int Id { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual NguoiDung? CreatedByNavigation { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();

    public virtual NguoiDung? UpdatedByNavigation { get; set; }
}
