using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Form goi mon: hien thi menu theo nhom, them vao gio hang
    /// </summary>
    public partial class frmMenu : Window
    {
        private MonAn monDaChon = null;

        public frmMenu()
        {
            InitializeComponent();
            Loaded += frmMenu_Loaded;
        }

        /// <summary>
        /// Khi form load: lay danh sach nhom mon va hien thi
        /// </summary>
        private void frmMenu_Loaded(object sender, RoutedEventArgs e)
        {
            TaiNhomMon();
            HienThiGioHangTam();
        }

        /// <summary>
        /// Tai danh sach nhom mon tu BLL
        /// </summary>
        private void TaiNhomMon()
        {
            var nhoms = frmChinh.monAnBLL.GetAllNhomMon();
            lstNhomMon.ItemsSource = nhoms;
            if (nhoms.Count > 0)
                lstNhomMon.SelectedIndex = 0;
        }

        /// <summary>
        /// Khi chon nhom mon: tai danh sach mon an tuong ung
        /// </summary>
        private void lstNhomMon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstNhomMon.SelectedItem == null) return;

            string nhom = lstNhomMon.SelectedItem.ToString();
            lblNhomSelected.Text = nhom;

            var mons = frmChinh.monAnBLL.GetByNhom(nhom);
            lstMonAn.ItemsSource = mons;

            // An panel chon mon
            panelChonMon.Visibility = Visibility.Collapsed;
            monDaChon = null;
        }

        /// <summary>
        /// Khi chon mon an: hien thi panel chon so luong
        /// </summary>
        private void lstMonAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstMonAn.SelectedItem == null)
            {
                panelChonMon.Visibility = Visibility.Collapsed;
                monDaChon = null;
                return;
            }

            monDaChon = lstMonAn.SelectedItem as MonAn;
            if (monDaChon != null)
            {
                lblMonDaChon.Text = monDaChon.TenMonAn;
                lblGiaDaChon.Text = $"{monDaChon.Gia:N0} VND";
                txtSoLuong.Text = "1";
                panelChonMon.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Tang so luong mon
        /// </summary>
        private void btnTangSoLuong_Click(object sender, RoutedEventArgs e)
        {
            int sl = int.Parse(txtSoLuong.Text);
            txtSoLuong.Text = Math.Min(sl + 1, 99).ToString();
        }

        /// <summary>
        /// Giam so luong mon
        /// </summary>
        private void btnGiamSoLuong_Click(object sender, RoutedEventArgs e)
        {
            int sl = int.Parse(txtSoLuong.Text);
            txtSoLuong.Text = Math.Max(sl - 1, 1).ToString();
        }

        /// <summary>
        /// Them mon vao gio hang
        /// </summary>
        private void btnThemVaoGio_Click(object sender, RoutedEventArgs e)
        {
            if (monDaChon == null) return;

            int soLuong = int.Parse(txtSoLuong.Text);
            frmChinh.hoaDonBLL.ThemVaoGioHang(monDaChon, soLuong);
            HienThiGioHangTam();
        }

        /// <summary>
        /// Hien thi gio hang tam trong form nay
        /// </summary>
        private void HienThiGioHangTam()
        {
            lstGioHangTam.ItemsSource = null;
            lstGioHangTam.ItemsSource = frmChinh.hoaDonBLL.GioHang;
            lblTongGioHang.Text = $"{frmChinh.hoaDonBLL.TinhTongTien():N0} VND";
        }

        /// <summary>
        /// Xoa mon duoc chon khoi gio hang
        /// </summary>
        private void btnXoaChon_Click(object sender, RoutedEventArgs e)
        {
            var selected = lstGioHangTam.SelectedItem as ChiTietHoaDon;
            if (selected == null)
            {
                MessageBox.Show("Vui long chon mon can xoa trong gio hang!", "Thong bao",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            frmChinh.hoaDonBLL.XoaKhoiGioHang(selected.MaMonAn);
            HienThiGioHangTam();
        }

        /// <summary>
        /// Chon item trong gio hang tam
        /// </summary>
        private void lstGioHangTam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Co the mo rong
        }

        /// <summary>
        /// Xong: dong form va quay ve form chinh
        /// </summary>
        private void btnXong_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
