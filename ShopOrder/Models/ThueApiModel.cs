using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI.WebControls;

namespace ShopOrder.Models
{
    public class ThueApiModel
    {
        public string phone { set; get; }
        public string money { set; get; }
        public string type { set; get; }
        public string gateway { set; get; }
        public string txn_id { set; get; }
        public string content { set; get; }
        public string datetime { set; get; }
    }
}