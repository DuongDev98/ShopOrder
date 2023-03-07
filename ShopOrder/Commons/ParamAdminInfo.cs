using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopOrder.Commons
{
    public class ParamAdminInfo
    {
        public string dtNGAY { set; get; }
        public string DKHACHHANGID { set; get; }
        public string DNHANVIENID { set; get; }
        public int LOAIVANCHUYEN { set; get; }
        public string DNHAXEID { set; get; }
        public string DMATHANGID { set; get; }

        public ParamAdminInfo()
        {
            DateTime toDay = DateTime.Now.Date;
            LOAIVANCHUYEN = -1;
            dtNGAY = toDay.AddDays(-30).ToString("MM/dd/yyyy") + " - " + toDay.ToString("MM/dd/yyyy");
        }
    }
}