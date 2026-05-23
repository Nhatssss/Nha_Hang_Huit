using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.DAL
{
    /// <summary>
    /// Lop DAL xu ly du lieu bang tblKhachHang
    /// </summary>
    public class KhachHangDAL
    {
        /// <summary>
        /// Lay tat ca khach hang
        /// </summary>
        public List<KhachHang> GetAll()
        {
            var list = new List<KhachHang>();
            string query = "SELECT * FROM tblKhachHang ORDER BY TenKhachHang";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToKhachHang(row));
            return list;
        }

        /// <summary>
        /// Tim kiem khach hang theo so dien thoai
        /// </summary>
        public KhachHang GetByPhone(string soDienThoai)
        {
            string query = "SELECT * FROM tblKhachHang WHERE SoDienThoai = @SoDienThoai";
            var parameters = new SqlParameter[] {
                new SqlParameter("@SoDienThoai", soDienThoai)
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
                return MapRowToKhachHang(dt.Rows[0]);
            return null;
        }

        /// <summary>
        /// Lay khach hang theo MaKhachHang
        /// </summary>
        public KhachHang GetById(int maKhachHang)
        {
            string query = "SELECT * FROM tblKhachHang WHERE MaKhachHang = @MaKhachHang";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaKhachHang", maKhachHang)
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
                return MapRowToKhachHang(dt.Rows[0]);
            return null;
        }

        /// <summary>
        /// Them khach hang moi
        /// </summary>
        public int Insert(KhachHang kh)
        {
            string query = @"INSERT INTO tblKhachHang (TenKhachHang, SoDienThoai, NgayDangKy, TongDiemTichLuy, HangThe)
                             VALUES (@TenKhachHang, @SoDienThoai, GETDATE(), 0, N'Thuong');
                             SELECT SCOPE_IDENTITY();";
            var parameters = new SqlParameter[] {
                new SqlParameter("@TenKhachHang", kh.TenKhachHang),
                new SqlParameter("@SoDienThoai", kh.SoDienThoai)
            };
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// Cap nhat diem tich luy cho khach hang (goi spCapNhatDiemKhachHang)
        /// </summary>
        public void CapNhatDiem(int maKhachHang, decimal tongTien)
        {
            string query = "spCapNhatDiemKhachHang";
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                cmd.Parameters.AddWithValue("@TongTien", tongTien);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Cap nhat thong tin khach hang
        /// </summary>
        public int Update(KhachHang kh)
        {
            string query = @"UPDATE tblKhachHang SET TenKhachHang=@TenKhachHang, SoDienThoai=@SoDienThoai,
                             TongDiemTichLuy=@TongDiem, HangThe=@HangThe
                             WHERE MaKhachHang=@MaKhachHang";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaKhachHang", kh.MaKhachHang),
                new SqlParameter("@TenKhachHang", kh.TenKhachHang),
                new SqlParameter("@SoDienThoai", kh.SoDienThoai),
                new SqlParameter("@TongDiem", kh.TongDiemTichLuy),
                new SqlParameter("@HangThe", kh.HangThe)
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Lay so luong khach moi trong mot ca
        /// </summary>
        public int DemKhachMoiTrongCa(DateTime gioBatDau, DateTime gioKetThuc)
        {
            string query = "SELECT COUNT(*) FROM tblKhachHang WHERE NgayDangKy >= @Tu AND NgayDangKy <= @Den";
            var parameters = new SqlParameter[] {
                new SqlParameter("@Tu", gioBatDau),
                new SqlParameter("@Den", gioKetThuc)
            };
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// Chuyen dong DataRow sang doi tuong KhachHang
        /// </summary>
        private KhachHang MapRowToKhachHang(DataRow row)
        {
            return new KhachHang
            {
                MaKhachHang = Convert.ToInt32(row["MaKhachHang"]),
                TenKhachHang = row["TenKhachHang"].ToString(),
                SoDienThoai = row["SoDienThoai"].ToString(),
                NgayDangKy = Convert.ToDateTime(row["NgayDangKy"]),
                TongDiemTichLuy = Convert.ToInt32(row["TongDiemTichLuy"]),
                HangThe = row["HangThe"].ToString()
            };
        }
    }
}
