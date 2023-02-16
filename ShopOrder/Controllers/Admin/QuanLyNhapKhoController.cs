using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShopOrder.Commons;
using ShopOrder.Controllers.Model;
using ShopOrder.Entities;
using ShopOrder.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopOrder.Controllers.Admin
{
    public class QuanLyNhapKhoController : BaseController
    {
        private ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index(ParamInfo info)
        {
            if (info == null)
            {
                info = new ParamInfo();
            }

            //param
            string[] splitDate = info.dtNGAY.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            DateTime fromDate = DateTime.Parse(splitDate[0]);
            DateTime toDate = DateTime.Parse(splitDate[1]);

            //filter
            IQueryable<TDONHANG> lst = db.TDONHANGs.Where(x => x.LOAI == 1);
            lst = lst.Where(x => x.NGAY >= fromDate.Date && x.NGAY <= toDate.Date);            
            lst = lst.Where(x => x.DNHANVIENID == info.DNHANVIENID || (info.DNHANVIENID ?? "") == "");
            lst = lst.Where(x => x.TDONHANGCHITIETs.Any(ct=>ct.DMATHANGID == info.DMATHANGID) || (info.DMATHANGID??"") == "");

            ViewBag.dtNGAY = info.dtNGAY;

            List<DNHANVIEN> lstNhanVien = db.DNHANVIENs.OrderBy(x => x.NAME).ToList();
            lstNhanVien.Insert(0, new DNHANVIEN() { ID = "", NAME = "" });
            ViewBag.DNHANVIENID = new SelectList(lstNhanVien, "ID", "NAME", info.DNHANVIENID);

            List<DMATHANG> lstMatHang = db.DMATHANGs.OrderBy(x => x.NAME).ToList();
            lstMatHang.Insert(0, new DMATHANG() { ID = "", NAME = "" });
            ViewBag.DMATHANGID = new SelectList(lstMatHang, "ID", "NAME", info.DMATHANGID);

            return View(lst.ToList());
        }

        public ActionResult Edit(string id)
        {
            TDONHANG model;
            if (id == null)
            {
                model = new TDONHANG();
                model.LOAI = 1;
                model.NGAY = DateTime.Now.Date;
                model.NAME = "Tự động";
            }
            else
            {
                model = db.TDONHANGs.Find(id);
            }
            if (model == null || model.LOAI != 1)
            {
                return HttpNotFound();
            }

            List<DNHANVIEN> lstNhanVien = db.DNHANVIENs.OrderBy(x => x.NAME).ToList();
            lstNhanVien.Insert(0, new DNHANVIEN() { ID = "", NAME = "" });
            ViewBag.DNHANVIENID = new SelectList(lstNhanVien, "ID", "NAME", model.DNHANVIENID);
            ViewBag.DsMatHang = db.DMATHANGs.OrderBy(x=>x.NAME).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(TDONHANG tmp)
        {
            if (ModelState.IsValid)
            {
                TDONHANG dhRow;
                if (tmp.ID == null)
                {
                    dhRow = new TDONHANG();
                    dhRow.ID = Guid.NewGuid().ToString();
                    dhRow.LOAI = 1;
                    dhRow.TIMECREATED = DateTime.Now;
                    dhRow.NAME = DatabaseUtils.GenCode("NAME", "TDONHANG", null, 1);
                    dhRow.DATHANHTOAN = 30;
                    db.TDONHANGs.Add(dhRow);
                    db.SaveChanges();
                }
                else
                {
                    dhRow = db.TDONHANGs.Find(tmp.ID);
                    dhRow.NGAY = tmp.NGAY;
                    dhRow.DNHANVIENID = tmp.DNHANVIENID;
                    dhRow.NOTE = tmp.NOTE;
                    db.Entry(dhRow).State = EntityState.Modified;
                    db.SaveChanges();
                }

                db.TDONHANGCHITIETs.RemoveRange(db.TDONHANGCHITIETs.Where(x=>x.TDONHANGID == dhRow.ID));
                db.SaveChanges();

                //thêm chi tiết
                foreach (TDONHANGCHITIET tmpCt in tmp.TDONHANGCHITIETs)
                {
                    TDONHANGCHITIET ctRow = new TDONHANGCHITIET();
                    ctRow.ID = Guid.NewGuid().ToString();
                    ctRow.TDONHANGID = dhRow.ID;
                    ctRow.DMATHANGID = tmpCt.DMATHANGID;
                    ctRow.DMAUID = tmpCt.DMAUID;
                    ctRow.DSIZEID = tmpCt.DSIZEID;
                    ctRow.DONGIA = tmpCt.DONGIA;
                    ctRow.SOLUONG = tmpCt.SOLUONG;
                    ctRow.THANHTIEN = ctRow.SOLUONG * ctRow.DONGIA;
                    db.TDONHANGCHITIETs.Add(ctRow);
                }
                db.SaveChanges();

                dhRow = db.TDONHANGs.Find(dhRow.ID);
                dhRow.TIENHANG = dhRow.TDONHANGCHITIETs.Sum(x => x.THANHTIEN);
                dhRow.TONGCONG = dhRow.TIENHANG;
                db.Entry(dhRow).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(tmp);
        }

        [HttpPost]
        public ActionResult FormThemMatHang(string id)
        {
            if (id == null) return HttpNotFound();
            DMATHANG mhRow = db.DMATHANGs.Find(id);
            if (mhRow == null) return HttpNotFound();
            return PartialView(mhRow);
        }

        [HttpPost]
        public ActionResult DetailTbody(string id)
        {
            if (id == null) return HttpNotFound();
            TDONHANG dhRow = db.TDONHANGs.Find(id);
            if (dhRow == null) return HttpNotFound();
            return PartialView(dhRow.TDONHANGCHITIETs.ToList());
        }
    }
}