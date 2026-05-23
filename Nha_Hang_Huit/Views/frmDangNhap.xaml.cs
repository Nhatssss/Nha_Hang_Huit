using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using Nha_Hang_Huit.DAL;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Form dang nhap he thong — kiem tra tu database tblNguoiDung
    /// Tai khoan mac dinh: admin / 123456  |  nv01 / 123456  |  nv02 / 123456
    /// </summary>
    public partial class frmDangNhap : Window
    {
        private int maNguoiDung;
        private string vaiTro;

        public frmDangNhap()
        {
            InitializeComponent();
        }

        private void btnDangNhap_Click(object sender, RoutedEventArgs e)
        {
            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Password;

            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                lblThongBao.Text = "Vui long nhap ten dang nhap va mat khau!";
                lblThongBao.Visibility = Visibility.Visible;
                return;
            }

            DataTable dt = DangNhap(tenDangNhap, matKhau);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                maNguoiDung = Convert.ToInt32(row["MaNguoiDung"]);
                vaiTro = row["VaiTro"].ToString();
                string hoTen = row["HoTen"].ToString();

                var frmChinh = new frmChinh();
                frmChinh.Show();
                this.Close();
            }
            else
            {
                lblThongBao.Text = "Ten dang nhap hoac mat khau khong dung!";
                lblThongBao.Visibility = Visibility.Visible;
            }
        }

        private DataTable DangNhap(string taiKhoan, string matKhau)
        {
            try
            {
                string query = "spDangNhap";
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TaiKhoan", taiKhoan);
                    cmd.Parameters.AddWithValue("@MatKhau", matKhau);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi dang nhap: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
