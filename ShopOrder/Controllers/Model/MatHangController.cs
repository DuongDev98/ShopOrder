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
using ShopOrder.Entities;
using ShopOrder.Utils;

namespace ShopOrder.Controllers.Model
{
    
    public class MatHangController : BaseController
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
            ViewBag.DDANGID = new SelectList(db.DDANGs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", dMATHANG.DDANGID);
            ViewBag.DNHOMHANGID = new SelectList(db.DNHOMHANGs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", dMATHANG.DNHOMHANGID);
            ViewBag.DPHANLOAIID = new SelectList(db.DPHANLOAIs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", dMATHANG.DPHANLOAIID);
            ViewBag.DTHOIGIANDATID = new SelectList(db.DTHOIGIANDATs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", dMATHANG.DTHOIGIANDATID);
            ViewBag.Sizes = db.DSIZEs.ToList();
            ViewBag.Maus = db.DMAUs.ToList();
            ViewBag.imgs = GetDicAnhs(db, dMATHANG);
            ViewBag.SizeSelect = LayDanhSachSizeDangChon(dMATHANG);
            ViewBag.MauSelect = LayDanhSachMauDangChon(dMATHANG);
            return View(dMATHANG);
        }

        private Dictionary<string, string> GetDicAnhs(ShopOrderEntities db, DMATHANG mhRow)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            List<DANH> lstAnhs = db.DANHs.Where(x => x.DMATHANGID == mhRow.ID).ToList();
            foreach (DANH anh in lstAnhs)
            {
                dic.Add(anh.ID, string.Format("/Images/{0}/", FileFolders.MatHang) + anh.NAME);
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

                string error = "";
                if (dMATHANG.NAME != null && dMATHANG.NAME.Length > 0)
                {
                    if (!hasImg)
                    {
                        error = "Hình ảnh sản phẩm không được trống";
                    }
                }
                else
                {
                    error = "Tên sản phẩm trống";
                }

                if (error.Length == 0)
                {
                    if (dMATHANG.ID == null)
                    {
                        dMATHANG.ID = Guid.NewGuid().ToString();
                        dMATHANG.CODE = DatabaseUtils.GenCode("CODE", "DMATHANG");
                        dMATHANG.TIMECREATED = DateTime.Now;
                        db.DMATHANGs.Add(dMATHANG);
                    }
                    else
                    {
                        db.Entry(dMATHANG).State = EntityState.Modified;
                    }
                    db.SaveChanges();

                    //xóa dữ liệu chi tiết
                    db.DSIZECHITIETs.RemoveRange(db.DSIZECHITIETs.Where(x => x.DMATHANGID == dMATHANG.ID).ToList());
                    db.DMAUCHITIETs.RemoveRange(db.DMAUCHITIETs.Where(x => x.DMATHANGID == dMATHANG.ID).ToList());
                    db.SaveChanges();

                    //luu size
                    if (Request.Params.AllKeys.Contains("sizeTmp"))
                    {
                        string[] data = Request.Params.GetValues("sizeTmp");
                        if (data != null && data.Length > 0)
                        {
                            foreach (string val in data)
                            {
                                DSIZECHITIET ctRow = new DSIZECHITIET();
                                ctRow.ID = Guid.NewGuid().ToString();
                                ctRow.DSIZEID = val;
                                ctRow.DMATHANGID = dMATHANG.ID;
                                db.DSIZECHITIETs.Add(ctRow);
                            }
                            db.SaveChanges();
                        }
                    }

                    //luu mau
                    if (Request.Params.AllKeys.Contains("mauTmp"))
                    {
                        string[] data = Request.Params.GetValues("mauTmp");
                        if (data != null && data.Length > 0)
                        {
                            foreach (string val in data)
                            {
                                DMAUCHITIET ctRow = new DMAUCHITIET();
                                ctRow.ID = Guid.NewGuid().ToString();
                                ctRow.DMAUID = val;
                                ctRow.DMATHANGID = dMATHANG.ID;
                                db.DMAUCHITIETs.Add(ctRow);
                            }
                            db.SaveChanges();
                        }
                    }

                    //upload anh
                    uploadAnhMatHang(db, Server, preloaded, files, dMATHANG);

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.errorMessage = error;
                }
            }
            ViewBag.DDANGID = new SelectList(db.DDANGs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", dMATHANG.DDANGID);
            ViewBag.DNHOMHANGID = new SelectList(db.DNHOMHANGs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", dMATHANG.DNHOMHANGID);
            ViewBag.DPHANLOAIID = new SelectList(db.DPHANLOAIs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", dMATHANG.DPHANLOAIID);
            ViewBag.DTHOIGIANDATID = new SelectList(db.DTHOIGIANDATs.OrderBy(x => x.NAME).ToList(), "ID", "NAME", dMATHANG.DTHOIGIANDATID);
            ViewBag.Sizes = db.DSIZEs.ToList();
            ViewBag.Maus = db.DMAUs.ToList();
            ViewBag.imgs = GetDicAnhs(db, dMATHANG);
            ViewBag.SizeSelect = LayDanhSachSizeDangChon(dMATHANG);
            ViewBag.MauSelect = LayDanhSachMauDangChon(dMATHANG);
            return View(dMATHANG);
        }

        private List<DMAU> LayDanhSachMauDangChon(DMATHANG dMATHANG)
        {
            var rs = new List<DMAU>();
            var lst = db.DMAUCHITIETs.Where(x => x.DMATHANGID == dMATHANG.ID).ToList();
            foreach (var x in lst)
            {
                rs.Add(x.DMAU);
            }
            return rs;
        }

        private List<DSIZE> LayDanhSachSizeDangChon(DMATHANG dMATHANG)
        {
            var rs = new List<DSIZE>();
            var lst = db.DSIZECHITIETs.Where(x => x.DMATHANGID == dMATHANG.ID).ToList();
            foreach (var x in lst)
            {
                rs.Add(x.DSIZE);
            }
            return rs;
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
                        FileUploads.DeleteMatHang(httpServer, itemAnh.NAME);
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
                    string file = FileUploads.UploadMatHang(httpServer, fileItem);
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

            //xóa dữ liệu chi tiết trước
            db.DSIZECHITIETs.RemoveRange(db.DSIZECHITIETs.Where(x => x.DMATHANGID == dMATHANG.ID).ToList());
            db.DMAUCHITIETs.RemoveRange(db.DMAUCHITIETs.Where(x => x.DMATHANGID == dMATHANG.ID).ToList());
            db.DANHs.RemoveRange(db.DANHs.Where(x => x.DMATHANGID == dMATHANG.ID).ToList());
            db.SaveChanges();

            //xóa dữ liệu ảnh
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
