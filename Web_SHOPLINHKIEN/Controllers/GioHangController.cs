using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_SHOPLINHKIEN.Models;

namespace Web_SHOPLINHKIEN.Controllers
{
    public class GioHangController : Controller
    {
        SHOPLINHKIENDataClassDataContext c = new SHOPLINHKIENDataClassDataContext();
        // GET: GioHang
        public ActionResult Index()
        {
            return View();
        }

        public List<GioHang> LayGioHang()
        {
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if(listGioHang == null)
            {
                listGioHang = new List<GioHang>();
                Session["GioHang"] = listGioHang;
            }
            return listGioHang;
        }

        public ActionResult ThemGioHang(string sMaSP, string sURL)
        {
            if (Session["admin"] != null)
                return RedirectToAction("trangChuAdmin", "Admin");
            List<GioHang> listGioHang = LayGioHang();
            GioHang sanpham = listGioHang.Find(x => x.sMaSP == sMaSP);
            if(sanpham == null)
            {
                sanpham = new GioHang(sMaSP);
                listGioHang.Add(sanpham);
                return Redirect(sURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(sURL);
            }
        }

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if(listGioHang != null)
            {
                iTongSoLuong = listGioHang.Sum(x => x.iSoLuong);
            }
            return iTongSoLuong;
        }

        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if(listGioHang != null)
            {
                iTongTien = listGioHang.Sum(x => x.dThanhTien);
            }
            return iTongTien;
        }

        public ActionResult GioHang()
        {
            if (Session["admin"] != null)
                return RedirectToAction("trangChuAdmin", "Admin");
            List<GioHang> listGioHang = LayGioHang();
            if(listGioHang.Count == 0)
            {
                return RedirectToAction("trangChu", "Guest");
            }
            else
            {
                ViewBag.TongSoLuong = TongSoLuong();
                ViewBag.TongTien = TongTien();
            }
            return View(listGioHang);
        }

        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        public ActionResult XoaGioHang(string sMaSP)
        {
            List<GioHang> listGioHang = LayGioHang();
            GioHang sanpham = listGioHang.SingleOrDefault(x => x.sMaSP == sMaSP);
            if(sanpham != null)
            {
                listGioHang.RemoveAll(x => x.sMaSP == sMaSP);
                return RedirectToAction("GioHang", "GioHang");
            }
            if(listGioHang.Count == 0)
            {
                return RedirectToAction("trangChu", "Guest");
            }
            return RedirectToAction("GioHang", "GioHang");
        }

        public ActionResult CapNhatGioHang(string sMaSP, string soluong)
        {
            List<GioHang> listGioHang = LayGioHang();
            GioHang sanpham = listGioHang.SingleOrDefault(x => x.sMaSP == sMaSP);
            if(sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(soluong);
            }
            return RedirectToAction("GioHang", "GioHang");
        }

        public ActionResult XoaTatCaGioHang()
        {
            List<GioHang> listGioHang = LayGioHang();
            listGioHang.Clear();
            return RedirectToAction("trangChu", "Guest");
        }

        public ActionResult DatHang()
        {
            if (Session["admin"] != null)
                return RedirectToAction("trangChuAdmin", "Admin");
            TempData["controller"] = "GioHang";
            TempData["action"] = "DatHang";
            if(Session["user"] == null || Session["khachhang"] == null)
            {
                return RedirectToAction("trangDangNhap", "User");
            }
                    
            List<GioHang> listGioHang = LayGioHang();
            if (listGioHang.Count == 0)
            {
                return RedirectToAction("trangChu", "Guest");
            }
            else
            {
                ViewBag.TongSoLuong = TongSoLuong();
                ViewBag.TongTien = TongTien();
            }
            return View(listGioHang);
        }

        [HttpPost]
        public ActionResult DatHang(string sdt, string diachi, string ghichu)
        {
            bool allIsTrue = true;

            DONHANG dh = new DONHANG();
            DateTime dathang = DateTime.Now;
            KHACHHANG khang = (KHACHHANG)Session["khachhang"];

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
            
            if (ghichu.Length > 500)
            {
                ViewData["loighichu"] = "Phải nhập ghi chú ít hơn 500 kí tự";
                allIsTrue = false;
            }

            if (allIsTrue)
            {
                dh.MaKH = khang.MaKH.Trim();
                dh.SDT = sdt;
                dh.DiaChi = diachi;
                dh.NgayDatHang = dathang;
                dh.MaDH = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "DHG";
                dh.MaTrangThai = "DZH";
                c.DONHANGs.InsertOnSubmit(dh);
                c.SubmitChanges();

                List<GioHang> listGioHang = LayGioHang();
                foreach (var itemkt in listGioHang)
                {
                    SANPHAM sp = c.SANPHAMs.SingleOrDefault(x => x.MaSP == itemkt.sMaSP);
                    if (sp.SoLuong - itemkt.iSoLuong < 0)
                    {
                        ViewBag.thongBao = "Có lỗi xảy ra! Sản phẩm " + sp.TenSP +
                            " có số lượng nhiều hơn số lượng hiện có trong kho (" + sp.SoLuong + " cái).";
                        CapNhatGioHang(sp.MaSP, sp.SoLuong.ToString());
                        c.DONHANGs.DeleteOnSubmit(dh);
                        return DatHang();
                    }
                }
                foreach (var item in listGioHang)
                {
                    DONHANGCHITIET dhct = new DONHANGCHITIET();
                    SANPHAM sp = c.SANPHAMs.SingleOrDefault(x => x.MaSP == item.sMaSP);
                    dhct.MaDH = dh.MaDH;
                    dhct.MaSP = item.sMaSP;
                    dhct.SoLuong = item.iSoLuong;
                    dhct.DonGia = (decimal)item.dDonGia;
                    sp.SoLuong -= item.iSoLuong;
                    sp.SoLuongDaBan += item.iSoLuong;
                    c.DONHANGCHITIETs.InsertOnSubmit(dhct);
                    c.SubmitChanges();
                }
                Session["GioHang"] = null;
                return RedirectToAction("XacNhanDonHang", "GioHang", new { id = dh.MaDH });
            }
            return DatHang();  
        }

        public ActionResult XacNhanDonHang(string id)
        {
            if (Session["admin"] != null)
                return RedirectToAction("trangChuAdmin", "Admin");
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