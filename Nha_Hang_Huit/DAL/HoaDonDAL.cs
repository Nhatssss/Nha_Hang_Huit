using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.DAL
{
    /// <summary>
    /// Lop DAL xu ly du lieu hoa don va chi tiet hoa don
    /// </summary>
    public class HoaDonDAL
    {
        // ==================== HOA DON ====================

        /// <summary>
        /// Lay hoa don theo MaHoaDon
        /// </summary>
        public HoaDon GetById(int maHoaDon)
        {
            string query = "SELECT * FROM tblHoaDon WHERE MaHoaDon = @MaHoaDon";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaHoaDon", maHoaDon)
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
                return MapRowToHoaDon(dt.Rows[0]);
            return null;
        }

        /// <summary>
        /// Lay danh sach hoa don trong mot ca
        /// </summary>
        public List<HoaDon> GetByCa(int maCa)
        {
            var list = new List<HoaDon>();
            string query = "SELECT * FROM tblHoaDon WHERE MaCa = @MaCa ORDER BY NgayTao";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaCa", maCa)
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToHoaDon(row));
            return list;
        }

        /// <summary>
        /// Lay danh sach hoa don da thanh toan trong ca (de bao cao)
        /// </summary>
        public List<HoaDon> GetDaThanhToanByCa(int maCa)
        {
            var list = new List<HoaDon>();
            string query = "SELECT * FROM tblHoaDon WHERE MaCa = @MaCa AND TrangThai = N'DaThanhToan' ORDER BY NgayTao";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaCa", maCa)
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToHoaDon(row));
            return list;
        }

        /// <summary>
        /// Tao hoa don moi (trang thai ChoThanhToan), tra ve MaHoaDon
        /// </summary>
        public int Insert(HoaDon hd)
        {
            string query = @"INSERT INTO tblHoaDon (MaKhachHang, NgayTao, TongTien, TienGiamGia, ThanhTien, TrangThai, MaCa)
                             VALUES (@MaKhachHang, GETDATE(), @TongTien, @TienGiamGia, @ThanhTien, N'ChoThanhToan', @MaCa);
                             SELECT SCOPE_IDENTITY();";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaKhachHang", (object)hd.MaKhachHang ?? DBNull.Value),
                new SqlParameter("@TongTien", hd.TongTien),
                new SqlParameter("@TienGiamGia", hd.TienGiamGia),
                new SqlParameter("@ThanhTien", hd.ThanhTien),
                new SqlParameter("@MaCa", (object)hd.MaCa ?? DBNull.Value)
            };
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// Cap nhat trang thai hoa don va phuong thuc thanh toan
        /// </summary>
        public int UpdateTrangThai(int maHoaDon, string trangThai, string phuongThucThanhToan)
        {
            string query = @"UPDATE tblHoaDon SET TrangThai=@TrangThai, PhuongThucThanhToan=@PhuongThuc
                             WHERE MaHoaDon=@MaHoaDon";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaHoaDon", maHoaDon),
                new SqlParameter("@TrangThai", trangThai),
                new SqlParameter("@PhuongThuc", (object)phuongThucThanhToan ?? DBNull.Value)
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Cap nhat trang thai hoa don thanh DaHuy (fix orphan invoice)
        /// </summary>
        public int UpdateTrangThaiHuy(int maHoaDon, string ghiChu)
        {
            string query = @"UPDATE tblHoaDon SET TrangThai=N'DaHuy', GhiChu=@GhiChu
                             WHERE MaHoaDon=@MaHoaDon AND TrangThai=N'ChoThanhToan'";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaHoaDon", maHoaDon),
                new SqlParameter("@GhiChu", (object)ghiChu ?? DBNull.Value)
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Lay MaCa hien tai (ca dang mo)
        /// </summary>
        public int? GetCurrentCa()
        {
            string query = "SELECT TOP 1 MaCa FROM tblLichSuCa WHERE GioKetThuc IS NULL ORDER BY MaCa DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0]["MaCa"]);
            return null;
        }

        // ==================== CHI TIET HOA DON ====================

        /// <summary>
        /// Lay danh sach chi tiet hoa don theo MaHoaDon
        /// </summary>
        public List<ChiTietHoaDon> GetChiTietByHoaDon(int maHoaDon)
        {
            var list = new List<ChiTietHoaDon>();
            string query = "SELECT * FROM tblChiTietHoaDon WHERE MaHoaDon = @MaHoaDon";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaHoaDon", maHoaDon)
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToChiTiet(row));
            return list;
        }

        /// <summary>
        /// Them chi tiet hoa don
        /// </summary>
        public int InsertChiTiet(ChiTietHoaDon ct)
        {
            string query = @"INSERT INTO tblChiTietHoaDon (MaHoaDon, MaMonAn, TenMonAn, SoLuong, DonGia, ThanhTien)
                             VALUES (@MaHoaDon, @MaMonAn, @TenMonAn, @SoLuong, @DonGia, @ThanhTien);
                             SELECT SCOPE_IDENTITY();";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaHoaDon", ct.MaHoaDon),
                new SqlParameter("@MaMonAn", ct.MaMonAn),
                new SqlParameter("@TenMonAn", ct.TenMonAn),
                new SqlParameter("@SoLuong", ct.SoLuong),
                new SqlParameter("@DonGia", ct.DonGia),
                new SqlParameter("@ThanhTien", ct.ThanhTien)
            };
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// Chuyen dong DataRow sang HoaDon
        /// </summary>
        private HoaDon MapRowToHoaDon(DataRow row)
        {
            return new HoaDon
            {
                MaHoaDon = Convert.ToInt32(row["MaHoaDon"]),
                MaKhachHang = row["MaKhachHang"] != DBNull.Value ? Convert.ToInt32(row["MaKhachHang"]) : (int?)null,
                NgayTao = Convert.ToDateTime(row["NgayTao"]),
                TongTien = Convert.ToDecimal(row["TongTien"]),
                TienGiamGia = Convert.ToDecimal(row["TienGiamGia"]),
                ThanhTien = Convert.ToDecimal(row["ThanhTien"]),
                PhuongThucThanhToan = row["PhuongThucThanhToan"] != DBNull.Value ? row["PhuongThucThanhToan"].ToString() : null,
                TrangThai = row["TrangThai"].ToString(),
                MaCa = row["MaCa"] != DBNull.Value ? Convert.ToInt32(row["MaCa"]) : (int?)null,
                GhiChu = row["GhiChu"] != DBNull.Value ? row["GhiChu"].ToString() : null
            };
        }

        /// <summary>
        /// Chuyen dong DataRow sang ChiTietHoaDon
        /// </summary>
        private ChiTietHoaDon MapRowToChiTiet(DataRow row)
        {
            return new ChiTietHoaDon
            {
                MaChiTiet = Convert.ToInt32(row["MaChiTiet"]),
                MaHoaDon = Convert.ToInt32(row["MaHoaDon"]),
                MaMonAn = Convert.ToInt32(row["MaMonAn"]),
                TenMonAn = row["TenMonAn"].ToString(),
                SoLuong = Convert.ToInt32(row["SoLuong"]),
                DonGia = Convert.ToDecimal(row["DonGia"])
            };
        }
    }
}
