using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_SHOPLINHKIEN.Models;

namespace Web_SHOPLINHKIEN.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        SHOPLINHKIENDataClassDataContext c = new SHOPLINHKIENDataClassDataContext();
        //Gioi Tinh KH Nam=0, Nu=1, DataType:Bit
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult trangDangNhap()
        {
            Session["user"] = null;
            Session["khachhang"] = null;
            Session["admin"] = null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult trangDangNhap(string username, string pass)
        {
            bool allIsTrue = true;
            if(String.IsNullOrEmpty(username))
            {
                ViewData["loiusername"] = "Phải nhập tên tài khoản";
                allIsTrue = false;
            }
            if(String.IsNullOrEmpty(pass))
            {
                ViewData["loipass"] = "Phải nhập mật khẩu";
                allIsTrue = false;
            }
            if(allIsTrue)
            {
                Session["user"] = null;
                TAIKHOAN us = new TAIKHOAN();
                us.ID = username;
                us.MatKhau = pass;
                TAIKHOAN kt = c.TAIKHOANs.SingleOrDefault(x => x.ID == us.ID && x.MatKhau == us.MatKhau);
                if(kt != null && kt.PhanQuyen == "USR")
                {
                    Session["user"] = kt.MaTK.Trim();
                    KHACHHANG kh = c.KHACHHANGs.SingleOrDefault(x => x.MaTK == Session["user"].ToString());
                    kh.MaKH.Trim();
                    kh.MaTK.Trim();
                    Session["khachhang"] = kh;
                    try
                    {
                        if (TempData["action"] == null || TempData["controller"] == null)
                        {
                            return RedirectToAction("trangChu", "Guest");
                        }
                        else
                        {
                            if (TempData["id"] == null)
                                return RedirectToAction(TempData["action"].ToString(), TempData["controller"].ToString());
                            else
                                return RedirectToAction(TempData["action"].ToString(), TempData["controller"].ToString(), new { id = TempData["id"].ToString() });

                        }

                    }
                    catch
                    {
                        ViewBag.thongBao = "Đăng nhập thất bại";
                        return View();
                    }
                }
                else if(kt != null && kt.PhanQuyen == "ADM")
                {
                    Session["admin"] = kt.MaTK.Trim();
                    return RedirectToAction("trangChuAdmin", "Admin");
                }
                else
                {
                    ViewData["loi"] = "Nhập sai tên đăng nhập hoặc mật khẩu";
                }
            }
            return View();
        }

        public ActionResult dangxuat()
        {
            Session["user"] = null;
            Session["khachhang"] = null;
            Session["admin"] = null;
            if (TempData["action"] == null || TempData["controller"] == null)
            {
                return RedirectToAction("trangChu", "Guest");
            }
            else
            {
                if (TempData["id"] == null)
                    return RedirectToAction(TempData["action"].ToString(), TempData["controller"].ToString());
                else
                    return RedirectToAction(TempData["action"].ToString(), TempData["controller"].ToString(), new { id = TempData["id"].ToString() });
            }
        }

        public ActionResult trangDangKy()
        {
            //Nam:0, Nu:1
            if (Session["admin"] != null)
                return RedirectToAction("trangChuAdmin", "Admin");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult trangDangKy(string tenKH, string gioitinh, string email, string sdt, 
            string username, string pass, string repass, string ngaysinh, string diachi)
        {
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

            bool allIsTrue = true;

            if(String.IsNullOrEmpty(tenKH))
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
                if(checkemail.Any())
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
                    ViewBag.thongBao = "Tạo tài khoản thất bại";
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
                    ViewBag.thongBao = "Tạo tài khoản thành công";
                }
                catch
                {
                    c.TAIKHOANs.DeleteOnSubmit(tk);
                    ViewBag.thongBao = "Tạo tài khoản thất bại";
                }
            }
            return View();
        }
    }
}