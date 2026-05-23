using System;

namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Lop Model dai dien cho khach hang va thong tin tich diem
    /// </summary>
    public class KhachHang
    {
        public int MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime NgayDangKy { get; set; }
        public int TongDiemTichLuy { get; set; }
        public string HangThe { get; set; }  // Thuong, Bac, Vang, KimCuong

        // Hang so giam gia theo hang the
        public const double GIAM_GIA_THUONG = 0.0;
        public const double GIAM_GIA_BAC = 0.03;
        public const double GIAM_GIA_VANG = 0.05;
        public const double GIAM_GIA_KIM_CUONG = 0.10;
        public const int DIEM_PER_10K = 1;  // 10,000 VND = 1 diem

        /// <summary>
        /// Lay ti le giam gia tuong ung voi hang the
        /// </summary>
        public double LayTiLeGiamGia()
        {
            switch (HangThe)
            {
                case "KimCuong": return GIAM_GIA_KIM_CUONG;
                case "Vang":     return GIAM_GIA_VANG;
                case "Bac":      return GIAM_GIA_BAC;
                default:         return GIAM_GIA_THUONG;
            }
        }

        /// <summary>
        /// Tinh hang the tu tong diem tich luy
        /// </summary>
        public static string TinhHangThe(int tongDiem)
        {
            if (tongDiem >= 1000) return "KimCuong";
            if (tongDiem >= 500)  return "Vang";
            if (tongDiem >= 100)  return "Bac";
            return "Thuong";
        }

        public string HienThi => $"{TenKhachHang} - {SoDienThoai} ({HangThe})";
    }
}
