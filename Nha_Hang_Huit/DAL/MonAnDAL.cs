using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.DAL
{
    /// <summary>
    /// Lop DAL xu ly du lieu bang tblMonAn
    /// </summary>
    public class MonAnDAL
    {
        /// <summary>
        /// Lay tat ca mon an tu database
        /// </summary>
        public List<MonAn> GetAll()
        {
            var list = new List<MonAn>();
            string query = "SELECT * FROM tblMonAn ORDER BY NhomMon, TenMonAn";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToMonAn(row));
            }
            return list;
        }

        /// <summary>
        /// Lay danh sach mon an theo nhom
        /// </summary>
        public List<MonAn> GetByNhom(string nhomMon)
        {
            var list = new List<MonAn>();
            string query = "SELECT * FROM tblMonAn WHERE NhomMon = @NhomMon AND TrangThai = N'Con' ORDER BY TenMonAn";
            var parameters = new SqlParameter[] {
                new SqlParameter("@NhomMon", nhomMon)
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToMonAn(row));
            }
            return list;
        }

        /// <summary>
        /// Lay tat ca nhom mon (de phan loai)
        /// </summary>
        public List<string> GetAllNhomMon()
        {
            var list = new List<string>();
            string query = "SELECT DISTINCT NhomMon FROM tblMonAn ORDER BY NhomMon";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(row["NhomMon"].ToString());
            }
            return list;
        }

        /// <summary>
        /// Lay mon an theo MaMonAn
        /// </summary>
        public MonAn GetById(int maMonAn)
        {
            string query = "SELECT * FROM tblMonAn WHERE MaMonAn = @MaMonAn";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaMonAn", maMonAn)
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
                return MapRowToMonAn(dt.Rows[0]);
            return null;
        }

        /// <summary>
        /// Them mon an moi
        /// </summary>
        public int Insert(MonAn monAn)
        {
            string query = @"INSERT INTO tblMonAn (TenMonAn, Gia, NhomMon, HinhAnh, MoTa, TrangThai)
                             VALUES (@TenMonAn, @Gia, @NhomMon, @HinhAnh, @MoTa, @TrangThai);
                             SELECT SCOPE_IDENTITY();";
            var parameters = new SqlParameter[] {
                new SqlParameter("@TenMonAn", monAn.TenMonAn),
                new SqlParameter("@Gia", monAn.Gia),
                new SqlParameter("@NhomMon", monAn.NhomMon),
                new SqlParameter("@HinhAnh", (object)monAn.HinhAnh ?? DBNull.Value),
                new SqlParameter("@MoTa", (object)monAn.MoTa ?? DBNull.Value),
                new SqlParameter("@TrangThai", monAn.TrangThai ?? "Con")
            };
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// Cap nhat thong tin mon an
        /// </summary>
        public int Update(MonAn monAn)
        {
            string query = @"UPDATE tblMonAn SET TenMonAn=@TenMonAn, Gia=@Gia, NhomMon=@NhomMon,
                             HinhAnh=@HinhAnh, MoTa=@MoTa, TrangThai=@TrangThai
                             WHERE MaMonAn=@MaMonAn";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaMonAn", monAn.MaMonAn),
                new SqlParameter("@TenMonAn", monAn.TenMonAn),
                new SqlParameter("@Gia", monAn.Gia),
                new SqlParameter("@NhomMon", monAn.NhomMon),
                new SqlParameter("@HinhAnh", (object)monAn.HinhAnh ?? DBNull.Value),
                new SqlParameter("@MoTa", (object)monAn.MoTa ?? DBNull.Value),
                new SqlParameter("@TrangThai", monAn.TrangThai ?? "Con")
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Xoa mon an theo MaMonAn (chuyen trang thai Het thay vi xoa that)
        /// de tranh loi FK constraint voi tblChiTietHoaDon
        /// </summary>
        public int Delete(int maMonAn)
        {
            // Soft-delete: chuyen trang thai ve "Het" thay vi DELETE
            // tranh vi pham khoa ngoai voi tblChiTietHoaDon
            string query = "UPDATE tblMonAn SET TrangThai=N'Het' WHERE MaMonAn = @MaMonAn";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaMonAn", maMonAn)
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Chuyen dong DataRow sang doi tuong MonAn
        /// </summary>
        private MonAn MapRowToMonAn(DataRow row)
        {
            return new MonAn
            {
                MaMonAn = Convert.ToInt32(row["MaMonAn"]),
                TenMonAn = row["TenMonAn"].ToString(),
                Gia = Convert.ToDecimal(row["Gia"]),
                NhomMon = row["NhomMon"].ToString(),
                HinhAnh = row["HinhAnh"] != DBNull.Value ? row["HinhAnh"].ToString() : null,
                MoTa = row["MoTa"] != DBNull.Value ? row["MoTa"].ToString() : null,
                TrangThai = row["TrangThai"].ToString(),
                NgayTao = Convert.ToDateTime(row["NgayTao"])
            };
        }
    }
}
