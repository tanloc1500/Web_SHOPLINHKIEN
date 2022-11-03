using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_SHOPLINHKIEN.Models
{
    public class joinTable_KHG_DONHG
    {
        public KHACHHANG KHACHHANG { get; set; }
        public DONHANG DONHANG { get; set; }
        public TRANGTHAIDONHANG TRANGTHAIDONHANG { get; set; }
    }
}