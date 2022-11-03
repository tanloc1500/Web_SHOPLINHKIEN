using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_SHOPLINHKIEN.Models
{
    public class joinTable_SANPHAM
    {
        public SANPHAM SANPHAM { get; set; }
        public HANGSANXUAT HANGSANXUAT { get; set; }
        public LOAISANPHAM LOAISANPHAM { get; set; }
        public ANHSANPHAM ANHSANPHAM { get; set; }
    }
}