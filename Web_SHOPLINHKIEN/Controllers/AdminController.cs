using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_SHOPLINHKIEN.Models;

namespace Web_SHOPLINHKIEN.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        SHOPLINHKIENDataClassDataContext c = new SHOPLINHKIENDataClassDataContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult trangChuAdmin()
        {
            if (Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            return View();
        }

        public ActionResult trangQLLoaiSP()
        {
            if (Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            if (TempData["TB"] != null)
            {
                ViewBag.thongBao = TempData["TB"];
                
                var b = from t1 in c.LOAISANPHAMs
                        select t1;
                return View(b);
            }
            var a = from t1 in c.LOAISANPHAMs
                    select t1;
            return View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult trangQLLoaiSP(string ma, string ten)
        {
            bool allIsTrue = true;

            ma.Trim();

            if (String.IsNullOrEmpty(ten))
            {
                ViewData["loiten"+ma] = "Phải nhập tên loại sản phẩm";
                allIsTrue = false;
            }
            else if (ten.Length < 5)
            {
                ViewData["loiten"+ma] = "Phải nhập tên loại sản phẩm có ít nhất 5 kí tự";
                allIsTrue = false;
            }
            else if (ten.Length > 50)
            {
                ViewData["loiten"+ma] = "Phải nhập tên loại sản phẩm có ít hơn 50 kí tự";
                allIsTrue = false;
            }
            if(allIsTrue)
            {
                LOAISANPHAM lsp = c.LOAISANPHAMs.SingleOrDefault(x => x.MaLoaiSP == ma);
                lsp.TenLoaiSP = ten.Trim();
                try
                {
                    c.SubmitChanges();
                    ViewData.Clear();
                    ViewBag.thongBao = "Cập nhật thành công";
                }
                catch
                {
                    ViewData.Clear();
                    ViewBag.thongBao = "Cập nhật thất bại";
                }
            }
            return trangQLLoaiSP();
        }

        public ActionResult trangQLLoaiSP_xoa(string id)
        {
            id.Trim();
            LOAISANPHAM lsp = c.LOAISANPHAMs.SingleOrDefault(x => x.MaLoaiSP == id);
            try
            {
                c.LOAISANPHAMs.DeleteOnSubmit(lsp);
                c.SubmitChanges();
                TempData["TB"] = "Xóa thành công";
            }
            catch
            {
                TempData["TB"] = "Không xóa được";
            }
            return RedirectToAction("trangQLLoaiSP", "Admin");
        }

        public ActionResult trangQLLoaiSP_themmoi()
        {
            if(Session["admin"] == null)
            {
                return RedirectToAction("trangChu", "Guest");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult trangQLLoaiSP_themmoi(string MaLoaiSP, string TenLoaiSP)
        {
            MaLoaiSP.Trim();
            ViewData["maloaisp"] = MaLoaiSP;
            ViewData["tenloaisp"] = TenLoaiSP;

            bool allIsTrue = true;
            if (String.IsNullOrEmpty(MaLoaiSP))
            {
                ViewData["loimaloaisp"] = "Phải nhập mã loại sản phẩm";
                allIsTrue = false;
            }
            else if (MaLoaiSP.Length != 5)
            {
                ViewData["loimaloaisp"] = "Mã loại sản phẩm phải có 5 kí tự";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(TenLoaiSP))
            {
                ViewData["loitenloaisp"] = "Phải nhập tên loại sản phẩm";
                allIsTrue = false;
            }
            else if (TenLoaiSP.Length < 5)
            {
                ViewData["loitenloaisp"] = "Phải nhập tên loại sản phẩm có ít nhất 5 kí tự";
                allIsTrue = false;
            }
            else if (TenLoaiSP.Length > 50)
            {
                ViewData["loitenloaisp"] = "Phải nhập tên loại sản phẩm không quá 50 kí tự";
                allIsTrue = false;
            }
            
            var kt = c.LOAISANPHAMs.Where(x => x.MaLoaiSP == MaLoaiSP);
            if (kt.Any())
            {
                LOAISANPHAM ktlsp = (LOAISANPHAM)kt;
                ViewData["loimaloaisp"] = "Mã loại sản phẩm " + ktlsp.MaLoaiSP +
                    " đã được nhập cho loại sản phẩm " + ktlsp.TenLoaiSP;
                allIsTrue = false;
            }

            if (allIsTrue)
            {
                LOAISANPHAM lsp = new LOAISANPHAM();
                lsp.MaLoaiSP = MaLoaiSP;
                lsp.TenLoaiSP = TenLoaiSP;
                try
                {
                    c.LOAISANPHAMs.InsertOnSubmit(lsp);
                    c.SubmitChanges();
                    ViewData.Clear();
                    TempData["TB"] = "Thêm thành công";
                    return RedirectToAction("trangQLLoaiSP", "Admin");
                }
                catch
                {
                    ViewData["loimaloaisp"] = "Mã loại sản phẩm không được sử dụng các ký tự đặc biệt!";
                    ViewBag.thongBao = "Thêm thất bại";
                }
            }
            return View();
        }

        public ActionResult trangQLHangSX()
        {
            if (Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            if (TempData["TB"] != null)
            {
                ViewBag.thongBao = TempData["TB"];
                
                var b = from t1 in c.HANGSANXUATs
                        select t1;
                return View(b);
            }
            var a = from t1 in c.HANGSANXUATs
                    select t1;
            return View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult trangQLHangSX(string ma, string ten)
        {
            ma.Trim();

            bool allIsTrue = true;
            if (String.IsNullOrEmpty(ten))
            {
                ViewData["loiten"+ma] = "Phải nhập tên hãng sản xuất";
                allIsTrue = false;
            }
            else if (ten.Length < 5)
            {
                ViewData["loiten"+ma] = "Phải nhập tên hãng sản xuất có ít nhất 5 kí tự";
                allIsTrue = false;
            }
            else if (ten.Length > 50)
            {
                ViewData["loiten"+ma] = "Phải nhập tên hãng sản xuất có ít hơn 50 kí tự";
                allIsTrue = false;
            }
            if(allIsTrue)
            {
                HANGSANXUAT hsx = c.HANGSANXUATs.SingleOrDefault(x => x.MaHangSX == ma);
                hsx.TenHangSX = ten.Trim();
                try
                {
                    c.SubmitChanges();
                    ViewData.Clear();
                    ViewBag.thongBao = "Cập nhật thành công";
                }
                catch
                {
                    ViewData.Clear();
                    ViewBag.thongBao = "Cập nhật thất bại";
                }
            }
            return trangQLHangSX();
        }

        public ActionResult trangQLHangSX_xoa(string id)
        {
            id.Trim();
            HANGSANXUAT hsx = c.HANGSANXUATs.SingleOrDefault(x => x.MaHangSX == id);
            try
            {
                c.HANGSANXUATs.DeleteOnSubmit(hsx);
                c.SubmitChanges();
                TempData["TB"] = "Xóa thành công";
            }
            catch
            {
                TempData["TB"] = "Không xóa được";
            }
            return RedirectToAction("trangQLHangSX", "Admin");
        }

        public ActionResult trangQLHangSX_themmoi()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("trangChu", "Guest");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult trangQLHangSX_themmoi(string ma, string ten)
        {
            bool allIsTrue = true;

            ma.Trim();
            ViewData["ma"] = ma;
            ViewData["ten"] = ten;
            if (String.IsNullOrEmpty(ma))
            {
                ViewData["loima"] = "Phải nhập mã hãng sản xuất";
                allIsTrue = false;
            }
            else if (ma.Length != 5)
            {
                ViewData["loima"] = "Mã hãng sản xuất phải có 5 kí tự";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(ten))
            {
                ViewData["loiten"] = "Phải nhập tên hãng sản xuất";
                allIsTrue = false;
            }
            else if (ten.Length < 5)
            {
                ViewData["loiten"] = "Phải nhập tên hãng sản xuất có ít nhất 5 kí tự";
                allIsTrue = false;
            }
            else if (ten.Length > 50)
            {
                ViewData["loiten"] = "Phải nhập tên hãng sản xuất có ít hơn 50 kí tự";
                allIsTrue = false;
            }

            var kt = c.HANGSANXUATs.Where(x => x.MaHangSX == ma);
            if (kt.Any())
            {
                HANGSANXUAT kthsx = (HANGSANXUAT)kt;
                ViewData["loima"] = "Mã hãng sản xuất " + kthsx.MaHangSX +
                    " đã được nhập cho hãng sản xuất " + kthsx.TenHangSX;
                allIsTrue = false;
            }

            if (allIsTrue)
            {
                HANGSANXUAT hsx = new HANGSANXUAT();
                hsx.MaHangSX = ma;
                hsx.TenHangSX = ten;
                try
                {
                    c.HANGSANXUATs.InsertOnSubmit(hsx);
                    c.SubmitChanges();
                    ViewData.Clear();
                    TempData["TB"] = "Thêm thành công";
                    return RedirectToAction("trangQLHangSX", "Admin");
                }
                catch
                {
                    ViewData["loima"] = "Mã hãng sản xuất không được sử dụng các ký tự đặc biệt!";
                    ViewBag.thongBao = "Thêm thất bại";
                }
            }
            return View();
        }

    }
}