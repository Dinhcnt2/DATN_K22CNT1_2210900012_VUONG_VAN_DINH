using System;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Models;

public partial class BaiViet
{
    public int Id { get; set; }

    public string? TieuDe { get; set; }

    public string? Slug { get; set; }

    public string? HinhAnh { get; set; }

    public string? NoiDung { get; set; }

    public bool? IsPublished { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual NguoiDung? CreatedByNavigation { get; set; }
}
