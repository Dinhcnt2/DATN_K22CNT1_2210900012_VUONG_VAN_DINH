using System;
using System.Collections.Generic;

namespace VVD_2210900012_DATN.Models;

public partial class SanPham
{
    public int Id { get; set; }

    public int DanhMucId { get; set; }

    public string TenSach { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? TacGia { get; set; }

    public string? NhaXuatBan { get; set; }

    public int? NamXuatBan { get; set; }

    public int? SoTrang { get; set; }

    public string? Isbn { get; set; }

    public string? MoTaNgan { get; set; }

    public string? MoTaChiTiet { get; set; }

    public decimal GiaGoc { get; set; }

    public int? PhanTramGiam { get; set; }

    public decimal? GiaSauGiam { get; set; }

    public string? AnhBia { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public string? MetaKeyword { get; set; }

    public bool? NoiBat { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<BienTheSach> BienTheSaches { get; set; } = new List<BienTheSach>();

    public virtual NguoiDung? CreatedByNavigation { get; set; }

    public virtual ICollection<DanhGia> DanhGia { get; set; } = new List<DanhGia>();

    public virtual DanhMuc? DanhMuc { get; set; }

    public virtual NguoiDung? UpdatedByNavigation { get; set; }
}
