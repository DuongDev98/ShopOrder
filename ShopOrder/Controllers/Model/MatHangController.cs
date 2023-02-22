using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.DynamicData.ModelProviders;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ShopOrder.Entities;
using ShopOrder.Models;
using ShopOrder.Utils;

namespace ShopOrder.Controllers.Model
{
    public class MatHangController : BaseController
    {
        private ShopOrderEntities db = new ShopOrderEntities();

        public ActionResult Index()
        {
            UserModel userModel = CookieUtils.UserLogin();
            var dMATHANGs = db.DMATHANGs.Include(d => d.DDANG).Include(d => d.DNHOMHANG).Include(d => d.DPHANLOAI).Include(d => d.DTHOIGIANDAT);
            if (userModel.sUSER.DNHANVIEN != null && userModel.sUSER.DNHANVIEN.LOAITAIKHOAN == (int)LoaiTaiKhoan.CuaHang)
            {
                dMATHANGs = dMATHANGs.Where(x => x.DNHANVIENID == userModel.sUSER.DNHANVIENID);
            }
            return View(dMATHANGs.OrderBy(x=>x.CODE).ToList());
        }

        public ActionResult Edit(string id)
        {
            DMATHANG dMATHANG;
            UserModel userModel = CookieUtils.UserLogin();
            if (id == null)
            {
                dMATHANG = new DMATHANG();
                if (userModel.sUSER.DNHANVIEN != null && userModel.sUSER.DNHANVIEN.LOAITAIKHOAN == (int)LoaiTaiKhoan.CuaHang)
                {
                    dMATHANG.DNHANVIENID = userModel.sUSER.DNHANVIENID;
                }
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

            List<DNHANVIEN> listCuaHang = db.DNHANVIENs.Where(x => x.LOAITAIKHOAN == (int)LoaiTaiKhoan.CuaHang).OrderBy(x => x.NAME).ToList();
            listCuaHang.Insert(0, new DNHANVIEN() { ID = "", NAME = "" });
            ViewBag.DNHANVIENID = new SelectList(listCuaHang, "ID", "NAME", dMATHANG.DNHANVIENID);

            ViewBag.Sizes = db.DSIZEs.ToList();
            ViewBag.Maus = db.DMAUs.ToList();
            ViewBag.imgs = GetDicAnhs(db, dMATHANG);
            ViewBag.SizeSelect = LayDanhSachSizeDangChon(dMATHANG);
            ViewBag.MauSelect = LayDanhSachMauDangChon(dMATHANG);
            ViewBag.View = userModel.sUSER.DNHANVIEN != null && userModel.sUSER.DNHANVIEN.LOAITAIKHOAN == (int)LoaiTaiKhoan.CuaHang;
            return View(dMATHANG);
        }

        private Dictionary<string, string> GetDicAnhs(ShopOrderEntities db, DMATHANG mhRow)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            List<DANH> lstAnhs = db.DANHs.Where(x => x.DMATHANGID != null && x.DMATHANGID == mhRow.ID).ToList();
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
            UserModel userModel = CookieUtils.UserLogin();
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
                    FileUploads.uploadAnh(db, Server, preloaded, files, dMATHANG, null);

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

            List<DNHANVIEN> listCuaHang = db.DNHANVIENs.Where(x => x.LOAITAIKHOAN == (int)LoaiTaiKhoan.CuaHang).OrderBy(x => x.NAME).ToList();
            listCuaHang.Insert(0, new DNHANVIEN() { ID = "", NAME = "" });
            ViewBag.DNHANVIENID = new SelectList(listCuaHang, "ID", "NAME", dMATHANG.DNHANVIENID);

            ViewBag.Sizes = db.DSIZEs.ToList();
            ViewBag.Maus = db.DMAUs.ToList();
            ViewBag.imgs = GetDicAnhs(db, dMATHANG);
            ViewBag.SizeSelect = LayDanhSachSizeDangChon(dMATHANG);
            ViewBag.MauSelect = LayDanhSachMauDangChon(dMATHANG);
            ViewBag.View = userModel.sUSER.DNHANVIEN != null && userModel.sUSER.DNHANVIEN.LOAITAIKHOAN == (int)LoaiTaiKhoan.CuaHang;
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
