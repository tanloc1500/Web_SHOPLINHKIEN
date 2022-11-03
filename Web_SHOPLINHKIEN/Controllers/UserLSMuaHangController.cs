using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_SHOPLINHKIEN.Models;

namespace Web_SHOPLINHKIEN.Controllers
{
    public class UserLSMuaHangController : Controller
    {
        // GET: UserLSMuaHang
        SHOPLINHKIENDataClassDataContext c = new SHOPLINHKIENDataClassDataContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult trangLSMuaHang()
        {
            if (Session["admin"] != null)
                return RedirectToAction("trangChuAdmin", "Admin");
            if (Session["user"] == null || Session["khachhang"] == null)
                return RedirectToAction("trangChu", "Guest");
            if (TempData["TB"] != null)
            {
                ViewBag.thongBao = TempData["TB"];
            }
                
            var a = from t1 in c.KHACHHANGs
                    join t2 in c.DONHANGs on t1.MaKH equals t2.MaKH
                    join t3 in c.TRANGTHAIDONHANGs on t2.MaTrangThai equals t3.MaTrangThai
                    where t1.MaTK == Session["user"].ToString()
                    orderby t2.MaDH descending
                    select new joinTable_KHG_DONHG
                    {
                        KHACHHANG = t1,
                        DONHANG = t2,
                        TRANGTHAIDONHANG = t3,
                    };
            return View(a);
        }

        public ActionResult trangLSMuaHang_xoa(string id)
        {
            if (Session["user"] == null || Session["khachhang"] == null || String.IsNullOrEmpty(id))
                return RedirectToAction("trangChu", "Guest");
            DONHANG dh = c.DONHANGs.SingleOrDefault(x => x.MaDH == id);
            if(dh.MaTrangThai == "DGH")
            {
                TempData["TB"] = "Đơn hàng đã được giao, không được hủy đơn";
                return RedirectToAction("trangLSMuaHang", "UserLSMuaHang");
            }
            try
            {
                List<DONHANGCHITIET> dhct = c.DONHANGCHITIETs.Where(x => x.MaDH == dh.MaDH).ToList();
                foreach(DONHANGCHITIET ct in dhct)
                {
                    SANPHAM sp = c.SANPHAMs.SingleOrDefault(x => x.MaSP == ct.MaSP);
                    sp.SoLuongDaBan -= (int)ct.SoLuong;
                    sp.SoLuong += (int)ct.SoLuong;
                    c.DONHANGCHITIETs.DeleteOnSubmit(ct);
                    c.SubmitChanges();
                }
                c.DONHANGs.DeleteOnSubmit(dh);
                c.SubmitChanges();
            }
            catch
            {
                TempData["TB"] = "Không xóa được";
            }
            return RedirectToAction("trangLSMuaHang", "UserLSMuaHang");
        }

        public ActionResult trangLSMuaHang_xemchitiet(string id)
        {
            if (id == null || Session["user"] == null || Session["khachhang"] == null)
            {
                return RedirectToAction("trangChu", "Guest");
            }

            var b = from t1 in c.DONHANGs
                    join t2 in c.TRANGTHAIDONHANGs on t1.MaTrangThai equals t2.MaTrangThai
                    join t3 in c.DONHANGCHITIETs on t1.MaDH equals t3.MaDH
                    join t4 in c.KHACHHANGs on t1.MaKH equals t4.MaKH
                    join t5 in c.SANPHAMs on t3.MaSP equals t5.MaSP
                    join t6 in c.ANHSANPHAMs on t5.MaSP equals t6.MaSP
                    where t1.MaDH == id
                    select new joinTable_KHG_DONHG_CHITIET
                    {
                        DONHANG = t1,
                        TRANGTHAIDONHANG = t2,
                        DONHANGCHITIET = t3,
                        KHACHHANG = t4,
                        SANPHAM = t5,
                        ANHSANPHAM = t6,
                    };
            var a = (from p in b
                     group p by new { p.SANPHAM.MaSP }
                     into anh
                     select anh.FirstOrDefault());
            return View(a);
        }

    }
}