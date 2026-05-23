using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using Nha_Hang_Huit.DAL;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// Lop BLL xu ly bao cao dong ca va thong ke
    /// </summary>
    public class BaoCaoBLL
    {
        /// <summary>
        /// Mo ca moi: them ban ghi vao tblLichSuCa, tra ve MaCa
        /// </summary>
        public int MoCa(string nhanVien = "NhanVien")
        {
            try
            {
                // Kiem tra ca dang mo
                var hoaDonDAL = new HoaDonDAL();
                int? caHienTai = hoaDonDAL.GetCurrentCa();
                if (caHienTai.HasValue)
                {
                    MessageBox.Show("Da co ca dang mo (MaCa=" + caHienTai + ")!", "Thong bao",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return caHienTai.Value;
                }

                string query = "INSERT INTO tblLichSuCa (GioBatDau, NhanVien) VALUES (GETDATE(), @NhanVien); SELECT SCOPE_IDENTITY();";
                var parameters = new SqlParameter[] {
                    new SqlParameter("@NhanVien", nhanVien)
                };
                return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi mo ca: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }

        /// <summary>
        /// Dong ca: tinh toan thong ke va cap nhat vao DB
        /// Tra ve LichSuCa da duoc cap nhat day du neu thanh cong, null neu that bai
        /// Fix: Dung thoi gian dong ca nhat quan cho ca update lan dem khach moi
        /// </summary>
        public LichSuCa DongCa(int maCa)
        {
            try
            {
                var hoaDonDAL = new HoaDonDAL();
                var khachHangDAL = new KhachHangDAL();
                var caInfo = GetCaInfo(maCa);
                if (caInfo == null) return null;

                // Lay danh sach hoa don da thanh toan trong ca
                var hoaDons = hoaDonDAL.GetDaThanhToanByCa(maCa);
                int tongHoaDon = hoaDons.Count;
                decimal tongDoanhThuTruocGiam = 0;
                decimal tongGiamGia = 0;
                decimal tongThucNhan = 0;

                foreach (var hd in hoaDons)
                {
                    tongDoanhThuTruocGiam += hd.TongTien;
                    tongGiamGia += hd.TienGiamGia;
                    tongThucNhan += hd.ThanhTien;
                }

                // Thoi gian dong ca: lay ngay bay gio, dung nhat cho ca update lan dem
                DateTime thoiGianDongCa = DateTime.Now;

                // Dem khach moi trong ca
                int soKhachMoi = khachHangDAL.DemKhachMoiTrongCa(
                    caInfo.GioBatDau,
                    thoiGianDongCa   // <-- Fix: dung thoi gian dong ca thay vi DateTime.Now rieng
                );

                // Cap nhat vao DB
                string query = @"UPDATE tblLichSuCa SET
                    GioKetThuc = @GioKetThuc,
                    TongSoHoaDon = @TongHoaDon,
                    TongDoanhThuTruocGiamGia = @TongTruocGiam,
                    TongTienGiamGia = @TongGiam,
                    TongDoanhThuThucNhan = @TongThucNhan,
                    SoKhachMoi = @SoKhachMoi
                    WHERE MaCa = @MaCa";
                var parameters = new SqlParameter[] {
                    new SqlParameter("@MaCa", maCa),
                    new SqlParameter("@GioKetThuc", thoiGianDongCa),
                    new SqlParameter("@TongHoaDon", tongHoaDon),
                    new SqlParameter("@TongTruocGiam", tongDoanhThuTruocGiam),
                    new SqlParameter("@TongGiam", tongGiamGia),
                    new SqlParameter("@TongThucNhan", tongThucNhan),
                    new SqlParameter("@SoKhachMoi", soKhachMoi)
                };
                DatabaseHelper.ExecuteNonQuery(query, parameters);

                return new LichSuCa
                {
                    MaCa = maCa,
                    GioBatDau = caInfo.GioBatDau,
                    GioKetThuc = thoiGianDongCa,
                    NhanVien = caInfo.NhanVien,
                    TongSoHoaDon = tongHoaDon,
                    TongDoanhThuTruocGiamGia = tongDoanhThuTruocGiam,
                    TongTienGiamGia = tongGiamGia,
                    TongDoanhThuThucNhan = tongThucNhan,
                    SoKhachMoi = soKhachMoi
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi dong ca: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        /// Lay thong tin ca hien tai
        /// </summary>
        public LichSuCa GetCaInfo(int maCa)
        {
            try
            {
                string query = "SELECT * FROM tblLichSuCa WHERE MaCa = @MaCa";
                var parameters = new SqlParameter[] {
                    new SqlParameter("@MaCa", maCa)
                };
                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    return new LichSuCa
                    {
                        MaCa = Convert.ToInt32(row["MaCa"]),
                        GioBatDau = Convert.ToDateTime(row["GioBatDau"]),
                        GioKetThuc = row["GioKetThuc"] != DBNull.Value
                            ? (DateTime?)Convert.ToDateTime(row["GioKetThuc"])
                            : null,
                        NhanVien = row["NhanVien"].ToString(),
                        TongSoHoaDon = Convert.ToInt32(row["TongSoHoaDon"]),
                        TongDoanhThuTruocGiamGia = Convert.ToDecimal(row["TongDoanhThuTruocGiamGia"]),
                        TongTienGiamGia = Convert.ToDecimal(row["TongTienGiamGia"]),
                        TongDoanhThuThucNhan = Convert.ToDecimal(row["TongDoanhThuThucNhan"]),
                        SoKhachMoi = Convert.ToInt32(row["SoKhachMoi"])
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi lay thong tin ca: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        /// Lay top 5 mon ban chay trong ca (goi spThongKeTopMonBanChay)
        /// </summary>
        public DataTable GetTopMonBanChay(int maCa)
        {
            try
            {
                // Su dung stored procedure
                string query = "spThongKeTopMonBanChay";
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaCa", maCa);
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
                MessageBox.Show("Loi thong ke mon ban chay: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return new DataTable();
            }
        }

        /// <summary>
        /// Lay danh sach tat ca cac ca da dong, sap xep moi nhat len truoc
        /// </summary>
        public List<LichSuCa> GetAllCaDaDong()
        {
            var list = new List<LichSuCa>();
            try
            {
                string query = "SELECT * FROM tblLichSuCa WHERE GioKetThuc IS NOT NULL ORDER BY GioKetThuc DESC";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new LichSuCa
                    {
                        MaCa = Convert.ToInt32(row["MaCa"]),
                        GioBatDau = Convert.ToDateTime(row["GioBatDau"]),
                        GioKetThuc = row["GioKetThuc"] != DBNull.Value
                            ? (DateTime?)Convert.ToDateTime(row["GioKetThuc"])
                            : null,
                        NhanVien = row["NhanVien"].ToString(),
                        TongSoHoaDon = Convert.ToInt32(row["TongSoHoaDon"]),
                        TongDoanhThuTruocGiamGia = Convert.ToDecimal(row["TongDoanhThuTruocGiamGia"]),
                        TongTienGiamGia = Convert.ToDecimal(row["TongTienGiamGia"]),
                        TongDoanhThuThucNhan = Convert.ToDecimal(row["TongDoanhThuThucNhan"]),
                        SoKhachMoi = Convert.ToInt32(row["SoKhachMoi"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi lay danh sach ca: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return list;
        }

        /// <summary>
        /// Lay top 10 mon ban chay tu TAT CA cac ca da dong (tong hop)
        /// </summary>
        public DataTable GetTopMonBanChayTongCa()
        {
            try
            {
                string query = @"
                    SELECT TOP 10
                        ct.TenMonAn,
                        SUM(ct.SoLuong) AS TongSoLuong,
                        SUM(ct.ThanhTien) AS TongDoanhThu
                    FROM tblChiTietHoaDon ct
                    INNER JOIN tblHoaDon hd ON ct.MaHoaDon = hd.MaHoaDon
                    WHERE hd.TrangThai = N'DaThanhToan'
                    GROUP BY ct.TenMonAn
                    ORDER BY TongSoLuong DESC";
                return DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi thong ke mon ban chay tong hop: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return new DataTable();
            }
        }

        /// <summary>
        /// Xuat bao cao ra file .txt
        /// </summary>
        public bool XuatBaoCaoTxt(LichSuCa ca, DataTable topMon, string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine("============================================");
                    writer.WriteLine("     BAO CAO DONG CA - NHA HANG HUIT");
                    writer.WriteLine("============================================");
                    writer.WriteLine();
                    writer.WriteLine("Ngay:            " + ca.GioBatDau.ToString("dd/MM/yyyy"));
                    writer.WriteLine("Gio bat dau:     " + ca.GioBatDau.ToString("HH:mm:ss"));
                    writer.WriteLine("Gio ket thuc:    " + (ca.GioKetThuc?.ToString("HH:mm:ss") ?? "---"));
                    writer.WriteLine("Nhan vien:       " + ca.NhanVien);
                    writer.WriteLine("Ma Ca:           #" + ca.MaCa);
                    writer.WriteLine();
                    writer.WriteLine("--- THONG KE ---");
                    writer.WriteLine("Tong so hoa don:               " + ca.TongSoHoaDon);
                    writer.WriteLine("Doanh thu truoc giam gia:      " + ca.TongDoanhThuTruocGiamGia.ToString("N0") + " VND");
                    writer.WriteLine("Tong tien giam gia:            " + ca.TongTienGiamGia.ToString("N0") + " VND");
                    writer.WriteLine("Doanh thu thuc nhan:           " + ca.TongDoanhThuThucNhan.ToString("N0") + " VND");
                    writer.WriteLine("So khach hang moi dang ky:     " + ca.SoKhachMoi);
                    writer.WriteLine();

                    writer.WriteLine("--- TOP 5 MON BAN CHAY ---");
                    if (topMon.Rows.Count > 0)
                    {
                        int stt = 1;
                        foreach (DataRow row in topMon.Rows)
                        {
                            writer.WriteLine("  " + stt + ". " + row["TenMonAn"] + " - " +
                                row["TongSoLuong"] + " phan - " +
                                Convert.ToDecimal(row["TongDoanhThu"]).ToString("N0") + " VND");
                            stt++;
                        }
                    }
                    else
                    {
                        writer.WriteLine("  (Khong co du lieu)");
                    }

                    writer.WriteLine();
                    writer.WriteLine("============================================");
                    writer.WriteLine("  Phan mem quan ly nha hang Nha_Hang_Huit");
                    writer.WriteLine("============================================");
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi xuat file bao cao: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
