using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Form bao cao doanh thu danh cho admin: tong hop tat ca cac ca,
    /// hien thi danh sach ca, top mon ban chay, va cho phep xem chi tiet tung ca
    /// </summary>
    public partial class frmAdminBaoCao : Window
    {
        private List<LichSuCa> danhSachCa;

        public frmAdminBaoCao()
        {
            InitializeComponent();
            Loaded += frmAdminBaoCao_Loaded;
        }

        private void frmAdminBaoCao_Loaded(object sender, RoutedEventArgs e)
        {
            TaiDuLieu();
        }

        /// <summary>
        /// Tai toan bo du lieu: danh sach ca, top mon tong hop, thong ke
        /// </summary>
        private void TaiDuLieu()
        {
            // Lay danh sach ca da dong
            danhSachCa = frmChinh.baoCaoBLL.GetAllCaDaDong();
            dgCaDaDong.ItemsSource = danhSachCa;

            // Tinh tong quan
            lblTongSoCa.Text = danhSachCa.Count.ToString();
            decimal tongDoanhThu = danhSachCa.Sum(c => c.TongDoanhThuTruocGiamGia);
            decimal tongGiamGia = danhSachCa.Sum(c => c.TongTienGiamGia);
            decimal tongThucNhan = danhSachCa.Sum(c => c.TongDoanhThuThucNhan);

            lblTongDoanhThu.Text = $"{tongDoanhThu:N0} VND";
            lblTongGiamGia.Text = $"{tongGiamGia:N0} VND";
            lblDoanhThuThucNhan.Text = $"{tongThucNhan:N0} VND";

            // Top mon tong hop tu tat ca ca
            TaiTopMonTongCa();
        }

        /// <summary>
        /// Tai top mon ban chay tong hop tu TAT CA cac ca
        /// </summary>
        private void TaiTopMonTongCa()
        {
            DataTable data = frmChinh.baoCaoBLL.GetTopMonBanChayTongCa();
            var items = new List<frmBaoCaoCa.TopMonItem>();
            int stt = 1;

            foreach (DataRow row in data.Rows)
            {
                items.Add(new frmBaoCaoCa.TopMonItem
                {
                    STT = stt++,
                    TenMonAn = row["TenMonAn"].ToString(),
                    SoLuong = Convert.ToInt32(row["TongSoLuong"]),
                    DoanhThu = Convert.ToDecimal(row["TongDoanhThu"])
                });
            }

            lstTopMonTong.ItemsSource = items;
        }

        /// <summary>
        /// Khi double-click vao mot ca: mo form bao cao chi tiet cua ca do
        /// </summary>
        private void dgCaDaDong_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgCaDaDong.SelectedItem is LichSuCa ca)
            {
                var frm = new frmBaoCaoCa(ca.MaCa);
                frm.Owner = this;
                frm.ShowDialog();
            }
        }

        /// <summary>
        /// Xuat bao cao tong hop ra file .txt
        /// </summary>
        private void btnXuatTongBaoCao_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Luu bao cao tong hop",
                Filter = "Text files (*.txt)|*.txt",
                FileName = $"BaoCao_TongHop_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (var writer = new System.IO.StreamWriter(dialog.FileName, false, System.Text.Encoding.UTF8))
                    {
                        writer.WriteLine("============================================");
                        writer.WriteLine("  BAO CAO DOANH THU TONG HOP - NHA HANG HUIT");
                        writer.WriteLine("============================================");
                        writer.WriteLine();
                        writer.WriteLine($"Ngay xuat bao cao: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                        writer.WriteLine();
                        writer.WriteLine("--- TONG QUAN ---");
                        writer.WriteLine($"Tong so ca:                      {danhSachCa.Count}");
                        writer.WriteLine($"Tong doanh thu truoc giam gia:  {danhSachCa.Sum(c => c.TongDoanhThuTruocGiamGia):N0} VND");
                        writer.WriteLine($"Tong tien giam gia:             {danhSachCa.Sum(c => c.TongTienGiamGia):N0} VND");
                        writer.WriteLine($"Tong doanh thu thuc nhan:       {danhSachCa.Sum(c => c.TongDoanhThuThucNhan):N0} VND");
                        writer.WriteLine($"Tong khach hang moi:            {danhSachCa.Sum(c => c.SoKhachMoi)}");
                        writer.WriteLine();
                        writer.WriteLine("--- TOP 10 MON BAN CHAY (Tat ca ca) ---");
                        var topMon = lstTopMonTong.ItemsSource as System.Collections.IList;
                        if (topMon != null && topMon.Count > 0)
                        {
                            foreach (frmBaoCaoCa.TopMonItem item in topMon)
                            {
                                writer.WriteLine($"  {item.STT}. {item.TenMonAn} - {item.SoLuong} phan - {item.DoanhThu:N0} VND");
                            }
                        }
                        writer.WriteLine();
                        writer.WriteLine("--- CHI TIET TUNG CA ---");
                        foreach (var ca in danhSachCa)
                        {
                            writer.WriteLine();
                            writer.WriteLine($"  Ca #{ca.MaCa} | {ca.NhanVien} | {ca.GioBatDau:dd/MM HH:mm} -> {ca.GioKetThuc:HH:mm}");
                            writer.WriteLine($"    Hoa don: {ca.TongSoHoaDon} | Truoc giam: {ca.TongDoanhThuTruocGiamGia:N0} | Giam: {ca.TongTienGiamGia:N0} | Thuc nhan: {ca.TongDoanhThuThucNhan:N0} VND");
                        }
                        writer.WriteLine();
                        writer.WriteLine("============================================");
                        writer.WriteLine("  Phan mem quan ly nha hang Nha_Hang_Huit");
                        writer.WriteLine("============================================");
                    }
                    MessageBox.Show($"Xuat bao cao tong hop thanh cong!\nFile: {dialog.FileName}", "Thanh Cong",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Loi xuat bao cao: " + ex.Message, "Loi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
