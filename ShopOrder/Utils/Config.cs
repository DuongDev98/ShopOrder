using ShopOrder.Entities;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;

namespace ShopOrder.Utils
{
    public class Config
    {
        ShopOrderEntities db = new ShopOrderEntities();

        public static SCONFIG DATA
        {
            get
            {
                ShopOrderEntities db = new ShopOrderEntities();
                return db.SCONFIGs.FirstOrDefault() ?? new SCONFIG();
            }
        }

        public static void setThongBao(string value)
        {
            Update("THONGBAO", value);
        }

        public static void setThongTin(string value)
        {
            Update("THONGTIN", value);
        }

        public static void setFooterContent(string value)
        {
            Update("NOIDUNGCHANTRAN", value);
        }

        private static void Update(string field, string value)
        {
            ShopOrderEntities db = new ShopOrderEntities();
            SCONFIG sCONFIG = db.SCONFIGs.FirstOrDefault();
            bool addNew = false;
            if (sCONFIG == null)
            {
                addNew = true;
                sCONFIG = new SCONFIG();
                sCONFIG.ID = Guid.NewGuid().ToString();
            }
            switch (field)
            {
                case "THONGBAO": sCONFIG.THONGBAO = value; break;
                case "THONGTIN": sCONFIG.THONGTIN = value; break;
                case "NOIDUNGCHANTRAN": sCONFIG.NOIDUNGCHANTRAN = value; break;
            }
            if (addNew) db.SCONFIGs.Add(sCONFIG);
            else db.Entry(sCONFIG);
            db.SaveChanges();
        }
    }
}