using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VVD_2210900012_DATN.Models;

public partial class BookstoreContext : DbContext
{
    public BookstoreContext()
    {
    }

    public BookstoreContext(DbContextOptions<BookstoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BaiViet> BaiViets { get; set; }

    public virtual DbSet<BienTheSach> BienTheSaches { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<DanhGia> DanhGia { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<GioHangChiTiet> GioHangChiTiets { get; set; }

    public virtual DbSet<LienHe> LienHes { get; set; }

    public virtual DbSet<LoaiBia> LoaiBia { get; set; }

    public virtual DbSet<NgonNgu> NgonNgus { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=LAPTOP-5802MSHJ\\SQLEXPRESS; Database=bookstore; Trusted_Connection=True; MultipleActiveResultSets=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaiViet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BaiViet__3213E83F831CBC0D");

            entity.ToTable("BaiViet");

            entity.HasIndex(e => e.Slug, "UQ__BaiViet__32DD1E4CF9178141").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy).HasColumnName("createdBy");
            entity.Property(e => e.HinhAnh)
                .HasMaxLength(255)
                .HasColumnName("hinhAnh");
            entity.Property(e => e.IsPublished)
                .HasDefaultValue(true)
                .HasColumnName("isPublished");
            entity.Property(e => e.NoiDung).HasColumnName("noiDung");
            entity.Property(e => e.Slug)
                .HasMaxLength(250)
                .HasColumnName("slug");
            entity.Property(e => e.TieuDe)
                .HasMaxLength(200)
                .HasColumnName("tieuDe");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BaiViets)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__BaiViet__created__5441852A");
        });

        modelBuilder.Entity<BienTheSach>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BienTheS__3213E83FEF3D7FA6");

            entity.ToTable("BienTheSach");

            entity.HasIndex(e => new { e.SanPhamId, e.LoaiBiaId, e.NgonNguId }, "UQ_BienTheSach").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LoaiBiaId).HasColumnName("loaiBiaId");
            entity.Property(e => e.NgonNguId).HasColumnName("ngonNguId");
            entity.Property(e => e.SanPhamId).HasColumnName("sanPhamId");
            entity.Property(e => e.SoLuongTon)
                .HasDefaultValue(0)
                .HasColumnName("soLuongTon");

            entity.HasOne(d => d.LoaiBia).WithMany(p => p.BienTheSaches)
                .HasForeignKey(d => d.LoaiBiaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BienTheSa__loaiB__2B3F6F97");

            entity.HasOne(d => d.NgonN).WithMany(p => p.BienTheSaches)
                .HasForeignKey(d => d.NgonNguId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BienTheSa__ngonN__2C3393D0");

            entity.HasOne(d => d.SanPham).WithMany(p => p.BienTheSaches)
                .HasForeignKey(d => d.SanPhamId)
                .HasConstraintName("FK__BienTheSa__sanPh__2A4B4B5E");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietD__3213E83F661AF67B");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BienTheId).HasColumnName("bienTheId");
            entity.Property(e => e.DonGia)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("donGia");
            entity.Property(e => e.MaDonHang).HasColumnName("maDonHang");
            entity.Property(e => e.SoLuong).HasColumnName("soLuong");
            entity.Property(e => e.ThanhTien)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("thanhTien");

            entity.HasOne(d => d.BienThe).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.BienTheId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__bienT__48CFD27E");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__ChiTietDo__maDon__47DBAE45");
        });

        modelBuilder.Entity<DanhGia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DanhGia__3213E83FBFD5FB38");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MaNguoiDung).HasColumnName("maNguoiDung");
            entity.Property(e => e.NgayDanhGia)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ngayDanhGia");
            entity.Property(e => e.NoiDung).HasColumnName("noiDung");
            entity.Property(e => e.SanPhamId).HasColumnName("sanPhamId");
            entity.Property(e => e.SoSao).HasColumnName("soSao");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__maNguoi__4E88ABD4");

            entity.HasOne(d => d.SanPham).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.SanPhamId)
                .HasConstraintName("FK__DanhGia__sanPham__4D94879B");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DanhMuc__3213E83F701D188E");

            entity.ToTable("DanhMuc");

            entity.HasIndex(e => e.Slug, "UQ__DanhMuc__32DD1E4C83D3B9A9").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy).HasColumnName("createdBy");
            entity.Property(e => e.Slug)
                .HasMaxLength(200)
                .HasColumnName("slug");
            entity.Property(e => e.TenDanhMuc)
                .HasMaxLength(150)
                .HasColumnName("tenDanhMuc");
            entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DanhMucCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__DanhMuc__created__173876EA");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DanhMucUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__DanhMuc__updated__182C9B23");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DonHang__871D38191719F0B4");

            entity.ToTable("DonHang");

            entity.HasIndex(e => e.MaDonHangCode, "UQ__DonHang__5B9062AFA1407B4B").IsUnique();

            entity.Property(e => e.MaDonHang).HasColumnName("maDonHang");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .HasColumnName("diaChi");
            entity.Property(e => e.GhiChu).HasColumnName("ghiChu");
            entity.Property(e => e.HoTen)
                .HasMaxLength(100)
                .HasColumnName("hoTen");
            entity.Property(e => e.MaDonHangCode)
                .HasMaxLength(20)
                .HasColumnName("maDonHangCode");
            entity.Property(e => e.MaNguoiDung).HasColumnName("maNguoiDung");
            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ngayCapNhat");
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ngayDat");
            entity.Property(e => e.PhiVanChuyen)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("phiVanChuyen");
            entity.Property(e => e.PhuongThucThanhToan)
                .HasMaxLength(20)
                .HasColumnName("phuongThucThanhToan");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(20)
                .HasColumnName("soDienThoai");
            entity.Property(e => e.TongTien)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("tongTien");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("ChoXacNhan")
                .HasColumnName("trangThai");
            entity.Property(e => e.TrangThaiThanhToan)
                .HasMaxLength(20)
                .HasDefaultValue("ChuaThanhToan")
                .HasColumnName("trangThaiThanhToan");
            entity.Property(e => e.VoucherId).HasColumnName("voucherId");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK__DonHang__maNguoi__440B1D61");

            entity.HasOne(d => d.Voucher).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK__DonHang__voucher__44FF419A");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGioHang).HasName("PK__GioHang__2C76D2034F2081B3");

            entity.ToTable("GioHang");

            entity.Property(e => e.MaGioHang).HasColumnName("maGioHang");
            entity.Property(e => e.GhiChu).HasColumnName("ghiChu");
            entity.Property(e => e.MaNguoiDung).HasColumnName("maNguoiDung");
            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ngayCapNhat");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ngayTao");
            entity.Property(e => e.SessionId)
                .HasMaxLength(255)
                .HasColumnName("sessionId");
            entity.Property(e => e.TongTien)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("tongTien");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("DangSuDung")
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK__GioHang__maNguoi__32E0915F");
        });

        modelBuilder.Entity<GioHangChiTiet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GioHangC__3213E83F96303D4D");

            entity.ToTable("GioHangChiTiet");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BienTheId).HasColumnName("bienTheId");
            entity.Property(e => e.DonGia)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("donGia");
            entity.Property(e => e.GioHangId).HasColumnName("gioHangId");
            entity.Property(e => e.SoLuong).HasColumnName("soLuong");
            entity.Property(e => e.ThanhTien)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("thanhTien");

            entity.HasOne(d => d.BienThe).WithMany(p => p.GioHangChiTiets)
                .HasForeignKey(d => d.BienTheId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHangCh__bienT__36B12243");

            entity.HasOne(d => d.GioHang).WithMany(p => p.GioHangChiTiets)
                .HasForeignKey(d => d.GioHangId)
                .HasConstraintName("FK__GioHangCh__gioHa__35BCFE0A");
        });

        modelBuilder.Entity<LienHe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LienHe__3213E83F0D8BC80D");

            entity.ToTable("LienHe");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.HoTen)
                .HasMaxLength(100)
                .HasColumnName("hoTen");
            entity.Property(e => e.NgayGui)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ngayGui");
            entity.Property(e => e.NoiDung).HasColumnName("noiDung");
            entity.Property(e => e.TieuDe)
                .HasMaxLength(200)
                .HasColumnName("tieuDe");
        });

        modelBuilder.Entity<LoaiBia>(entity =>
        {
            entity.HasKey(e => e.MaLoaiBia).HasName("PK__LoaiBia__38B38A6EA29947B0");

            entity.Property(e => e.MaLoaiBia).HasColumnName("maLoaiBia");
            entity.Property(e => e.TenLoaiBia)
                .HasMaxLength(50)
                .HasColumnName("tenLoaiBia");
        });

        modelBuilder.Entity<NgonNgu>(entity =>
        {
            entity.HasKey(e => e.MaNgonNgu).HasName("PK__NgonNgu__D859902319BDB6CF");

            entity.ToTable("NgonNgu");

            entity.Property(e => e.MaNgonNgu).HasColumnName("maNgonNgu");
            entity.Property(e => e.TenNgonNgu)
                .HasMaxLength(50)
                .HasColumnName("tenNgonNgu");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__446439EA255D413D");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.TenDangNhap, "UQ__NguoiDun__59267D4A9BB65F49").IsUnique();

            entity.Property(e => e.MaNguoiDung).HasColumnName("maNguoiDung");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.HoTen)
                .HasMaxLength(100)
                .HasColumnName("hoTen");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .HasColumnName("matKhau");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ngayTao");
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .HasColumnName("sdt");
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .HasColumnName("tenDangNhap");
            entity.Property(e => e.VaiTro)
                .HasMaxLength(20)
                .HasDefaultValue("khachhang")
                .HasColumnName("vaiTro");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SanPham__3213E83FE228DF31");

            entity.ToTable("SanPham");

            entity.HasIndex(e => e.Slug, "UQ__SanPham__32DD1E4CC3BC2C4E").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnhBia)
                .HasMaxLength(255)
                .HasColumnName("anhBia");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy).HasColumnName("createdBy");
            entity.Property(e => e.DanhMucId).HasColumnName("danhMucId");
            entity.Property(e => e.GiaGoc)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("giaGoc");
            entity.Property(e => e.GiaSauGiam)
                .HasComputedColumnSql("(case when [phanTramGiam]>(0) then [giaGoc]-([giaGoc]*[phanTramGiam])/(100) else [giaGoc] end)", false)
                .HasColumnType("decimal(28, 6)")
                .HasColumnName("giaSauGiam");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .HasColumnName("isbn");
            entity.Property(e => e.MetaDescription)
                .HasMaxLength(300)
                .HasColumnName("metaDescription");
            entity.Property(e => e.MetaKeyword)
                .HasMaxLength(300)
                .HasColumnName("metaKeyword");
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(200)
                .HasColumnName("metaTitle");
            entity.Property(e => e.MoTaChiTiet).HasColumnName("moTaChiTiet");
            entity.Property(e => e.MoTaNgan).HasColumnName("moTaNgan");
            entity.Property(e => e.NamXuatBan).HasColumnName("namXuatBan");
            entity.Property(e => e.NhaXuatBan)
                .HasMaxLength(200)
                .HasColumnName("nhaXuatBan");
            entity.Property(e => e.NoiBat)
                .HasDefaultValue(false)
                .HasColumnName("noiBat");
            entity.Property(e => e.PhanTramGiam)
                .HasDefaultValue(0)
                .HasColumnName("phanTramGiam");
            entity.Property(e => e.Slug)
                .HasMaxLength(250)
                .HasColumnName("slug");
            entity.Property(e => e.SoTrang).HasColumnName("soTrang");
            entity.Property(e => e.TacGia)
                .HasMaxLength(200)
                .HasColumnName("tacGia");
            entity.Property(e => e.TenSach)
                .HasMaxLength(200)
                .HasColumnName("tenSach");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SanPhamCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SanPham__created__24927208");

            entity.HasOne(d => d.DanhMuc).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.DanhMucId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__danhMuc__239E4DCF");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SanPhamUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__SanPham__updated__25869641");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Voucher__3213E83FFD17FA1C");

            entity.ToTable("Voucher");

            entity.HasIndex(e => e.MaCode, "UQ__Voucher__366294EBB1AEA233").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GiamGia)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("giamGia");
            entity.Property(e => e.Loai)
                .HasMaxLength(20)
                .HasColumnName("loai");
            entity.Property(e => e.MaCode)
                .HasMaxLength(50)
                .HasColumnName("maCode");
            entity.Property(e => e.NgayBatDau)
                .HasColumnType("datetime")
                .HasColumnName("ngayBatDau");
            entity.Property(e => e.NgayKetThuc)
                .HasColumnType("datetime")
                .HasColumnName("ngayKetThuc");
            entity.Property(e => e.SoLuong).HasColumnName("soLuong");
            entity.Property(e => e.TrangThai)
                .HasDefaultValue(true)
                .HasColumnName("trangThai");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
