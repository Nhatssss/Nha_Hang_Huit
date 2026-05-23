using System;

namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Lop Model dai dien cho chi tiet tung mon trong hoa don
    /// </summary>
    public class ChiTietHoaDon
    {
        public int MaChiTiet { get; set; }
        public int MaHoaDon { get; set; }
        public int MaMonAn { get; set; }
        public string TenMonAn { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;

        /// <summary>
        /// Item tam cho gio hang truoc khi luu vao DB
        /// </summary>
        public bool IsTempItem { get; set; }

        public string HienThi => $"{TenMonAn} x{SoLuong} = {ThanhTien:N0} VND";
    }
}
