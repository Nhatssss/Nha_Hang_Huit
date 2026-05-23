using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Nha_Hang_Huit.Models;
using QRCoder;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Form thanh toan: ho tro tien mat va QR code
    /// </summary>
    public partial class frmThanhToan : Window
    {
        private int? maHoaDonHienTai = null;
        private CancellationTokenSource ctsQR = null;
        private bool daThanhToanQR = false;

        public frmThanhToan()
        {
            InitializeComponent();
            Loaded += frmThanhToan_Loaded;
        }

        /// <summary>
        /// Khi form load: cap nhat thong tin hoa don
        /// </summary>
        private void frmThanhToan_Loaded(object sender, RoutedEventArgs e)
        {
            CapNhatHoaDonInfo();
        }

        /// <summary>
        /// Khi dong form: huy hoa don neu chua thanh toan (fix orphan invoice)
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Neu da tao hoa don trong DB ma chua thanh toan -> huy no
            if (maHoaDonHienTai.HasValue && !daThanhToanQR)
            {
                // Kiem tra xem hoa don con o trang thai "ChoThanhToan" khong
                var hd = frmChinh.hoaDonBLL.GetHoaDonWithDetails(maHoaDonHienTai.Value);
                if (hd != null && hd.TrangThai == "ChoThanhToan")
                {
                    frmChinh.hoaDonBLL.HuyHoaDon(maHoaDonHienTai.Value);
                }
            }

            // Huy timer neu dang chay
            ctsQR?.Cancel();
        }

        /// <summary>
        /// Cap nhat thong tin hoa don: tong tien, giam gia, thanh toan
        /// </summary>
        private void CapNhatHoaDonInfo()
        {
            decimal tongTien = frmChinh.hoaDonBLL.TinhTongTien();
            decimal tienGiam = frmChinh.hoaDonBLL.TienGiamGiaHienTai;
            decimal thanhToan = frmChinh.hoaDonBLL.TinhThanhTien();

            lblTongTien.Text = $"{tongTien:N0} VND";
            lblGiamGia.Text = $"{tienGiam:N0} VND";
            lblThanhToan.Text = $"{thanhToan:N0} VND";
        }

        /// <summary>
        /// Tinh tien thua khi nhap so tien khach dua
        /// </summary>
        private void txtTienKhachDua_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            decimal thanhToan = frmChinh.hoaDonBLL.TinhThanhTien();
            if (decimal.TryParse(txtTienKhachDua.Text, out decimal tienKhachDua))
            {
                decimal tienThua = tienKhachDua - thanhToan;
                if (tienThua >= 0)
                {
                    lblTienThua.Text = $"{tienThua:N0} VND";
                    lblTienThua.Foreground = System.Windows.Media.Brushes.Green;
                    lblThongBaoTT.Visibility = Visibility.Collapsed;
                }
                else
                {
                    lblTienThua.Text = $"{tienThua:N0} VND (thieu)";
                    lblTienThua.Foreground = System.Windows.Media.Brushes.Orange;
                    lblThongBaoTT.Text = "Khach chua du tien!";
                    lblThongBaoTT.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lblTienThua.Text = "0 VND";
            }
        }

        /// <summary>
        /// Xac nhan thanh toan tien mat
        /// </summary>
        private void btnXacNhanTT_Click(object sender, RoutedEventArgs e)
        {
            decimal thanhToan = frmChinh.hoaDonBLL.TinhThanhTien();
            decimal tienKhachDua = 0;
            if (!decimal.TryParse(txtTienKhachDua.Text, out tienKhachDua) || tienKhachDua < thanhToan)
            {
                MessageBox.Show("So tien khach dua khong hop le hoac chua du!", "Thong bao",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tao hoa don trong DB
            int? maHD = frmChinh.hoaDonBLL.TaoHoaDon();
            if (maHD == null) return;

            // Xac nhan thanh toan
            bool result = frmChinh.hoaDonBLL.XacNhanThanhToan(maHD.Value, "TienMat");
            if (result)
            {
                MessageBox.Show($"Thanh toan thanh cong!\n" +
                    $"Tien thua: {tienKhachDua - thanhToan:N0} VND\n" +
                    $"Ma Hoa Don: #{maHD}",
                    "Thanh Cong", MessageBoxButton.OK, MessageBoxImage.Information);
                CapNhatHoaDonInfo();
                this.Close();
            }
            else
            {
                MessageBox.Show("Thanh toan that bai!", "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Tao QR Code cho hoa don
        /// </summary>
        private void btnTaoQR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Tao hoa don truoc
                if (maHoaDonHienTai == null)
                {
                    maHoaDonHienTai = frmChinh.hoaDonBLL.TaoHoaDon();
                    if (maHoaDonHienTai == null) return;
                }

                decimal thanhToan = frmChinh.hoaDonBLL.TinhThanhTien();
                string noiDungQR = $"THANH TOAN HOA DON #{maHoaDonHienTai} - " +
                    $"SO TIEN: {thanhToan:N0} VND - NHA HANG HUIT";

                // Tao QR code bang QRCoder
                using (var qrGenerator = new QRCodeGenerator())
                using (var qrData = qrGenerator.CreateQrCode(noiDungQR, QRCodeGenerator.ECCLevel.Q))
                using (var qrCode = new QRCode(qrData))
                {
                    using (var qrBitmap = qrCode.GetGraphic(20))
                    {
                        // Convert Bitmap to BitmapImage
                        using (var ms = new MemoryStream())
                        {
                            qrBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            ms.Position = 0;
                            var bi = new BitmapImage();
                            bi.BeginInit();
                            bi.StreamSource = ms;
                            bi.CacheOption = BitmapCacheOption.OnLoad;
                            bi.EndInit();
                            imgQRCode.Source = bi;
                        }
                    }
                }

                lblQRInfo.Text = $"QR Hoa Don #{maHoaDonHienTai}";
                panelQRCountdown.Visibility = Visibility.Visible;
                btnGiaLapQR.IsEnabled = true;

                // Bat dau dem nguoc 30 giay
                BatDauDemNguocQR();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi tao QR Code: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Bat dau dem nguoc 30 giay cho thanh toan QR
        /// </summary>
        private async void BatDauDemNguocQR()
        {
            ctsQR?.Cancel();
            ctsQR = new CancellationTokenSource();
            daThanhToanQR = false;

            try
            {
                for (int i = 30; i >= 0; i--)
                {
                    if (ctsQR.Token.IsCancellationRequested || daThanhToanQR)
                        return;

                    lblQRCountdown.Text = $"{i}s";

                    if (i > 0)
                        await Task.Delay(1000, ctsQR.Token);
                }

                // Het 30 giay: tu dong xac nhan
                if (!daThanhToanQR && !ctsQR.Token.IsCancellationRequested)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        TuDongXacNhanQR();
                    });
                }
            }
            catch (TaskCanceledException)
            {
                // Normal cancellation
            }
        }

        /// <summary>
        /// Gia lap thanh toan QR thanh cong
        /// </summary>
        private void btnGiaLapQR_Click(object sender, RoutedEventArgs e)
        {
            GiaLapThanhCong();
        }

        /// <summary>
        /// Tu dong xac nhan khi het 30 giay
        /// </summary>
        private void TuDongXacNhanQR()
        {
            GiaLapThanhCong();
        }

        /// <summary>
        /// Xu ly gia lap thanh toan QR thanh cong
        /// Danh dau daThanhToanQR de OnClosed khong huy hoa don nham
        /// </summary>
        private void GiaLapThanhCong()
        {
            if (daThanhToanQR || maHoaDonHienTai == null) return;

            daThanhToanQR = true;
            ctsQR?.Cancel();

            bool result = frmChinh.hoaDonBLL.XacNhanThanhToan(maHoaDonHienTai.Value, "QRCode");
            if (result)
            {
                lblQRResult.Text = $"✅ Thanh toan QR thanh cong!\nHoa Don #{maHoaDonHienTai}";
                lblQRResult.Foreground = System.Windows.Media.Brushes.Green;
                lblQRResult.Visibility = Visibility.Visible;
                btnGiaLapQR.IsEnabled = false;
                panelQRCountdown.Visibility = Visibility.Collapsed;

                MessageBox.Show("Thanh toan QR thanh cong!", "Thanh Cong",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                lblQRResult.Text = "❌ Thanh toan that bai!";
                lblQRResult.Foreground = System.Windows.Media.Brushes.Orange;
                lblQRResult.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Huy hoa don chu chua thanh toan (fix orphan invoice)
        /// </summary>
        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            if (!maHoaDonHienTai.HasValue)
            {
                // Chua tao hoa don trong DB, chi can dong form
                this.Close();
                return;
            }

            var result = MessageBox.Show("Huy hoa don nay?", "Xac nhan",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ctsQR?.Cancel();
                maHoaDonHienTai = null;  // OnClosed se khong huy nua
                this.Close();
            }
        }
    }
}
