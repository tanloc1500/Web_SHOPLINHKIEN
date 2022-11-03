using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_SHOPLINHKIEN.Models;

namespace Web_SHOPLINHKIEN.Controllers
{
    public class AdminQLTaiKhoanController : Controller
    {
        SHOPLINHKIENDataClassDataContext c = new SHOPLINHKIENDataClassDataContext();
        // GET: AdminQLTaiKhoan
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult trangAdminQLTaiKhoan()
        {
            if (Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            if (TempData["TB"] != null)
                ViewBag.thongBao = TempData["TB"];
            var a = from t1 in c.KHACHHANGs
                    join t2 in c.TAIKHOANs on t1.MaTK equals t2.MaTK
                    orderby t1.MaTK descending
                    select new joinTable_KHG_TAIKHOAN
                    {
                        KHACHHANG = t1,
                        TAIKHOAN = t2,
                    };
            return View(a);
        }

        public ActionResult trangQLTaiKhoan_themmoi()
        {
            //Nam:0, Nu:1
            if (Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult trangQLTaiKhoan_themmoi(string tenKH, string gioitinh, string email, string sdt,
            string username, string pass, string repass, string ngaysinh, string diachi)
        {
            bool allIsTrue = true;

            DateTime dt = new DateTime();

            ViewData["tenKH"] = ViewData["gioitinh"] = ViewData["email"] = ViewData["sdt"] =
            ViewData["username"] = ViewData["ngaysinh"] = ViewData["diachi"] = null;

            ViewData["tenKH"] = tenKH;
            ViewData["gioitinh"] = gioitinh;
            ViewData["email"] = email;
            ViewData["sdt"] = sdt;
            ViewData["username"] = username;
            ViewData["ngaysinh"] = ngaysinh;
            ViewData["diachi"] = diachi;
            if (String.IsNullOrEmpty(tenKH))
            {
                ViewData["loitenKH"] = "Phải nhập họ tên";
                allIsTrue = false;
            }
            else if (tenKH.Length < 5)
            {
                ViewData["loitenKH"] = "Phải nhập họ tên trên 5 kí tự";
                allIsTrue = false;
            }
            else if (tenKH.Length > 50)
            {
                ViewData["loitenKH"] = "Phải nhập họ tên ít hơn 50 kí tự";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(gioitinh))
            {
                ViewData["loigioitinh"] = "Phải chọn giới tính";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(email))
            {
                ViewData["loiemail"] = "Phải nhập email";
                allIsTrue = false;
            }
            else if (email.Length < 10)
            {
                ViewData["loiemail"] = "Phải nhập email trên 10 kí tự";
                allIsTrue = false;
            }
            else if (email.Length > 50)
            {
                ViewData["loiemail"] = "Phải nhập email ít hơn 50 kí tự";
                allIsTrue = false;
            }
            else if (email.Contains(" "))
            {
                ViewData["loiemail"] = "Phải nhập email không có khoảng trắng";
                allIsTrue = false;
            }
            else
            {
                var checkemail = c.KHACHHANGs.Where(x => x.Email == email);
                if (checkemail.Any())
                {
                    ViewData["loiemail"] = "Email đã có người đăng kí, vui lòng nhập email khác";
                    allIsTrue = false;
                }
            }

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
            else
            {
                var checksdt = c.KHACHHANGs.Where(x => x.SDT == sdt);
                if (checksdt.Any())
                {
                    ViewData["loisdt"] = "Số điện thoại đã có người đăng ký, vui lòng nhập số điện thoại khác";
                    allIsTrue = false;
                }
            }

            if (String.IsNullOrEmpty(username))
            {
                ViewData["loiusername"] = "Phải nhập tên tài khoản";
                allIsTrue = false;
            }
            else if (username.Length < 5)
            {
                ViewData["loiusername"] = "Phải nhập tên tài khoản trên 5 kí tự";
                allIsTrue = false;
            }
            else if (username.Length > 20)
            {
                ViewData["loiusername"] = "Phải nhập tên tài khoản ít hơn 20 kí tự";
                allIsTrue = false;
            }
            else
            {
                var tkl = c.TAIKHOANs.Where(x => x.ID == username.Trim());
                if (tkl.Any())
                {
                    ViewData["loiusername"] = "Tên tài khoản đã có người đăng kí, mời chọn tên tài khoản khác";
                    allIsTrue = false;
                }

            }

            if (String.IsNullOrEmpty(pass))
            {
                ViewData["loipass"] = "Phải nhập mật khẩu";
                allIsTrue = false;
            }
            else if (pass.Length < 8)
            {
                ViewData["loipass"] = "Phải nhập mật khẩu trên 8 ký tự";
                allIsTrue = false;
            }
            else if (pass.Length > 20)
            {
                ViewData["loipass"] = "Phải nhập mật khẩu không quá 20 ký tự";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(repass))
            {
                ViewData["loirepass"] = "Phải nhập lại mật khẩu";
                allIsTrue = false;
            }
            else if (repass != pass)
            {
                ViewData["loirepass"] = "Phải nhập lại mật khẩu giống mật khẩu";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(ngaysinh))
            {
                ViewData["loingaysinh"] = "Phải nhập ngày sinh";
                allIsTrue = false;
            }
            else
            {
                try
                {
                    dt = Convert.ToDateTime(ngaysinh);
                    if ((DateTime.Now - dt).TotalDays < 6575 && (DateTime.Now - dt).TotalDays >= 0)
                    {
                        ViewData["loingaysinh"] = "Phải nhập ngày sinh trên 18 tuổi";
                        ViewData["ngaysinh"] = dt.ToString("yyyy-MM-dd");
                        allIsTrue = false;
                    }
                    else if ((DateTime.Now - dt).TotalDays < 6575 && (DateTime.Now - dt).TotalDays < 0)
                    {
                        ViewData["loingaysinh"] = "Phải nhập ngày sinh là ngày quá khứ";
                        ViewData["ngaysinh"] = dt.ToString("yyyy-MM-dd");
                        allIsTrue = false;
                    }
                }
                catch
                {
                    ViewData["loingaysinh"] = "Nhập ngày sinh không hợp lệ";
                    allIsTrue = false;
                }
            }

            if (String.IsNullOrEmpty(diachi))
            {
                ViewData["loidiachi"] = "Phải nhập địa chỉ";
                allIsTrue = false;
            }
            else if (diachi.Length > 200)
            {
                ViewData["loidiachi"] = "Phải nhập địa chỉ không quá 200 ký tự";
                allIsTrue = false;
            }
            else if (diachi.Length < 20)
            {
                ViewData["loidiachi"] = "Phải nhập địa chỉ không ít hơn 20 ký tự";
                allIsTrue = false;
            }

            if (allIsTrue)
            {
                TAIKHOAN tk = new TAIKHOAN();
                tk.ID = username.Trim();
                tk.MaTK = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "USR";
                tk.MatKhau = pass;
                tk.PhanQuyen = "USR";
                try
                {
                    c.TAIKHOANs.InsertOnSubmit(tk);
                    c.SubmitChanges();
                }
                catch
                {
                    ViewBag.thongBao = "Thêm mới tài khoản thất bại";
                    return View();
                }
                KHACHHANG kh = new KHACHHANG();
                bool gt = new bool();
                if (gioitinh == "0")
                {
                    gt = false;
                }
                else if (gioitinh == "1")
                {
                    gt = true;
                }
                kh.MaKH = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "KHG";
                kh.HoTen = tenKH;
                kh.GioiTinh = gt;
                kh.Email = email;
                kh.SDT = sdt;
                kh.NgaySinh = dt;
                kh.DiaChi = diachi;
                kh.MaTK = tk.MaTK;
                try
                {
                    c.KHACHHANGs.InsertOnSubmit(kh);
                    c.SubmitChanges();
                    ViewData.Clear();
                    TempData["TB"] = "Thêm mới tài khoản thành công";
                    return RedirectToAction("trangAdminQLTaiKhoan", "AdminQLTaiKhoan");
                }
                catch
                {
                    c.TAIKHOANs.DeleteOnSubmit(tk);
                    ViewBag.thongBao = "Thêm mới tài khoản thất bại";
                }
            }
            return View();
        }

        public ActionResult trangQLTaiKhoan_sua(string id)
        {
            if (Session["admin"] == null || String.IsNullOrEmpty(id))
                return RedirectToAction("trangChu", "Guest");
            var a = (from t1 in c.KHACHHANGs
                     join t2 in c.TAIKHOANs on t1.MaTK equals t2.MaTK
                     where t1.MaTK == id
                     select new joinTable_KHG_TAIKHOAN
                     {
                         KHACHHANG = t1,
                         TAIKHOAN = t2
                     }).SingleOrDefault();
            return View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult trangQLTaiKhoan_sua(string id, string tenKH, string gioitinh, string email, string sdt,
            string username, string ngaysinh, string diachi)
        {
            bool allIsTrue = true;

            DateTime dt = new DateTime();

            if (String.IsNullOrEmpty(tenKH))
            {
                ViewData["loitenKH"] = "Phải nhập họ tên";
                allIsTrue = false;
            }
            else if (tenKH.Length < 5)
            {
                ViewData["loitenKH"] = "Phải nhập họ tên trên 5 kí tự";
                allIsTrue = false;
            }
            else if (tenKH.Length > 50)
            {
                ViewData["loitenKH"] = "Phải nhập họ tên ít hơn 50 kí tự";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(gioitinh))
            {
                ViewData["loigioitinh"] = "Phải chọn giới tính";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(email))
            {
                ViewData["loiemail"] = "Phải nhập email";
                allIsTrue = false;
            }
            else if (email.Length < 10)
            {
                ViewData["loiemail"] = "Phải nhập email trên 10 kí tự";
                allIsTrue = false;
            }
            else if (email.Length > 50)
            {
                ViewData["loiemail"] = "Phải nhập email ít hơn 50 kí tự";
                allIsTrue = false;
            }
            else if (email.Contains(" "))
            {
                ViewData["loiemail"] = "Phải nhập email không có khoảng trắng";
                allIsTrue = false;
            }
            else
            {
                var checkemail = c.KHACHHANGs.Where(x => x.Email == email);
                if (checkemail.Any())
                {
                    KHACHHANG chemail = checkemail.SingleOrDefault();
                    if (chemail.MaTK != Session["user"].ToString())
                    {
                        ViewData["loisdt"] = "Email đã có người đăng ký, vui lòng nhập số điện thoại khác";
                        allIsTrue = false;
                    }
                }
            }
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
            else
            {
                var checksdt = c.KHACHHANGs.Where(x => x.SDT == sdt);
                if (checksdt.Any())
                {
                    KHACHHANG chsdt = checksdt.SingleOrDefault();
                    if(chsdt.MaTK != Session["user"].ToString())
                    {
                        ViewData["loisdt"] = "Số điện thoại đã có người đăng ký, vui lòng nhập số điện thoại khác";
                        allIsTrue = false;
                    }
                }
            }
            if (String.IsNullOrEmpty(username))
            {
                ViewData["loiusername"] = "Phải nhập tên tài khoản";
                allIsTrue = false;
            }
            else if (username.Length < 5)
            {
                ViewData["loiusername"] = "Phải nhập tên tài khoản trên 5 kí tự";
                allIsTrue = false;
            }
            else if (username.Length > 20)
            {
                ViewData["loiusername"] = "Phải nhập tên tài khoản ít hơn 20 kí tự";
                allIsTrue = false;
            }
            else
            {
                var tkl = c.TAIKHOANs.Where(x => x.ID == username.Trim());
                if (tkl.Any())
                {
                    TAIKHOAN kt = tkl.SingleOrDefault();
                    if (kt.MaTK != Session["user"].ToString())
                    {
                        ViewData["loiusername"] = "Tên tài khoản đã có người đăng kí, mời chọn tên tài khoản khác";
                        allIsTrue = false;
                    }
                }
            }

            if (String.IsNullOrEmpty(ngaysinh))
            {
                ViewData["loingaysinh"] = "Phải nhập ngày sinh";
                allIsTrue = false;
            }
            else
            {
                try
                {
                    dt = Convert.ToDateTime(ngaysinh);
                    if ((DateTime.Now - dt).TotalDays < 6575 && (DateTime.Now - dt).TotalDays >= 0)
                    {
                        ViewData["loingaysinh"] = "Phải nhập ngày sinh trên 18 tuổi";
                        ViewData["ngaysinh"] = dt.ToString("yyyy-MM-dd");
                        allIsTrue = false;
                    }
                    else if ((DateTime.Now - dt).TotalDays < 6575 && (DateTime.Now - dt).TotalDays < 0)
                    {
                        ViewData["loingaysinh"] = "Phải nhập ngày sinh là ngày quá khứ";
                        ViewData["ngaysinh"] = dt.ToString("yyyy-MM-dd");
                        allIsTrue = false;
                    }
                }
                catch
                {
                    ViewData["loingaysinh"] = "Nhập ngày sinh không hợp lệ";
                    allIsTrue = false;
                }

            }
            if (String.IsNullOrEmpty(diachi))
            {
                ViewData["loidiachi"] = "Phải nhập địa chỉ";
                allIsTrue = false;
            }
            else if (diachi.Length > 200)
            {
                ViewData["loidiachi"] = "Phải nhập địa chỉ không quá 200 ký tự";
                allIsTrue = false;
            }
            else if (diachi.Length < 20)
            {
                ViewData["loidiachi"] = "Phải nhập địa chỉ không ít hơn 20 ký tự";
                allIsTrue = false;
            }

            if (allIsTrue)
            {
                TAIKHOAN tk = c.TAIKHOANs.SingleOrDefault(x => x.MaTK == id);
                tk.ID = username.Trim();
                try
                {
                    c.SubmitChanges();
                }
                catch
                {
                    ViewBag.thongBao = "Cập nhật thông tin tài khoản thất bại";
                    var b = (from t1 in c.KHACHHANGs
                             join t2 in c.TAIKHOANs on t1.MaTK equals t2.MaTK
                             where t1.MaTK == id
                             select new joinTable_KHG_TAIKHOAN
                             {
                                 KHACHHANG = t1,
                                 TAIKHOAN = t2,
                             }).SingleOrDefault();
                    return View(b);
                }
                KHACHHANG kh = c.KHACHHANGs.SingleOrDefault(x => x.MaTK == id);
                bool gt = new bool();
                if (gioitinh == "0")
                {
                    gt = false;
                }
                else if (gioitinh == "1")
                {
                    gt = true;
                }
                kh.HoTen = tenKH;
                kh.GioiTinh = gt;
                kh.Email = email;
                kh.SDT = sdt;
                kh.NgaySinh = dt;
                kh.DiaChi = diachi;
                kh.MaTK = tk.MaTK;
                try
                {
                    c.SubmitChanges();
                    ViewData.Clear();
                    TempData["TB"] = "Cập nhật thông tin tài khoản thành công";
                    return RedirectToAction("trangAdminQLTaiKhoan", "AdminQLTaiKhoan");
                }
                catch
                {
                    ViewBag.thongBao = "Cập nhật thông tin tài khoản thất bại";
                }
            }
            var a = (from t1 in c.KHACHHANGs
                     join t2 in c.TAIKHOANs on t1.MaTK equals t2.MaTK
                     where t1.MaTK == id
                     select new joinTable_KHG_TAIKHOAN
                     {
                         KHACHHANG = t1,
                         TAIKHOAN = t2,
                     }).SingleOrDefault();
            return View(a);
        }

        public ActionResult trangQLTaiKhoan_suaMatKhau(string id)
        {
            if (Session["admin"] == null || String.IsNullOrEmpty(id))
                return RedirectToAction("trangChu", "Guest");
            var a = (from t1 in c.TAIKHOANs
                     where t1.MaTK == id
                     select t1).SingleOrDefault();
            return View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult trangQLTaiKhoan_suaMatKhau(string id, string oldpass, string newpass, string renewpass)
        {
            bool allIsTrue = true;

            TAIKHOAN acc = c.TAIKHOANs.Where(x => x.MaTK == id).SingleOrDefault();
            if (String.IsNullOrEmpty(oldpass))
            {
                ViewData["loioldpass"] = "Phải nhập mật khẩu cũ";
                allIsTrue = false;
            }
            else if (acc.MatKhau != oldpass)
            {
                ViewData["loioldpass"] = "Mật khẩu cũ không đúng";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(newpass))
            {
                ViewData["loinewpass"] = "Phải nhập mật khẩu mới";
                allIsTrue = false;
            }
            else if (newpass.Length < 8)
            {
                ViewData["loinewpass"] = "Phải nhập mật khẩu mới trên 8 ký tự";
                allIsTrue = false;
            }
            else if (newpass.Length > 20)
            {
                ViewData["loinewpass"] = "Phải nhập mật khẩu mới không quá 20 ký tự";
                allIsTrue = false;
            }
            if (String.IsNullOrEmpty(renewpass))
            {
                ViewData["loirenewpass"] = "Phải nhập lại mật khẩu mới";
                allIsTrue = false;
            }
            else if (renewpass != newpass)
            {
                ViewData["loirenewpass"] = "Phải nhập lại mật khẩu giống mật khẩu mới";
                allIsTrue = false;
            }

            if(allIsTrue)
            {
                acc.MatKhau = newpass;
                try
                {
                    c.SubmitChanges();
                    ViewBag.thongBao = "Đổi mật khẩu thành công";
                    
                }
                catch
                {
                    ViewBag.thongBao = "Đổi mật khẩu thất bại";
                }
            }
            return View(acc);
        }

        public ActionResult trangQLTaiKhoan_xoa(string id)
        {
            if (id == null || Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            try
            {
                KHACHHANG kh = c.KHACHHANGs.SingleOrDefault(x => x.MaTK == id);
                TAIKHOAN tk = c.TAIKHOANs.SingleOrDefault(x => x.MaTK == id);
                c.KHACHHANGs.DeleteOnSubmit(kh);
                c.TAIKHOANs.DeleteOnSubmit(tk);
                c.SubmitChanges();
            }
            catch
            {
                TempData["TB"] = "Tài khoản đã có mua hàng, không xóa được do có đơn hàng";
            }
            return RedirectToAction("trangAdminQLTaiKhoan", "AdminQLTaiKhoan");
        }

        public ActionResult trangQLTaiKhoan_xem(string id)
        {
            if (id == null || Session["admin"] == null)
                return RedirectToAction("trangChu", "Guest");
            var a = (from t1 in c.KHACHHANGs
                     join t2 in c.TAIKHOANs on t1.MaTK equals t2.MaTK
                     where t1.MaTK == id
                     select new joinTable_KHG_TAIKHOAN
                     {
                         KHACHHANG = t1,
                         TAIKHOAN = t2
                     }).SingleOrDefault();
            return View(a);
        }

    }
}