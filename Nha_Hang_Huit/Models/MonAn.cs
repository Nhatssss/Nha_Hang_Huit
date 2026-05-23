using System;

namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Lop Model dai dien cho mon an trong menu nha hang
    /// </summary>
    public class MonAn
    {
        public int MaMonAn { get; set; }
        public string TenMonAn { get; set; }
        public decimal Gia { get; set; }
        public string NhomMon { get; set; }
        public string HinhAnh { get; set; }
        public string MoTa { get; set; }
        public string TrangThai { get; set; }  // Con / Het
        public DateTime NgayTao { get; set; }

        // Hien thi de dang cho ListBox/ComboBox
        public string HienThi => $"{TenMonAn} - {Gia:N0} VND";
    }
}
