using System;
using System.Linq;
using System.Windows;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Form chinh / Dashboard: dieu huong cac chuc nang chinh
    /// </summary>
    public partial class frmChinh : Window
    {
        // BLL instance dung chung cho toan bo ung dung
        public static MonAnBLL monAnBLL = new MonAnBLL();
        public static KhachHangBLL khachHangBLL = new KhachHangBLL();
        public static HoaDonBLL hoaDonBLL = new HoaDonBLL();
        public static BaoCaoBLL baoCaoBLL = new BaoCaoBLL();

        private int? maCaHienTai = null;

        public frmChinh()
        {
            InitializeComponent();
            Loaded += frmChinh_Loaded;
        }

        /// <summary>
        /// Khi form load: kiem tra ca hien tai va cap nhat thong tin
        /// </summary>
        private void frmChinh_Loaded(object sender, RoutedEventArgs e)
        {
            maCaHienTai = hoaDonBLL.GetCurrentCa();
            CapNhatTrangThaiCa();
            CapNhatThongTin();
            HienThiGioHang();
        }

        /// <summary>
        /// Cap nhat trang thai ca (dang mo / dong)
        /// </summary>
        private void CapNhatTrangThaiCa()
        {
            if (maCaHienTai.HasValue)
            {
                lblCaHienTai.Text = $"Ca #{maCaHienTai} - DANG MO";
                btnMoCa.IsEnabled = false;
                btnDongCa.IsEnabled = true;
            }
            else
            {
                lblCaHienTai.Text = "CA: CHUA MO";
                btnMoCa.IsEnabled = true;
                btnDongCa.IsEnabled = false;
            }
        }

        /// <summary>
        /// Cap nhat thong tin nhanh (hoa don, doanh thu)
        /// </summary>
        private void CapNhatThongTin()
        {
            if (!maCaHienTai.HasValue)
            {
                lblTongHoaDon.Text = "Hoa don: 0";
                lblDoanhThu.Text = "Doanh thu: 0 VND";
                return;
            }

            var hoaDons = hoaDonBLL.GetHoaDonDaThanhToanByCa(maCaHienTai.Value);
            int tongHoaDon = hoaDons.Count;
            decimal tongDoanhThu = hoaDons.Sum(h => h.ThanhTien);
            lblTongHoaDon.Text = $"Hoa don: {tongHoaDon}";
            lblDoanhThu.Text = $"Doanh thu: {tongDoanhThu:N0} VND";
        }

        /// <summary>
        /// Hien thi gio hang hien tai
        /// </summary>
        private void HienThiGioHang()
        {
            lstGioHang.ItemsSource = null;
            lstGioHang.ItemsSource = hoaDonBLL.GioHang;
            lblTongTien.Text = $"{hoaDonBLL.TinhTongTien():N0} VND";
            lblGiamGia.Text = $"{hoaDonBLL.TienGiamGiaHienTai:N0} VND";

            if (hoaDonBLL.MaKhachHangHienTai.HasValue)
            {
                var kh = khachHangBLL.GetById(hoaDonBLL.MaKhachHangHienTai.Value);
                if (kh != null)
                    lblKhachHangInfo.Text = $"{kh.TenKhachHang} ({kh.HangThe})";
            }
            else
            {
                lblKhachHangInfo.Text = "(Chua chon)";
            }
        }

        /// <summary>
        /// Mo ca moi
        /// </summary>
        private void btnMoCa_Click(object sender, RoutedEventArgs e)
        {
            int maCa = baoCaoBLL.MoCa("admin");
            if (maCa > 0)
            {
                maCaHienTai = maCa;
                CapNhatTrangThaiCa();
                CapNhatThongTin();
                lblThongBaoHienTai.Text = $"Da mo ca #{maCa} thanh cong!";
            }
        }

        /// <summary>
        /// Dong ca hien tai
        /// </summary>
        private void btnDongCa_Click(object sender, RoutedEventArgs e)
        {
            if (!maCaHienTai.HasValue) return;

            var result = MessageBox.Show("Xac nhan dong ca?", "Xac nhan",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            var ca = baoCaoBLL.DongCa(maCaHienTai.Value);
            if (ca != null)
            {
                // Hien thi bao cao
                var frmBaoCao = new frmBaoCaoCa(maCaHienTai.Value);
                frmBaoCao.ShowDialog();

                maCaHienTai = null;
                CapNhatTrangThaiCa();
                CapNhatThongTin();
                lblThongBaoHienTai.Text = "Da dong ca thanh cong!";
            }
        }

        /// <summary>
        /// Mo form goi mon
        /// </summary>
        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!maCaHienTai.HasValue)
            {
                MessageBox.Show("Vui long mo ca truoc!", "Thong bao",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var frmMenu = new frmMenu();
            frmMenu.Owner = this;
            frmMenu.ShowDialog();
            HienThiGioHang();
            CapNhatThongTin();
        }

        /// <summary>
        /// Mo form thanh toan
        /// </summary>
        private void btnThanhToan_Click(object sender, RoutedEventArgs e)
        {
            if (hoaDonBLL.GioHang.Count == 0)
            {
                MessageBox.Show("Gio hang trong! Vui long goi mon truoc.", "Thong bao",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var frmThanhToan = new frmThanhToan();
            frmThanhToan.Owner = this;
            frmThanhToan.ShowDialog();
            HienThiGioHang();
            CapNhatThongTin();
        }

        /// <summary>
        /// Mo form bao cao doanh thu (admin)
        /// </summary>
        private void btnBaoCaoDoanhThu_Click(object sender, RoutedEventArgs e)
        {
            var frm = new frmAdminBaoCao();
            frm.Owner = this;
            frm.ShowDialog();
        }

        /// <summary>
        /// Mo form quan ly khach hang
        /// </summary>
        private void btnKhachHang_Click(object sender, RoutedEventArgs e)
        {
            var frmKhachHang = new frmKhachHang();
            frmKhachHang.Owner = this;
            frmKhachHang.ShowDialog();
            HienThiGioHang();
        }

        /// <summary>
        /// Mo form bao cao ca
        /// </summary>
        private void btnBaoCao_Click(object sender, RoutedEventArgs e)
        {
            if (!maCaHienTai.HasValue)
            {
                MessageBox.Show("Khong co ca nao dang mo.", "Thong bao",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var frmBaoCao = new frmBaoCaoCa(maCaHienTai.Value);
            frmBaoCao.Owner = this;
            frmBaoCao.ShowDialog();
        }

        /// <summary>
        /// Xoa gio hang
        /// </summary>
        private void btnXoaGioHang_Click(object sender, RoutedEventArgs e)
        {
            if (hoaDonBLL.GioHang.Count == 0) return;

            var result = MessageBox.Show("Xoa toan bo gio hang?", "Xac nhan",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                hoaDonBLL.XoaGioHang();
                HienThiGioHang();
            }
        }

        /// <summary>
        /// Refresh thong tin
        /// </summary>
        private void btnRefreshThongTin_Click(object sender, RoutedEventArgs e)
        {
            HienThiGioHang();
            CapNhatThongTin();
            lblThongBaoHienTai.Text = "Da cap nhat thong tin!";
        }

        /// <summary>
        /// Tim khach hang nhanh
        /// </summary>
        private void btnTimKhachHang_Click(object sender, RoutedEventArgs e)
        {
            btnKhachHang_Click(sender, e);
        }

        /// <summary>
        /// Hien thi top mon ban chay (tong hop tu tat ca cac ca)
        /// </summary>
        private void btnTopMon_Click(object sender, RoutedEventArgs e)
        {
            var topMon = baoCaoBLL.GetTopMonBanChayTongCa();
            if (topMon.Rows.Count == 0)
            {
                lblThongBaoHienTai.Text = "Chua co du lieu mon ban chay.";
                return;
            }

            string msg = ">>> TOP MON BAN CHAY (TAT CA CA) <<<\n\n";
            int stt = 1;
            foreach (System.Data.DataRow row in topMon.Rows)
            {
                msg += $"{stt}. {row["TenMonAn"]} - {row["TongSoLuong"]} phan - {Convert.ToDecimal(row["TongDoanhThu"]):N0} VND\n";
                stt++;
            }
            MessageBox.Show(msg, "Top Mon Ban Chay - Tong hop");
        }

        /// <summary>
        /// In hoa don mau (demo)
        /// </summary>
        private void btnInHoaDonMau_Click(object sender, RoutedEventArgs e)
        {
            lblThongBaoHienTai.Text = "Chuc nang in hoa don se duoc bo sung sau.";
        }

        /// <summary>
        /// Xu ly chon item trong gio hang
        /// </summary>
        private void lstGioHang_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Co the mo rong: cho phep xoa item duoc chon
        }
    }
}
