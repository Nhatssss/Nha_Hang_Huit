using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Form quan ly khach hang: tim kiem, tao moi, chon khach cho hoa don
    /// </summary>
    public partial class frmKhachHang : Window
    {
        private KhachHang khTimDuoc = null;

        public frmKhachHang()
        {
            InitializeComponent();
            Loaded += frmKhachHang_Loaded;
        }

        /// <summary>
        /// Khi form load: tai danh sach khach hang
        /// </summary>
        private void frmKhachHang_Loaded(object sender, RoutedEventArgs e)
        {
            TaiDanhSachKH();
        }

        /// <summary>
        /// Tai danh sach khach hang tu BLL
        /// </summary>
        private void TaiDanhSachKH(string filter = "")
        {
            var list = frmChinh.khachHangBLL.GetAll();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                list = list.Where(k =>
                    k.TenKhachHang.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    k.SoDienThoai.Contains(filter)
                ).ToList();
            }
            lstKhachHang.ItemsSource = list;
            lblTongKH.Text = $"Tong so: {list.Count} khach hang";
        }

        /// <summary>
        /// Tim khach hang theo so dien thoai
        /// </summary>
        private void btnTim_Click(object sender, RoutedEventArgs e)
        {
            string sdt = txtSoDienThoai.Text.Trim();
            if (string.IsNullOrWhiteSpace(sdt))
            {
                MessageBox.Show("Nhap so dien thoai can tim!", "Thong bao",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            khTimDuoc = frmChinh.khachHangBLL.GetByPhone(sdt);
            if (khTimDuoc != null)
            {
                HienThiThongTinKH(khTimDuoc);
                btnChonKH.IsEnabled = true;
            }
            else
            {
                panelThongTinKH.Visibility = Visibility.Collapsed;
                btnChonKH.IsEnabled = false;
                MessageBox.Show("Khong tim thay khach hang voi so dien thoai nay!", "Thong bao",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Hien thi thong tin khach hang len panel
        /// </summary>
        private void HienThiThongTinKH(KhachHang kh)
        {
            lblTenKH.Text = kh.TenKhachHang;
            lblSDTKH.Text = $"SDT: {kh.SoDienThoai}";
            lblDiem.Text = $"{kh.TongDiemTichLuy} diem";
            lblHangThe.Text = kh.HangThe;

            // Mau cho hang the
            switch (kh.HangThe)
            {
                case "KimCuong":
                    lblHangThe.Foreground = System.Windows.Media.Brushes.DarkCyan;
                    break;
                case "Vang":
                    lblHangThe.Foreground = System.Windows.Media.Brushes.DarkGoldenrod;
                    break;
                case "Bac":
                    lblHangThe.Foreground = System.Windows.Media.Brushes.Silver;
                    break;
                default:
                    lblHangThe.Foreground = System.Windows.Media.Brushes.Gray;
                    break;
            }

            panelThongTinKH.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Them khach hang moi
        /// </summary>
        private void btnThemKH_Click(object sender, RoutedEventArgs e)
        {
            string ten = txtTenKH.Text.Trim();
            string sdt = txtSDTNew.Text.Trim();

            if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(sdt))
            {
                MessageBox.Show("Vui long nhap ten va so dien thoai!", "Thong bao",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newKh = new KhachHang
            {
                TenKhachHang = ten,
                SoDienThoai = sdt
            };

            int maKH = frmChinh.khachHangBLL.Add(newKh);
            if (maKH > 0)
            {
                MessageBox.Show($"Them khach hang thanh cong! Ma KH: #{maKH}", "Thanh Cong",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                txtTenKH.Clear();
                txtSDTNew.Clear();
                TaiDanhSachKH();
            }
        }

        /// <summary>
        /// Chon khach hang cho hoa don hien tai
        /// </summary>
        private void btnChonKH_Click(object sender, RoutedEventArgs e)
        {
            if (khTimDuoc == null) return;

            // Gan khach hang vao gio hang
            frmChinh.hoaDonBLL.MaKhachHangHienTai = khTimDuoc.MaKhachHang;

            // Tinh tien giam gia
            decimal tongTien = frmChinh.hoaDonBLL.TinhTongTien();
            decimal tienGiam = frmChinh.khachHangBLL.TinhTienGiamGia(khTimDuoc, tongTien);
            frmChinh.hoaDonBLL.TienGiamGiaHienTai = tienGiam;

            MessageBox.Show($"Da chon khach hang: {khTimDuoc.TenKhachHang}\n" +
                $"Hang the: {khTimDuoc.HangThe}\n" +
                $"Giam gia: {tienGiam:N0} VND ({(khTimDuoc.LayTiLeGiamGia() * 100):F0}%)",
                "Chon khach hang", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }

        /// <summary>
        /// Loc danh sach khi nhap tu khoa
        /// </summary>
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TaiDanhSachKH(txtSearch.Text.Trim());
        }

        /// <summary>
        /// Chon khach hang tu danh sach
        /// </summary>
        private void lstKhachHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            khTimDuoc = lstKhachHang.SelectedItem as KhachHang;
            if (khTimDuoc != null)
            {
                HienThiThongTinKH(khTimDuoc);
                btnChonKH.IsEnabled = true;
                txtSoDienThoai.Text = khTimDuoc.SoDienThoai;
            }
        }
    }
}
