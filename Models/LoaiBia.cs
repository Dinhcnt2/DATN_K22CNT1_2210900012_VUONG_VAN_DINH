using System;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Models;

public partial class LoaiBia
{
    public int MaLoaiBia { get; set; }

    public string TenLoaiBia { get; set; } = null!;

    public virtual ICollection<BienTheSach> BienTheSaches { get; set; } = new List<BienTheSach>();
}
