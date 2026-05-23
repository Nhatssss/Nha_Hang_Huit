using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using Microsoft.Win32;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Form bao cao dong ca: hien thi thong ke ca va top mon ban chay
    /// </summary>
    public partial class frmBaoCaoCa : Window
    {
        private int maCa;
        private LichSuCa caInfo;
        private DataTable topMonData;

        /// <summary>
        /// Khoi tao form voi MaCa can xem bao cao
        /// </summary>
        public frmBaoCaoCa(int maCa)
        {
            InitializeComponent();
            this.maCa = maCa;
            Loaded += frmBaoCaoCa_Loaded;
        }

        /// <summary>
        /// Khi form load: tai thong tin ca va top mon
        /// </summary>
        private void frmBaoCaoCa_Loaded(object sender, RoutedEventArgs e)
        {
            TaiThongTinCa();
            TaiTopMonBanChay();
        }

        /// <summary>
        /// Tai thong tin tong quan ve ca
        /// </summary>
        private void TaiThongTinCa()
        {
            caInfo = frmChinh.baoCaoBLL.GetCaInfo(maCa);
            if (caInfo == null)
            {
                MessageBox.Show("Khong tim thay thong tin ca!", "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            lblMaCa.Text = $"Ca #{caInfo.MaCa}";
            lblGioBatDau.Text = $"Bat dau: {caInfo.GioBatDau:dd/MM/yyyy HH:mm:ss}";
            lblGioKetThuc.Text = $"Ket thuc: {caInfo.GioKetThuc:dd/MM/yyyy HH:mm:ss}";
            lblNhanVien.Text = $"Nhan vien: {caInfo.NhanVien}";
            lblTongHoaDon.Text = $"Tong hoa don: {caInfo.TongSoHoaDon}";
            lblDoanhThuTruocGiam.Text = $"Truoc giam gia: {caInfo.TongDoanhThuTruocGiamGia:N0} VND";
            lblTongGiamGia.Text = $"Giam gia: {caInfo.TongTienGiamGia:N0} VND";
            lblDoanhThuThucNhan.Text = $"Thuc nhan: {caInfo.TongDoanhThuThucNhan:N0} VND";
            lblSoKhachMoi.Text = $"Khach moi: {caInfo.SoKhachMoi}";
        }

        /// <summary>
        /// Tai top 5 mon ban chay trong ca
        /// </summary>
        private void TaiTopMonBanChay()
        {
            topMonData = frmChinh.baoCaoBLL.GetTopMonBanChay(maCa);
            var items = new List<TopMonItem>();
            int stt = 1;

            foreach (DataRow row in topMonData.Rows)
            {
                items.Add(new TopMonItem
                {
                    STT = stt++,
                    TenMonAn = row["TenMonAn"].ToString(),
                    SoLuong = Convert.ToInt32(row["TongSoLuong"]),
                    DoanhThu = Convert.ToDecimal(row["TongDoanhThu"])
                });
            }

            lstTopMon.ItemsSource = items;
        }

        /// <summary>
        /// Xuat bao cao ra file .txt
        /// </summary>
        private void btnXuatBaoCao_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Luu bao cao",
                Filter = "Text files (*.txt)|*.txt",
                FileName = $"BaoCao_Ca_{maCa}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                bool result = frmChinh.baoCaoBLL.XuatBaoCaoTxt(caInfo, topMonData, dialog.FileName);
                if (result)
                {
                    MessageBox.Show($"Xuat bao cao thanh cong!\nFile: {dialog.FileName}", "Thanh Cong",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// Dong form
        /// </summary>
        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Lop phu tro hien thi top mon ban chay
        /// </summary>
        public class TopMonItem
        {
            public int STT { get; set; }
            public string TenMonAn { get; set; }
            public int SoLuong { get; set; }
            public decimal DoanhThu { get; set; }
        }
    }
}
