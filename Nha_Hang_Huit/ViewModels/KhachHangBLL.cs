using System;
using System.Collections.Generic;
using System.Windows;
using Nha_Hang_Huit.DAL;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// Lop BLL xu ly logic cho khach hang va tich diem
    /// </summary>
    public class KhachHangBLL
    {
        private KhachHangDAL khachHangDAL = new KhachHangDAL();

        /// <summary>
        /// Tim khach hang theo so dien thoai
        /// </summary>
        public KhachHang GetByPhone(string soDienThoai)
        {
            try
            {
                return khachHangDAL.GetByPhone(soDienThoai);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi tim khach hang: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        /// Lay khach hang theo MaKhachHang
        /// </summary>
        public KhachHang GetById(int maKhachHang)
        {
            try
            {
                return khachHangDAL.GetById(maKhachHang);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi lay thong tin khach hang: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        /// Lay danh sach tat ca khach hang
        /// </summary>
        public List<KhachHang> GetAll()
        {
            try
            {
                return khachHangDAL.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi lay danh sach khach hang: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<KhachHang>();
            }
        }

        /// <summary>
        /// Them khach hang moi
        /// </summary>
        public int Add(KhachHang kh)
        {
            try
            {
                return khachHangDAL.Insert(kh);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi them khach hang: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }

        /// <summary>
        /// Tim hoac tao khach hang theo so dien thoai
        /// Tra ve MaKhachHang neu tim thay hoac tao moi thanh cong, null neu that bai
        /// </summary>
        public int? FindOrCreate(string tenKhachHang, string soDienThoai)
        {
            try
            {
                KhachHang existing = khachHangDAL.GetByPhone(soDienThoai);
                if (existing != null)
                    return existing.MaKhachHang;

                // Khach moi: tao moi
                var newKh = new KhachHang
                {
                    TenKhachHang = tenKhachHang,
                    SoDienThoai = soDienThoai
                };
                return khachHangDAL.Insert(newKh);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi tim/tao khach hang: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        /// Tinh tien giam gia dua tren hang the khach hang
        /// </summary>
        public decimal TinhTienGiamGia(KhachHang kh, decimal tongTien)
        {
            if (kh == null) return 0;
            double tiLeGiam = kh.LayTiLeGiamGia();
            return Math.Round(tongTien * (decimal)tiLeGiam, 0);
        }

        /// <summary>
        /// Cap nhat diem tich luy cho khach hang sau khi thanh toan
        /// </summary>
        public void CapNhatDiem(int maKhachHang, decimal tongTien)
        {
            try
            {
                khachHangDAL.CapNhatDiem(maKhachHang, tongTien);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi cap nhat diem tich luy: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Dem so khach hang moi dang ky trong khoang thoi gian
        /// </summary>
        public int DemKhachMoiTrongCa(DateTime tu, DateTime den)
        {
            try
            {
                return khachHangDAL.DemKhachMoiTrongCa(tu, den);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi dem khach moi: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }
    }
}
