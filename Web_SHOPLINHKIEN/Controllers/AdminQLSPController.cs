using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Web_SHOPLINHKIEN.Models;

using PagedList;
using PagedList.Mvc;


namespace Web_SHOPLINHKIEN.Controllers
{
    public class AdminQLSPController : Controller
    {
        SHOPLINHKIENDataClassDataContext c = new SHOPLINHKIENDataClassDataContext();
        // GET: AdminQLSP
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult trangQLSP(int ? page)
        {
            if (Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            if (TempData["TB"] != null)
                ViewBag.thongBao = TempData["TB"];
            var b = (from t1 in c.SANPHAMs
                     join t2 in c.HANGSANXUATs on t1.MaHangSX equals t2.MaHangSX
                     join t3 in c.LOAISANPHAMs on t1.MaLoaiSP equals t3.MaLoaiSP
                     join t4 in c.ANHSANPHAMs on t1.MaSP equals t4.MaSP
                     where t1.SoLuong > 0
                     orderby t1.MaSP descending
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
            int pageSize = 6;
            int pageNum = (page ?? 1);
            return View(links.ToPagedList(pageNum, pageSize));
        }

        public ActionResult trangQLSP_xem(string id)
        {
            if (id == null || Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            var b = (from t1 in c.SANPHAMs
                     join t2 in c.HANGSANXUATs on t1.MaHangSX equals t2.MaHangSX
                     join t3 in c.LOAISANPHAMs on t1.MaLoaiSP equals t3.MaLoaiSP
                     join t4 in c.ANHSANPHAMs on t1.MaSP equals t4.MaSP
                     where t1.MaSP == id
                     select new joinTable_SANPHAM
                     {
                         SANPHAM = t1,
                         HANGSANXUAT = t2,
                         LOAISANPHAM = t3,
                         ANHSANPHAM = t4,
                     }).FirstOrDefault();
            return View(b);
        }

        public ActionResult trangQLSP_xoa(string id)
        {
            if (id == null || Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            SANPHAM sp = c.SANPHAMs.SingleOrDefault(x => x.MaSP == id);
            try
            {
                c.SANPHAMs.DeleteOnSubmit(sp);
                c.SubmitChanges();
                TempData["TB"] = "Xóa sản phẩm thành công";
            }
            catch
            {
                TempData["TB"] = "Xóa sản phẩm thất bại, do sản phẩm có trong vài đơn hàng";
            }
            return RedirectToAction("trangQLSP", "AdminQLSP");
        }

        public ActionResult trangQLSP_themmoi()
        {
            if (Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult trangQLSP_themmoi(string tensp, string gia, string sl, string loaisp, 
            string hangsx, string masku, string ngayph, string ndngan, HttpPostedFileBase anh1, 
            HttpPostedFileBase anh2, HttpPostedFileBase anh3, HttpPostedFileBase anh4)
        {
            bool allIsTrue = true;

            ViewData["tensp"] = ViewData["gia"] = ViewData["sl"] = ViewData["loaisp"] =
            ViewData["hangsx"] = ViewData["masku"] = ViewData["ngayph"] = ViewData["ndngan"] = null;

            ViewData["tensp"] = tensp;
            ViewData["gia"] = gia;
            ViewData["sl"] = sl;
            ViewData["loaisp"] = loaisp;
            ViewData["hangsx"] = hangsx;
            ViewData["masku"] = masku;
            ViewData["ndngan"] = ndngan;
            ViewData["ngayph"] = ngayph;

            if (String.IsNullOrEmpty(tensp))
            {
                ViewData["loitensp"] = "Phải nhập tên sản phẩm";
                allIsTrue = false;
            }
            else if (tensp.Length < 10)
            {
                ViewData["loitensp"] = "Phải nhập tên sản phẩm trên 10 ký tự";
                allIsTrue = false;
            }
            else if (tensp.Length > 500)
            {
                ViewData["loitensp"] = "Phải nhập tên sản phẩm không quá 500 ký tự";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(gia))
            {
                ViewData["loigia"] = "Phải nhập đơn giá sản phẩm";
                allIsTrue = false;
            }
            else if (gia.Length < 5)
            {
                ViewData["loigia"] = "Phải nhập đơn giá sản phẩm từ 5 ký tự trở lên";
                allIsTrue = false;
            }
            else if (gia.Length > 18)
            {
                ViewData["loigia"] = "Phải nhập đơn giá sản phẩm không quá 18 ký tự";
                allIsTrue = false;
            }
            else if (!gia.EndsWith("000"))
            {
                ViewData["loigia"] = "Phải nhập đơn giá sản phẩm kết thúc bằng \"000\"";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(sl))
            {
                ViewData["loisl"] = "Phải nhập số lượng sản phẩm";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(loaisp))
            {
                ViewData["loiloaisp"] = "Phải chọn loại sản phẩm";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(hangsx))
            {
                ViewData["loihangsx"] = "Phải chọn hãng sản xuất";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(masku))
            {
                ViewData["loimasku"] = "Phải nhập mã SKU của sản phẩm";
                allIsTrue = false;
            }
            else if (masku.Length < 8)
            {
                ViewData["loimasku"] = "Phải nhập mã SKU của sản phẩm từ 8 ký tự trở lên";
                allIsTrue = false;
            }
            else if (masku.Length > 50)
            {
                ViewData["loimasku"] = "Phải nhập mã SKU của sản phẩm không quá 50 ký tự";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(ndngan))
            {
                ViewData["loindngan"] = "Phải nhập nội dung ngắn của sản phẩm";
                allIsTrue = false;
            }
            else if (ndngan.Length < 10)
            {
                ViewData["loindngan"] = "Phải nhập nội dung ngắn của sản phẩm từ 10 ký tự trở lên";
                allIsTrue = false;
            }
            if (anh1 != null)
            {
                if (!(anh1.FileName.EndsWith(".png") || anh1.FileName.EndsWith(".jpg") || anh1.FileName.EndsWith(".jpeg")))
                {
                    ViewData["loianh1"] = "Phải chọn tập tin ảnh có định dạng .png, .jpg, .jpeg";
                    allIsTrue = false;
                }
                else if (anh1.ContentLength > 4194304)
                {
                    ViewData["loianh1"] = "Phải chọn tập tin ảnh có dung lượng không quá 4MB";
                    allIsTrue = false;
                }    
            }
            else
            {
                ViewData["loianh1"] = "Phải chọn 1 tập tin ảnh có định dạng .png, .jpg, .jpeg, không được bỏ trống";
                allIsTrue = false;
            }

            if (anh2 != null)
            {
                if (!(anh2.FileName.EndsWith(".png") || anh2.FileName.EndsWith(".jpg") || anh2.FileName.EndsWith(".jpeg")))
                {
                    ViewData["loianh2"] = "Phải chọn tập tin ảnh có định dạng .png, .jpg, .jpeg";
                    allIsTrue = false;
                }
                else if (anh2.ContentLength > 4194304)
                {
                    ViewData["loianh2"] = "Phải chọn tập tin ảnh có dung lượng không quá 4MB";
                    allIsTrue = false;
                }
            }
            else
            {
                ViewData["loianh2"] = "Phải chọn 1 tập tin ảnh có định dạng .png, .jpg, .jpeg, không được bỏ trống";
                allIsTrue = false;
            }

            if (anh3 != null)
            {
                if (!(anh3.FileName.EndsWith(".png") || anh3.FileName.EndsWith(".jpg") || anh3.FileName.EndsWith(".jpeg")))
                {
                    ViewData["loianh3"] = "Phải chọn tập tin ảnh có định dạng .png, .jpg, .jpeg";
                    allIsTrue = false;
                }
                else if (anh3.ContentLength > 4194304)
                {
                    ViewData["loianh3"] = "Phải chọn tập tin ảnh có dung lượng không quá 4MB";
                    allIsTrue = false;
                }
            }
            else
            {
                ViewData["loianh3"] = "Phải chọn 1 tập tin ảnh có định dạng .png, .jpg, .jpeg, không được bỏ trống";
                allIsTrue = false;
            }

            if (anh4 != null)
            {
                if (!(anh4.FileName.EndsWith(".png") || anh4.FileName.EndsWith(".jpg") || anh4.FileName.EndsWith(".jpeg")))
                {
                    ViewData["loianh4"] = "Phải chọn tập tin ảnh có định dạng .png, .jpg, .jpeg";
                    allIsTrue = false;
                }
                else if (anh4.ContentLength > 4194304)
                {
                    ViewData["loianh4"] = "Phải chọn tập tin ảnh có dung lượng không quá 4MB";
                    allIsTrue = false;
                }
            }
            else
            {
                ViewData["loianh4"] = "Phải chọn 1 tập tin ảnh có định dạng .png, .jpg, .jpeg, không được bỏ trống";
                allIsTrue = false;
            }

            if (String.IsNullOrEmpty(ngayph))
            {
                ViewData["loingayph"] = "Phải nhập ngày phát hành sản phẩm";
                allIsTrue = false;
            }

            DateTime dt = new DateTime();
            try
            {
                dt = Convert.ToDateTime(ngayph);
                if (DateTime.Compare(dt, DateTime.Today) >= 0)
                {
                    ViewData["loingayph"] = "Ngày phát hành phải là ngày quá khứ";
                    allIsTrue = false;
                }
            }
            catch
            {
                ViewData["loingayph"] = "Nhập ngày phát hành không hợp lệ";
                allIsTrue = false;
            }

            Decimal gt = new Decimal();
            try
            {
                gt = Convert.ToDecimal(gia);
            }
            catch
            {
                ViewData["loigia"] = "Nhập đơn giá không hợp lệ";
                allIsTrue = false;
            }

            int soluong = new int();
            try
            {
                soluong = Convert.ToInt32(sl);
            }
            catch
            {
                ViewData["loisl"] = "Nhập số lượng không hợp lệ";
                allIsTrue = false;
            }

            if (allIsTrue)
            {
                SANPHAM sp = new SANPHAM();
                sp.MaSP = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "SPH";
                sp.TenSP = tensp;
                sp.GiaTien = gt;
                sp.SoLuong = soluong;
                sp.SoLuongDaBan = 0;
                sp.MaLoaiSP = loaisp;
                sp.MaHangSX = hangsx;
                sp.MaSKU = masku;
                sp.NoiDungNgan = ndngan;
                sp.NgayPhatHanh = dt;
                try
                {
                    c.SANPHAMs.InsertOnSubmit(sp);
                    c.SubmitChanges();

                    if (anh1 != null && !String.IsNullOrEmpty(anh1.FileName))
                    {
                        ANHSANPHAM anhsp = new ANHSANPHAM();
                        string fileAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN1" + Path.GetExtension(anh1.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/ANHSANPHAM/") + fileAnh);
                        anh1.SaveAs(ServerSavePath);
                        anhsp.FileAnh = fileAnh;
                        anhsp.MaAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN1";
                        anhsp.MaSP = sp.MaSP;
                        c.ANHSANPHAMs.InsertOnSubmit(anhsp);
                        c.SubmitChanges();
                    }

                    if (anh2 != null && !String.IsNullOrEmpty(anh2.FileName))
                    {
                        ANHSANPHAM anhsp = new ANHSANPHAM();
                        string fileAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN2" + Path.GetExtension(anh2.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/ANHSANPHAM/") + fileAnh);
                        anh2.SaveAs(ServerSavePath);
                        anhsp.FileAnh = fileAnh;
                        anhsp.MaAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN2";
                        anhsp.MaSP = sp.MaSP;
                        c.ANHSANPHAMs.InsertOnSubmit(anhsp);
                        c.SubmitChanges();
                    }

                    if (anh3 != null && !String.IsNullOrEmpty(anh3.FileName))
                    {
                        ANHSANPHAM anhsp = new ANHSANPHAM();
                        string fileAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN3" + Path.GetExtension(anh1.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/ANHSANPHAM/") + fileAnh);
                        anh3.SaveAs(ServerSavePath);
                        anhsp.FileAnh = fileAnh;
                        anhsp.MaAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN3";
                        anhsp.MaSP = sp.MaSP;
                        c.ANHSANPHAMs.InsertOnSubmit(anhsp);
                        c.SubmitChanges();
                    }

                    if (anh4 != null && !String.IsNullOrEmpty(anh4.FileName))
                    {
                        ANHSANPHAM anhsp = new ANHSANPHAM();
                        string fileAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN4" + Path.GetExtension(anh2.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/ANHSANPHAM/") + fileAnh);
                        anh4.SaveAs(ServerSavePath);
                        anhsp.FileAnh = fileAnh;
                        anhsp.MaAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN4";
                        anhsp.MaSP = sp.MaSP;
                        c.ANHSANPHAMs.InsertOnSubmit(anhsp);
                        c.SubmitChanges();
                    }
                    ViewData.Clear();
                    return RedirectToAction("trangQLSP_xem", "AdminQLSP", new { id = sp.MaSP.Trim() });
                }
                catch
                {
                    c.SANPHAMs.DeleteOnSubmit(sp);
                    ViewBag.thongBao = "Thêm mới sản phẩm thất bại";
                    return View();
                }

            }
            return View();
        }

        public ActionResult trangQLSP_comboboxLoaiSP(string idLoaiSP)
        {
            List<LOAISANPHAM> lsp = c.LOAISANPHAMs.ToList();
            ViewBag.loaiSP = idLoaiSP;
            return View(lsp);
        }

        public ActionResult trangQLSP_comboboxHangSX(string idHangSX)
        {
            List<HANGSANXUAT> hsx = c.HANGSANXUATs.ToList();
            ViewBag.hangSX = idHangSX;
            return View(hsx);
        }

        public ActionResult trangQLSP_sua(string id)
        {
            if (Session["admin"] == null || String.IsNullOrEmpty(id))
                return RedirectToAction("trangChu", "Guest");
            var b = (from t1 in c.SANPHAMs
                     join t2 in c.HANGSANXUATs on t1.MaHangSX equals t2.MaHangSX
                     join t3 in c.LOAISANPHAMs on t1.MaLoaiSP equals t3.MaLoaiSP
                     join t4 in c.ANHSANPHAMs on t1.MaSP equals t4.MaSP
                     where t1.MaSP == id
                     select new joinTable_SANPHAM
                     {
                         SANPHAM = t1,
                         HANGSANXUAT = t2,
                         LOAISANPHAM = t3,
                         ANHSANPHAM = t4,
                     });
            return View(b);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult trangQLSP_sua(string id, string tensp, string gia, string sl, string loaisp,
            string hangsx, string masku, string ngayph, string ndngan, HttpPostedFileBase anh1,
            HttpPostedFileBase anh2, HttpPostedFileBase anh3, HttpPostedFileBase anh4)
        {
            bool allIsTrue = true;

            if (String.IsNullOrEmpty(tensp))
            {
                ViewData["loitensp"] = "Phải nhập tên sản phẩm";
                allIsTrue = false;
            }
            else if (tensp.Length < 10)
            {
                ViewData["loitensp"] = "Phải nhập tên sản phẩm trên 10 ký tự";
                allIsTrue = false;
            }
            else if (tensp.Length > 500)
            {
                ViewData["loitensp"] = "Phải nhập tên sản phẩm dưới 500 ký tự";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(gia))
            {
                ViewData["loigia"] = "Phải nhập đơn giá sản phẩm";
                allIsTrue = false;
            }
            else if (gia.Length < 6)
            {
                ViewData["loigia"] = "Phải nhập đơn giá sản phẩm trên 5 ký tự";
                allIsTrue = false;
            }
            else if (gia.Length > 18)
            {
                ViewData["loigia"] = "Phải nhập đơn giá sản phẩm dưới 18 ký tự";
                allIsTrue = false;
            }
            else if (!gia.EndsWith("000"))
            {
                ViewData["loigia"] = "Phải nhập đơn giá sản phẩm kết thúc bằng \"000\"";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(sl))
            {
                ViewData["loisl"] = "Phải nhập số lượng sản phẩm";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(loaisp))
            {
                ViewData["loiloaisp"] = "Phải chọn loại sản phẩm";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(hangsx))
            {
                ViewData["loihangsx"] = "Phải chọn hãng sản xuất";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(masku))
            {
                ViewData["loimasku"] = "Phải nhập mã SKU của sản phẩm";
                allIsTrue = false;
            }
            else if (masku.Length <= 8)
            {
                ViewData["loimasku"] = "Phải nhập mã SKU của sản phẩm trên 8 ký tự";
                allIsTrue = false;
            }
            else if (masku.Length > 50)
            {
                ViewData["loimasku"] = "Phải nhập mã SKU của sản phẩm dưới 50 ký tự";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(ndngan))
            {
                ViewData["loindngan"] = "Phải nhập nội dung ngắn của sản phẩm";
                allIsTrue = false;
            }
            else if (ndngan.Length < 10)
            {
                ViewData["loindngan"] = "Phải nhập nội dung ngắn của sản phẩm trên 10 ký tự";
                allIsTrue = false;
            }
            if (anh1 != null)
            {
                if (!(anh1.FileName.EndsWith(".png") || anh1.FileName.EndsWith(".jpg") || anh1.FileName.EndsWith(".jpeg")))
                {
                    ViewData["loianh1"] = "Phải chọn tập tin ảnh có định dạng .png, .jpg, .jpeg";
                    allIsTrue = false;
                }
            }
            if (anh2 != null)
            {
                if (!(anh2.FileName.EndsWith(".png") || anh2.FileName.EndsWith(".jpg") || anh2.FileName.EndsWith(".jpeg")))
                {
                    ViewData["loianh2"] = "Phải chọn tập tin ảnh có định dạng .png, .jpg, .jpeg";
                    allIsTrue = false;
                }
            }
            if (anh3 != null)
            {
                if (!(anh3.FileName.EndsWith(".png") || anh3.FileName.EndsWith(".jpg") || anh3.FileName.EndsWith(".jpeg")))
                {
                    ViewData["loianh3"] = "Phải chọn tập tin ảnh có định dạng .png, .jpg, .jpeg";
                    allIsTrue = false;
                }
            }
            if (anh4 != null)
            {
                if (!(anh4.FileName.EndsWith(".png") || anh4.FileName.EndsWith(".jpg") || anh4.FileName.EndsWith(".jpeg")))
                {
                    ViewData["loianh4"] = "Phải chọn tập tin ảnh có định dạng .png, .jpg, .jpeg";
                    allIsTrue = false;
                }
            }
            if (String.IsNullOrEmpty(ngayph))
            {
                ViewData["loingayph"] = "Phải nhập ngày phát hành sản phẩm";
                allIsTrue = false;
            }

            DateTime dt = new DateTime();
            try
            {
                dt = Convert.ToDateTime(ngayph);
                if (DateTime.Compare(dt, DateTime.Today) >= 0)
                {
                    ViewData["loingayph"] = "Ngày phát hành phải là ngày quá khứ";
                    allIsTrue = false;
                }
            }
            catch
            {
                ViewData["loingayph"] = "Nhập ngày phát hành không hợp lệ";
                allIsTrue = false;
            }

            Decimal gt = new Decimal();
            try
            {
                gt = Convert.ToDecimal(gia);
            }
            catch
            {
                ViewData["loigia"] = "Nhập đơn giá không hợp lệ";
                allIsTrue = false;
            }

            int soluong = new int();
            try
            {
                soluong = Convert.ToInt32(sl);
            }
            catch
            {
                ViewData["loisl"] = "Nhập số lượng không hợp lệ";
                allIsTrue = false;
            }

            if (allIsTrue)
            {
                SANPHAM sp = c.SANPHAMs.SingleOrDefault(x => x.MaSP == id);
                sp.TenSP = tensp;
                sp.GiaTien = gt;
                sp.SoLuong = soluong;
                sp.SoLuongDaBan = 0;
                sp.MaLoaiSP = loaisp;
                sp.MaHangSX = hangsx;
                sp.MaSKU = masku;
                sp.NoiDungNgan = ndngan;
                sp.NgayPhatHanh = dt;

                try
                {
                    c.SubmitChanges();
                    List<ANHSANPHAM> dsanhsp = c.ANHSANPHAMs.Where(x => x.MaSP == id).ToList();
                    if (anh1 != null && !String.IsNullOrEmpty(anh1.FileName))
                    {
                        ANHSANPHAM anhsp = dsanhsp.SingleOrDefault(x => x.MaAnh.Contains("AN1"));
                        string fileAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN1" + Path.GetExtension(anh1.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/ANHSANPHAM/") + fileAnh);
                        anh1.SaveAs(ServerSavePath);
                        anhsp.FileAnh = fileAnh;
                        c.SubmitChanges();
                    }

                    if (anh2 != null && !String.IsNullOrEmpty(anh2.FileName))
                    {
                        if(dsanhsp.Count >= 2)
                        {
                            ANHSANPHAM anhsp = dsanhsp.SingleOrDefault(x=>x.MaAnh.Contains("AN2"));
                            string fileAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN2" + Path.GetExtension(anh2.FileName);
                            var ServerSavePath = Path.Combine(Server.MapPath("~/ANHSANPHAM/") + fileAnh);
                            anh2.SaveAs(ServerSavePath);
                            anhsp.FileAnh = fileAnh;
                            c.SubmitChanges();
                        }
                        else
                        {
                            ANHSANPHAM anhsp = new ANHSANPHAM();
                            string fileAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN2" + Path.GetExtension(anh2.FileName);
                            var ServerSavePath = Path.Combine(Server.MapPath("~/ANHSANPHAM/") + fileAnh);
                            anh2.SaveAs(ServerSavePath);
                            anhsp.FileAnh = fileAnh;
                            anhsp.MaAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN2";
                            anhsp.MaSP = sp.MaSP;
                            c.ANHSANPHAMs.InsertOnSubmit(anhsp);
                            c.SubmitChanges();
                        }
                        
                    }

                    if (anh3 != null && !String.IsNullOrEmpty(anh3.FileName))
                    {
                        if (dsanhsp.Count >= 3)
                        {
                            ANHSANPHAM anhsp = dsanhsp.SingleOrDefault(x => x.MaAnh.Contains("AN3"));
                            string fileAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN3" + Path.GetExtension(anh3.FileName);
                            var ServerSavePath = Path.Combine(Server.MapPath("~/ANHSANPHAM/") + fileAnh);
                            anh3.SaveAs(ServerSavePath);
                            anhsp.FileAnh = fileAnh;
                            c.SubmitChanges();
                        }
                        else
                        {
                            ANHSANPHAM anhsp = new ANHSANPHAM();
                            string fileAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN3" + Path.GetExtension(anh3.FileName);
                            var ServerSavePath = Path.Combine(Server.MapPath("~/ANHSANPHAM/") + fileAnh);
                            anh3.SaveAs(ServerSavePath);
                            anhsp.FileAnh = fileAnh;
                            anhsp.MaAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN3";
                            anhsp.MaSP = sp.MaSP;
                            c.ANHSANPHAMs.InsertOnSubmit(anhsp);
                            c.SubmitChanges();
                        }
                    }
                        if (anh4 != null && !String.IsNullOrEmpty(anh4.FileName))
                        {
                            if (dsanhsp.Count >= 4)
                            {
                                ANHSANPHAM anhsp = dsanhsp.SingleOrDefault(x => x.MaAnh.Contains("AN4"));
                                string fileAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN4" + Path.GetExtension(anh4.FileName);
                                var ServerSavePath = Path.Combine(Server.MapPath("~/ANHSANPHAM/") + fileAnh);
                                anh4.SaveAs(ServerSavePath);
                                anhsp.FileAnh = fileAnh;
                                c.SubmitChanges();
                            }
                            else
                            {
                                ANHSANPHAM anhsp = new ANHSANPHAM();
                                string fileAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN4" + Path.GetExtension(anh4.FileName);
                                var ServerSavePath = Path.Combine(Server.MapPath("~/ANHSANPHAM/") + fileAnh);
                                anh4.SaveAs(ServerSavePath);
                                anhsp.FileAnh = fileAnh;
                                anhsp.MaAnh = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "AN4";
                                anhsp.MaSP = sp.MaSP;
                                c.ANHSANPHAMs.InsertOnSubmit(anhsp);
                                c.SubmitChanges();
                            }

                        }
                    ViewBag.thongBao = "Cập nhật sản phẩm thành công";

                    var e = (from t1 in c.SANPHAMs
                             join t2 in c.HANGSANXUATs on t1.MaHangSX equals t2.MaHangSX
                             join t3 in c.LOAISANPHAMs on t1.MaLoaiSP equals t3.MaLoaiSP
                             join t4 in c.ANHSANPHAMs on t1.MaSP equals t4.MaSP
                             where t1.MaSP == id
                             select new joinTable_SANPHAM
                             {
                                 SANPHAM = t1,
                                 HANGSANXUAT = t2,
                                 LOAISANPHAM = t3,
                                 ANHSANPHAM = t4,
                             });
                    return View(e);
                }
                catch
                {
                    ViewBag.thongBao = "Cập nhật sản phẩm thất bại";
                    var e = (from t1 in c.SANPHAMs
                             join t2 in c.HANGSANXUATs on t1.MaHangSX equals t2.MaHangSX
                             join t3 in c.LOAISANPHAMs on t1.MaLoaiSP equals t3.MaLoaiSP
                             join t4 in c.ANHSANPHAMs on t1.MaSP equals t4.MaSP
                             where t1.MaSP == id
                             select new joinTable_SANPHAM
                             {
                                 SANPHAM = t1,
                                 HANGSANXUAT = t2,
                                 LOAISANPHAM = t3,
                                 ANHSANPHAM = t4,
                             });
                    return View(e);
                }

            }
            var b = (from t1 in c.SANPHAMs
                     join t2 in c.HANGSANXUATs on t1.MaHangSX equals t2.MaHangSX
                     join t3 in c.LOAISANPHAMs on t1.MaLoaiSP equals t3.MaLoaiSP
                     join t4 in c.ANHSANPHAMs on t1.MaSP equals t4.MaSP
                     where t1.MaSP == id
                     select new joinTable_SANPHAM
                     {
                         SANPHAM = t1,
                         HANGSANXUAT = t2,
                         LOAISANPHAM = t3,
                         ANHSANPHAM = t4,
                     });
            return View(b);
        }


    }
}