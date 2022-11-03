using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_SHOPLINHKIEN.Models
{
    public class GioHang
    {
        SHOPLINHKIENDataClassDataContext data=new SHOPLINHKIENDataClassDataContext();
        public string sMaSP { get; set; }
        public string sTenSP { get; set; }
        public string sAnhSP { get; set; }
        public Double dDonGia { get; set; }
        public int iSoLuong { get; set; }
        public int iSLMax { get; set; }
        public Double dThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }

        public GioHang(string MaSP)
        {
            sMaSP = MaSP;
            SANPHAM sp = data.SANPHAMs.Single(x => x.MaSP == sMaSP);
            ANHSANPHAM anhSP = data.ANHSANPHAMs.First(x => x.MaSP == sMaSP);
            sTenSP = sp.TenSP;
            sAnhSP = anhSP.FileAnh;
            dDonGia = double.Parse(sp.GiaTien.ToString());
            iSoLuong = 1;
            iSLMax = sp.SoLuong;
        }
    }
}