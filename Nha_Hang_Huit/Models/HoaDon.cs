using System;

namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Lop Model dai dien cho hoa don
    /// </summary>
    public class HoaDon
    {
        public int MaHoaDon { get; set; }
        public int? MaKhachHang { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal TongTien { get; set; }
        public decimal TienGiamGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string PhuongThucThanhToan { get; set; }  // TienMat, QRCode
        public string TrangThai { get; set; }  // ChoThanhToan, DaThanhToan, DaHuy
        public int? MaCa { get; set; }
        public string GhiChu { get; set; }

        public string HienThi => $"HD#{MaHoaDon} - {ThanhTien:N0} VND [{TrangThai}]";
    }
}
