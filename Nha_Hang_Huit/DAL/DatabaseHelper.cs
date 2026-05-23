using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Nha_Hang_Huit.DAL
{
    /// <summary>
    /// Lop helper xu ly ket noi SQL Server
    /// Doc chuoi ket noi tu App.config
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Lay chuoi ket noi tu file App.config
        /// </summary>
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["NhaHangHuit"].ConnectionString;
        }

        /// <summary>
        /// Tao va mo mot ket noi SQL moi
        /// </summary>
        public static SqlConnection GetConnection()
        {
            var conn = new SqlConnection(GetConnectionString());
            if (conn.State != ConnectionState.Open)
                conn.Open();
            return conn;
        }

        /// <summary>
        /// Thuc thi cau lenh INSERT/UPDATE/DELETE, tra ve so dong bi anh huong
        /// </summary>
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Thuc thi cau lenh SELECT, tra ve DataTable
        /// </summary>
        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Thuc thi cau lenh va tra ve gia tri dau tien (voi SCOPE_IDENTITY)
        /// </summary>
        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }
    }
}
