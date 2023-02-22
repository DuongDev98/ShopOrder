using ShopOrder.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ShopOrder.Utils
{
    public class FileUploads
    {
        public static void uploadAnh(ShopOrderEntities db, HttpServerUtilityBase httpServer, string[] olds, List<HttpPostedFileBase> files, DMATHANG dMATHANG, TGIAOHANG tGIAOHANG)
        {
            //Lấy danh sách ảnh trong database cái nào không còn trong olds thì xóa.
            List<DANH> lstAnhs = null;

            if (dMATHANG != null)
            {
                lstAnhs = db.DANHs.Where(x => x.DMATHANGID == dMATHANG.ID).ToList();
            }
            else if (tGIAOHANG != null)
            {
                lstAnhs = db.DANHs.Where(x => x.TGIAOHANGID == tGIAOHANG.ID).ToList();
            }

            if (lstAnhs != null && lstAnhs.Count > 0)
            {
                foreach (DANH itemAnh in lstAnhs)
                {
                    if (olds == null || !olds.Contains(itemAnh.ID))
                    {
                        if (dMATHANG != null)
                        {
                            DeleteMatHang(httpServer, itemAnh.NAME);
                        }
                        else if (tGIAOHANG != null)
                        {
                            DeleteDonHang(httpServer, itemAnh.NAME);
                        }
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
                    string file = "";
                    if (dMATHANG != null) file = UploadMatHang(httpServer, fileItem);
                    else if (tGIAOHANG != null) file = UploadDonHang(httpServer, fileItem);
                    DANH itemAnh = new DANH();
                    itemAnh.ID = Guid.NewGuid().ToString();
                    if (dMATHANG != null) itemAnh.DMATHANGID = dMATHANG.ID;
                    else if (tGIAOHANG != null) itemAnh.TGIAOHANGID = tGIAOHANG.ID;
                    itemAnh.NAME = file;
                    db.DANHs.Add(itemAnh);
                }
            }
            db.SaveChanges();
        }

        public static string UploadMatHang(HttpServerUtilityBase server, HttpPostedFileBase file)
        {
            return Upload(server, file, FileFolders.MatHang);
        }

        public static void DeleteMatHang(HttpServerUtilityBase server, string fileName)
        {
            Delete(server, fileName, FileFolders.MatHang);
        }

        public static string UploadDonHang(HttpServerUtilityBase server, HttpPostedFileBase file)
        {
            return Upload(server, file, FileFolders.DonHang);
        }

        public static void DeleteDonHang(HttpServerUtilityBase server, string fileName)
        {
            Delete(server, fileName, FileFolders.DonHang);
        }

        private static string Upload(HttpServerUtilityBase server, HttpPostedFileBase file, string folders)
        {
            string tempFolder = Path.Combine(server.MapPath(string.Format("~/Images/{0}/", folders)));
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            string temp = file.FileName;
            string[] options = temp.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            string fileName = Guid.NewGuid().ToString();
            if (options.Length > 1)
            {
                fileName = fileName + "." + options[options.Length - 1];
            }
            else
            {
                fileName = fileName + ".jpg";
            }

            string path = server.MapPath(string.Format("~/Images/{0}/{1}", folders, fileName));
            if (File.Exists(path)) File.Delete(path);

            file.SaveAs(path);

            return fileName;
        }

        private static void Delete(HttpServerUtilityBase server, string fileName, string folders)
        {
            if (fileName.ToLower().Contains("noimage.jpg") || fileName.ToLower().Contains("no-image.jpg")) return;
            string path = server.MapPath(string.Format("~/Images/{0}/{1}", folders, fileName));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    public class FileFolders {
        public const string MatHang = "Items";
        public const string DonHang = "Invoices";
    }
}