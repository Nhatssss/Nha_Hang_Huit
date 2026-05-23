using System;
using System.Collections.Generic;
using System.Windows;
using Nha_Hang_Huit.DAL;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// Lop BLL xu ly logic cho mon an, goi DAL de lay/ghi du lieu
    /// </summary>
    public class MonAnBLL
    {
        private MonAnDAL monAnDAL = new MonAnDAL();

        /// <summary>
        /// Lay danh sach tat ca mon an
        /// </summary>
        public List<MonAn> GetAll()
        {
            try
            {
                return monAnDAL.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi lay danh sach mon an: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<MonAn>();
            }
        }

        /// <summary>
        /// Lay mon an theo nhom (Lo Nuong, Mon Nhung, Mon Them, Do Uong)
        /// </summary>
        public List<MonAn> GetByNhom(string nhomMon)
        {
            try
            {
                return monAnDAL.GetByNhom(nhomMon);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi lay mon an theo nhom: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<MonAn>();
            }
        }

        /// <summary>
        /// Lay danh sach cac nhom mon
        /// </summary>
        public List<string> GetAllNhomMon()
        {
            try
            {
                return monAnDAL.GetAllNhomMon();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi lay danh sach nhom mon: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<string>();
            }
        }

        /// <summary>
        /// Lay mon an theo MaMonAn
        /// </summary>
        public MonAn GetById(int maMonAn)
        {
            try
            {
                return monAnDAL.GetById(maMonAn);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi lay thong tin mon an: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        /// Them mon an moi
        /// </summary>
        public bool Add(MonAn monAn)
        {
            try
            {
                return monAnDAL.Insert(monAn) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi them mon an: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Cap nhat mon an
        /// </summary>
        public bool Update(MonAn monAn)
        {
            try
            {
                return monAnDAL.Update(monAn) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi cap nhat mon an: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Xoa mon an
        /// </summary>
        public bool Delete(int maMonAn)
        {
            try
            {
                return monAnDAL.Delete(maMonAn) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi xoa mon an: " + ex.Message, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
