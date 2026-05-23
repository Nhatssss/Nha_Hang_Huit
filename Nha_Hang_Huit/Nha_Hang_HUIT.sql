/*
============================================================================
  Nha_Hang_Huit - Database Schema
  .NET Framework 4.8 - WPF App | SQL Server (LocalDB / Express)
============================================================================
  Chay script nay tren master de tao database va bang.
  Connection string trong App.config: 
    Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=NhaHangHuit;Integrated Security=True
============================================================================
*/

-- ==================== TAO DATABASE ====================
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'NhaHangHuit')
BEGIN
    ALTER DATABASE NhaHangHuit SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE NhaHangHuit;
END
GO

CREATE DATABASE NhaHangHuit COLLATE Vietnamese_CI_AS;
GO
USE NhaHangHuit;
GO

-- ================================================================
-- PHAN 1: TAO BANG
-- ================================================================

-- tblNguoiDung: Dang nhap he thong (thay cho tblNhanVien cua schema cu)
CREATE TABLE tblNguoiDung (
    MaNguoiDung INT           NOT NULL IDENTITY(1,1),
    HoTen       NVARCHAR(100) NOT NULL,
    TaiKhoan    VARCHAR(50)   NOT NULL,
    MatKhau     VARCHAR(100)  NOT NULL,
    VaiTro      NVARCHAR(50)  NOT NULL DEFAULT N'NhanVien',  -- Admin / ThuNgan / PhucVu
    TrangThai   BIT           NOT NULL DEFAULT 1,
    CONSTRAINT PK_tblNguoiDung PRIMARY KEY (MaNguoiDung),
    CONSTRAINT UQ_NguoiDung_TK UNIQUE (TaiKhoan)
);
GO

-- tblMonAn: Menu mon an
CREATE TABLE tblMonAn (
    MaMonAn   INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    TenMonAn  NVARCHAR(200)   NOT NULL,
    Gia       DECIMAL(18,2)   NOT NULL,
    NhomMon   NVARCHAR(50)    NOT NULL,       -- Lo Nuong, Mon Nhung, Mon Them, Do Uong
    HinhAnh   NVARCHAR(500)   NULL,
    MoTa      NVARCHAR(500)   NULL,
    TrangThai NVARCHAR(20)    NOT NULL DEFAULT N'Con',   -- Con / Het
    NgayTao   DATETIME2       NOT NULL DEFAULT GETDATE()
);
GO

-- tblKhachHang: Thong tin khach hang + tich diem
CREATE TABLE tblKhachHang (
    MaKhachHang       INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
    TenKhachHang      NVARCHAR(200) NOT NULL,
    SoDienThoai       NVARCHAR(20)  NOT NULL,
    NgayDangKy        DATETIME2     NOT NULL DEFAULT GETDATE(),
    TongDiemTichLuy   INT           NOT NULL DEFAULT 0,
    HangThe           NVARCHAR(50)  NOT NULL DEFAULT N'Thuong'  -- Thuong / Bac / Vang / KimCuong
);
GO

-- tblHoaDon: Thong tin hoa don
CREATE TABLE tblHoaDon (
    MaHoaDon              INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
    MaKhachHang           INT           NULL,
    NgayTao               DATETIME2     NOT NULL DEFAULT GETDATE(),
    TongTien              DECIMAL(18,2) NOT NULL DEFAULT 0,
    TienGiamGia           DECIMAL(18,2) NOT NULL DEFAULT 0,
    ThanhTien             DECIMAL(18,2) NOT NULL DEFAULT 0,
    PhuongThucThanhToan   NVARCHAR(50)  NULL,           -- TienMat / QRCode
    TrangThai             NVARCHAR(50)  NOT NULL DEFAULT N'ChoThanhToan',  -- ChoThanhToan / DaThanhToan / DaHuy
    MaCa                  INT           NULL,
    GhiChu                NVARCHAR(500) NULL,
    FOREIGN KEY (MaKhachHang) REFERENCES tblKhachHang(MaKhachHang)
);
GO

-- tblChiTietHoaDon: Chi tiet tung mon trong hoa don
CREATE TABLE tblChiTietHoaDon (
    MaChiTiet   INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    MaHoaDon    INT             NOT NULL,
    MaMonAn     INT             NOT NULL,
    TenMonAn    NVARCHAR(200)   NOT NULL,      -- Snapshot ten mon tai thoi diem dat
    SoLuong     INT             NOT NULL DEFAULT 1,
    DonGia      DECIMAL(18,2)   NOT NULL,      -- Snapshot don gia tai thoi diem dat
    ThanhTien   DECIMAL(18,2)   NOT NULL,
    FOREIGN KEY (MaHoaDon) REFERENCES tblHoaDon(MaHoaDon) ON DELETE CASCADE,
    FOREIGN KEY (MaMonAn)  REFERENCES tblMonAn(MaMonAn)
);
GO

-- tblLichSuCa: Lich su dong ca
CREATE TABLE tblLichSuCa (
    MaCa                        INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    GioBatDau                   DATETIME2       NOT NULL DEFAULT GETDATE(),
    GioKetThuc                  DATETIME2       NULL,
    NhanVien                    NVARCHAR(200)   NOT NULL DEFAULT N'NhanVien',
    TongSoHoaDon                INT             NOT NULL DEFAULT 0,
    TongDoanhThuTruocGiamGia    DECIMAL(18,2)   NOT NULL DEFAULT 0,
    TongTienGiamGia             DECIMAL(18,2)   NOT NULL DEFAULT 0,
    TongDoanhThuThucNhan        DECIMAL(18,2)   NOT NULL DEFAULT 0,
    SoKhachMoi                  INT             NOT NULL DEFAULT 0,
    GhiChu                      NVARCHAR(500)   NULL
);
GO

-- ================================================================
-- PHAN 2: INDEX
-- ================================================================
CREATE INDEX IX_MonAn_NhomMon   ON tblMonAn(NhomMon);
CREATE INDEX IX_MonAn_TrangThai ON tblMonAn(TrangThai);
CREATE INDEX IX_KhachHang_SDT   ON tblKhachHang(SoDienThoai);
CREATE INDEX IX_HoaDon_MaCa     ON tblHoaDon(MaCa);
CREATE INDEX IX_HoaDon_TrangThai ON tblHoaDon(TrangThai);
CREATE INDEX IX_ChiTiet_HoaDon  ON tblChiTietHoaDon(MaHoaDon);
GO

-- ================================================================
-- PHAN 3: STORED PROCEDURES
-- ================================================================

-- spDangNhap: Kiem tra dang nhap
CREATE PROCEDURE spDangNhap
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

-- spCapNhatDiemKhachHang: Cap nhat diem tich luy va hang the
CREATE PROCEDURE spCapNhatDiemKhachHang
    @MaKhachHang INT,
    @TongTien DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DiemCong INT;
    SET @DiemCong = FLOOR(@TongTien / 10000);

    UPDATE tblKhachHang
    SET TongDiemTichLuy = TongDiemTichLuy + @DiemCong
    WHERE MaKhachHang = @MaKhachHang;

    -- Cap nhat hang the
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

-- spThongKeTopMonBanChay: Top 5 mon ban chay trong ca
CREATE PROCEDURE spThongKeTopMonBanChay
    @MaCa INT
AS
BEGIN
    SET NOCOUNT ON;
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

-- spHuyHoaDonCho: Huy cac hoa don bi bo quen (orphan) - goi khi mo ca hoac dinh ky
CREATE PROCEDURE spHuyHoaDonCho
    @MaCa INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE tblHoaDon
    SET TrangThai = N'DaHuy', GhiChu = N'Tu dong huy (qua han)'
    WHERE TrangThai = N'ChoThanhToan'
      AND NgayTao < DATEADD(MINUTE, -30, GETDATE())
      AND (@MaCa IS NULL OR MaCa = @MaCa);
END
GO

-- ================================================================
-- PHAN 4: DU LIEU MAU (SEED DATA)
-- ================================================================

-- Nguoi dung mac dinh
INSERT INTO tblNguoiDung (HoTen, TaiKhoan, MatKhau, VaiTro) VALUES
(N'Admin',       'admin',     '123456', N'Admin'),
(N'Nguyen Van A', 'nv01',    '123456', N'PhucVu'),
(N'Tran Thi B',   'nv02',    '123456', N'ThuNgan');
GO

-- Mon an mac dinh
IF NOT EXISTS (SELECT 1 FROM tblMonAn)
BEGIN
    INSERT INTO tblMonAn (TenMonAn, Gia, NhomMon, MoTa, TrangThai) VALUES
    -- Nhom Lo Nuong
    (N'Lau Bo Nhung Giat',      299000, N'Lo Nuong',   N'Lau bo nhung gat Haidilao chinh goc', N'Con'),
    (N'Lau Cay Szechuan',       259000, N'Lo Nuong',   N'Lau cay phong cach Tu Xuyen', N'Con'),
    (N'Lau Hai San Tong Hop',   399000, N'Lo Nuong',   N'Lau hai san tom, muc, ngao, ca', N'Con'),
    (N'Lau Nam Huong',           239000, N'Lo Nuong',   N'Lau nam huong thanh mat', N'Con'),
    (N'Lau Ca Chua',            219000, N'Lo Nuong',   N'Lau ca chua chua ngot', N'Con'),
    -- Nhom Mon Nhung
    (N'Thit Bo My Nhung Giat',  199000, N'Mon Nhung',  N'Thit bo My nhung gat tuoi ngon', N'Con'),
    (N'Thit Heo Nhung Giat',    149000, N'Mon Nhung',  N'Thit heo nhung gat thom ngon', N'Con'),
    (N'Tom Su Nhung Giat',      179000, N'Mon Nhung',  N'Tom su tuoi nhung gat', N'Con'),
    (N'Muc Nhung Giat',         169000, N'Mon Nhung',  N'Muc tuoi nhung gat gion ngot', N'Con'),
    (N'Rau Cu Tong Hop',        89000,  N'Mon Nhung',  N'Rau cu cac loai nhung lau', N'Con'),
    (N'Vien Ca Hoi',            99000,  N'Mon Nhung',  N'Vien ca hoi tuoi ngon', N'Con'),
    -- Nhom Mon Them
    (N'Com Trang',              10000,  N'Mon Them',   N'Com trang nau mem', N'Con'),
    (N'Mi Udon',                35000,  N'Mon Them',   N'Mi Udon Nhat Ban', N'Con'),
    (N'Mi Trung',               25000,  N'Mon Them',   N'Mi trung tuoi', N'Con'),
    (N'Banh Trang Cuon',        29000,  N'Mon Them',   N'Banh trang cuon thit luon', N'Con'),
    (N'Khoai Tay Chien',        49000,  N'Mon Them',   N'Khoai tay chien gion', N'Con'),
    -- Nhom Do Uong
    (N'Coca Cola',              15000,  N'Do Uong',    N'Coca Cola lon 330ml', N'Con'),
    (N'Nuoc Ep Cam',            35000,  N'Do Uong',    N'Nuoc ep cam tuoi nguyen chat', N'Con'),
    (N'Tra Dao',                29000,  N'Do Uong',    N'Tra dao house made', N'Con'),
    (N'Bia Tiger',              25000,  N'Do Uong',    N'Bia Tiger lon 330ml', N'Con');
END
GO

-- ================================================================
-- PHAN 5: KIEM TRA
-- ================================================================
SELECT 'OK' AS 'DATABASE READY', DB_NAME() AS DB_NAME;
PRINT N'=== NhaHangHuit da tao thanh cong! ===';
GO
