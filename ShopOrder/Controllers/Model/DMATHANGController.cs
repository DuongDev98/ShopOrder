using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ShopOrder.Models;
using ShopOrder.Utils;

namespace ShopOrder.Controllers.Model
{
    public class DMATHANGController : Controller
    {
        private ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index()
        {
            var dMATHANGs = db.DMATHANGs.Include(d => d.DDANG).Include(d => d.DNHOMHANG).Include(d => d.DPHANLOAI).Include(d => d.DTHOIGIANDAT);
            return View(dMATHANGs.OrderBy(x=>x.CODE).ToList());
        }

        public ActionResult Edit(string id)
        {
            DMATHANG dMATHANG;
            if (id == null)
            {
                dMATHANG = new DMATHANG();
            }
            else
            {
                dMATHANG = db.DMATHANGs.Find(id);
            }
            if (dMATHANG == null)
            {
                return HttpNotFound();
            }
            ViewBag.DDANGID = new SelectList(db.DDANGs, "ID", "NAME", dMATHANG.DDANGID);
            ViewBag.DNHOMHANGID = new SelectList(db.DNHOMHANGs, "ID", "NAME", dMATHANG.DNHOMHANGID);
            ViewBag.DPHANLOAIID = new SelectList(db.DPHANLOAIs, "ID", "NAME", dMATHANG.DPHANLOAIID);
            ViewBag.DTHOIGIANDATID = new SelectList(db.DTHOIGIANDATs, "ID", "NAME", dMATHANG.DTHOIGIANDATID);
            ViewBag.Sizes = db.DSIZEs.ToList();
            ViewBag.imgs = GetDicAnhs(db, dMATHANG);
            return View(dMATHANG);
        }

        private Dictionary<string, string> GetDicAnhs(ShopOrderEntities db, DMATHANG mhRow)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            List<DANH> lstAnhs = db.DANHs.Where(x => x.DMATHANGID == mhRow.ID).ToList();
            foreach (DANH anh in lstAnhs)
            {
                dic.Add(anh.ID, "/Images/Upload/" + anh.NAME);
            }
            return dic;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DMATHANG dMATHANG)
        {
            if (ModelState.IsValid)
            {
                //kiểm tra xem up ảnh hay chưa
                List<HttpPostedFileBase> files = new List<HttpPostedFileBase>();
                files.AddRange(Request.Files.GetMultiple("images[]"));
                string[] preloaded = Request.Params.GetValues("preloaded[]");
                bool hasImg = false;
                foreach (HttpPostedFileBase file in files)
                {
                    if (file.ContentLength > 0) hasImg = true;
                }
                if (!hasImg) hasImg = preloaded != null && preloaded.Length > 0;

                if (dMATHANG.ID == null)
                {
                    dMATHANG.ID = Guid.NewGuid().ToString();
                    dMATHANG.CODE = DatabaseUtils.GenCode("CODE", "DMATHANG");
                    db.DMATHANGs.Add(dMATHANG);
                }
                else
                {
                    db.Entry(dMATHANG).State = EntityState.Modified;
                }                
                db.SaveChanges();

                //upload anh
                uploadAnhMatHang(db, Server, preloaded, files, dMATHANG);

                return RedirectToAction("Index");
            }
            ViewBag.DDANGID = new SelectList(db.DDANGs, "ID", "NAME", dMATHANG.DDANGID);
            ViewBag.DNHOMHANGID = new SelectList(db.DNHOMHANGs, "ID", "NAME", dMATHANG.DNHOMHANGID);
            ViewBag.DPHANLOAIID = new SelectList(db.DPHANLOAIs, "ID", "NAME", dMATHANG.DPHANLOAIID);
            ViewBag.DTHOIGIANDATID = new SelectList(db.DTHOIGIANDATs, "ID", "NAME", dMATHANG.DTHOIGIANDATID);
            return View(dMATHANG);
        }

        private void uploadAnhMatHang(ShopOrderEntities db, HttpServerUtilityBase httpServer, string[] olds, List<HttpPostedFileBase> files, DMATHANG item)
        {
            //Lấy danh sách ảnh trong database cái nào không còn trong olds thì xóa.
            List<DANH> lstAnhs = db.DANHs.Where(x => x.DMATHANGID == item.ID).ToList();
            if (lstAnhs != null && lstAnhs.Count > 0)
            {
                foreach (DANH itemAnh in lstAnhs)
                {
                    if (olds == null || !olds.Contains(itemAnh.ID))
                    {
                        FileUploads.Delete(httpServer, itemAnh.NAME);
                        db.DANHs.Remove(itemAnh);
                    }
                }
            }
            //Thêm ảnh từ file
            if (files != null && files.Count > 0)
            {
                foreach (HttpPostedFileBase fileItem in files)
                {
                    if (fileItem.ContentLength == 0) continue;
                    string file = FileUploads.Upload(httpServer, fileItem);
                    DANH itemAnh = new DANH();
                    itemAnh.ID = Guid.NewGuid().ToString();
                    itemAnh.DMATHANGID = item.ID;
                    itemAnh.NAME = file;
                    db.DANHs.Add(itemAnh);
                }
            }
            db.SaveChanges();
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DMATHANG dMATHANG = db.DMATHANGs.Find(id);
            if (dMATHANG == null)
            {
                return HttpNotFound();
            }
            return View(dMATHANG);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DMATHANG dMATHANG = db.DMATHANGs.Find(id);
            db.DMATHANGs.Remove(dMATHANG);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
