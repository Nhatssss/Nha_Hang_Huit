using System;

namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Lop Model dai dien cho lich su dong ca
    /// </summary>
    public class LichSuCa
    {
        public int MaCa { get; set; }
        public DateTime GioBatDau { get; set; }
        public DateTime? GioKetThuc { get; set; }
        public string NhanVien { get; set; }
        public int TongSoHoaDon { get; set; }
        public decimal TongDoanhThuTruocGiamGia { get; set; }
        public decimal TongTienGiamGia { get; set; }
        public decimal TongDoanhThuThucNhan { get; set; }
        public int SoKhachMoi { get; set; }
        public string GhiChu { get; set; }

        public string HienThi => $"Ca #{MaCa} - {GioBatDau:dd/MM HH:mm}";
    }
}
