using System;
using System.Collections.Generic;
using System.Windows;
using Nha_Hang_Huit.DAL;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// Lop BLL xu ly logic cho hoa don, gio hang va thanh toan
    /// </summary>
    public class HoaDonBLL
    {
        private HoaDonDAL hoaDonDAL = new HoaDonDAL();

        // Gio hien tai (dung chung cho ca ung dung)
        public List<ChiTietHoaDon> GioHang { get; private set; } = new List<ChiTietHoaDon>();
        public int? MaKhachHangHienTai { get; set; }
        public decimal TienGiamGiaHienTai { get; set; }

        /// <summary>
        /// Them mon an vao gio hang
        /// </summary>
        public void ThemVaoGioHang(MonAn monAn, int soLuong)
        {
            var item = GioHang.Find(i => i.MaMonAn == monAn.MaMonAn);
            if (item != null)
            {
                item.SoLuong += soLuong;
            }
            else
            {
                GioHang.Add(new ChiTietHoaDon
                {
                    MaMonAn = monAn.MaMonAn,
                    TenMonAn = monAn.TenMonAn,
                    SoLuong = soLuong,
                    DonGia = monAn.Gia,
                    IsTempItem = true
                });
            }
        }

        /// <summary>
        /// Cap nhat so luong mon trong gio hang
        /// </summary>
        public void CapNhatSoLuong(int maMonAn, int soLuongMoi)
        {
            var item = GioHang.Find(i => i.MaMonAn == maMonAn);
            if (item != null)
            {
                if (soLuongMoi <= 0)
                    GioHang.Remove(item);
                else
                    item.SoLuong = soLuongMoi;
            }
        }

        /// <summary>
        /// Xoa mon khoi gio hang
        /// </summary>
        public void XoaKhoiGioHang(int maMonAn)
        {
            GioHang.RemoveAll(i => i.MaMonAn == maMonAn);
        }

        /// <summary>
        /// Xoa toan bo gio hang
        /// </summary>
        public void XoaGioHang()
        {
            GioHang.Clear();
            MaKhachHangHienTai = null;
            TienGiamGiaHienTai = 0;
        }

        /// <summary>
        /// Tinh tong tien gio hang (chua giam gia)
        /// </summary>
        public decimal TinhTongTien()
        {
            decimal tong = 0;
            foreach (var item in GioHang)
                tong += item.ThanhTien;
            return tong;
        }

        /// <summary>
        /// Tinh thanh tien sau giam gia
        /// </summary>
        public decimal TinhThanhTien()
        {
            return TinhTongTien() - TienGiamGiaHienTai;
        }

        /// <summary>
        /// Tao hoa don moi, luu vao DB, tra ve MaHoaDon
        /// </summary>
        public int? TaoHoaDon()
        {
            try
            {
                if (GioHang.Count == 0)
                {
                    MessageBox.Show("Gio hang trong!", "Thong bao",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

                int? maCa = hoaDonDAL.GetCurrentCa();
                if (maCa == null)
                {
                    MessageBox.Show("Vui long mo ca truoc khi tao hoa don!", "Thong bao",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

                var hd = new HoaDon
                {
                    MaKhachHang = MaKhachHangHienTai,
                    TongTien = TinhTongTien(),
                    TienGiamGia = TienGiamGiaHienTai,
                    ThanhTien = TinhThanhTien(),
                    MaCa = maCa,
                    TrangThai = "ChoThanhToan"
                };

                int maHoaDon = hoaDonDAL.Insert(hd);

                // Luu chi tiet hoa don
                foreach (var item in GioHang)
                {
                    item.MaHoaDon = maHoaDon;
                    hoaDonDAL.InsertChiTiet(item);
                }

                return maHoaDon;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi tao hoa don: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        /// Xac nhan thanh toan hoa don
        /// </summary>
        public bool XacNhanThanhToan(int maHoaDon, string phuongThuc)
        {
            try
            {
                int result = hoaDonDAL.UpdateTrangThai(maHoaDon, "DaThanhToan", phuongThuc);
                if (result > 0)
                {
                    // Cap nhat diem cho khach hang neu co
                    if (MaKhachHangHienTai.HasValue)
                    {
                        var khBLL = new KhachHangBLL();
                        var hd = hoaDonDAL.GetById(maHoaDon);
                        if (hd != null)
                            khBLL.CapNhatDiem(MaKhachHangHienTai.Value, hd.ThanhTien);
                    }

                    XoaGioHang();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi xac nhan thanh toan: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Huy hoa don (fix orphan invoice)
        /// </summary>
        public bool HuyHoaDon(int maHoaDon)
        {
            try
            {
                string ghiChu = "Huy bo - nguoi dung dong form thanh toan.";
                int result = hoaDonDAL.UpdateTrangThaiHuy(maHoaDon, ghiChu);
                return result > 0;
            }
            catch (Exception ex)
            {
                // Khong show message box o day vi duoc goi tu OnClosed
                System.Diagnostics.Debug.WriteLine("Loi huy hoa don: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Lay thong tin hoa don kem chi tiet
        /// </summary>
        public HoaDon GetHoaDonWithDetails(int maHoaDon)
        {
            try
            {
                return hoaDonDAL.GetById(maHoaDon);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi lay thong tin hoa don: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        /// Lay chi tiet hoa don
        /// </summary>
        public List<ChiTietHoaDon> GetChiTietHoaDon(int maHoaDon)
        {
            try
            {
                return hoaDonDAL.GetChiTietByHoaDon(maHoaDon);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi lay chi tiet hoa don: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<ChiTietHoaDon>();
            }
        }

        /// <summary>
        /// Lay hoa don trong ca hien tai
        /// </summary>
        public List<HoaDon> GetHoaDonDaThanhToanByCa(int maCa)
        {
            try
            {
                return hoaDonDAL.GetDaThanhToanByCa(maCa);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi lay hoa don trong ca: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<HoaDon>();
            }
        }

        /// <summary>
        /// Lay MaCa hien tai
        /// </summary>
        public int? GetCurrentCa()
        {
            try
            {
                return hoaDonDAL.GetCurrentCa();
            }
            catch
            {
                return null;
            }
        }
    }
}
