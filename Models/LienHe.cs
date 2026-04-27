using System;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Models;

public partial class LienHe
{
    public int Id { get; set; }

    public string? HoTen { get; set; }

    public string? Email { get; set; }

    public string? TieuDe { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? NgayGui { get; set; }
}
