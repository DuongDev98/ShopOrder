using ShopOrder.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopOrder.Models
{
    public class UserModel
    {
        public string USERNAME { set; get; }
        public string PASSWORD { set; get; }
        public SUSER sUSER { set; get; }
        public DKHACHHANG dKHACHHANG { set; get; }

        public bool IsAdmin
        {
            get { return sUSER != null && sUSER.ISADMIN > 0; }
        }

        public bool IsCustomer
        {
            get { return dKHACHHANG != null; }
        }
    }
}