using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_SHOPLINHKIEN.Models;

using PagedList;
using PagedList.Mvc;

namespace Web_SHOPLINHKIEN.Controllers
{
    public class GuestController : Controller
    {
        // GET: Guest
        SHOPLINHKIENDataClassDataContext c = new SHOPLINHKIENDataClassDataContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult trangChu()
        {
            if (Session["admin"] != null)
                return RedirectToAction("trangChuAdmin", "Admin");
            return View();
        }

        public ActionResult trangChu_SPmoi()
        {
            var b = (from t1 in c.SANPHAMs
                     join t2 in c.HANGSANXUATs on t1.MaHangSX equals t2.MaHangSX
                     join t3 in c.LOAISANPHAMs on t1.MaLoaiSP equals t3.MaLoaiSP
                     join t4 in c.ANHSANPHAMs on t1.MaSP equals t4.MaSP
                     orderby t1.NgayPhatHanh descending
                     where t1.SoLuong > 0
                     select new joinTable_SANPHAM
                     {
                         SANPHAM = t1,
                         HANGSANXUAT = t2,
                         LOAISANPHAM = t3,
                         ANHSANPHAM = t4,
                     });
            var a = (from p in b
                     group p by new { p.SANPHAM.MaSP }
                    into anh
                     select anh.FirstOrDefault()).Take(12);
            return View(a);
        }

        public ActionResult trangChu_SPbanchay()
        {
            var b = (from t1 in c.SANPHAMs
                     join t2 in c.HANGSANXUATs on t1.MaHangSX equals t2.MaHangSX
                     join t3 in c.LOAISANPHAMs on t1.MaLoaiSP equals t3.MaLoaiSP
                     join t4 in c.ANHSANPHAMs on t1.MaSP equals t4.MaSP
                     orderby t1.SoLuongDaBan descending, t1.NgayPhatHanh ascending
                     where t1.SoLuong > 0
                     select new joinTable_SANPHAM
                     {
                         SANPHAM = t1,
                         HANGSANXUAT = t2,
                         LOAISANPHAM = t3,
                         ANHSANPHAM = t4,
                     });
            var a = (from p in b
                     group p by new { p.SANPHAM.MaSP }
                    into anh
                     select anh.FirstOrDefault()).Take(6);
            return View(a);
        }

        public ActionResult _Layout_trangChu_DanhMucSP()
        {
            var a = from t1 in c.LOAISANPHAMs
                    select t1;
            return View(a);
        }

        public ActionResult _Layout_trangChu_DanhMucSP_soluong(string id)
        {
            var a = c.SANPHAMs.Where(x => x.MaLoaiSP == id);
            return View(a);
        }

        public ActionResult _Layout_trangChu_HangSX()
        {
            var a = from t1 in c.HANGSANXUATs
                    select t1;
            return View(a);
        }

        public ActionResult _Layout_trangChu_HangSX_soluong(string id)
        {
            var a = c.SANPHAMs.Where(x => x.MaHangSX == id);
            return View(a);
        }

        [HttpPost]
        public ActionResult trangTimKiem(string srchFld)
        {
            if (!String.IsNullOrEmpty(srchFld))
                return RedirectToAction("trangTimKiem", "Guest", new { ten = srchFld });
            TempData["controller"] = "Guest";
            TempData["action"] = "trangTimKiem";
            var b = (from t1 in c.SANPHAMs
                     join t2 in c.HANGSANXUATs on t1.MaHangSX equals t2.MaHangSX
                     join t3 in c.LOAISANPHAMs on t1.MaLoaiSP equals t3.MaLoaiSP
                     join t4 in c.ANHSANPHAMs on t1.MaSP equals t4.MaSP
                     where t1.SoLuong > 0
                     select new joinTable_SANPHAM
                     {
                         SANPHAM = t1,
                         HANGSANXUAT = t2,
                         LOAISANPHAM = t3,
                         ANHSANPHAM = t4,
                     });
            var links = (from p in b
                         group p by new { p.SANPHAM.MaSP }
                    into anh
                         select anh.FirstOrDefault());
            return View(links);
        }

        public ActionResult trangTimKiem(string ten, string idHangSX, string idLoaiSP, int ? page)
        {
            if (Session["admin"] != null)
                return RedirectToAction("trangChuAdmin", "Admin");
            TempData["controller"] = "Guest";
            TempData["action"] = "trangTimKiem";
            var b = (from t1 in c.SANPHAMs
                     join t2 in c.HANGSANXUATs on t1.MaHangSX equals t2.MaHangSX
                     join t3 in c.LOAISANPHAMs on t1.MaLoaiSP equals t3.MaLoaiSP
                     join t4 in c.ANHSANPHAMs on t1.MaSP equals t4.MaSP
                     where t1.SoLuong > 0
                     select new joinTable_SANPHAM
                     {
                         SANPHAM = t1,
                         HANGSANXUAT = t2,
                         LOAISANPHAM = t3,
                         ANHSANPHAM = t4,
                     });
            var links = (from p in b
                     group p by new { p.SANPHAM.MaSP }
                    into anh
                     select anh.FirstOrDefault());
            if(!String.IsNullOrEmpty(ten))
            {
                links = links.Where(x => x.SANPHAM.TenSP.Contains(ten));
                ViewBag.ten = ten;
                
            }
            if(!String.IsNullOrEmpty(idHangSX))
            {
                links = links.Where(x => x.SANPHAM.MaHangSX == idHangSX.Trim());
                ViewBag.idHangSX = idHangSX.Trim();
                
            }
            if (!String.IsNullOrEmpty(idLoaiSP))
            {
                links = links.Where(x => x.SANPHAM.MaLoaiSP == idLoaiSP.Trim());
                ViewBag.idLoaiSP = idLoaiSP.Trim();
                
            }

            int pageSize = 6;
            int pageNum = (page ?? 1);

            return View(links.ToPagedList(pageNum, pageSize));
        }

        public ActionResult trangSP(string id)
        {
            if (Session["admin"] != null)
                return RedirectToAction("trangChuAdmin", "Admin");
            TempData["controller"] = "Guest";
            TempData["action"] = "trangSP";
            TempData["id"] = id;
            if (id == null)
                return RedirectToAction("trangChu", "Guest");
            var b = (from t1 in c.SANPHAMs
                     join t2 in c.HANGSANXUATs on t1.MaHangSX equals t2.MaHangSX
                     join t3 in c.LOAISANPHAMs on t1.MaLoaiSP equals t3.MaLoaiSP
                     join t4 in c.ANHSANPHAMs on t1.MaSP equals t4.MaSP
                     where t1.MaSP==id
                     select new joinTable_SANPHAM
                     {
                         SANPHAM = t1,
                         HANGSANXUAT = t2,
                         LOAISANPHAM = t3,
                         ANHSANPHAM = t4,
                     }).FirstOrDefault();
            return View(b);
        }

        public ActionResult trangSP_anh(string id)
        {
            var b = c.ANHSANPHAMs.Where(x => x.MaSP == id).ToList();
            return View(b);
        }

        public ActionResult trangSP_lienquan(string id)
        {
            SANPHAM sp = c.SANPHAMs.Where(x=>x.MaSP == id).FirstOrDefault();

            var b = (from t1 in c.SANPHAMs
                     join t2 in c.HANGSANXUATs on t1.MaHangSX equals t2.MaHangSX
                     join t3 in c.LOAISANPHAMs on t1.MaLoaiSP equals t3.MaLoaiSP
                     join t4 in c.ANHSANPHAMs on t1.MaSP equals t4.MaSP
                     where t1.SoLuong > 0
                     where t1.MaSP != id
                     where t1.MaLoaiSP == sp.MaLoaiSP.Trim()
                     select new joinTable_SANPHAM
                     {
                         SANPHAM = t1,
                         HANGSANXUAT = t2,
                         LOAISANPHAM = t3,
                         ANHSANPHAM = t4,
                     });
            var a = (from p in b
                     group p by new { p.SANPHAM.MaSP }
                    into anh
                     select anh.FirstOrDefault())
                     .ToList().OrderBy(x => Guid.NewGuid())
                     .ToList().Take(6);
            return View(a);
        }

    }
}