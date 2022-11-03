using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_SHOPLINHKIEN.Models;

namespace Web_SHOPLINHKIEN.Controllers
{
    public class UserQLTaiKhoanController : Controller
    {
        // GET: UserQLTaiKhoan
        SHOPLINHKIENDataClassDataContext c = new SHOPLINHKIENDataClassDataContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult trangThongTinTaiKhoan()
        {
            if (Session["admin"] != null)
                return RedirectToAction("trangChuAdmin", "Admin");
            if (Session["khachhang"] == null || Session["user"] == null)
            {
                return RedirectToAction("trangChu", "Guest");
            }

            if(TempData["TB"] != null)
            {
                ViewBag.thongBao = TempData["TB"];
            }
            var a = (from t1 in c.KHACHHANGs
                    join t2 in c.TAIKHOANs on t1.MaTK equals t2.MaTK
                    where t1.MaTK == Session["user"].ToString()
                    select new joinTable_KHG_TAIKHOAN
                    {
                        KHACHHANG = t1,
                        TAIKHOAN = t2,
                    }).SingleOrDefault();
            return View(a);
        }

        public ActionResult trangDoiMatKhau()
        {
            if (Session["admin"] != null)
                return RedirectToAction("trangChuAdmin", "Admin");
            if (Session["khachhang"] == null || Session["user"] == null)
                return RedirectToAction("trangChu", "Guest");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult trangDoiMatKhau(string oldpass, string newpass, string renewpass)
        {
            bool allIsTrue = true;
            TAIKHOAN acc = c.TAIKHOANs.Where(x => x.MaTK == Session["user"].ToString()).SingleOrDefault();
            if(String.IsNullOrEmpty(oldpass))
            {
                ViewData["loioldpass"] = "Phải nhập mật khẩu cũ";
                allIsTrue = false;
            }
            else if(acc.MatKhau != oldpass)
            {
                ViewData["loioldpass"] = "Mật khẩu cũ không đúng";
                allIsTrue = false;
            }
            if(String.IsNullOrEmpty(newpass))
            {
                ViewData["loinewpass"] = "Phải nhập mật khẩu mới";
                allIsTrue = false;
            }
            else if(newpass.Length < 8)
            {
                ViewData["loinewpass"] = "Phải nhập mật khẩu mới trên 8 ký tự";
                allIsTrue = false;
            }
            else if(newpass.Length > 20)
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
            return View();
        }

        public ActionResult trangThongTinTaiKhoan_sua()
        {
            if (Session["admin"] != null)
                return RedirectToAction("trangChuAdmin", "Admin");
            if (Session["khachhang"] == null || Session["user"] == null)
                return RedirectToAction("trangChu", "Guest");
            var a = (from t1 in c.KHACHHANGs
                     join t2 in c.TAIKHOANs on t1.MaTK equals t2.MaTK
                     where t1.MaTK == Session["user"].ToString()
                     select new joinTable_KHG_TAIKHOAN
                     {
                         KHACHHANG = t1,
                         TAIKHOAN = t2
                     }).SingleOrDefault();
            return View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult trangThongTinTaiKhoan_sua(string tenKH, string gioitinh, string email, string sdt,
            string username, string ngaysinh, string diachi)
        {
            DateTime dt = new DateTime();
            bool allIsTrue = true;
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
                        ViewData["loisdt"] = "Số điện thoại đã có người đăng ký, vui lòng nhập số điện thoại khác";
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
                    if (chsdt.MaTK != Session["user"].ToString())
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
                TAIKHOAN tk = c.TAIKHOANs.SingleOrDefault(x => x.MaTK == Session["user"].ToString());
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
                            where t1.MaTK == Session["user"].ToString()
                            select new joinTable_KHG_TAIKHOAN
                            {
                                KHACHHANG = t1,
                                TAIKHOAN = t2,
                            }).SingleOrDefault();
                    return View(b);
                }
                KHACHHANG kh = c.KHACHHANGs.SingleOrDefault(x => x.MaTK == Session["user"].ToString());
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
                    return RedirectToAction("trangThongTinTaiKhoan", "UserQLTaiKhoan");
                }
                catch
                {
                    ViewBag.thongBao = "Cập nhật thông tin tài khoản thất bại";
                }
            }
            return trangThongTinTaiKhoan_sua();
        }

    }
}