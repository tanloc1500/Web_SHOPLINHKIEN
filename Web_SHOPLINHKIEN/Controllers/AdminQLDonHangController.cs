using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_SHOPLINHKIEN.Models;

namespace Web_SHOPLINHKIEN.Controllers
{
    public class AdminQLDonHangController : Controller
    {
        SHOPLINHKIENDataClassDataContext c = new SHOPLINHKIENDataClassDataContext();
        // GET: AdminQLDonHang
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult trangQLDonHang()
        {
            if (Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            if (TempData["TB"] != null)
            {
                ViewBag.thongBao = TempData["TB"];
            }

            List<joinTable_KHG_DONHG> a = (from t1 in c.KHACHHANGs
                                           join t2 in c.DONHANGs on t1.MaKH equals t2.MaKH
                                           join t3 in c.TRANGTHAIDONHANGs on t2.MaTrangThai equals t3.MaTrangThai
                                           orderby t2.NgayGiaoHang descending
                                           select new joinTable_KHG_DONHG
                                           {
                                               KHACHHANG = t1,
                                               DONHANG = t2,
                                               TRANGTHAIDONHANG = t3,
                                           }).ToList();
            return View(a);
        }

        public ActionResult UserName(string id)
        {
            if (Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            var us = (from t1 in c.KHACHHANGs
                      join t2 in c.TAIKHOANs on t1.MaTK equals t2.MaTK
                      where t1.MaTK == id
                      select new joinTable_KHG_TAIKHOAN
                      {
                          KHACHHANG = t1,
                          TAIKHOAN = t2,
                      }).SingleOrDefault();
            return View(us);
        }

        public ActionResult DaGiaoHang(string id)
        {
            if (Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            DONHANG dh = c.DONHANGs.SingleOrDefault(x => x.MaDH == id);
            dh.MaTrangThai = "DGH";
            dh.NgayGiaoHang = DateTime.Now;
            c.SubmitChanges();
            return RedirectToAction("trangQLDonHang", "AdminQLDonHang");
        }

        public ActionResult trangQLDonHang_xoa(string id)
        {
            if (Session["admin"] == null || String.IsNullOrEmpty(id))
                return RedirectToAction("trangChu", "Guest");
            DONHANG dh = c.DONHANGs.SingleOrDefault(x => x.MaDH == id);
            if (dh.MaTrangThai == "DGH")
            {
                TempData["TB"] = "Đơn hàng đã được giao, không được hủy đơn";
                return RedirectToAction("trangLSMuaHang", "UserLSMuaHang");
            }
            try
            {
                List<DONHANGCHITIET> dhct = c.DONHANGCHITIETs.Where(x => x.MaDH == dh.MaDH).ToList();
                foreach (DONHANGCHITIET ct in dhct)
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
            return RedirectToAction("trangQLDonHang", "AdminQLDonHang");
        }

        public ActionResult trangQLDonHang_xem(string id)
        {
            if (id == null || Session["admin"] == null)
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

        public ActionResult trangQLDonHang_sua(string id)
        {
            if (Session["admin"] == null || String.IsNullOrEmpty(id))
                return RedirectToAction("trangChu", "Guest");
            List<joinTable_KHG_DONHG_CHITIET> b = (from t1 in c.DONHANGs
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
                                                   }).ToList();
            joinTable_KHG_DONHG_CHITIET d = b.First();
            if (d.DONHANG.MaTrangThai == "DGH")
            {
                TempData["TB"] = "Đơn hàng đã được giao, không được sửa";
                return RedirectToAction("trangQLDonHang", "AdminQLDonHang");
            }
            var a = (from p in b
                     group p by new { p.SANPHAM.MaSP }
                     into anh
                     select anh.FirstOrDefault());
            return View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult trangQLDonHang_sua(string MaDH, string sdt, string diachi, string ghichu)
        {
            bool allIsTrue = true;

            DONHANG dh = c.DONHANGs.SingleOrDefault(x => x.MaDH == MaDH.Trim());

            if (String.IsNullOrEmpty(sdt))
            {
                ViewData["loisdt"] = "Phải nhập số điện thoại";
                allIsTrue = false;
            }
            else if (sdt.Length != 10)
            {
                ViewData["loisdt"] = "Phải nhập số điện thoại có 10 kí tự số";
                allIsTrue = false;
            }
            else if (!sdt.StartsWith("0"))
            {
                ViewData["loisdt"] = "Phải nhập số điện thoại bắt đầu từ số 0";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(diachi))
            {
                ViewData["loidiachi"] = "Phải nhập địa chỉ";
                allIsTrue = false;
            }
            else if (diachi.Length < 20)
            {
                ViewData["loidiachi"] = "Phải nhập địa chỉ ít nhất 20 kí tự";
                allIsTrue = false;
            }
            else if (diachi.Length > 200)
            {
                ViewData["loidiachi"] = "Phải nhập địa chỉ không quá 200 kí tự";
                allIsTrue = false;
            }

            if (ghichu.Length > 200)
            {
                ViewData["loighichu"] = "Phải nhập ghi chú không quá 200 kí tự";
                allIsTrue = false;
            }

            List<joinTable_KHG_DONHG_CHITIET> b = (from t1 in c.DONHANGs
                                                   join t2 in c.TRANGTHAIDONHANGs on t1.MaTrangThai equals t2.MaTrangThai
                                                   join t3 in c.DONHANGCHITIETs on t1.MaDH equals t3.MaDH
                                                   join t4 in c.KHACHHANGs on t1.MaKH equals t4.MaKH
                                                   join t5 in c.SANPHAMs on t3.MaSP equals t5.MaSP
                                                   join t6 in c.ANHSANPHAMs on t5.MaSP equals t6.MaSP
                                                   where t1.MaDH == MaDH
                                                   select new joinTable_KHG_DONHG_CHITIET
                                                   {
                                                       DONHANG = t1,
                                                       TRANGTHAIDONHANG = t2,
                                                       DONHANGCHITIET = t3,
                                                       KHACHHANG = t4,
                                                       SANPHAM = t5,
                                                       ANHSANPHAM = t6,
                                                   }).ToList();
            joinTable_KHG_DONHG_CHITIET d = b.First();
            if (d.DONHANG.MaTrangThai == "DGH")
            {
                allIsTrue = false;
                TempData["TB"] = "Đơn hàng đã được giao, không được sửa";
                return RedirectToAction("trangQLDonHang", "AdminQLDonHang");
            }

            if (allIsTrue)
            {
                
                dh.SDT = sdt;
                dh.DiaChi = diachi;
                dh.GhiChu = ghichu;
                try
                {
                    c.SubmitChanges();
                }
                catch
                {
                    TempData["TB"] = "Không sửa được, có lỗi xảy ra";
                }
            }

            return trangQLDonHang_sua(b.First().SANPHAM.MaSP.Trim());
        }
    }
}