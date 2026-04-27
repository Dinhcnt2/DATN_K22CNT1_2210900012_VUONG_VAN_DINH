using System;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Models;

public partial class NgonNgu
{
    public int MaNgonNgu { get; set; }

    public string TenNgonNgu { get; set; } = null!;

    public virtual ICollection<BienTheSach> BienTheSaches { get; set; } = new List<BienTheSach>();
}
