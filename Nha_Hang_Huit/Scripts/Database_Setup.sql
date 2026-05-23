/*
  Database Setup Script for Nha_Hang_Huit
  .NET Framework 4.8 - WPF App
  SQL Server (LocalDB or Express)

  === DONG BO VOI Nha_Hang_HUIT.sql ===
  Chi chay script nay khi can setup lai tu dau,
  hoac them bang moi vao database hien co.
*/

-- ==================== TAO DATABASE ====================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'NhaHangHuit')
BEGIN
    CREATE DATABASE NhaHangHuit;
END
GO

USE NhaHangHuit;
GO

-- ==================== BANG MOI: tblNguoiDung ====================
-- Dung cho dang nhap he thong
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblNguoiDung]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[tblNguoiDung] (
        [MaNguoiDung] INT IDENTITY(1,1) PRIMARY KEY,
        [HoTen]       NVARCHAR(100) NOT NULL,
        [TaiKhoan]    VARCHAR(50)   NOT NULL,
        [MatKhau]     VARCHAR(100)  NOT NULL,
        [VaiTro]      NVARCHAR(50)  NOT NULL DEFAULT N'NhanVien',
        [TrangThai]   BIT           NOT NULL DEFAULT 1,
        CONSTRAINT UQ_NguoiDung_TK UNIQUE (TaiKhoan)
    );

    -- Nguoi dung mac dinh
    INSERT INTO [dbo].[tblNguoiDung] (HoTen, TaiKhoan, MatKhau, VaiTro) VALUES
    (N'Admin',       'admin',  '123456', N'Admin'),
    (N'Nguyen Van A', 'nv01', '123456', N'PhucVu'),
    (N'Tran Thi B',   'nv02', '123456', N'ThuNgan');
END
GO

-- ============================================
-- Bang tblMonAn: Menu mon an
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblMonAn]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[tblMonAn] (
        [MaMonAn]       INT IDENTITY(1,1) PRIMARY KEY,
        [TenMonAn]      NVARCHAR(200)   NOT NULL,
        [Gia]           DECIMAL(18,2)   NOT NULL,
        [NhomMon]       NVARCHAR(50)    NOT NULL,
        [HinhAnh]       NVARCHAR(500)   NULL,
        [MoTa]          NVARCHAR(500)   NULL,
        [TrangThai]     NVARCHAR(20)    NOT NULL DEFAULT N'Con',
        [NgayTao]       DATETIME2       NOT NULL DEFAULT GETDATE()
    );

    -- Du lieu mau
    INSERT INTO [dbo].[tblMonAn] (TenMonAn, Gia, NhomMon, MoTa, TrangThai) VALUES
    (N'Lau Bo Nhung Giat',      299000, N'Lo Nuong',   N'Lau bo nhung gat Haidilao chinh goc', N'Con'),
    (N'Lau Cay Szechuan',       259000, N'Lo Nuong',   N'Lau cay phong cach Tu Xuyen', N'Con'),
    (N'Lau Hai San Tong Hop',   399000, N'Lo Nuong',   N'Lau hai san tom, muc, ngao, ca', N'Con'),
    (N'Lau Nam Huong',          239000, N'Lo Nuong',   N'Lau nam huong thanh mat', N'Con'),
    (N'Lau Ca Chua',            219000, N'Lo Nuong',   N'Lau ca chua chua ngot', N'Con'),
    (N'Thit Bo My Nhung Giat',  199000, N'Mon Nhung',  N'Thit bo My nhung gat tuoi ngon', N'Con'),
    (N'Thit Heo Nhung Giat',    149000, N'Mon Nhung',  N'Thit heo nhung gat thom ngon', N'Con'),
    (N'Tom Su Nhung Giat',      179000, N'Mon Nhung',  N'Tom su tuoi nhung gat', N'Con'),
    (N'Muc Nhung Giat',         169000, N'Mon Nhung',  N'Muc tuoi nhung gat gion ngot', N'Con'),
    (N'Rau Cu Tong Hop',        89000,  N'Mon Nhung',  N'Rau cu cac loai nhung lau', N'Con'),
    (N'Vien Ca Hoi',            99000,  N'Mon Nhung',  N'Vien ca hoi tuoi ngon', N'Con'),
    (N'Com Trang',              10000,  N'Mon Them',   N'Com trang nau mem', N'Con'),
    (N'Mi Udon',                35000,  N'Mon Them',   N'Mi Udon Nhat Ban', N'Con'),
    (N'Mi Trung',               25000,  N'Mon Them',   N'Mi trung tuoi', N'Con'),
    (N'Banh Trang Cuon',        29000,  N'Mon Them',   N'Banh trang cuon thit luon', N'Con'),
    (N'Khoai Tay Chien',        49000,  N'Mon Them',   N'Khoai tay chien gion', N'Con'),
    (N'Coca Cola',              15000,  N'Do Uong',    N'Coca Cola lon 330ml', N'Con'),
    (N'Nuoc Ep Cam',            35000,  N'Do Uong',    N'Nuoc ep cam tuoi nguyen chat', N'Con'),
    (N'Tra Dao',                29000,  N'Do Uong',    N'Tra dao house made', N'Con'),
    (N'Bia Tiger',              25000,  N'Do Uong',    N'Bia Tiger lon 330ml', N'Con');
END
GO

-- ============================================
-- Bang tblKhachHang
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblKhachHang]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[tblKhachHang] (
        [MaKhachHang]       INT IDENTITY(1,1) PRIMARY KEY,
        [TenKhachHang]      NVARCHAR(200)   NOT NULL,
        [SoDienThoai]       NVARCHAR(20)    NOT NULL,
        [NgayDangKy]        DATETIME2       NOT NULL DEFAULT GETDATE(),
        [TongDiemTichLuy]   INT             NOT NULL DEFAULT 0,
        [HangThe]           NVARCHAR(50)    NOT NULL DEFAULT N'Thuong'
    );
END
GO

-- ============================================
-- Bang tblHoaDon
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblHoaDon]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[tblHoaDon] (
        [MaHoaDon]          INT IDENTITY(1,1) PRIMARY KEY,
        [MaKhachHang]       INT             NULL,
        [NgayTao]           DATETIME2       NOT NULL DEFAULT GETDATE(),
        [TongTien]          DECIMAL(18,2)   NOT NULL DEFAULT 0,
        [TienGiamGia]       DECIMAL(18,2)   NOT NULL DEFAULT 0,
        [ThanhTien]         DECIMAL(18,2)   NOT NULL DEFAULT 0,
        [PhuongThucThanhToan] NVARCHAR(50)  NULL,
        [TrangThai]         NVARCHAR(50)    NOT NULL DEFAULT N'ChoThanhToan',
        [MaCa]              INT             NULL,
        [GhiChu]            NVARCHAR(500)   NULL,
        FOREIGN KEY ([MaKhachHang]) REFERENCES [dbo].[tblKhachHang]([MaKhachHang])
    );
END
GO

-- ============================================
-- Bang tblChiTietHoaDon
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblChiTietHoaDon]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[tblChiTietHoaDon] (
        [MaChiTiet]     INT IDENTITY(1,1) PRIMARY KEY,
        [MaHoaDon]      INT             NOT NULL,
        [MaMonAn]       INT             NOT NULL,
        [TenMonAn]      NVARCHAR(200)   NOT NULL,
        [SoLuong]       INT             NOT NULL DEFAULT 1,
        [DonGia]        DECIMAL(18,2)   NOT NULL,
        [ThanhTien]     DECIMAL(18,2)   NOT NULL,
        FOREIGN KEY ([MaHoaDon]) REFERENCES [dbo].[tblHoaDon]([MaHoaDon]) ON DELETE CASCADE,
        FOREIGN KEY ([MaMonAn]) REFERENCES [dbo].[tblMonAn]([MaMonAn])
    );
END
GO

-- ============================================
-- Bang tblLichSuCa
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblLichSuCa]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[tblLichSuCa] (
        [MaCa]              INT IDENTITY(1,1) PRIMARY KEY,
        [GioBatDau]         DATETIME2       NOT NULL DEFAULT GETDATE(),
        [GioKetThuc]        DATETIME2       NULL,
        [NhanVien]          NVARCHAR(200)   NOT NULL DEFAULT N'NhanVien',
        [TongSoHoaDon]      INT             NOT NULL DEFAULT 0,
        [TongDoanhThuTruocGiamGia] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [TongTienGiamGia]   DECIMAL(18,2)   NOT NULL DEFAULT 0,
        [TongDoanhThuThucNhan] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [SoKhachMoi]        INT             NOT NULL DEFAULT 0,
        [GhiChu]            NVARCHAR(500)   NULL
    );
END
GO

-- ============================================
-- Stored Procedures
-- ============================================

-- spDangNhap
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spDangNhap]') AND type = 'P')
    DROP PROCEDURE [dbo].[spDangNhap]
GO
CREATE PROCEDURE [dbo].[spDangNhap]
    @TaiKhoan VARCHAR(50),
    @MatKhau VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaNguoiDung, HoTen, TaiKhoan, VaiTro
    FROM tblNguoiDung
    WHERE TaiKhoan = @TaiKhoan AND MatKhau = @MatKhau AND TrangThai = 1;
END
GO

-- spCapNhatDiemKhachHang
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spCapNhatDiemKhachHang]') AND type = 'P')
    DROP PROCEDURE [dbo].[spCapNhatDiemKhachHang]
GO
CREATE PROCEDURE [dbo].[spCapNhatDiemKhachHang]
    @MaKhachHang INT,
    @TongTien DECIMAL(18,2)
AS
BEGIN
    DECLARE @DiemCong INT;
    SET @DiemCong = FLOOR(@TongTien / 10000);
    
    UPDATE tblKhachHang
    SET TongDiemTichLuy = TongDiemTichLuy + @DiemCong
    WHERE MaKhachHang = @MaKhachHang;
    
    UPDATE tblKhachHang
    SET HangThe = CASE
        WHEN TongDiemTichLuy >= 1000 THEN N'KimCuong'
        WHEN TongDiemTichLuy >= 500  THEN N'Vang'
        WHEN TongDiemTichLuy >= 100  THEN N'Bac'
        ELSE N'Thuong'
    END
    WHERE MaKhachHang = @MaKhachHang;
END
GO

-- spThongKeTopMonBanChay
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spThongKeTopMonBanChay]') AND type = 'P')
    DROP PROCEDURE [dbo].[spThongKeTopMonBanChay]
GO
CREATE PROCEDURE [dbo].[spThongKeTopMonBanChay]
    @MaCa INT
AS
BEGIN
    SELECT TOP 5
        ctd.TenMonAn,
        SUM(ctd.SoLuong)   AS TongSoLuong,
        SUM(ctd.ThanhTien) AS TongDoanhThu
    FROM tblChiTietHoaDon ctd
    INNER JOIN tblHoaDon hd ON ctd.MaHoaDon = hd.MaHoaDon
    WHERE hd.MaCa = @MaCa AND hd.TrangThai = N'DaThanhToan'
    GROUP BY ctd.TenMonAn
    ORDER BY TongSoLuong DESC;
END
GO

PRINT N'=== Database setup complete! ===';
GO
