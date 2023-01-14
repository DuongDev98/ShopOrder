using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopOrder.Models
{
    public class ItemModel
    {
        public string DMATHANGID { set; get; }
        public string DMATHANG_NAME { set; get; }
        public long DONGIA { set; get; }
        public long SOLUONG { set; get; }
        public long THANHTIEN { set; get; }
        public string DMAUID { set; get; }
        public string DMAU_NAME { set; get; }
        public string DSIZEID { set; get; }
        public string DSIZE_NAME { set; get; }
        public string AVATAR { set; get; }
    }
}